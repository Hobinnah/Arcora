---
name: architecture-review
description: Evaluate a proposed or existing software architecture for boundaries, dependencies, scalability, reliability, security, operability, evolvability, and cost. Use for design reviews, ADRs, major changes, service boundaries, and platform decisions.
argument-hint: "[design, ADR, system, or proposed change]"
---

# Architecture Review

Evaluate architecture against explicit quality attributes and constraints, not fashion.

## Workflow

1. Establish context:
   - business capability and users
   - scale and latency targets
   - availability/recovery expectations
   - consistency requirements
   - security/compliance constraints
   - team ownership and operational model
   - expected rate of change
2. Draw or infer the major components, boundaries, dependencies, data flows, and trust boundaries.
3. Identify the highest-risk decisions and irreversible choices.
4. Evaluate:
   - separation of responsibilities
   - dependency direction and coupling
   - data ownership and consistency
   - failure isolation and degradation
   - scalability bottlenecks
   - security boundaries
   - deployment and rollback
   - observability and supportability
   - testability
   - compatibility and migration
   - operational and cognitive cost
5. Challenge hidden assumptions with concrete failure scenarios.
6. Compare alternatives only on meaningful tradeoffs. Avoid listing technologies without decision criteria.
7. Prefer simple, reversible designs until complexity is justified by measured requirements.

## Output

Provide:

- architecture summary
- strengths
- risks ordered by severity
- questions/unknowns that affect the decision
- recommended changes
- rejected alternatives and why, when relevant
- migration/rollout considerations

Distinguish facts, assumptions, and recommendations.
