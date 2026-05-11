# Story 007-v2: Day Phase Swap + Shove — X-Axis Soul Management

> **Epic**: player-controller
> **Status**: Complete
> **Layer**: Core
> **Type**: Logic
> **Priority**: P1

## Context

**GDD**: Master GDD V5.0 — Section 2.1
**ADR**: ADR-0003-v2

Day phase: Space=Swap (exchange positions with a soul), F=Shove (push soul out of shadow).
Both only active during DayService phase.

## Acceptance Criteria

- [x] Space key fires OnSwap event during DayService phase.
- [x] F key fires OnShove event during DayService phase.
- [x] Both blocked when PlayerInputMode != DayUI.
- [x] Swap fires OnSwap event (soul targeting handled by Soul system).
- [x] Shove fires OnShove event (direction handled by Soul system).

## Test Evidence

**Story Type**: Logic
**Required**: unit tests in Editor/Tests/

**Status**: [x] Complete — 12/12 tests passing

- `Assets/_Project/Application/Editor/Tests/DayActionTests.cs` (12 tests)

