# Story 005-v2: Coyote Time + Jump Buffering — Platformer Feel

> **Epic**: player-controller
> **Status**: Complete
> **Layer**: Core
> **Type**: Logic
> **Priority**: P1

## Context

**GDD**: Master GDD V5.0 — Section 3.1 (Yêu cầu)
**ADR**: ADR-0003-v2

Standard precision platformer feel requirements.

## Acceptance Criteria

- [x] Coyote Time: player can jump up to 0.1s after walking off a ledge.
- [x] Jump Buffering: jump input registered up to 0.15s before landing.
- [x] Both only active during NightSurvival phase.
- [x] Coyote Time and Jump Buffer durations are configurable tuning knobs.

## Test Evidence

**Story Type**: Logic
**Required**: unit tests in Editor/Tests/

**Status**: [x] Complete — 17/17 tests passing

- `Assets/_Project/Application/Editor/Tests/PlatformerFeelTests.cs` (17 tests)

