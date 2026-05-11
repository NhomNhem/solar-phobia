# Story 001: Day Phase Timeline — 4 Pressure Phases

> **Epic**: phase-state-machine
> **Status**: Complete (tests exist at Assets/_Project/Application/Editor/Tests/DayPhaseTimelineTests.cs)
> **Layer**: Foundation
> **Type**: Logic
> **Manifest Version**: N/A (no control-manifest.md exists)

## Context

**GDD**: `design/gdd/game-state-phase-state-machine.md`
**Requirement**: `TR-state-001` (Phase state machine with day/night cycle transitions)

**ADR Governing Implementation**: ADR-0001: Phase State Machine Architecture
**ADR Decision Summary**: R3 ReactiveProperty-based state machine with 7 states and phase contracts for action gating.

**Engine**: Unity 6000.3.11f1 (Unity 6) | **Risk**: HIGH
**Engine Notes**: R3 and VContainer are post-cutoff libraries — verify behavior against Unity 6 docs.

---

## Acceptance Criteria

*From GDD `design/gdd/game-state-phase-state-machine.md`, scoped to this story:*

- [ ] 5-minute timeline with escalating pressure phases (Stability → Tension → Crisis → Collapse)
- [ ] 0:00–1:30 (Stability): Space for 4 people (Tú + 3 souls)
- [ ] 1:30–3:00 (Tension): Shadows shrink 30%, Light Interrupts appear randomly, Souls enter Panic AI
- [ ] 3:00–4:30 (Crisis): Only space for 3 people, player must Swap positions
- [ ] 4:30–5:00 (Collapse): One soul MUST be abandoned

---

## Implementation Notes

*Derived from ADR-0001 Implementation Guidelines:*

- Use R3 ReactiveProperty to track current timeline phase (Stability, Tension, Crisis, Collapse)
- Phase contracts already define allowed actions per phase in ADR-0001
- Timeline advances automatically based on elapsed time from DayService entry
- Emit TimelinePhaseChangedEvent when transitioning between timeline phases

---

## Out of Scope

*Handled by neighbouring stories — do not implement here:*

- Story 002: Handles Swap/Shove mechanics at Collapse end
- Story 003: Handles NightSurvival movement (different phase)
- Story 008: Handles Ward Timer initialization at day→night transition

---

## QA Test Cases

**[For Logic stories — automated test specs]:**

- **AC-1**: Timeline starts at 0:00 and progresses through phases
  - Given: DayService phase entered
  - When: Time elapses to 90 seconds
  - Then: Current timeline phase transitions from Stability to Tension
  - Edge cases: Pause menu stops timer, game speed affects progression

- **AC-2**: Crisis phase reduces available space
  - Given: Timeline enters Crisis phase (180s elapsed)
  - When: Player checks soul positions
  - Then: Only 3 souls can fit in shadow zone (not 4)
  - Edge cases: Player has saved 3 souls (impossible by design)

- **AC-3**: Collapse phase forces abandonment
  - Given: Timeline reaches Collapse phase (270s elapsed)
  - When: Player attempts to confirm selection
  - Then: Shove mechanic activates automatically
  - Edge cases: Timer expires at exact 300s (transition to ChoiceLock)

---

## Test Evidence

**Story Type**: Logic
**Required evidence**: `Assets/_Project/Application/Editor/Tests/DayPhaseTimelineTests.cs` — must exist and pass

**Status**: [x] Exists and covers all ACs

---

## Dependencies

- Depends on: None (foundational — DayService already works)
- Unlocks: Story 002 (Day Phase Mechanics)
