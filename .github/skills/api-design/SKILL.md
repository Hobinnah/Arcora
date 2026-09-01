---
name: api-design
description: Design or review public APIs, service endpoints, SDK surfaces, interfaces, messages, or contracts for clarity, consistency, evolvability, validation, errors, versioning, and consumer usability.
argument-hint: "[API, endpoint, interface, or contract]"
---

# API Design

Design from the consumer's perspective and treat compatibility as a first-class constraint.

## Workflow

1. Identify consumers, use cases, frequency, scale, and expected evolution.
2. Define the behavioral contract before implementation details.
3. Keep the surface small and cohesive.
4. Prefer names and types that make invalid usage difficult.
5. Define explicitly:
   - required vs optional inputs
   - defaults
   - validation and limits
   - error semantics
   - idempotency
   - cancellation/timeouts where applicable
   - ordering/pagination
   - concurrency semantics
   - serialization/wire format
6. Review compatibility risks:
   - signature changes
   - enum expansion
   - new required fields
   - changed defaults
   - exception/error changes
   - ordering changes
   - nullability changes
7. For network APIs, consider retries, rate limits, idempotency keys, status/error mapping, and partial failure.
8. Provide examples for common and failure cases.
9. Prefer additive evolution and deprecation over breaking changes.

## Output

Include proposed contract, rationale, examples, compatibility analysis, and alternatives for contentious decisions.
