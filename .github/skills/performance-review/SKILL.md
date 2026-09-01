---
name: performance-review
description: Investigate or review performance, latency, throughput, CPU, memory, allocations, I/O, database access, caching, or scalability. Use when asked to optimize code or diagnose a performance regression.
argument-hint: "[hot path, metric, benchmark, or symptom]"
---

# Performance Review

Measure first. Optimize the dominant cost, not code that merely looks expensive.

## Workflow

1. Define the performance objective and workload:
   - latency percentile or throughput target
   - request/data size
   - concurrency
   - environment
2. Establish a baseline from benchmarks, profiling, tracing, metrics, or reproducible timing.
3. Find the dominant bottleneck.
   - CPU
   - allocations/GC
   - blocking or lock contention
   - network/filesystem I/O
   - database queries
   - serialization
   - excessive calls/chattiness
4. Inspect algorithmic complexity and data structures where input size can grow.
5. Look for common systemic issues:
   - N+1 queries
   - repeated enumeration/materialization
   - unbounded concurrency
   - sync-over-async
   - large object allocation
   - unnecessary copying
   - missing batching
   - ineffective caching
   - oversized payloads
6. Propose the smallest change likely to affect the measured bottleneck.
7. Re-measure using the same workload and compare against baseline.
8. Check for regressions in correctness, memory, tail latency, and operational complexity.

## Output

Report baseline, bottleneck evidence, change, before/after result, tradeoffs, and measurement limitations. Never claim an optimization without measurements when measurement is feasible.
