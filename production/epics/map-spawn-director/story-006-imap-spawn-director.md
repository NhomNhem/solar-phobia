# Story 006: IMapSpawnDirector Interface — Player Controller Signals

> **Epic**: map-spawn-director
> **Status**: Complete
> **Layer**: Foundation
> **Type**: Logic
> **Priority**: P0
> **Estimate**: 4 hours

## Context

**GDD**: `design/gdd/map-and-spawn-director.md` — Player Controller interactions
**TR-ID**: `TR-map-007`
**ADR**: ADR-0006

This story unblocks player-controller story-007 (Strike Warning Integration).
IMapSpawnDirector exposes the events Player Controller subscribes to.

## Acceptance Criteria

- [x] IMapSpawnDirector.OnStrikeWarning observable exists and fires.
- [x] IMapSpawnDirector.UpdatePlayerPosition(Vector2, Bounds) method exists.
- [x] OnEnterCover / OnExitCover events fire when player overlaps cover triggers.
- [x] Interface is in Application layer — no Unity scene dependency.

## Implementation Notes

- Define `IMapSpawnDirector` interface in Application/Services/Interfaces/
- Stub implementation `MapSpawnDirectorStub` for testing
- Player Controller story-007 subscribes to `OnStrikeWarning`

## Test Evidence

**Story Type**: Logic
**Required**: `Assets/_Project/Application/Editor/Tests/MapSpawnDirectorInterfaceTests.cs`

**Status**: [x] Complete — 11/11 tests passing

- `Assets/_Project/Application/Editor/Tests/MapSpawnDirectorInterfaceTests.cs` (11 tests)

