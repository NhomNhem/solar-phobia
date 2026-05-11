# Story 002: Day Phase Mechanics — Swap/Shove Souls

> **Epic**: phase-state-machine
> **Status**: Complete
> **Layer**: Foundation
> **Type**: Logic
> **Manifest Version**: N/A

## Context

**GDD**: `design/gdd/game-state-phase-state-machine.md`
**Requirement**: `TR-state-001` (Phase state machine with day/night cycle transitions)

**ADR Governing Implementation**: ADR-0001: Phase State Machine Architecture
**ADR Decision Summary**: R3 ReactiveProperty-based state machine with 7 states and phase contracts for action gating.

**Engine**: Unity 6000.3.11f1 (Unity 6) | **Risk**: HIGH
**Engine Notes**: R3 and VContainer are post-cutoff libraries.

---

## Acceptance Criteria

*From GDD `design/gdd/game-state-phase-state-machine.md`, scoped to this story:*

- [ ] Swap mechanic allows repositioning souls (0.5s animation, no jump-over)
- [ ] Shove mechanic forces one soul abandonment at phase end
- [ ] `sacrificed_ghost_id` is written and persisted to night phase

---

## Implementation Notes

*Derived from ADR-0001 Implementation Guidelines:*

- Swap: When player approaches soul from edge and presses interact, swap positions (0.5s animation)
- Shove: At Collapse phase end (or timer expiry), player MUST push one soul out of shadow
- Screen shake + audio impact + soul burn animation triggers on shove
- sacrificed_ghost_id written to SoulRepository before ChoiceLock → NightSurvival transition

---

## Out of Scope

*Handled by neighbouring stories — do not implement here:*

- Story 001: Timeline phases (Stability → Tension → Crisis → Collapse)
- Story 006: Karma hazards (what spawns based on sacrificed_ghost_id)
- Story 008: Ward initialization (how value is calculated)

---

## QA Test Cases

**[For Logic stories — automated test specs]:**

- **AC-1**: Swap repositions soul with 0.5s animation
  - Given: Player in DayService, adjacent to soul at shadow edge
  - When: Player initiates Swap (interact key near soul)
  - Then: 0.5s animation plays, player and soul swap positions
  - Edge cases: Player attempts swap during animation (ignored), soul in Panic AI (may fail)

- **AC-2**: Shove forces one soul out of shadow at Collapse end
  - Given: DayService timeline reaches Collapse phase (270s+)
  - When: Player confirms or timer expires
  - Then: Shove animation triggers, soul pushed into sunlight, soul burns with scream
  - Edge cases: No soul selected for abandonment (auto-select via fallback priority)

- **AC-3**: sacrificed_ghost_id persists to night phase
  - Given: Shove mechanic executed, soul marked Abandoned
  - When: Transition from ChoiceLock to NightSurvival
  - Then: SoulRepository.NightOutcomeState reflects abandoned soul ID
  - Edge cases: Multiple souls abandoned (impossible by design), auto-commit with fallback

---

## Test Evidence

**Story Type**: Logic
**Required evidence**: `tests/unit/phase-state-machine/day-phase-mechanics_test.cs` — must exist and pass

**Status**: [ ] Not yet created

---

## Dependencies

- Depends on: Story 001 (Day Phase Timeline) — Collapse phase must exist before Shove
- Unlocks: Story 006 (Karma Hazards) — needs sacrificed_ghost_id

---

## Completion Notes

**Completed**: 2026-05-07
**Criteria**: 3/3 passing (all acceptance criteria met)

**Files Modified**:
- `Assets/_Project/Application/Services/DayPhaseMechanicsService.cs` (implemented)
- `Assets/_Project/Application/Editor/Tests/DayPhaseMechanicsTests.cs` (12 tests)

**Test Evidence**: Logic: `Assets/_Project/Application/Editor/Tests/DayPhaseMechanicsTests.cs` (12 tests, passing)

**Code Review**: APPROVED WITH SUGGESTIONS (from `/code-review` run)

**Deviations**: None — Implementation matches ADR-0001 and story requirements.

**Tech debt logged**: None

**Next recommended**: `/story-readiness production/epics/phase-state-machine/story-003-night-phase-movement.md`
