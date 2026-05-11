# Story 001: Phase-Gated Input — Day UI / Night Movement / Disabled

> **Epic**: player-controller
> **Status**: Complete
> **Layer**: Core
> **Type**: Logic
> **Priority**: P0
> **Estimate**: 8 hours
> **Manifest Version**: N/A

## Context

**GDD**: `design/gdd/player-controller.md`
**TR-ID**: `TR-player-001`, `TR-player-008`
**ADR Governing Implementation**: ADR-0003: Player Controller Pattern
**ADR Decision Summary**: New Input System + CharacterController with phase-gated enable/disable via IPhaseStateMachine subscription. Switch `PlayerInputMode` enum (DayUI / NightMovement / Disabled) on phase change.

**Engine**: Unity 6000.3.11f1 (Unity 6) | **Risk**: HIGH (New Input System, post-cutoff API)

---

## Acceptance Criteria

- [x] During `DayService`, WASD/mouse movement is ignored; only UI clicks processed.
- [x] During `NightSurvival`, WASD/mouse-look works; UI clicks on NPCs ignored.
- [x] During `ChoiceLock`, `Resolve`, `Reset`, all input ignored except skip/continue prompts.
- [x] No combat inputs (attack, block, counter) exist in any phase.
- [x] On `NightSurvival` exit, all movement/interact inputs disabled cleanly — no orphaned states.

---

## Implementation Notes

*Derived from ADR-0003 Implementation Guidelines:*

- Subscribe to `IPhaseStateMachine.CurrentPhase` (R3 ReactiveProperty)
- Switch `PlayerInputMode` enum: `DayUI` / `NightMovement` / `Disabled`
- `PlayerInputActions` asset generated via New Input System
- Phase change must be synchronous — no frame delay on mode switch
- `OnPhaseChanged(PhaseState)` handles `NightSurvival` → Enable(), all others → Disable()

---

## Out of Scope

*Handled by neighbouring stories — do not implement here:*

- Actual movement physics (Story 002)
- Sprint logic (Story 003)
- Cover detection (Story 004)
- E-key interactions (Story 005)
- Cursor visibility (Story 006)

---

## QA Test Cases

- **AC-1**: DayService blocks WASD
  - Given: Phase = DayService
  - When: Player presses WASD
  - Then: No movement occurs; `PlayerInputMode == DayUI`

- **AC-2**: NightSurvival enables movement
  - Given: Phase = NightSurvival
  - When: Player presses WASD
  - Then: Movement input is processed; `PlayerInputMode == NightMovement`

- **AC-3**: ChoiceLock disables all input
  - Given: Phase = ChoiceLock
  - When: Player presses any input
  - Then: No action occurs; `PlayerInputMode == Disabled`

- **AC-4**: Clean exit from NightSurvival
  - Given: Phase transitions from NightSurvival to EndingEvaluation
  - When: Transition fires
  - Then: `PlayerInputMode == Disabled`; no sprint/interact state persists

- **AC-5**: No combat inputs exist
  - Given: Any phase
  - When: Inspecting PlayerInputActions asset
  - Then: No attack, block, or counter action bindings present

---

## Test Evidence

**Story Type**: Logic
**Required evidence**: `tests/unit/player-controller/phase-gated-input_test.cs` — unit tests must exist and pass

**Status**: [x] Complete — 17/17 tests passing

- `Assets/_Project/Application/Editor/Tests/PhaseGatedInputTests.cs` (17 tests, all AC-1 through AC-5 + mode transitions)

---

## Dependencies

- Depends on: phase-state-machine epic (Complete ✅)
- Unlocks: All other player-controller stories
