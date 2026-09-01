---
name: security-review
description: Review code or design for security vulnerabilities, trust-boundary mistakes, authorization failures, data exposure, injection, unsafe deserialization, secret handling, and abuse cases. Use for security-focused reviews and threat analysis.
argument-hint: "[code, endpoint, component, or design]"
---

# Security Review

Assume inputs and external systems can be malicious or compromised. Focus on exploitable issues and meaningful defense in depth.

## Workflow

1. Identify assets, actors, entry points, trust boundaries, secrets, and sensitive data.
2. Trace untrusted input from source to security-sensitive sinks.
3. Verify authentication and authorization separately.
   - ensure authorization is enforced server-side at the correct resource boundary
   - check horizontal and vertical privilege escalation
4. Review for relevant vulnerability classes:
   - injection and command execution
   - path traversal
   - SSRF
   - XSS/CSRF where applicable
   - insecure deserialization
   - broken access control
   - sensitive data exposure
   - cryptographic misuse
   - secret leakage
   - unsafe redirects/URLs
   - denial-of-service amplification
   - race conditions/time-of-check-time-of-use
   - dependency/supply-chain risk
5. Inspect logs and errors for sensitive information leakage.
6. Check secure defaults, validation, rate limits, timeouts, resource limits, and failure behavior.
7. For each issue, explain exploit preconditions and impact; avoid theoretical warnings without a credible path.

## Finding format

For each issue include:

- severity
- vulnerable boundary/code path
- attack scenario
- impact
- recommended remediation
- defense-in-depth option, if useful

Do not expose real secrets discovered during review; redact them and identify the location.
