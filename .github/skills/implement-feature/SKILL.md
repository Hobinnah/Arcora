---
name: implement-feature
description: Implement a production-quality feature from a requirement, issue, or acceptance criteria. Use when asked to add behavior while preserving architecture, compatibility, tests, and operational quality.
argument-hint: "[feature, issue, or acceptance criteria]"
---

# Implement Feature

Implement the requested behavior with the smallest coherent change that fits the existing system.

## Workflow

1. Understand the requirement.
   - Restate the observable behavior and acceptance criteria internally.
   - Identify ambiguous requirements that materially affect correctness; resolve them from repository context when possible.
2. Inspect the existing design.
   - Find the closest analogous feature.
   - Identify extension points, public contracts, validation, logging, configuration, persistence, and tests.
3. Plan before editing.
   - List files likely to change.
   - Identify compatibility, migration, security, and rollout risks.
4. Implement in small increments.
   - Reuse established patterns.
   - Keep responsibilities localized.
   - Avoid unrelated cleanup.
   - Preserve existing public behavior unless the requirement explicitly changes it.
5. Handle production concerns when relevant.
   - validation
   - cancellation/timeouts
   - retries/idempotency
   - authorization
   - concurrency
   - error mapping
   - observability
   - configuration defaults
6. Add or update tests covering:
   - happy path
   - important edge cases
   - failure behavior
   - regression behavior
7. Run the narrowest relevant checks first, then broader tests/build/lint as appropriate.
8. Review the final diff for accidental changes and unnecessary complexity.

## Completion report

Summarize:

- behavior implemented
- important design choices
- files changed
- tests/checks executed and results
- compatibility or rollout considerations
- unresolved assumptions, if any

Never claim a test or command passed unless it was actually run and observed.
