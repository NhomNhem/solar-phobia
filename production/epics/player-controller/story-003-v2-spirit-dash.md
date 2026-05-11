# Story 003-v2: Spirit Dash — Khăn Tang Burst + Ward Cost

> **Epic**: player-controller
> **Status**: Complete
> **Layer**: Core
> **Type**: Logic
> **Priority**: P1
> **Replaces**: story-003-sprint.md (Sprint multiplier — archived)

## Context

**GDD**: Master GDD V5.0 — Section 3.1 (Bộ Điều Khiển Kỹ Năng)
**ADR**: ADR-0003-v2

Spirit Dash: Shift key → horizontal burst, invincible frames, costs -5.0s Ward.
Replaces the sprint multiplier concept entirely.

## Acceptance Criteria

- [x] Shift key triggers Spirit Dash: horizontal burst movement.
- [x] Spirit Dash costs -5.0s from Ward Timer on activation.
- [x] OnWardCostIncurred(5.0f) event fires on each dash.
- [x] Dash only active during NightSurvival phase.
- [x] Dash has cooldown to prevent spam (configurable, default 0.5s).
- [x] Dash blocked when Ward <= 5.0s (not enough Ward to pay cost).

## Implementation Notes

- DashController.TryDash(bool input, PlayerInputMode mode)
- Fires OnWardCostIncurred(5.0f) — Ward Timer subscribes and deducts
- No stamina system — Ward Timer IS the resource
- Dash direction: current horizontal input direction (or last facing if no input)

## Test Evidence

**Story Type**: Logic
**Required**: unit tests in Editor/Tests/

**Status**: [x] Complete — 21/21 tests passing

- `Assets/_Project/Application/Editor/Tests/DashTests.cs` (21 tests, AC-1 through AC-6 + cooldown clamping)

