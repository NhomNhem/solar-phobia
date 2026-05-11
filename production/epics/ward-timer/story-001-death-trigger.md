# Story 001: Death Trigger (Ward=0 → EndingEvaluation)

> **Epic**: ward-timer
> **Status**: Ready
> **Layer**: Foundation
> **Type**: Logic
> **Estimate**: 0.5h

## Context

**GDD**: `design/gdd/health-stamina-damage-rules.md`
**Requirement**: When Ward Timer reaches 0, player dies and transitions to EndingEvaluation (Resolve).

**ADR**: ADR-0005 (Ward Timer Implementation) — R3 ReactiveProperty pattern
**Engine**: Unity 6000.3.11f1 | **Risk**: LOW

## Acceptance Criteria

- [ ] Subscribes to `IWardTimerService.OnDepleted`
- [ ] On depletion, calls `IPhaseStateMachine.TryTransition(PhaseState.EndingEvaluation)`
- [ ] Only active during `NightSurvival` phase
- [ ] Does not fire if already in EndingEvaluation
- [ ] One-shot: only triggers once per depletion (guarded by state)

## Implementation Notes

- Pure logic service — no Unity dependencies
- Injected via constructor: `IWardTimerService` + `IPhaseStateMachine`
- Subscribe to `OnDepleted` in constructor
- Guard: check `CurrentState != PhaseState.EndingEvaluation` before transitioning

## Test Evidence

**Type**: Logic
**Required**: `Assets/_Project/Application/Editor/Tests/WardTimerDeathTriggerTests.cs`

## Dependencies

- Depends on: WardTimerService (already implemented), PhaseStateMachine
- Unlocks: Story 002 (Countdown HUD), Story 003 (Sensory Feedback)
