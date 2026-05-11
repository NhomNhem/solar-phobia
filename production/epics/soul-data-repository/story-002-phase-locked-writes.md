# Story 002: Phase-Locked Writes — Selection + Night Outcome Validation

> **Epic**: soul-data-repository
> **Status**: Complete
> **Layer**: Foundation
> **Type**: Logic
> **Priority**: P0

## Context

**ADR**: ADR-0002: Soul Data Repository Pattern — Status: Accepted

## Acceptance Criteria

- [x] TrySetSelection rejected outside DayService phase.
- [x] Cannot mark soul as both Saved and Abandoned.
- [x] TrySetNightOutcome only valid for Abandoned souls in ChoiceLock/NightSurvival.
- [x] OnSelectionChanged fires on state change.
- [x] Reset() clears all run-scoped state.

## Test Evidence

**Status**: [x] Complete — covered by SoulRepositoryTests.cs (16 tests)
