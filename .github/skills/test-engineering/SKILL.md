---
name: test-engineering
description: Design, add, repair, or evaluate automated tests for production code. Use when asked for unit, integration, contract, regression, property, or end-to-end testing, or when assessing test quality and coverage.
argument-hint: "[code, behavior, or test goal]"
---

# Test Engineering

Write tests that protect behavior and remain useful during refactoring.

## Strategy

1. Identify the risk being tested, not merely the method being called.
2. Choose the lowest-cost test level that can prove the behavior:
   - unit for isolated policy/logic
   - integration for boundaries such as database, filesystem, network, serialization, DI, or framework behavior
   - contract for producer/consumer compatibility
   - end-to-end only for critical cross-system journeys
3. Test observable outcomes rather than private implementation details.
4. Cover the important set, not every permutation:
   - representative happy path
   - boundaries
   - invalid input
   - failure/exception behavior
   - concurrency/idempotency when relevant
   - regression for known defects
5. Keep tests deterministic.
   - avoid real wall-clock time where controllable
   - isolate random values with seeds or deterministic inputs
   - avoid order dependence and shared mutable fixtures
   - do not use arbitrary sleeps for synchronization
6. Make failures diagnostic with clear assertions and focused test names.
7. Prefer realistic collaborators at important boundaries; mock only where isolation provides value.
8. Avoid asserting incidental details such as exact internal call counts unless they are part of the contract.
9. Run the new test independently, then the relevant suite.

## When fixing tests

Determine first whether production behavior changed incorrectly, the test expectation is obsolete, or the test is flaky. Never weaken a valid assertion solely to make CI green.
