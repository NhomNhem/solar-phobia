# Story 004: Route Viability Check — Fairness Guard

> **Epic**: map-spawn-director
> **Status**: Complete
> **Layer**: Foundation
> **Type**: Logic
> **Priority**: P1
> **Estimate**: 3 hours

## Context

**GDD**: `design/gdd/map-and-spawn-director.md` — Route Viability Check formula
**TR-ID**: `TR-map-005`
**ADR**: ADR-0006

**Formula**: `is_viable = ward_remaining > (distance / speed) + safety_buffer`

## Acceptance Criteria

- [x] IsRouteViable returns true when Ward > travel time + buffer.
- [x] IsRouteViable returns false when Ward insufficient.
- [x] SafetyBufferSec is configurable (default 12s, range 5-30).
- [x] Zero speed returns false (cannot move).

## Test Evidence

**Story Type**: Logic
**Required**: `Assets/_Project/Application/Editor/Tests/RouteViabilityTests.cs`

**Status**: [x] Complete — 8/8 tests passing

- `Assets/_Project/Application/Editor/Tests/RouteViabilityTests.cs` (8 tests)

