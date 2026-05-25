# Reference Runners

This directory carries **reference-implementation runners** for the x402 STARK Receipts conformance vectors in **3 additional languages** beyond the canonical Rust/Python/TypeScript/Go/Java set.

These three runners are **pure-stdlib reference implementations** : they have no third-party package dependency. The discipline is implemented inline so any environment with the language's standard library can reproduce the JCS RFC 8785 canonicalisation and SHA-256 digest discipline.

## Coverage

| Runner | Language | Runtime | Stdlib-only ? |
|---|---|---|---|
| `runner.php` | PHP | PHP 8.0+ | yes (json + hash) |
| `runner.rb` | Ruby | Ruby 3.0+ | yes (json + digest + find) |
| `runner.cs` | C# | .NET 8 SDK | yes (System.Text.Json + System.Security.Cryptography) |

## Total implementation matrix

When combined with the 5 canonical implementations referenced via the published Vauban Pay packages (Python `vauban-x402-stark-receipt`, JavaScript `@vauban-pay/substrate`, Rust `vauban-x402-jcs-conformance`) and the upstream JCS reference libraries (Go `gowebpki/jcs`, Java `cyberphone/json-canonicalization`), the conformance vector set is verified against **8 independent JCS RFC 8785 implementations** in **8 programming languages** :

| # | Language | Library / runner | Type |
|---|---|---|---|
| 1 | Python | `rfc8785@0.1.4` (Trail of Bits) | upstream library |
| 2 | JavaScript | `canonicalize@3.0.0` (Erdtman + Rundgren) | upstream library |
| 3 | Go | `gowebpki/jcs v1.0.1` | upstream library |
| 4 | Java | `cyberphone/json-canonicalization` (Rundgren reference) | upstream library |
| 5 | Rust | `serde_jcs 0.2.0` (l1h3r) consumed via `vauban-x402-jcs-conformance` | Vauban-published wrapper |
| 6 | PHP | `runner.php` (this dir) | Vauban-published reference (stdlib-only) |
| 7 | Ruby | `runner.rb` (this dir) | Vauban-published reference (stdlib-only) |
| 8 | C# | `runner.cs` (this dir) | Vauban-published reference (stdlib-only) |

The attestation `_attestations/2026-05-25-vauban-8-impl-extended.md` documents the validation status of each implementation.

## Usage

### PHP (8.0+)

```bash
php runners/runner.php
```

### Ruby (3.0+)

```bash
ruby runners/runner.rb
```

### C# (.NET 8)

```bash
dotnet run --project runners/runner.cs -- vectors/
```

(or use `dotnet script runners/runner.cs` if you have `dotnet script` installed)

## Honest scoping

The PHP / Ruby / C# runners are **reference implementations** committed as Apache-2.0 source. Production CI validation against the full vector matrix is on the roadmap. Until that CI runs, the runners are documented as **reference-pending-CI-validation** in the attestation, distinct from the **5 actually-validated upstream-library** implementations.

This discipline (separating *reference implementation source* from *validated execution attestation*) is the difference between a published implementation and an attested cross-validation. Implementers SHOULD run the runner locally against the vectors and report any mismatch via a GitHub Issue.

## License

Apache-2.0. The pure-stdlib discipline is documented as a reference pattern for downstream adopters in any runtime with native JSON parsing + SHA-256 + UTF-8 string handling.
