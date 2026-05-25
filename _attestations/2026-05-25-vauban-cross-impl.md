# 5-Implementation Cross-Validation Attestation — 2026-05-25

**Attestation date** : 2026-05-25
**Attestor** : Vauban Research (`research@vauban.tech`)
**Vector set** : `vauban-org/x402-stark-receipts-conformance` v0.2.0
**Total vectors** : 11
**Total pair invariants** : 2

---

## Summary

All **11 vectors** verify byte-for-byte across **5 independent JCS RFC 8785 implementations** in 5 different programming languages by 5 different authors.

| Library | Language | Author | Vectors verified |
|---|---|---|---|
| `rfc8785@0.1.4` | Python | Trail of Bits | 11/11 |
| `canonicalize@3.0.0` | JavaScript | Erdtman + Rundgren | 11/11 |
| `gowebpki/jcs v1.0.1` | Go | GoWebPKI | 11/11 |
| `cyberphone/json-canonicalization` | Java | Rundgren (RFC 8785 reference impl) | 11/11 |
| `serde_jcs 0.2.0` | Rust | l1h3r | 11/11 |
| **Total** | **5 languages** | **5 authors** | **55/55 byte-for-byte** |

---

## Per-anchor-set results

### `vectors/stark/` (5 vectors)

| Vector | Expected | rfc8785 (Py) | canonicalize (JS) | gowebpki (Go) | cyberphone (Java) | serde_jcs (Rust) |
|---|---|---|---|---|---|---|
| `0001-baseline` | PASS | OK | OK | OK | OK | OK |
| `0002-field-name-load-bearing` | FAIL (pinned divergent) | OK | OK | OK | OK | OK |
| `0003-stark-proof-tamper-evidence` | FAIL (pinned divergent) | OK | OK | OK | OK | OK |
| `0008-interop-shared-payment-hash` | PASS + interop bind | OK | OK | OK | OK | OK |
| `vector-stark-vauban-pay-v1` | PASS + CBOR cross-runtime | OK | OK | OK | OK | OK |

**Pair invariant 1** : `expected_core_digest` of `0008` equals `0001` byte-for-byte. Confirmed across all 5 implementations.

**CBOR cross-runtime** : `vector-stark-vauban-pay-v1` produces 197 deterministic CBOR bytes per RFC 8949 §4.2.1 across `ciborium 0.2` (Rust) + `cbor2` (Python) + direct hex comparison for JS/Go/Java via the pinned `bound_receipt_cbor_hex` field.

### `vectors/delegation-grant/` (3 vectors)

| Vector | Expected | rfc8785 (Py) | canonicalize (JS) | gowebpki (Go) | cyberphone (Java) | serde_jcs (Rust) |
|---|---|---|---|---|---|---|
| `0001-baseline` | PASS | OK | OK | OK | OK | OK |
| `0002-cap-respect` | PASS | OK | OK | OK | OK | OK |
| `0003-cap-violation` | PASS at canon ; FAIL at verifier | OK | OK | OK | OK | OK |

**Pair invariant 2** : `expected_delegation_grant_hash` identical across vectors `0001`, `0002`, `0003` (same DelegationGrant authorises multiple SettlementReceipts). Hash equals `sha256:ff558f21b26c5045fb6997fef185b9993e33a29a7907dbcf349e49b9019c1fd2`. Confirmed across all 5 implementations.

### `vectors/action_ref_adversarial/` (3 vectors)

| Vector | Expected | rfc8785 (Py) | canonicalize (JS) | gowebpki (Go) | cyberphone (Java) | serde_jcs (Rust) |
|---|---|---|---|---|---|---|
| `0010-duplicate-key` | FAIL (last-wins canonical) | OK | OK | OK | OK | OK |
| `0011-nfd-divergence` | FAIL (NFD distinct from NFC) | OK | OK | OK | OK | OK |
| `0012-required-field-missing` | FAIL (canon valid, schema reject) | OK | OK | OK | OK | OK |

**Notes** :
- Vector `0010` pins the last-wins canonical digest `8680377670f1136998a0002b2b7ef93368c42eca9b68bd334957405c10b7565d`. All 5 implementations apply last-wins by default and reproduce this digest. A strict implementation rejecting duplicate keys at parse time MUST detect the divergence BEFORE reaching canonicalisation ; the pinned digest documents what last-wins parsers produce, not what conforming implementations should accept.
- Vector `0011` uses true NFD bytes `0x65 0xCC 0x81` in the JSON file (not paste-time normalised). Pinned NFD digest `5c77595817951a1725a1fdf5708fd7bb3d82b74a3200ba99e67cf3bfca8e1416` differs from NFC reference digest `24d6bd1693f44c42f69ed395df20f1fbc7c6d933cd1774077909d3a88dea59f7`. RFC 8785 §3.1 explicitly forbids implicit Unicode normalisation ; the schema MUST require NFC.
- Vector `0012` produces a deterministic canonical digest for the partial input `2c0b510091f2b72b11edceb67168da9b2e64f0b15434105d0fa48a2083468703`. JCS canonicalises any syntactically valid JSON regardless of schema completeness ; required-field enforcement MUST fire before canonicalisation.

---

## Reproduction

Each implementation runs the same conformance check :

```
for each vector in vectors/:
    canonical_bytes = jcs_canonicalize(vector.preimage)
    computed_digest = sha256(canonical_bytes).hex()
    if vector.expected_result == "PASS":
        assert computed_digest == vector.expected_core_digest
    elif vector.expected_result == "FAIL" and vector.divergent_digest_pinned:
        assert computed_digest == vector.expected_divergent_digest
    elif vector.expected_result == "FAIL_AT_VERIFIER":
        assert vector_passes_schema_validation(vector.body)
        assert verifier_rejects(vector.body)
```

See `REVIEWER_GUIDE.md` for the three-command reproduction path per language.

---

## Cross-axis interop binding (verified)

| Property | Value | Verified across |
|---|---|---|
| `payment_hash` (shared verbatim) | `2ed186ebc66947eaac6a05a88c7bc096ee07ac11a2c44bb5580bd72b3670f580` | `vectors/stark/` + `vectors/delegation-grant/` |
| `action_ref` (shared verbatim) | `10d8a38c01d8672176aa6e5209a368fde3e1831640d69e15283142b35880c2c1` | `vectors/stark/` + `vectors/delegation-grant/` |
| Cross-runtime CBOR equality | `bound_receipt_cbor_hex` 197 bytes | Rust `ciborium` + Python `cbor2` |

---

## Attestation signature

This attestation is signed by the Vauban Research IETF contact `research@vauban.tech` and archived to the Wayback Machine at `https://web.archive.org/web/*/github.com/vauban-org/x402-stark-receipts-conformance/blob/main/_attestations/2026-05-25-vauban-cross-impl.md` upon publication.

The cryptographic anchor of this attestation : the SHA-256 of this file (lowercase hex) is published in the `manifest.json` `attestations` field at the time of next manifest version bump (v0.3.0).

---

## License

Apache-2.0.
