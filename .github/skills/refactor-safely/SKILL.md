---
name: refactor-safely
description: Improve structure, readability, modularity, or technical debt without changing externally observable behavior. Use for refactoring, cleanup, decomposition, or modernization where behavior preservation matters.
argument-hint: "[code area or refactoring goal]"
---

# Refactor Safely

Treat behavior preservation as the primary requirement.

## Workflow

1. Define the refactoring boundary and the behavior that must remain unchanged.
2. Inspect callers, tests, public contracts, serialization, persistence, reflection, configuration, and dependency injection registrations that may depend on current structure.
3. Establish a safety net.
   - run existing relevant tests
   - add characterization tests when behavior is important but poorly covered
4. Make small mechanical changes before semantic restructuring.
5. Keep commits/edits conceptually isolated when possible so failures are easy to localize.
6. Preserve public signatures, exceptions, ordering, side effects, defaults, and wire formats unless explicitly authorized to change them.
7. Reduce complexity only when the resulting abstraction has a clear responsibility and improves changeability.
8. Avoid creating generic abstractions without multiple real use cases.
9. Run focused tests after each meaningful step, then broader verification.
10. Compare the final behavior and diff against the original goal; remove unrelated changes.

## Good refactoring signals

Prefer changes that improve one or more of:

- coupling/cohesion
- naming and intent
- duplication with a stable shared concept
- dependency direction
- testability
- error handling clarity
- separation of policy from mechanism

Do not turn a local cleanup into an architecture rewrite unless requested.
