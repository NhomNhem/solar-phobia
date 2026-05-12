# Story 002: Shrine Arrival and Win Condition

> **Epic**: shrine-objective-win-lose-rules
> **Status**: Complete
> **Layer**: Core
> **Type**: Logic
> **Manifest Version**: N/A
> **Estimate**: 0.5 days

## Context

**GDD**: `design/gdd/shrine-objective-win-lose-rules.md`
**Requirement**: TR-shrine-002 (Shrine arrival triggers win condition and phase transition)

**ADR Governing Implementation**: None — no ADR exists for this system yet; follow `docs/architecture/architecture.md`

**Engine**: Unity 6000.3.11f1 (Unity 6) | **Risk**: LOW

---

## Acceptance Criteria

*From GDD `design/gdd/shrine-objective-win-lose-rules.md`, scoped to this story:*

- [x] During `NightSurvival`, pressing E inside the EndShrine trigger zone emits `OnShrineReached` and triggers transition to `ShrineArrival` phase.
- [x] On entering `ShrineArrival` phase, camera pans to the shrine glow using DOTween over 0.5s.
- [x] E presses outside the shrine zone or during non-night phases are ignored.
- [x] `OnShrineReached` fires only once per run; repeated presses after resolution do nothing.
- [x] Shrine arrival does not depend on visual, audio, or UI feedback systems for the phase transition.

---

## Implementation Notes

- Subscribe to `IPhaseStateMachine` so win detection is only active during `NightSurvival`.
- Validate proximity against the shrine trigger zone provided by the map/spawn layer.
- Emit the shrine-arrival event through the existing module contract; do not perform direct scene transitions here.
- For camera pan, use the `IDayNightCameraController` to trigger a shrine arrival cinematic.
- Keep the logic deterministic and side-effect light so it is easy to test.

---

## Out of Scope

*Handled by neighbouring stories — do not implement here:*

- Ward Timer depletion and death sequence (`ward-timer/story-001-death-trigger.md`)
- Shrine glow, relief stinger, and other feedback (`sensory-feedback-system`)
- EndShrine placement and trigger geometry (`map-spawn-director`)

---

## QA Test Cases

- **AC-1: Win Detection and Phase Transition**
  - Given: `NightSurvival` is active and the player is inside the EndShrine zone
  - When: Player presses E
  - Then: `OnShrineReached` is emitted once and phase transitions to `ShrineArrival`

- **AC-2: Camera Pan**
  - Given: Player has entered `ShrineArrival` phase
  - When: 0.5s elapses
  - Then: Camera has completed pan to shrine glow
  - *Note: Handled by `DayNightCameraController` (story Complete) — subscribes to `OnPhaseChanged` for `ShrineArrival`*

- **AC-3: Phase Gate**
  - Given: Any non-night phase
  - When: Player presses E inside the shrine zone
  - Then: No win event is emitted and no phase transition occurs

- **AC-4: Proximity Gate**
  - Given: `NightSurvival` is active but the player is outside the shrine zone
  - When: Player presses E
  - Then: No win event is emitted

- **AC-5: One-Shot Behavior**
  - Given: `OnShrineReached` has already fired
  - When: Player presses E again
  - Then: No duplicate event is emitted and no phase transition occurs

---

## Test Evidence

**Story Type**: Logic
**Required evidence**: `Assets/_Project/Application/Editor/Tests/ShrineArrivalWinConditionTests.cs` — must exist and pass

---

## Completion Notes

**Completed**: 2026-05-12
**Criteria**: 5/5 passing — proximity, phase-gating, one-shot guard, OnShrineReached event, ShrineArrival transition
**Deviations**: Camera pan (AC-2) handled by the already-complete DayNightCameraController story — no code change needed there.
**Test Evidence**: `Assets/_Project/Application/Editor/Tests/ShrineArrivalWinConditionTests.cs` (9 tests, all passing)
**File cleanup**: Removed duplicate `ShrineObjectiveService.cs` from `Application/Services/Objective/` namespace

**Files modified:**
- `Assets/_Project/Application/Services/Interfaces/IShrineObjectiveService.cs` — added `OnShrineReached` event
- `Assets/_Project/Application/Services/ShrineObjectiveService.cs` — transition to `ShrineArrival` instead of `EndingEvaluation`, added one-shot guard, `OnShrineReached` emission, `IDisposable`

**Files created:**
- `Assets/_Project/Application/Editor/Tests/ShrineArrivalWinConditionTests.cs` — 9 tests

**Files deleted:**
- `Assets/_Project/Application/Services/Objective/ShrineObjectiveService.cs` (duplicate)

---

## Dependencies

- Depends on: Phase State Machine, Player Controller, Map & Spawn Director shrine zone, Day Night Camera Controller
- Unlocks: Ending Evaluation phase (via shrine arrival triggering resolve)
