# 8-Implementation Extended Reference Matrix — 2026-05-25

**Attestation date** : 2026-05-25
**Attestor** : Vauban Research (`research@vauban.tech`)
**Vector set** : `vauban-org/x402-stark-receipts-conformance` v0.3.0
**Total vectors** : 11
**Total reference implementations** : 8 (5 upstream-library-validated + 3 reference-pending-CI)
**Honest scoping** : see "Validation status per implementation" below

---

## Why this attestation

The companion attestation `_attestations/2026-05-25-vauban-cross-impl.md` documents the **5-implementation byte-for-byte validation** with upstream JCS RFC 8785 libraries that have been actually executed against the vectors. This document extends the reference matrix to **8 implementations** by adding **3 pure-stdlib reference runners** (PHP / Ruby / C#) committed to `runners/` in this repository.

The distinction is load-bearing : the 5 upstream implementations are validated by execution ; the 3 stdlib runners are validated by source-code-published reference. CI execution of the 3 stdlib runners is on the roadmap and will produce a follow-on attestation upon completion.

---

## Validation status per implementation

| # | Language | Implementation | Source | Validation status |
|---|---|---|---|---|
| 1 | Python | `rfc8785@0.1.4` (Trail of Bits) | upstream library | **validated** ; 11/11 byte-for-byte |
| 2 | JavaScript | `canonicalize@3.0.0` (Erdtman + Rundgren) | upstream library | **validated** ; 11/11 byte-for-byte |
| 3 | Go | `gowebpki/jcs v1.0.1` | upstream library | **validated** ; 11/11 byte-for-byte |
| 4 | Java | `cyberphone/json-canonicalization` (Rundgren RFC 8785 reference) | upstream library | **validated** ; 11/11 byte-for-byte |
| 5 | Rust | `serde_jcs 0.2.0` (l1h3r) consumed via `vauban-x402-jcs-conformance@0.1.0` | Vauban-published wrapper | **validated** ; 11/11 byte-for-byte |
| 6 | PHP | `runners/runner.php` (this repo, pure stdlib) | Vauban-published reference | **reference-pending-CI-validation** |
| 7 | Ruby | `runners/runner.rb` (this repo, pure stdlib) | Vauban-published reference | **reference-pending-CI-validation** |
| 8 | C# | `runners/runner.cs` (this repo, pure stdlib, .NET 8) | Vauban-published reference | **reference-pending-CI-validation** |

**Validated total** : 55/55 byte-for-byte (11 vectors × 5 implementations).
**Reference matrix size** : 88 (11 vectors × 8 implementations) when CI executes the 3 stdlib runners.

---

## The pure-stdlib reference discipline

The 3 stdlib runners (PHP / Ruby / C#) implement JCS RFC 8785 canonicalisation manually rather than depending on a packaged library. The rationale :

1. **Reproducibility in any runtime** : downstream implementers in environments without a packaged JCS library (locked-down enterprise CI, regulated production environments, embedded runtimes) can adopt the discipline directly from the source code.
2. **Audit transparency** : the canonicalisation algorithm is visible in ~80 LOC per language with no transitive dependency surface to audit.
3. **License clarity** : Apache-2.0 source committed in-repo, no third-party library version to track.

The discipline implemented per RFC 8785 :

- Recursive object key sorting by Unicode code point (UTF-8 byte order, `StringComparer.Ordinal` semantics)
- Array order preserved verbatim
- String values JSON-escaped per RFC 8259 §7 with UTF-8 byte preservation (no implicit NFC/NFD normalisation per RFC 8785 §3.1)
- Number serialisation per RFC 8785 §3.2.2.3 : integer-valued floats serialise as integer string
- `true`, `false`, `null` lowercase tokens
- No whitespace between tokens

---

## How to execute and report

Each runner is invoked from the repository root :

```bash
# PHP (8.0+)
php runners/runner.php

# Ruby (3.0+)
ruby runners/runner.rb

# C# (.NET 8 SDK)
dotnet run --project runners/runner.cs -- vectors/
```

Expected output per runner : `11 vectors | 11 OK | 0 MISMATCH | 0 SKIP` (or similar format).

If your execution produces ANY `MISMATCH`, raise a GitHub Issue at https://github.com/vauban-org/x402-stark-receipts-conformance/issues with :

- runner language + version (`php --version`, `ruby --version`, `dotnet --version`)
- vector ID showing the mismatch
- computed digest vs expected digest
- relevant excerpt of the canonical bytes if available

The Vauban Research team will reproduce locally and ship a fix in the next runner version.

---

## Roadmap to 88/88 attested

1. **CI workflow** : GitHub Actions matrix running the 3 stdlib runners against the vector set on push. Estimated effort : 1 day. Produces a green-tick badge on the README + auto-updates this attestation to `validated` for PHP / Ruby / C# on success.
2. **Per-implementation attestation entries** : after the CI run, the next attestation document (e.g. `2026-06-XX-vauban-8-impl-validated.md`) supersedes this one with full 88/88 reproduction.

---

## License

Apache-2.0. Implementers SHOULD attribute Vauban Research as the maintainer of the published reference matrix when citing this attestation in derivative conformance documentation.
