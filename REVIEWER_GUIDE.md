# Reviewer Guide — x402 STARK Receipts Conformance

This guide gives any reviewer a **three-command reproduction path** for the conformance vector set.

---

## Quick reproduction (Rust ; recommended)

```bash
git clone https://github.com/vauban-org/x402-stark-receipts-conformance.git
cd x402-stark-receipts-conformance && cargo install vauban-x402-jcs-conformance
vauban-x402-jcs-conformance validate vectors/
```

Expected output : `11 vectors | 11 conformant | 0 divergent`.

The runner reads each JSON file under `vectors/`, canonicalises with `serde_jcs 0.2.0` (RFC 8785), computes SHA-256 over the canonical bytes, and asserts the digest matches `expected_core_digest` (for PASS vectors) or `expected_divergent_digest` (for FAIL vectors with pinned divergent digests).

---

## Alternative reproduction (Python)

```bash
git clone https://github.com/vauban-org/x402-stark-receipts-conformance.git
cd x402-stark-receipts-conformance && pip install vauban-x402-stark-receipt
python -m vauban_x402_stark_receipt validate vectors/
```

Uses `rfc8785 0.1.4` (Trail of Bits) as JCS canonicaliser.

---

## Alternative reproduction (JavaScript)

```bash
git clone https://github.com/vauban-org/x402-stark-receipts-conformance.git
cd x402-stark-receipts-conformance && npm install @vauban-pay/substrate
node -e "require('@vauban-pay/substrate').validateDirectory('vectors/')"
```

Uses `canonicalize 3.0.0` (Erdtman + Rundgren) as JCS canonicaliser.

---

## Manual reproduction (any RFC 8785 implementation)

For any conforming JCS implementation in any language :

1. For each vector file in `vectors/<anchor_set>/`, take the field declared as the canonical preimage (typically `receipt_core`, `delegation_grant`, or the full vector body minus `expected_*` metadata fields ; consult the manifest for the exact preimage boundary).
2. Canonicalise with your JCS implementation per RFC 8785 (sorted keys, no whitespace, integer-vs-float discipline per RFC 8785 §3.2.2.3).
3. Compute `SHA-256(canonical_bytes)`, lowercase hex.
4. **For PASS vectors** : assert computed digest equals `expected_core_digest` (prefixed `sha256:`).
5. **For FAIL vectors** : assert computed digest equals `expected_divergent_digest` (the pinned actual digest of the divergent canonical form, which proves the failure mode is deterministic across implementations).
6. **For interop vectors** (e.g. `vectors/stark/0008-interop-shared-payment-hash.json`) : additionally assert byte equality between `payment_hash` and `action_ref` across baselines from other anchor sets (proves the cross-axis binding).

---

## Manifest verification

`manifest.json` carries the SHA-256 of every vector file and every schema file. To verify the manifest matches the on-disk content :

```bash
jq -r '.anchor_sets[].vectors[] | "\(.sha256)  \(.path)"' manifest.json | sed 's|sha256:||' > manifest_hashes.txt
sha256sum -c manifest_hashes.txt
```

Expected : all entries `OK`.

---

## Independent JCS implementations

The five reference implementations cross-validated for these vectors :

| Library | Language | Author |
|---|---|---|
| `rfc8785@0.1.4` | Python | Trail of Bits |
| `canonicalize@3.0.0` | JavaScript | Erdtman + Rundgren |
| `gowebpki/jcs v1.0.1` | Go | GoWebPKI |
| `cyberphone/json-canonicalization` | Java | Rundgren (RFC 8785 reference implementation) |
| `serde_jcs 0.2.0` | Rust | l1h3r |

The 5-implementation byte-for-byte attestation record is in `_attestations/2026-05-25-vauban-cross-impl.md`.

---

## Anchor set descriptions

| Set | Purpose | Vectors |
|---|---|---|
| `vectors/stark/` | STARK receipt canonicalisation under JCS ; cross-axis interop binding ; cross-runtime CBOR reference | 5 |
| `vectors/delegation-grant/` | DelegationGrant + SettlementReceipt pair under six-element Claim shape ; demonstrates the seven `fiscal_authority` qualification fields mapped to cryptographic primitives | 3 |
| `vectors/action_ref_adversarial/` | Follow-on FAIL vectors for action-ref derivation : duplicate-key, NFD divergence, required-field-missing | 3 |

See each anchor set's vector files for `case`, `expected_result`, `invariant`, and `notes` metadata.

---

## Reporting divergence

If your runner produces a different digest from the pinned `expected_*_digest`, the divergence is one of three :

1. **JCS implementation drift** : your library does not strictly implement RFC 8785 (the most common offender is JSON parsers that coerce integer-valued floats to decimal form before serialisation, violating RFC 8785 §3.2.2.3).
2. **Unicode normalisation drift** : your pipeline normalises NFC/NFD silently (RFC 8785 is byte-preserving and forbids implicit normalisation).
3. **Parser duplicate-key handling** : your JSON parser silently last-wins or first-wins on duplicate keys (RFC 7159 §4 leaves this undefined ; JCS requires reject-at-parse).

For each, raise an issue at https://github.com/vauban-org/x402-stark-receipts-conformance/issues with the vector ID, your computed digest, and the JCS library + version.

---

## License

Apache-2.0. Reproduction and integration into conforming x402 implementations unrestricted under §2 + §4.
