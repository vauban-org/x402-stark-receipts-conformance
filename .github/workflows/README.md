# CI Workflows

`conformance.yml` runs on every push and pull request. It executes the three
pure-stdlib runners (PHP 8.2, Ruby 3.2, C# .NET 8) against all vectors in
`vectors/` and asserts a zero-MISMATCH exit code. A green tick on the commit
badge means all 11 vectors passed canonicalisation and digest comparison in
every runtime. A red tick means at least one runner reported a MISMATCH ;
inspect the failing job log to see which vector, expected digest, and computed
digest diverged.
