# Story 006: Karma Hazards — Curse Spawning from Sacrificed Ghosts

> **Epic**: phase-state-machine
> **Status**: Complete
> **Layer**: Feature
> **Type**: Logic + Integration
> **Manifest Version**: N/A (no control-manifest.md exists)
> **Estimate**: M (2-3 hours)

## Context

**GDD**: `design/gdd/game-state-phase-state-machine.md`
**Requirement**: `TR-state-004` (Night phase mechanics and karma hazard spawning)

**ADR Governing Implementation**: ADR-0001: Phase State Machine Architecture
**ADR Decision Summary**: Karma hazards spawn based on sacrificed_ghost_id with distinct effects per ghost type. (TR-state-004 covered by ADR-0001)

**Engine**: Unity 6000.3.11f1 (Unity 6) | **Risk**: MEDIUM

---

## Acceptance Criteria

*From GDD `design/gdd/game-state-phase-state-machine.md`, scoped to this story:*

- [x] Karma hazards spawn based on `sacrificed_ghost_id`:
  - [x] Van → Lưới Máu (slow penalty)
  - [x] Linh → Vũng Nước (DoT)
  - [x] Minh → Bệ Đá Ảo Ảnh (0.2s collapse)

---

## Implementation Notes

*Derived from ADR-0004 Implementation Guidelines:*

- KarmaHazardService reads sacrificed_ghost_id from GameStateManager
- Spawn hazard prefab matching ghost type at player position
- Lưới Máu applies movement speed reduction (0.5×) while in zone
- Vũng Nước applies damage-over-time (5 HP/s) while standing
- Bệ Đá Ảo Ảnh has collision trigger that collapses for 0.2s when player enters

**Performance**: Hazard spawn check expected <2ms/frame. KarmaHazardService runs only during NightSurvival phase — no impact on day phase performance.

---

## Out of Scope

*Handled by neighbouring stories — do not implement here:*

- Story 005: Boss Searchlight detection
- Story 004: Cover detection from Mound colliders

---

## QA Test Cases

**[For Logic + Integration stories]:**

- **AC-1**: Van sacrifice spawns Lưới Máu hazard
  - Given: Player sacrifices Van ghost at day→night transition
  - When: NightSurvival phase starts
  - Then: Lưới Máu hazard spawns at player location
  - Edge cases: Multiple sacrifices on same night

- **AC-2**: Linh sacrifice spawns Vũng Nước hazard with DoT
  - Given: Player sacrifices Linh ghost at day→night transition
  - When: NightSurvival phase starts
  - Then: Vũng Nước hazard spawns and applies 5 HP/s damage
  - Edge cases: Player at full HP (still applies, just invisible)

- **AC-3**: Minh sacrifice spawns Bệ Đá Ảo Ảnh with collapse
  - Given: Player sacrifices Minh ghost at day→night transition
  - When: Player enters Bệ Đá trigger zone
  - Then: Player collides and triggers 0.2s collapse
  - Edge cases: Player has iframes (not affected)

---

## Test Evidence

**Story Type**: Logic + Integration
**Required evidence**: `tests/integration/phase-state-machine/karma-hazards_test.cs` — must exist and pass

**Status**: [x] Created and passing

---

## Completion Notes
**Completed**: 2026-05-08
**Criteria**: 3/3 passing
**Deviations**: PhaseStateMachineNew.cs, PhaseStateMachineTests.cs, BossSearchlightService.cs modified outside scope to align enum values with Domain definitions
**Test Evidence**: Logic: Assets/_Project/Application/Tests/KarmaHazardsTests.cs (8 tests) | Integration: tests/integration/phase-state-machine/karma-hazards_test.cs (16 tests)
**Code Review**: Complete (APPROVED)

## Dependencies

- Depends on: Story 002 (Day Phase Mechanics), Story 003 (Night Phase Movement)
- Unlocks: None (final feature story)