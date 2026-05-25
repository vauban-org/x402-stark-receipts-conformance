# x402 STARK Receipts Conformance Vectors

Authoritative conformance vector suite for the **x402 STARK Receipt Format Extension** and the **Vauban Pay** Internet-Draft family.

Maintained by **Vauban Research** (`research@vauban.tech`).

---

## Coverage

- **11 vectors** across 3 anchor sets : STARK receipt canonicalisation baseline + interop + cross-runtime CBOR, bounded-spend authorization (DelegationGrant + SettlementReceipt pair), and action-ref adversarial follow-on
- **2 JSON Schema** definitions (Draft 2020-12) for `DelegationGrant` and `SettlementReceipt` (six-element Claim sextuplet shape)
- **Cross-axis interop binding** : every vector shares the canonical `payment_hash = 2ed186eb...0f580` and `action_ref = 10d8a38c...0c2c1` to demonstrate that the STARK receipt format is independently verifiable across HTTP signature, work-receipt, and on-chain layers

| Anchor set | Vectors | Pair invariants | Status |
|---|---|---|---|
| `vectors/stark/` | 5 | 1 (interop) | byte-deterministic under RFC 8785 JCS |
| `vectors/delegation-grant/` | 3 | 1 (delegation_grant_hash across vectors) | byte-deterministic |
| `vectors/action_ref_adversarial/` | 3 | 0 | FAIL vectors pin actual divergent digests |
| **Total** | **11** | **2** | **all byte-for-byte** |

---

## IETF Internet-Draft contribution record

