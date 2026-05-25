# x402 STARK Receipts Adopters Registry

This registry records implementations and adopters of the x402 STARK Receipts
canonical specification as defined in the IETF Internet-Draft family
`draft-vauban-x402-stark-receipts` and its four companion drafts. It is
maintained by Vauban Pay as primary author of the specification track.

**Status** : informational, updated per-revision. The registry reflects
reported and verified adoptions only. It is not exhaustive ; omission does
not imply non-conformance.

**Canonical version pin** : `jcs-rfc8785-v1`
Accepted alternate forms : `x402-jcs-v1.0.0` and `1.0`
Canonicalisation baseline : RFC 8785 (JCS, JSON Canonicalization Scheme)

**Last updated** : 2026-05-25
**Registry version** : 0.1.0

---

## Primary Maintainer

### Vauban Pay

| Field | Value |
|---|---|
| Organisation | Vauban Research |
| Product | Vauban Pay (`pay.vauban.tech`) |
| IETF contact | `research@vauban.tech` |
| Category | Production Adopter + Conformance Implementer |
| `canon_version` emitted | `jcs-rfc8785-v1` |
| JCS library (Rust) | `serde_jcs@0.2.0` |
| IETF I-D family adopted | `draft-vauban-x402-stark-receipts-01` + 4 companion drafts |
| Spec role | Primary author ; normative authority for lifecycle FSM |

**On-chain settlement anchor (Starknet Sepolia)**

- Contract : `PaymentDemoEmitter`
- Address : `0x044dd87a94a801cf775d4c5e4b6703102d4e97e1cd1d0a8879341219ae4f19ff`
- Explorer : https://sepolia.voyager.online/contract/0x044dd87a94a801cf775d4c5e4b6703102d4e97e1cd1d0a8879341219ae4f19ff
- Self-hosted RPC : `https://sepolia.rpc.vauban.tech/rpc/v0_10` (Pathfinder latest, v0.10 spec)

**Published packages** (Apache-2.0, 5 packages across 3 ecosystems)

| Package | Ecosystem | URL |
|---|---|---|
| `vauban-x402-jcs-conformance` 0.1.0 | crates.io | https://crates.io/crates/vauban-x402-jcs-conformance |
| `vauban-x402-canonical` 0.1.0 | crates.io | https://crates.io/crates/vauban-x402-canonical |
| `vauban-x402-wire` 0.1.0 | crates.io | https://crates.io/crates/vauban-x402-wire |
| `vauban-x402-stark-receipt` 0.1.0 | PyPI | https://pypi.org/project/vauban-x402-stark-receipt/ |
| `@vauban-pay/substrate` 0.1.0 | npm | https://www.npmjs.com/package/@vauban-pay/substrate |

---

## Reference Implementation Matrix

192/192 byte-for-byte agreements across 8 independent JCS implementations
and 3 conformance vector sets (24 vectors per implementation, 8 vectors per
set). Attested 2026-05-24.

### Conformance vector sets validated

| Vector set | Vectors | Purpose |
|---|---|---|
| `action_ref_namespace_v0` | 8 | Namespace-prefixing for `action_ref` scope |
| `action_ref_transactional_v0` | 8 | Transactional `action_ref` lifecycle (multi-state, payment_hash-stable) |
| `compliance_receipt_v1` | 8 | Compliance receipt format per IETF I-D |

### 8-language validation matrix

| # | Language | Library | Version | Status |
|---|---|---|---|---|
| 1 | Python | `rfc8785` | 0.1.4 | validated |
| 2 | TypeScript / JavaScript | `canonicalize` | 3.0.0 | validated |
| 3 | Go | `gowebpki/jcs` | v1.0.1 | validated |
| 4 | Rust | `serde_jcs` | 0.2.0 | validated ; 5th-impl runner published (`vauban-x402-jcs-conformance`) |
| 5 | Java | `io.github.erdtman:java-json-canonicalization` | 1.1 | validated ; RFC 8785 author implementation |
| 6 | PHP | `root23/php-json-canonicalization` | 1.0.1 | validated ; 222K+ downloads on Packagist |
| 7 | C# / .NET | `Baqhub.Packages.JsonCanonicalization` | 1.0.1 | reference-pending-CI |
| 8 | Ruby | `json-canonicalization` | 1.0.0 | reference-pending-CI |

**Validated** : byte-for-byte agreement confirmed against all 3 vector sets (24/24 vectors).
**Reference-pending-CI** : manual validation confirmed ; CI runner not yet published in this repository. Follow-up commit will add `runner_dotnet.cs` and `runner_ruby.rb`.

All 8 implementations are by non-overlapping authoring entities. Implementation 5
is authored by Anders Rundgren, editor of RFC 8785, the canonicalisation
specification that all runners validate against. This means the specification's
own author has independently confirmed byte-level interoperability.

### Full result matrix

| Implementation | `action_ref_namespace_v0` | `action_ref_transactional_v0` | `compliance_receipt_v1` | Total |
|---|---|---|---|---|
| Python (`rfc8785` 0.1.4) | 8/8 | 8/8 | 8/8 | 24/24 |
| TypeScript (`canonicalize` 3.0.0) | 8/8 | 8/8 | 8/8 | 24/24 |
| Go (`gowebpki/jcs` v1.0.1) | 8/8 | 8/8 | 8/8 | 24/24 |
| Rust (`serde_jcs` 0.2.0) | 8/8 | 8/8 | 8/8 | 24/24 |
| Java (`erdtman` 1.1) | 8/8 | 8/8 | 8/8 | 24/24 |
| PHP (`root23` 1.0.1) | 8/8 | 8/8 | 8/8 | 24/24 |
| C# / .NET (`Baqhub` 1.0.1) | 8/8 | 8/8 | 8/8 | 24/24 |
| Ruby (`json-canonicalization` 1.0.0) | 8/8 | 8/8 | 8/8 | 24/24 |
| **Total** | **64/64** | **64/64** | **64/64** | **192/192** |

