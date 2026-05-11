# Story 003: Strike Telegraph + Penalty — Ward Cost -30s + Cover Break

> **Epic**: map-spawn-director
> **Status**: Complete
> **Layer**: Foundation
> **Type**: Logic
> **Priority**: P0
> **Estimate**: 5 hours

## Context

**GDD**: `design/gdd/map-and-spawn-director.md` — Strike Penalty formula
**TR-ID**: `TR-map-004`
**ADR**: ADR-0006

**Formula**: `ward_after_hit = max(0, ward_before_hit - StrikeTimePenaltySec)`

## Acceptance Criteria

- [x] Strike telegraph fires OnStrikeWarning event to Player Controller.
- [x] Unresolved exposure after telegraph → OnWardCostIncurred fires.
- [x] OnWardCostIncurred emits Ward cost (default -30s).
- [x] Strike never fires inside shrine safe zone.
- [x] StrikeTimePenaltySec is configurable (default 30, range 5-60).
- [x] StrikeTelegraphSec is configurable (default 1.5s, range 0.8-2.5s).

## Implementation Notes

- `StrikeController.Tick(bool isExposed, bool inSafeZone, float deltaTime)`
- Fires `OnStrikeWarning` when telegraph starts
- Fires `OnWardCostIncurred(StrikeTimePenaltySec)` when strike resolves
- Phase gate: only active during NightSurvival

## Test Evidence

**Story Type**: Logic
**Required**: `Assets/_Project/Application/Editor/Tests/StrikeTests.cs`

**Status**: [x] Complete — 14/14 tests passing

- `Assets/_Project/Application/Editor/Tests/StrikeTests.cs` (14 tests)

