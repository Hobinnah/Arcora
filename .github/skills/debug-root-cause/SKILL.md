---
name: debug-root-cause
description: Diagnose a bug, failing test, exception, incorrect output, or intermittent behavior by finding and proving the root cause before applying a fix. Use for debugging and regression investigation.
argument-hint: "[symptom, failing test, log, or error]"
---

# Debug Root Cause

Optimize for evidence, not guesswork.

## Workflow

1. Capture the symptom precisely.
   - expected behavior
   - actual behavior
   - reproducibility
   - environment or input conditions
2. Reproduce with the smallest reliable case available.
3. Trace the execution path from symptom toward source.
   - inspect logs, exceptions, state transitions, inputs, outputs, and relevant history
   - identify the first point where actual behavior diverges from expected behavior
4. Form hypotheses and rank them by evidence.
5. Test hypotheses with targeted inspection or experiments. Change one meaningful variable at a time.
6. Identify the root cause, not merely the location where the error surfaces.
7. Before fixing, determine whether the defect can affect other callers, data, environments, or versions.
8. Add a regression test that fails for the original defect when practical.
9. Apply the smallest durable fix at the correct abstraction boundary.
10. Re-run the reproduction/regression test, then relevant broader checks.

## Guardrails

- Do not randomly edit code to see what passes.
- Do not suppress exceptions or weaken assertions to hide symptoms.
- Do not add retries without understanding why the operation fails.
- For intermittent issues, explicitly inspect shared mutable state, timing, ordering, race conditions, resource exhaustion, and external dependencies.

## Completion report

Provide:

- symptom
- root cause
- evidence proving the cause
- fix
- regression coverage
- validation performed
- remaining risk or follow-up
