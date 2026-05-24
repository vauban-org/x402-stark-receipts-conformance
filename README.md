# x402 STARK Receipts Conformance Vectors

Authoritative conformance vector suite for the **x402 STARK Receipt Format Extension**, as defined in IETF Internet-Draft [`draft-vauban-x402-stark-receipts`](https://datatracker.ietf.org/doc/draft-vauban-x402-stark-receipts/).

Authored and maintained by **Vauban Research** (research@vauban.tech).

## Coverage

| Class | Vectors | Schemas | Status |
|---|---|---|---|
| STARK receipt | 5 (baseline + field-name + tamper-evidence + interop + payment-v1) | inline | byte-identical |
| DelegationGrant | 3 (baseline + cap-respect + cap-violation) | yes | byte-identical |
| Total | **8 vectors** | **2 schemas** | **published** |

Future expansion : action_ref namespace, settlement-receipt vectors, refund-claim vectors. Pull requests welcome.

## Independent Cross-Implementation Reference Matrix

Cross-validated byte-identical against five independent RFC 8785 implementations :

| Library | Lang | Upstream author |
|---|---|---|
| [`rfc8785`](https://pypi.org/project/rfc8785/) 0.1.4 | Python | Trail of Bits |
| [`canonicalize`](https://www.npmjs.com/package/canonicalize) 3.0.0 | TypeScript | Erdtman + Rundgren |
| [`gowebpki/jcs`](https://github.com/gowebpki/jcs) 1.0.1 | Go | GoWebPKI |
| [`cyberphone/json-canonicalization`](https://github.com/cyberphone/json-canonicalization) | Java | Rundgren (RFC 8785 reference) |
| [`vauban-x402-jcs-conformance`](https://crates.io/crates/vauban-x402-jcs-conformance) (uses `serde_jcs` 0.2.0) | Rust | Vauban Research (runner) ; l1h3r (`serde_jcs` upstream, Apache-2.0/MIT) |

The Rust 5th-implementation runner is published as the [`vauban-x402-jcs-conformance`](https://crates.io/crates/vauban-x402-jcs-conformance) crate (Apache-2.0). It is the reference implementation used to validate the vectors in this repository against the four other implementations above.

## Canonicalisation Discipline

All vectors derive `JCS_hash = SHA-256(JCS(object))` per RFC 8785, with the additional rules profiled in [`draft-vauban-x402-stark-receipts`](https://datatracker.ietf.org/doc/draft-vauban-x402-stark-receipts/) Section 7 :

- Timestamps encoded as integers (no RFC 3339 strings in `action_ref` preimage)
- Field names compared as byte strings
- Unicode NFC required (NFC and NFD are distinct canonical inputs)
- Type validation MUST occur before canonicalisation (reject floats for integer fields, reject missing canonical fields, reject duplicate keys)

The discipline is anchored in `urn:x402:canonicalisation:jcs-rfc8785-v1`. Implementations conforming to this discipline produce byte-identical canonical bytes for semantically equivalent inputs across language runtimes.

## Provenance

- **IETF Internet-Draft** : [`draft-vauban-x402-stark-receipts`](https://datatracker.ietf.org/doc/draft-vauban-x402-stark-receipts/) (Independent Submission)
- **Author IETF contact** : research@vauban.tech
- **Rust reference runner** : [`crates.io/crates/vauban-x402-jcs-conformance`](https://crates.io/crates/vauban-x402-jcs-conformance)
- **Live demo** : [`demo.pay.vauban.tech`](https://demo.pay.vauban.tech)
- **Documentation** : [`docs.pay.vauban.tech`](https://docs.pay.vauban.tech) (forthcoming)
- **Source repository** : [`github.com/vauban-org/vauban-zkpay`](https://github.com/vauban-org/vauban-zkpay)
- **On-chain settlement anchor** : Starknet Sepolia `0x044dd87a94a801cf775d4c5e4b6703102d4e97e1cd1d0a8879341219ae4f19ff` (verified via [Voyager block explorer](https://sepolia.voyager.online/contract/0x044dd87a94a801cf775d4c5e4b6703102d4e97e1cd1d0a8879341219ae4f19ff))

## Coalition Pull Requests

These vectors and the underlying canonicalisation discipline are referenced in the following x402 coalition Pull Requests :

- [`#2326`](https://github.com/x402-foundation/x402/issues/2326) ; shared canonicalisation discipline section (v3)
- [`#2412`](https://github.com/x402-foundation/x402/pull/2412) ; canonicalisation substrate fixtures (Axis 0)
- [`#2413`](https://github.com/x402-foundation/x402/pull/2413) ; `stark-vauban-pay-v1` baseline fixtures (Axis 1)
- [`#2432`](https://github.com/x402-foundation/x402/pull/2432) ; bounded-spend-authorization-sample (DelegationGrant ; 5-implementation matrix)
- [`#2440`](https://github.com/x402-foundation/x402/pull/2440) ; composite trust-query Axis 4 normative layer

## License

Apache License, Version 2.0. See [LICENSE](LICENSE) for the full text or [www.apache.org/licenses/LICENSE-2.0](https://www.apache.org/licenses/LICENSE-2.0).

## Contributing

Pull requests are welcome for :

- Additional language implementations cross-validating the existing vectors
- New vector classes (action_ref namespace, settlement-receipt, refund-claim)
- Adversarial vectors that expose canonicalisation edge cases

Open an issue or PR at [github.com/vauban-org/x402-stark-receipts-conformance](https://github.com/vauban-org/x402-stark-receipts-conformance).
