# Story 004-v2: Swing + Glide — Khăn Tang Skills

> **Epic**: player-controller
> **Status**: Complete
> **Layer**: Core
> **Type**: Logic
> **Priority**: P1

## Context

**GDD**: Master GDD V5.0 — Section 3.1
**ADR**: ADR-0003-v2

Swing: Left Click near anchor point → grapple arc movement, costs -2.0s Ward.
Glide: Hold (airborne) → slow fall + horizontal drift, costs -1.0s/sec Ward.

## Acceptance Criteria

- [x] Left Click near anchor: Swing activates, costs -2.0s Ward on attach.
- [x] Glide: holding while airborne reduces gravity scale, costs -1.0s/sec Ward.
- [x] Both skills blocked when PlayerInputMode != NightMovement.
- [x] Swing blocked when Ward <= 2.0s.
- [x] Glide auto-cancels when Ward reaches 0.

## Test Evidence

**Story Type**: Logic
**Required**: unit tests in Editor/Tests/

**Status**: [x] Complete — 19/19 tests passing

- `Assets/_Project/Application/Editor/Tests/SwingGlideTests.cs` (19 tests, Swing + Glide AC-1 through AC-2)

