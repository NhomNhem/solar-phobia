# Story 001: Death Trigger (Ward=0 → EndingEvaluation)

> **Epic**: ward-timer
> **Status**: Complete
> **Layer**: Foundation
> **Type**: Logic
> **Estimate**: 0.5h

## Context

**GDD**: `design/gdd/health-stamina-damage-rules.md`
**Requirement**: When Ward Timer reaches 0, player dies and transitions to EndingEvaluation (Resolve).

**ADR**: ADR-0005 (Ward Timer Implementation) — R3 ReactiveProperty pattern
**Engine**: Unity 6000.3.11f1 | **Risk**: LOW

## Acceptance Criteria

- [x] Subscribes to `IWardTimerService.OnDepleted`
- [x] On depletion, calls `IPhaseStateMachine.TryTransition(PhaseState.EndingEvaluation)`
- [x] Only active during `NightSurvival` phase
- [x] Does not fire if already in EndingEvaluation
- [x] One-shot: only triggers once per depletion (guarded by state)

## Implementation Notes

- Pure logic service — no Unity dependencies
- Injected via constructor: `IWardTimerService` + `IPhaseStateMachine`
- Subscribe to `OnDepleted` in constructor
- Guard: check `CurrentState != PhaseState.EndingEvaluation` before transitioning

## Test Evidence

**Type**: Logic
**Required**: `Assets/_Project/Application/Editor/Tests/WardTimerDeathTriggerTests.cs`

## Completion Notes

**Completed**: 2026-05-12
**Criteria**: 5/5 passing — all acceptance criteria verified via automated tests (12 tests, all passing)
**Deviations**: None

**Files created:**
- `Assets/_Project/Application/Services/Interfaces/IWardDeathTriggerService.cs` — interface
- `Assets/_Project/Application/Services/Objective/WardDeathTriggerService.cs` — implementation
- `Assets/_Project/Application/Editor/Tests/WardDeathTriggerTests.cs` — 12 tests

## Dependencies

- Depends on: WardTimerService (already implemented), PhaseStateMachine
- Unlocks: Story 002 (Countdown HUD), Story 003 (Sensory Feedback)
