# Story 002: Sweep Exposure Check — is_exposed = in_cone AND not in_cover

> **Epic**: map-spawn-director
> **Status**: Complete
> **Layer**: Foundation
> **Type**: Logic
> **Priority**: P0
> **Estimate**: 4 hours

## Context

**GDD**: `design/gdd/map-and-spawn-director.md` — Sweep Exposure Check formula
**TR-ID**: `TR-map-003`
**ADR**: ADR-0006

**Formula**: `is_exposed = in_sweep_cone AND (not in_valid_cover)`

## Acceptance Criteria

- [x] Player in sweep cone + not in cover → is_exposed = true.
- [x] Player in sweep cone + fully in cover → is_exposed = false.
- [x] Player outside sweep cone → is_exposed = false regardless of cover.
- [x] Missing cover collider treated as not in cover (exposed).
- [x] Sweep never applies inside shrine safe zone.

## Implementation Notes

- `SweepExposureCalculator.IsExposed(bool inCone, bool inValidCover)`
- Pure boolean logic — no Unity physics required
- Shrine safe zone: separate flag passed in

## Test Evidence

**Story Type**: Logic
**Required**: `Assets/_Project/Application/Editor/Tests/SweepExposureTests.cs`

**Status**: [x] Complete — 7/7 tests passing

- `Assets/_Project/Application/Editor/Tests/SweepExposureTests.cs` (7 tests)

