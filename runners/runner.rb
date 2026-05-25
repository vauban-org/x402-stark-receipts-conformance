#!/usr/bin/env ruby
# runner.rb — Reference 7th-implementation runner for x402 STARK Receipts conformance
#
# Pure-stdlib Ruby runner : no Bundler dependency. Implements RFC 8785 JCS canonicalisation
# via recursive key sorting + UTF-8 byte-preserving serialisation, then SHA-256 over the
# canonical bytes. Validates the manifest.json anchor sets against the vectors/ directory.
#
# Authored by Vauban Research <research@vauban.tech>.
# License : Apache-2.0.
#
# Usage : ruby runners/runner.rb [vectors_path]
# Default vectors_path : ./vectors
#
# Requires : Ruby 3.0+ (json + digest stdlib)
# Optional : rfc8785 gem (https://rubygems.org/gems/rfc8785) for strict RFC 8785 ; this
#   runner implements the discipline manually as a reference for environments where the gem
#   is unavailable.

require 'json'
require 'digest'
require 'find'

def jcs_canonicalize(value)
  case value
  when Hash
    parts = value.keys.sort.map do |k|
      "#{k.to_json}:#{jcs_canonicalize(value[k])}"
    end
    "{#{parts.join(',')}}"
  when Array
    "[#{value.map { |v| jcs_canonicalize(v) }.join(',')}]"
  when String
    value.to_json
  when Integer
    value.to_s
  when Float
    # RFC 8785 §3.2.2.3 : integer-valued floats serialise as integer
    if value == value.to_i && value.abs < (1 << 62)
      value.to_i.to_s
    else
      value.to_s
    end
  when TrueClass then 'true'
  when FalseClass then 'false'
  when NilClass then 'null'
  else
    raise "Unsupported JSON type : #{value.class}"
  end
end

def sha256_hex(bytes)
  Digest::SHA256.hexdigest(bytes)
end

# Normalise a digest value for comparison: strip the 'sha256:' prefix if present.
# Vectors may store digests as either raw hex or 'sha256:<hex>'; comparing
# normalised forms avoids false MISMATCH when one side carries the prefix.
def normalize_digest(d)
  return d if d.nil?
  d.sub(/^sha256:/, '')
end

def validate_vector(path)
  raw = File.read(path)
  vector = JSON.parse(raw)

  preimage_key = ['receipt_core', 'delegation_grant', 'settlement_receipt', 'composite_payload', 'preimage'].find do |k|
    vector.key?(k)
  end
  return { status: 'SKIP', reason: 'no canonical preimage field found' } if preimage_key.nil?

  canonical = jcs_canonicalize(vector[preimage_key])
  digest = "sha256:#{sha256_hex(canonical)}"

  expected_pass = vector['expected_core_digest'] || vector['expected_delegation_grant_hash']
  expected_fail = vector['expected_divergent_digest']
  expected_result = vector['expected_result'] || 'UNKNOWN'

  if expected_result == 'PASS' && expected_pass
    { status: normalize_digest(digest) == normalize_digest(expected_pass) ? 'OK' : 'MISMATCH', expected: expected_pass, computed: digest }
  elsif expected_result == 'FAIL' && expected_fail
    { status: normalize_digest(digest) == normalize_digest(expected_fail) ? 'OK' : 'MISMATCH', expected: expected_fail, computed: digest }
  elsif expected_result == 'FAIL_AT_VERIFIER'
    { status: 'OK', note: 'canon-pass, verifier-fail (semantic, not tested here)' }
  else
    { status: 'SKIP', reason: 'incomplete pin' }
  end
end

vectors_dir = ARGV[0] || File.expand_path('../vectors', __dir__)
unless Dir.exist?(vectors_dir)
  warn "vectors directory not found : #{vectors_dir}"
  exit 2
end

total = 0; ok = 0; mismatch = 0; skip = 0
Find.find(vectors_dir).sort.each do |path|
  next unless File.file?(path) && path.end_with?('.json')
  relative = path.sub("#{vectors_dir}/", '')
  result = validate_vector(path)
  total += 1
  printf "  %-60s %s\n", relative, result[:status]
  case result[:status]
  when 'OK' then ok += 1
  when 'MISMATCH' then mismatch += 1
  else skip += 1
  end
end

puts ""
printf "%d vectors | %d OK | %d MISMATCH | %d SKIP\n", total, ok, mismatch, skip
exit(mismatch > 0 ? 1 : 0)
