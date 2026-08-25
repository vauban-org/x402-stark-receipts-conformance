# Make us wrong

Vauban Pay makes a claim on one testnet payment, and publishes everything you
need to break it. There is no signup, no code of ours to run, and no endpoint of
ours to trust. If you break it, we say so in public, here.

## The claim

On 2026-08-16 we made one shielded settlement on Starknet Sepolia. A privacy
pool has two legs: a **deposit**, public by construction (an ERC-20 transfer is
visible to everyone), and a **transferred amount**, which moved inside the pool
and should appear nowhere on the chain.

- Pool: `0x07889af20edd41ed80c7e5a4163384b7428b6a980836de5a1f4fa3e78b09e1f0`
- Transaction: `0x40a9089446efdabff4bfa882693b0b22071c7b03e3e00ef23359ac6ab3d8d69`
- Block: `13575128`
- Deposit, public: `987654321000000` wei (`0x3824430f6ce40`)
- Transferred, hidden: `123456789000000` wei (`0x7048860daf40`)

We are telling you the hidden number, because knowing it is not the point. Our
claim is this:

> **`0x7048860daf40` appears in none of what the pool published — not in that
> transaction, not in its receipt, not in any event the pool has emitted since
> it was deployed.**

## Challenge 1 — find the hidden amount on-chain

Point the scan at any Starknet Sepolia RPC you trust, including one you run
yourself. Scan every event the pool emitted from its deployment block to the
head of the chain — data AND keys. If the felt `0x7048860daf40` shows up
anywhere, our claim is false and you have won.

```bash
POOL=0x07889af20edd41ed80c7e5a4163384b7428b6a980836de5a1f4fa3e78b09e1f0
RPC=${RPC:-https://starknet-sepolia-rpc.publicnode.com}

curl -sS -X POST -H 'content-type: application/json' "$RPC" \
  -d '{"jsonrpc":"2.0","id":1,"method":"starknet_getEvents","params":[{
       "from_block":{"block_number":13457017},"to_block":"latest",
       "address":"'"$POOL"'","chunk_size":1000}]}' \
| jq '[.result.events[] | (.data[], .keys[])]
      | {values: length,
         deposit_public:  (map(select(. == "0x3824430f6ce40")) | length),
         amount_we_hid:   (map(select(. == "0x7048860daf40")) | length)}'
```

The claim is one number: **`amount_we_hid: 0`**. The others grow with pool
activity — as of 2026-08-24 the full history is `values: 126`,
`deposit_public: 6` (the pool has seen six test deposits of that amount).

One honesty note on the scan itself: `starknet_getEvents` pages, and nodes
page differently. If `.result.continuation_token` is non-null in the raw
response, loop with that token until it is null before trusting a zero — the
default endpoint above returns this whole history in one page today, but
other nodes page by block range and give you a partial count if you stop at
page one. A zero only counts on the complete history. Here is the loop, so
"scan everything" is a command and not homework:

```bash
POOL=0x07889af20edd41ed80c7e5a4163384b7428b6a980836de5a1f4fa3e78b09e1f0
RPC=${RPC:-https://starknet-sepolia-rpc.publicnode.com}

token="" all='[]'
while :; do
  filter='{"from_block":{"block_number":13457017},"to_block":"latest","address":"'"$POOL"'","chunk_size":1000}'
  [ -n "$token" ] && filter=$(jq -c --arg t "$token" '. + {continuation_token:$t}' <<<"$filter")
  resp=$(curl -sS -X POST -H 'content-type: application/json' "$RPC" \
    -d '{"jsonrpc":"2.0","id":1,"method":"starknet_getEvents","params":['"$filter"']}')
  all=$(jq -c --argjson page "$(jq -c '[.result.events[] | (.data[], .keys[])]' <<<"$resp")" '. + $page' <<<"$all")
  token=$(jq -r '.result.continuation_token // empty' <<<"$resp")
  [ -z "$token" ] && break
done

jq '{values: length,
     deposit_public: (map(select(. == "0x3824430f6ce40")) | length),
     amount_we_hid:  (map(select(. == "0x7048860daf40")) | length)}' <<<"$all"
```

We ran this loop against two nodes that page differently (one page vs seven);
both return the same complete history: `values: 126, deposit_public: 6,
amount_we_hid: 0` as of 2026-08-24. If you get anything other than `0` for
`amount_we_hid` on the full scan, open an issue with the RPC you used and the
raw responses.

## Challenge 2 — get a wrong receipt accepted

Our STARK-receipt verifier is a public container image, and it ships a real
demo proof and receipt inside it — nothing of ours you have to trust beyond a
reproducible build. First, watch it accept the genuine pair:

```bash
IMG=ghcr.io/vauban-org/zkpay-stark-verify:0.1.1
docker run --rm $IMG \
  --proof /demo/payment_proof.json \
  --receipt /demo/canonical_receipt.json
# {"proof_hash":"5337c1a5…","status":"Valid","verify_ms":42}
```

Now break it. Copy the receipt out, change one field — the amount, the
nullifier, the subject, one byte — and get the verifier to still return
`Valid`:

```bash
# extract the genuine receipt
docker run --rm --entrypoint cat $IMG /demo/canonical_receipt.json > receipt.json
# edit receipt.json (e.g. bump one hex digit of "amount"), then feed it back:
docker run --rm -v "$PWD/receipt.json:/tmp/r.json:ro" $IMG \
  --proof /demo/payment_proof.json --receipt /tmp/r.json
```

We claim you get `Invalid` every time, with the reason spelled out — the proof
attests one receipt hash and your file hashes to another:

```
{"status":"Invalid","reason":"receipt binding failed (proof attests a
 different receipt): proof receipt_hash=0x03db2c16…, caller receipt_hash=…"}
```

`Valid` on a receipt we did not prove is a win.

## Rules

- One issue per attempt, in `vauban-org/x402-stark-receipts-conformance`, titled
  `challenge:` and the number.
- Include exactly what you ran and the raw output. A win has to reproduce on our
  side from your issue alone — which is the same bar we hold ourselves to.
- Wins are listed below, with attribution as you ask for it (handle, name, or
  anonymous). No bounty, no NDA, no catch. The reward is that we were wrong in
  public and you are the reason it is fixed.

## Hall of fame

_Empty. That is either because the claim holds, or because you have not tried
yet. We would genuinely rather it be the second._

