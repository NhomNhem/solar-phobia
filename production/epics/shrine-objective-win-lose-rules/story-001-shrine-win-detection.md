# Story 001: Shrine Win Detection — EndShrine E-Key → OnShrineReached

> **Epic**: shrine-objective-win-lose-rules
> **Status**: Ready
> **Layer**: Core
> **Type**: Logic
> **Manifest Version**: N/A
> **Estimate**: 0.5h

## Context

**GDD**: `design/gdd/shrine-objective-win-lose-rules.md`
**Requirement**: `TR-shrine-001` (Win/Lose detection with phase gating)
**ADR Governing Implementation**: None — no ADR exists for this system yet; follow `docs/architecture/architecture.md`
**ADR Decision Summary**: `ShrineObjective` owns shrine-zone validation and emits `OnShrineReached` once when the player confirms at the EndShrine during `NightSurvival`. Ward depletion and death are owned by the Ward Timer epic.

**Engine**: Unity 6000.3.11f1 (Unity 6) | **Risk**: LOW

---

## Acceptance Criteria

*From GDD `design/gdd/shrine-objective-win-lose-rules.md`, scoped to this story:*

- [ ] During `NightSurvival`, pressing E inside the EndShrine trigger zone emits `OnShrineReached`.
- [ ] E presses outside the shrine zone or during non-night phases are ignored.
- [ ] `OnShrineReached` fires only once per run; repeated presses after resolution do nothing.
- [ ] Shrine win detection does not depend on visual, audio, or UI feedback systems.

---

## Implementation Notes

- Subscribe to `IPhaseStateMachine` so win detection is only active during `NightSurvival`.
- Validate proximity against the shrine trigger zone provided by the map/spawn layer.
- Emit the shrine-arrival event through the existing module contract; do not perform direct scene transitions here.
- Keep the logic deterministic and side-effect light so it is easy to test.

---

## Out of Scope

*Handled by neighbouring stories — do not implement here:*

- Ward Timer depletion and death sequence (`ward-timer/story-001-death-trigger.md`)
- Shrine glow, relief stinger, and other feedback (`sensory-feedback-system`)
- EndShrine placement and trigger geometry (`map-spawn-director`)

---

## QA Test Cases

- **AC-1: Win Detection**
  - Given: `NightSurvival` is active and the player is inside the EndShrine zone
  - When: Player presses E
  - Then: `OnShrineReached` is emitted once

- **AC-2: Phase Gate**
  - Given: Any non-night phase
  - When: Player presses E inside the shrine zone
  - Then: No win event is emitted

- **AC-3: Proximity Gate**
  - Given: `NightSurvival` is active but the player is outside the shrine zone
  - When: Player presses E
  - Then: No win event is emitted

- **AC-4: One-Shot Behavior**
  - Given: `OnShrineReached` has already fired
  - When: Player presses E again
  - Then: No duplicate event is emitted

---

## Test Evidence

**Story Type**: Logic
**Required evidence**: `Assets/_Project/Application/Editor/Tests/ShrineObjectiveWinDetectionTests.cs` — must exist and pass

---

## Dependencies

- Depends on: Phase State Machine, Player Controller input, Map & Spawn Director shrine zone
- Unlocks: Night Survival Run completion flow and shrine feedback integration
