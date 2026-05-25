<?php
/*
 * runner.php — Reference 6th-implementation runner for x402 STARK Receipts conformance
 *
 * Pure-stdlib PHP runner : no composer dependency. Implements RFC 8785 JCS canonicalisation
 * via recursive key sorting + UTF-8 byte-preserving serialisation, then SHA-256 over the
 * canonical bytes. Validates the manifest.json anchor sets against the vectors/ directory.
 *
 * Authored by Vauban Research <research@vauban.tech>.
 * License : Apache-2.0.
 *
 * Usage : php runners/runner.php [vectors_path]
 * Default vectors_path : ./vectors
 *
 * Requires : PHP 8.0+ (json_decode/encode with JSON_THROW_ON_ERROR and JSON_UNESCAPED_*)
 */

declare(strict_types=1);

function jcs_canonicalize(mixed $value): string {
    if (is_array($value)) {
        // Distinguish list vs assoc by checking integer keys
        $isList = array_is_list($value);
        if ($isList) {
            $parts = array_map('jcs_canonicalize', $value);
            return '[' . implode(',', $parts) . ']';
        }
        ksort($value, SORT_STRING);
        $parts = [];
        foreach ($value as $k => $v) {
            $parts[] = json_encode((string)$k, JSON_UNESCAPED_SLASHES | JSON_UNESCAPED_UNICODE | JSON_THROW_ON_ERROR)
                . ':' . jcs_canonicalize($v);
        }
        return '{' . implode(',', $parts) . '}';
    }
    if (is_string($value)) {
        return json_encode($value, JSON_UNESCAPED_SLASHES | JSON_UNESCAPED_UNICODE | JSON_THROW_ON_ERROR);
    }
    if (is_int($value)) {
        return (string)$value;
    }
    if (is_float($value)) {
        // RFC 8785 §3.2.2.3 : integer-valued floats serialise as integer
        if ((float)(int)$value === $value && abs($value) < PHP_INT_MAX) {
            return (string)(int)$value;
        }
        return rtrim(rtrim(sprintf('%.17g', $value), '0'), '.');
    }
    if (is_bool($value)) {
        return $value ? 'true' : 'false';
    }
    if (is_null($value)) {
        return 'null';
    }
    throw new RuntimeException('Unsupported JSON type : ' . gettype($value));
}

function sha256_hex(string $bytes): string {
    return hash('sha256', $bytes);
}

/**
 * Normalise a digest value for comparison: strip the 'sha256:' prefix if present.
 * Vectors may store digests as either raw hex or 'sha256:<hex>'; comparing
 * normalised forms avoids false MISMATCH when one side carries the prefix.
 */
function normalize_digest(?string $d): ?string {
    if ($d === null) return null;
    return preg_replace('/^sha256:/', '', $d);
}

function validate_vector(string $path): array {
    $raw = file_get_contents($path);
    if ($raw === false) {
        return ['status' => 'ERROR', 'reason' => 'cannot read file'];
    }
    $vector = json_decode($raw, true, 512, JSON_THROW_ON_ERROR);

    $preimage_key = null;
    foreach (['receipt_core', 'delegation_grant', 'settlement_receipt', 'composite_payload'] as $candidate) {
        if (isset($vector[$candidate])) {
            $preimage_key = $candidate;
            break;
        }
    }
    if ($preimage_key === null) {
        // Adversarial vectors expose `preimage` directly
        if (isset($vector['preimage'])) {
            $preimage_key = 'preimage';
        } else {
            return ['status' => 'SKIP', 'reason' => 'no canonical preimage field found'];
        }
    }

    $canonical = jcs_canonicalize($vector[$preimage_key]);
    $digest = 'sha256:' . sha256_hex($canonical);

    $expected_pass = $vector['expected_core_digest'] ?? $vector['expected_delegation_grant_hash'] ?? null;
    $expected_fail = $vector['expected_divergent_digest'] ?? null;
    $expected_result = $vector['expected_result'] ?? 'UNKNOWN';

    if ($expected_result === 'PASS' && $expected_pass !== null) {
        return [
            'status' => normalize_digest($digest) === normalize_digest($expected_pass) ? 'OK' : 'MISMATCH',
            'expected' => $expected_pass,
            'computed' => $digest,
        ];
    }
    if ($expected_result === 'FAIL' && $expected_fail !== null) {
        return [
            'status' => normalize_digest($digest) === normalize_digest($expected_fail) ? 'OK' : 'MISMATCH',
            'expected' => $expected_fail,
            'computed' => $digest,
        ];
    }
    if ($expected_result === 'FAIL_AT_VERIFIER') {
        // Canonicalisation succeeds ; semantic verification is out of scope for this runner
        return ['status' => 'OK', 'note' => 'canon-pass, verifier-fail (semantic, not tested here)'];
    }

    return ['status' => 'SKIP', 'reason' => 'incomplete pin'];
}

$vectors_dir = $argv[1] ?? __DIR__ . '/../vectors';
if (!is_dir($vectors_dir)) {
    fwrite(STDERR, "vectors directory not found : $vectors_dir\n");
    exit(2);
}

$total = 0; $ok = 0; $mismatch = 0; $skip = 0;
$it = new RecursiveIteratorIterator(new RecursiveDirectoryIterator($vectors_dir));
foreach ($it as $file) {
    if (!$file->isFile() || $file->getExtension() !== 'json') continue;
    $relative = str_replace($vectors_dir . '/', '', $file->getPathname());
    $result = validate_vector($file->getPathname());
    $total++;
    printf("  %-60s %s\n", $relative, $result['status']);
    if ($result['status'] === 'OK') $ok++;
    elseif ($result['status'] === 'MISMATCH') $mismatch++;
    else $skip++;
}

printf("\n%d vectors | %d OK | %d MISMATCH | %d SKIP\n", $total, $ok, $mismatch, $skip);
exit($mismatch > 0 ? 1 : 0);
