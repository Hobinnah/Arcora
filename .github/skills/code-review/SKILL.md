---
name: code-review
description: Perform a high-signal engineering review of a change, pull request, diff, or set of files. Use when asked to review code for correctness, maintainability, security, performance, tests, concurrency, compatibility, or production risk.
argument-hint: "[PR, diff, files, or review focus]"
---

# Code Review

Act as a senior reviewer protecting correctness, operability, and long-term maintainability.

## Review process

1. Establish intent before judging implementation.
   - Read the task, PR description, nearby code, tests, and relevant contracts.
   - Identify behavior that must remain unchanged and behavior intentionally changed.
2. Review the smallest meaningful diff first, then inspect surrounding call sites and dependencies when needed.
3. Prioritize findings in this order:
   - correctness and data integrity
   - security and authorization
   - concurrency and failure handling
   - API or behavioral compatibility
   - performance and resource usage
   - test quality and observability
   - maintainability and clarity
4. For every finding, verify that it is actionable and supported by evidence from the code. Do not report speculative style preferences as defects.
5. Consider edge cases: null/empty input, boundaries, retries, cancellation, partial failure, timeouts, duplicate execution, race conditions, ordering, timezone/culture, overflow, serialization, and backward compatibility when relevant.
6. Check tests for behavior rather than implementation details. Look for missing negative, boundary, failure, and regression cases.
7. Prefer the smallest safe correction. Avoid recommending broad rewrites unless the current design creates material risk.

## Severity

Use only these severities:

- **Blocker**: likely security issue, data corruption/loss, severe production outage, or fundamentally incorrect behavior.
- **Major**: concrete bug, serious compatibility problem, reliability issue, or meaningful performance regression.
- **Minor**: maintainability or robustness issue worth fixing but not release-blocking.
- **Nit**: optional polish. Keep nits rare.

## Finding format

For each finding provide:

- severity
- file and relevant symbol/line if available
- what is wrong
- concrete failure scenario or impact
- minimal recommended fix

Do not invent line numbers. Do not praise every file. If no material issues are found, say so and list any residual risks or test gaps.

## Review constraints

- Do not modify code unless explicitly asked.
- Do not require abstractions merely to reduce line count.
- Do not reject an approach solely because another style is personally preferred.
- Treat public APIs, persisted data, events, schemas, and shared libraries as compatibility-sensitive.