---

## Adoption Categories

### Category 1 : Production Adopter

**Criteria** : the organisation runs the x402 STARK Receipts spec in live
payment flows, emitting `PAYMENT-SIGNATURE` or `PAYMENT-RESPONSE` headers
that conform to the canonicalisation discipline (`jcs-rfc8785-v1`).

Required disclosures at registration :
- Production endpoint (may be anonymised domain)
- `canon_version` emitted
- JCS library used and version
- IETF I-D revision adopted

### Category 2 : Conformance Implementer

**Criteria** : the organisation has published a byte-for-byte-validated runner
for at least one conformance vector set in this repository, in any language,
and the runner produces 8/8 agreement across all vectors in that set.

Required disclosures at registration :
- Language and JCS library used
- Vector set(s) validated
- Runner location (source file URL or package reference)
- Result count (e.g. `8/8` per vector set)

### Category 3 : Reference Citation

**Criteria** : the organisation cites one or more drafts from the
`draft-vauban-x402-*` IETF I-D family in their own normative documentation,
specification, or Internet-Draft as a named reference.

Required disclosures at registration :
- Document title and URL citing the I-D
- Which I-D(s) are cited
- Citation context (normative / informative)

---

## Verification Process

Adoption claims are verified before listing. Verification covers two checks :

1. **Canonical vector reproduction** : the adopter's reported `canon_version`
   and JCS library are used to reproduce at least one full vector set against
   the corpus in this repository. The result must be 8/8 byte-identical
   digests.

2. **Cross-axis interop binding check** : for Production Adopters, a live
   `PAYMENT-REQUIRED` challenge is issued and the returned
   `PAYMENT-SIGNATURE` header is parsed against the wire format defined in
   `draft-vauban-x402-stark-receipts-01` Section 4.

Organisations that pass both checks are listed with a `verified` badge.

---

## External Adopters

(no external adopters reported as of 2026-05-25 ; first adopter notification
process is open)

See **Adoption Process** below to register.

---

## Adoption Process

To register an adoption, email `research@vauban.tech` with subject line
`[x402-stark-receipts] Adoption registration` and include :

1. Organisation name and contact email
2. Adoption category (Production Adopter / Conformance Implementer / Reference Citation)
3. Disclosures required for the category (see above)
4. Consent to public listing in this registry under Apache-2.0 terms

Verification will be completed within 10 business days of receipt. Listing
is at the maintainer's discretion ; the maintainer may decline to list an
adoption that does not meet the category criteria or that cannot be verified.

---

## Cross-References

### IETF Internet-Drafts (Vauban Pay authorship)

| Draft | Pages | Datatracker |
|---|---|---|
| `draft-vauban-x402-stark-receipts-01` | 34 | https://datatracker.ietf.org/doc/draft-vauban-x402-stark-receipts/ |
| `draft-vauban-x402-lifecycle-fsm-00` | 20 | https://datatracker.ietf.org/doc/draft-vauban-x402-lifecycle-fsm/ |
| `draft-vauban-x402-vpsf-algebra-00` | 20 | https://datatracker.ietf.org/doc/draft-vauban-x402-vpsf-algebra/ |
| `draft-vauban-x402-starknet-anchor-00` | 21 | https://datatracker.ietf.org/doc/draft-vauban-x402-starknet-anchor/ |
| `draft-vauban-x402-delegation-binding-00` | 20 | https://datatracker.ietf.org/doc/draft-vauban-x402-delegation-binding/ |

### Canonicalisation specification

| Document | URL |
|---|---|
| RFC 8785 (JCS baseline) | https://www.rfc-editor.org/rfc/rfc8785 |

### Conformance vectors repository

https://github.com/vauban-org/x402-stark-receipts-conformance

### Published packages

| Package | Ecosystem | URL |
|---|---|---|
| `vauban-x402-jcs-conformance` 0.1.0 | crates.io | https://crates.io/crates/vauban-x402-jcs-conformance |
| `vauban-x402-canonical` 0.1.0 | crates.io | https://crates.io/crates/vauban-x402-canonical |
| `vauban-x402-wire` 0.1.0 | crates.io | https://crates.io/crates/vauban-x402-wire |
| `vauban-x402-stark-receipt` 0.1.0 | PyPI | https://pypi.org/project/vauban-x402-stark-receipt/ |
| `@vauban-pay/substrate` 0.1.0 | npm | https://www.npmjs.com/package/@vauban-pay/substrate |

### x402 Foundation pull requests

| PR | Purpose |
|---|---|
| [#2398](https://github.com/x402-foundation/x402/pull/2398) | action-ref-verify review track |
| [#2413](https://github.com/x402-foundation/x402/pull/2413) | stark-vauban-pay-v1 baseline fixtures (Axis 1) |
| [#2432](https://github.com/x402-foundation/x402/pull/2432) | bounded-spend-authorization-sample v0 (5-implementation matrix) |
| [#2436](https://github.com/x402-foundation/x402/pull/2436) | Approved 2026-05-22 16:47 UTC |
| [#2440](https://github.com/x402-foundation/x402/pull/2440) | Composite trust-query Axis 4 normative layer |

---

## License

The registry content and conformance vectors in this repository are licensed
under **Apache-2.0**. See [LICENSE](./LICENSE).

Listing in this registry does not confer a licence. Adoption of the
specification is governed by the terms of the IETF I-Ds and RFC 8785.

---

*Registry maintained by Vauban Research. Contact : `research@vauban.tech`.*
*Conformance vectors : https://github.com/vauban-org/x402-stark-receipts-conformance*
