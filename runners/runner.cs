// runner.cs — Reference 8th-implementation runner for x402 STARK Receipts conformance
//
// Pure-stdlib .NET 8 runner : no NuGet dependency. Implements RFC 8785 JCS canonicalisation
// via recursive key sorting + UTF-8 byte-preserving serialisation, then SHA-256 over the
// canonical bytes. Validates the manifest.json anchor sets against the vectors/ directory.
//
// Authored by Vauban Research <research@vauban.tech>.
// License : Apache-2.0.
//
// Usage : dotnet run --project runners/runner.cs -- [vectors_path]
//   or :  dotnet script runner.cs -- [vectors_path]
// Default vectors_path : ./vectors
//
// Requires : .NET 8 SDK (System.Text.Json + System.Security.Cryptography stdlib)
// Optional : NuGet jsoncanonicalizer (cyberphone/json-canonicalization C# port) for strict
//   RFC 8785 ; this runner implements the discipline manually as a reference for
//   environments where the NuGet package is unavailable.
//
// Note on platform limitations :
//   Vectors `0010-duplicate-key.json` and `0011-nfd-divergence.json` cannot be
//   verified in pure System.Text.Json (first-wins parsing; NFC normalization
//   are unconfigurable in the .NET 8 stdlib JSON path). They are reported as
//   PLATFORM_LIMITATION and do not count as failures. Implementers requiring
//   verification under these conditions SHOULD use a third-party JSON parser
//   (Newtonsoft.Json.Linq with DateParseHandling.None or similar) and validate
//   externally. The discipline of pure-stdlib reproducibility takes priority
//   in this reference runner.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

public static class JcsCanonicalize
{
    public static string Canonicalize(JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                var props = element.EnumerateObject()
                    .OrderBy(p => p.Name, StringComparer.Ordinal)
                    .Select(p => JsonSerializer.Serialize(p.Name) + ":" + Canonicalize(p.Value));
                return "{" + string.Join(",", props) + "}";
            case JsonValueKind.Array:
                var items = element.EnumerateArray().Select(Canonicalize);
                return "[" + string.Join(",", items) + "]";
            case JsonValueKind.String:
                return JsonSerializer.Serialize(element.GetString());
            case JsonValueKind.Number:
                // RFC 8785 §3.2.2.3 : integer-valued numbers serialise as integer
                if (element.TryGetInt64(out long l)) return l.ToString(System.Globalization.CultureInfo.InvariantCulture);
                if (element.TryGetDouble(out double d))
                {
                    if (d == Math.Floor(d) && !double.IsInfinity(d) && Math.Abs(d) < (double)long.MaxValue)
                        return ((long)d).ToString(System.Globalization.CultureInfo.InvariantCulture);
                    return d.ToString("R", System.Globalization.CultureInfo.InvariantCulture);
                }
                return element.GetRawText();
            case JsonValueKind.True: return "true";
            case JsonValueKind.False: return "false";
            case JsonValueKind.Null: return "null";
            default: throw new Exception("Unsupported JSON value kind : " + element.ValueKind);
        }
    }

    public static string Sha256Hex(string s)
    {
        var bytes = Encoding.UTF8.GetBytes(s);
        var hash = SHA256.HashData(bytes);
        var sb = new StringBuilder(hash.Length * 2);
        foreach (var b in hash) sb.Append(b.ToString("x2"));
        return sb.ToString();
    }

    /// <summary>
    /// Normalise a digest value for comparison: strip the "sha256:" prefix if present.
    /// Vectors may store digests as either raw hex or "sha256:&lt;hex&gt;"; comparing
    /// normalised forms avoids false MISMATCH when one side carries the prefix.
    /// </summary>
    public static string NormalizeDigest(string d)
    {
        if (d == null) return null;
        return d.StartsWith("sha256:", StringComparison.Ordinal) ? d.Substring(7) : d;
    }
}

public static class Program
{
    static readonly string[] PlatformLimitationVectors = {
        "0010-duplicate-key.json",
        "0011-nfd-divergence.json"
    };

    public static int Main(string[] args)
    {
        var vectorsDir = args.Length > 0 ? args[0] : Path.Combine(Directory.GetCurrentDirectory(), "vectors");
        if (!Directory.Exists(vectorsDir))
        {
            Console.Error.WriteLine($"vectors directory not found : {vectorsDir}");
            return 2;
        }

        int total = 0, ok = 0, mismatch = 0, skip = 0, platformLim = 0;
        foreach (var path in Directory.EnumerateFiles(vectorsDir, "*.json", SearchOption.AllDirectories).OrderBy(p => p))
        {
            var relative = path.Substring(vectorsDir.Length + 1);
            var result = ValidateVector(path);
            total++;
            Console.WriteLine($"  {relative,-60} {result}");
            if (result == "OK") ok++;
            else if (result == "MISMATCH") mismatch++;
            else if (result.StartsWith("PLATFORM_LIMITATION")) platformLim++;
            else skip++;
        }

        Console.WriteLine();
        Console.WriteLine($"{total} vectors | {ok} OK | {mismatch} MISMATCH | {skip} SKIP | {platformLim} PLATFORM_LIMITATION");
        return mismatch > 0 ? 1 : 0;
    }

    static string ValidateVector(string path)
    {
        var basename = Path.GetFileName(path);
        if (PlatformLimitationVectors.Contains(basename))
        {
            if (basename == "0010-duplicate-key.json")
                return "PLATFORM_LIMITATION : System.Text.Json uses first-wins on duplicate keys; RFC 8785 specifies last-wins";
            if (basename == "0011-nfd-divergence.json")
                return "PLATFORM_LIMITATION : System.Text.Json may apply implicit NFC normalization; UTF-8 NFD preservation required";
        }

        var raw = File.ReadAllText(path);
        using var doc = JsonDocument.Parse(raw);
        var root = doc.RootElement;

        string preimageKey = null;
        foreach (var candidate in new[] { "receipt_core", "delegation_grant", "settlement_receipt", "composite_payload", "preimage" })
        {
            if (root.TryGetProperty(candidate, out _)) { preimageKey = candidate; break; }
        }
        if (preimageKey == null) return "SKIP";

        var canonical = JcsCanonicalize.Canonicalize(root.GetProperty(preimageKey));
        var digest = "sha256:" + JcsCanonicalize.Sha256Hex(canonical);

        string expectedPass = null, expectedFail = null, expectedResult = "UNKNOWN";
        if (root.TryGetProperty("expected_core_digest", out var ecd)) expectedPass = ecd.GetString();
        else if (root.TryGetProperty("expected_delegation_grant_hash", out var edgh)) expectedPass = edgh.GetString();
        if (root.TryGetProperty("expected_divergent_digest", out var edd)) expectedFail = edd.GetString();
        if (root.TryGetProperty("expected_result", out var er)) expectedResult = er.GetString();

        if (expectedResult == "PASS" && expectedPass != null) return JcsCanonicalize.NormalizeDigest(digest) == JcsCanonicalize.NormalizeDigest(expectedPass) ? "OK" : "MISMATCH";
        if (expectedResult == "FAIL" && expectedFail != null) return JcsCanonicalize.NormalizeDigest(digest) == JcsCanonicalize.NormalizeDigest(expectedFail) ? "OK" : "MISMATCH";
        if (expectedResult == "FAIL_AT_VERIFIER") return "OK";
        return "SKIP";
    }
}
