---
name: incident-investigation
description: Investigate a production incident, outage, alert, severe degradation, or customer-impacting failure using logs, metrics, traces, deployments, configuration, and code. Use for incident triage and root-cause analysis.
argument-hint: "[incident, alert, symptom, or time range]"
---

# Incident Investigation

Prioritize customer impact, evidence preservation, and safe mitigation.

## Workflow

1. Establish the incident timeline and current impact.
   - affected users/regions/components
   - start time
   - error rate/latency/availability
   - data integrity risk
2. Identify recent changes:
   - deployments
   - configuration/feature flags
   - infrastructure changes
   - dependency/provider incidents
   - traffic/data-shape changes
3. Use observability to narrow the fault domain.
   - correlate metrics, logs, traces, and dependency health
   - compare healthy vs unhealthy requests/instances
4. Separate mitigation from root-cause work.
   - choose the safest reversible mitigation first
   - rollback/disable/scale/reroute only when evidence supports it
5. Preserve important evidence before destructive changes when possible.
6. After stabilization, reproduce or prove the causal chain.
7. Identify contributing factors and why safeguards did not catch the issue.
8. Define corrective actions at multiple levels when warranted:
   - code/config fix
   - tests
   - monitoring/alerts
   - deployment safeguards
   - runbook/documentation

## Output

Provide timeline, impact, evidence, mitigation, root cause, contributing factors, validation, and follow-up actions. Distinguish confirmed facts from hypotheses.
