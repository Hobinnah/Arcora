---
name: library-api-evolution
description: Evolve a shared library, framework, SDK, package, or platform API while minimizing consumer breakage. Use for public surface changes, deprecations, package releases, shared abstractions, and backward-compatibility reviews.
argument-hint: "[library change, API, or proposed release]"
---

# Library API Evolution

Optimize for consumer safety, predictable migration, and long-term maintainability.

## Workflow

1. Identify the public surface affected:
   - types and members
   - constructors
   - interfaces/abstract classes
   - attributes/annotations
   - configuration keys/defaults
   - serialization contracts
   - extension points
   - behavioral guarantees
2. Search representative consumers and tests to learn how the API is actually used.
3. Classify the change:
   - additive and compatible
   - source-breaking
   - binary-breaking
   - behavior-breaking
   - data/wire-format-breaking
4. Prefer additive evolution.
   - new overloads/options
   - default implementations where safe
   - adapters
   - obsolete/deprecation path
5. Avoid widening abstractions based on a single consumer. Keep the shared API focused on stable common concepts.
6. Check semantic compatibility, not only compilation.
   - defaults
   - exception behavior
   - ordering
   - threading
   - performance characteristics
   - lifecycle/disposal
7. Provide a migration path for unavoidable breaking changes, including before/after examples.
8. Add compatibility/regression tests and release notes for consumer-visible changes.
9. Consider versioning and rollout sequencing across producers/consumers.

## Completion report

State compatibility classification, affected consumers, migration approach, tests, versioning recommendation, and residual risks.
