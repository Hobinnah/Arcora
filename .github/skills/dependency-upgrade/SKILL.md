---
name: dependency-upgrade
description: Safely upgrade a package, framework, runtime, SDK, library, build tool, or dependency while identifying breaking changes, transitive effects, security implications, and validation requirements.
argument-hint: "[dependency and target version]"
---

# Dependency Upgrade

Treat upgrades as behavior changes until proven otherwise.

## Workflow

1. Identify current and target versions plus all projects/components that consume the dependency.
2. Read local usage first; consult authoritative release notes or migration guidance when available.
3. Identify:
   - breaking API changes
   - behavior/default changes
   - configuration changes
   - runtime/platform requirements
   - transitive dependency changes
   - security advisories
   - deprecated/removed features
4. Upgrade the smallest coherent dependency set. Avoid unrelated package churn.
5. Resolve compile/build failures by adapting to the new contract rather than suppressing warnings blindly.
6. Run tests focused on changed behavior, then broader build/test checks.
7. Inspect runtime-sensitive areas such as serialization, DI, HTTP, database providers, logging, reflection, and native dependencies when relevant.
8. Compare lockfiles/manifests for unexpected transitive changes.
9. Document migration notes for consumer-visible changes.

## Guardrails

- Do not use broad version ranges merely to make resolution succeed.
- Do not disable analyzers or security checks as an upgrade shortcut.
- Do not assume successful compilation proves behavioral compatibility.