| Draft | Status | Submitted |
|---|---|---|
| [draft-vauban-x402-stark-receipts-01](https://datatracker.ietf.org/doc/draft-vauban-x402-stark-receipts/) | Active | 2026-05-24 |
| [draft-vauban-x402-lifecycle-fsm-00](https://datatracker.ietf.org/doc/draft-vauban-x402-lifecycle-fsm/) | Active | 2026-05-24 |
| [draft-vauban-x402-vpsf-algebra-00](https://datatracker.ietf.org/doc/draft-vauban-x402-vpsf-algebra/) | Active | 2026-05-24 |
| [draft-vauban-x402-starknet-anchor-00](https://datatracker.ietf.org/doc/draft-vauban-x402-starknet-anchor/) | Active | 2026-05-24 |
| [draft-vauban-x402-delegation-binding-00](https://datatracker.ietf.org/doc/draft-vauban-x402-delegation-binding/) | Active | 2026-05-24 |
| [draft-vauban-x402-pqc-receipts-00](https://datatracker.ietf.org/doc/draft-vauban-x402-pqc-receipts/) | Active | 2026-05-24 |

All drafts : Independent Submission (ISE), Informational, sole author Vauban Research.

---

## Reference implementations

The vector set is published as an **8-implementation reference matrix** across 8 languages. **5 implementations are validated byte-for-byte** via execution against upstream JCS RFC 8785 libraries ; **3 implementations** are committed as pure-stdlib reference runners under `runners/` for CI execution. See [`_attestations/2026-05-25-vauban-8-impl-extended.md`](_attestations/2026-05-25-vauban-8-impl-extended.md) for honest validation status per implementation.

| # | Library / runner | Language | Status |
|---|---|---|---|
| 1 | `rfc8785@0.1.4` (Trail of Bits) | Python | validated 11/11 |
| 2 | `canonicalize@3.0.0` (Erdtman + Rundgren) | JavaScript | validated 11/11 |
| 3 | `gowebpki/jcs v1.0.1` | Go | validated 11/11 |
| 4 | `cyberphone/json-canonicalization` (Rundgren reference) | Java | validated 11/11 |
| 5 | `serde_jcs 0.2.0` (l1h3r) consumed via `vauban-x402-jcs-conformance` | Rust | validated 11/11 |
| 6 | [`runners/runner.php`](runners/runner.php) (pure stdlib) | PHP 8.0+ | reference, CI-pending |
| 7 | [`runners/runner.rb`](runners/runner.rb) (pure stdlib) | Ruby 3.0+ | reference, CI-pending |
| 8 | [`runners/runner.cs`](runners/runner.cs) (pure stdlib) | C# / .NET 8 | reference, CI-pending |

### Vauban Pay published packages

| Registry | Package | Version |
|---|---|---|
| crates.io | [`vauban-x402-jcs-conformance`](https://crates.io/crates/vauban-x402-jcs-conformance) | 0.1.0 |
| crates.io | [`vauban-x402-canonical`](https://crates.io/crates/vauban-x402-canonical) | 0.1.0 |
| crates.io | [`vauban-x402-wire`](https://crates.io/crates/vauban-x402-wire) | 0.1.0 |
| PyPI | [`vauban-x402-stark-receipt`](https://pypi.org/project/vauban-x402-stark-receipt/) | 0.1.0 |
| npm | [`@vauban-pay/substrate`](https://www.npmjs.com/package/@vauban-pay/substrate) | 0.1.0 |

---

## Provenance and on-chain anchor

- **IETF contact** : `research@vauban.tech`
- **Live demo** : https://demo.pay.vauban.tech
- **Documentation** : https://vauban.tech/vauban-zkpay/
- **On-chain anchor** : `PaymentDemoEmitter` deployed on Starknet Sepolia at [`0x044dd87a94a801cf775d4c5e4b6703102d4e97e1cd1d0a8879341219ae4f19ff`](https://sepolia.voyager.online/contract/0x044dd87a94a801cf775d4c5e4b6703102d4e97e1cd1d0a8879341219ae4f19ff) (Voyager-verified)
- **Starknet RPC** : self-hosted Pathfinder at `https://sepolia.rpc.vauban.tech/rpc/v0_10`

---

## Layout

```
x402-stark-receipts-conformance/
├── README.md                   (this file)
├── LICENSE                     (Apache-2.0)
├── REVIEWER_GUIDE.md           (3-command reproduction path)
├── manifest.json               (SHA-256 per vector + anchor set summary)
├── schemas/
│   ├── delegation-grant.schema.json     (DelegationGrant six-element Claim, Draft 2020-12)
│   └── settlement-receipt.schema.json   (SettlementReceipt six-element Claim, Draft 2020-12)
├── vectors/
│   ├── stark/                           (5 STARK receipt canonicalisation vectors)
│   ├── delegation-grant/                (3 DelegationGrant + SettlementReceipt pair vectors)
│   └── action_ref_adversarial/          (3 FAIL vectors with pinned divergent digests)
└── _attestations/
    └── 2026-05-25-vauban-cross-impl.md  (5-implementation byte-for-byte attestation)
```

---

## Canonicalisation discipline

JSON Canonicalization Scheme per **RFC 8785** (Erdtman + Rundgren 2020). SHA-256 over the canonical bytes, lowercase hex, prefixed `sha256:`.

- **Producer-loud, verifier-silent** : producers reject non-canonical inputs at issuance ; verifiers accept any equivalent canonical input
- **`canon_version`** field declared at the top level of both `DelegationGrant` and `SettlementReceipt`
- **`1.0` form** accepted (matches `stark-vauban-pay-v1` baseline) ; `x402-jcs-v1.0.0` form also accepted
- **Year-N retention property** : canonical rule produces identical bytes across observer-and-time, so an auditor in year 6 reproduces the digest against a retained off-VM manifest (load-bearing for EU AI Act Art. 12, MiCA Art. 76, AMLR Art. 56, DORA Art. 14)

---

## Cross-axis interop binding

All `vectors/stark/` and `vectors/delegation-grant/` vectors share two canonical values verbatim :

- **`payment_hash`** : `2ed186ebc66947eaac6a05a88c7bc096ee07ac11a2c44bb5580bd72b3670f580`
- **`action_ref`** : `10d8a38c01d8672176aa6e5209a368fde3e1831640d69e15283142b35880c2c1`

The binding is a pure hash reference ; each layer (STARK receipt, HTTP work-receipt, on-chain settlement) is independently verifiable. Mutating either canonical value invalidates verification on every layer simultaneously, demonstrating that the cross-axis binding is cryptographically enforced not policy-enforced.

---

## Reproducing the digests

See [REVIEWER_GUIDE.md](REVIEWER_GUIDE.md) for the 3-command reproduction path.

Short form (Rust) :

```bash
cargo install vauban-x402-jcs-conformance
vauban-x402-jcs-conformance validate vectors/
# Expected : 11/11 vectors verify byte-for-byte
```

Short form (Python) :

```bash
pip install vauban-x402-stark-receipt
python -m vauban_x402_stark_receipt validate vectors/
```

Short form (JavaScript) :

```bash
npm install @vauban-pay/substrate
node -e "require('@vauban-pay/substrate').validateDirectory('vectors/')"
```

---

## License

Apache-2.0. Per ADR-ECO-025 option B (Vauban Pay license decision) : Apache 2.0 for Rust crates + Cairo contracts + reference fixtures ; closed Vauban Facilitator service.

This fixture set is fully open ; reproduction, derivation, and integration into conforming x402 implementations are unrestricted under Apache 2.0 §2 + §4 attribution.

---

## Cross-references

- IETF I-D family : `draft-vauban-x402-*` (6 active drafts on datatracker.ietf.org)
- x402 V2 wire format : Linux Foundation x402 working group
- Stwo Circle STARK M31 prover : StarkWare 2024 (FRI-based, post-quantum sound)
- VPSF Claim Algebra : chain-agnostic invariant, Starknet-first reference implementation
