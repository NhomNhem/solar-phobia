# Story 003: Sprint — Shift Key Speed Multiplier + Stamina Integration

> **Epic**: player-controller
> **Superseded-by**: story-003-v2-spirit-dash.md
> **Reason**: Sprint multiplier replaced by Spirit Dash skill (-5s Ward cost).
> **Status**: Superseded (see story-003-v2-spirit-dash.md)
> **Layer**: Core
> **Type**: Logic
> **Priority**: P1
> **Estimate**: 4 hours
> **Manifest Version**: N/A

## Context

**GDD**: `design/gdd/player-controller.md`
**TR-ID**: `TR-player-003`
**ADR Governing Implementation**: ADR-0003: Player Controller Pattern
**ADR Decision Summary**: Sprint = `base_move_speed * sprint_multiplier` while Shift held and stamina > 0. Sends `is_sprinting` state to Health/Stamina system via event. Receives `OnStaminaDepleted` to force-exit sprint.

**Engine**: Unity 6000.3.11f1 (Unity 6) | **Risk**: HIGH

---

## Acceptance Criteria

- [x] Holding Shift activates sprint: `effective_speed = base_move_speed * sprint_multiplier`.
- [x] Releasing Shift returns speed to `base_move_speed * 1.0`.
- [x] Sprint activation fires `OnSprintChanged(true)` event to Health/Stamina system.
- [x] Receiving `OnStaminaDepleted` from Health/Stamina immediately disables sprint, returns to base speed.
- [x] Sprint with 0 stamina: input ignored, `is_sprinting = false`, player stays at base speed.
- [x] `sprint_multiplier` is a configurable tuning knob (default 1.8, range 1.5–3.0).
- [x] Sprint is only active during `NightSurvival` phase.

---

## Implementation Notes

*Derived from ADR-0003 Implementation Guidelines:*

- `_inputActions.Player.Sprint.IsPressed()` checked each frame in `ProcessMovement()`
- Fire `OnSprintChanged(bool)` only on state change (not every frame) to avoid event spam
- Subscribe to `IHealthStaminaService.OnStaminaDepleted` observable
- `sprint_multiplier` default: 1.8, safe range 1.5–3.0 (per GDD Tuning Knobs)
- Edge case: stamina depletes mid-sprint → receive `OnStaminaDepleted` → immediately drop to base speed

---

## Out of Scope

*Handled by neighbouring stories — do not implement here:*

- Stamina drain calculation (owned by Health/Stamina system)
- Visual FOV increase / motion blur on sprint (Visual/Feel story)

---

## QA Test Cases

- **AC-1**: Shift activates sprint speed
  - Given: Phase = NightSurvival, `base_move_speed = 5.0`, `sprint_multiplier = 1.8`
  - When: Player holds Shift + W
  - Then: `effective_speed = 9.0`; `OnSprintChanged(true)` fired

- **AC-2**: Releasing Shift returns to base speed
  - Given: Player is sprinting
  - When: Shift released
  - Then: `effective_speed = 5.0`; `OnSprintChanged(false)` fired

- **AC-3**: OnStaminaDepleted forces sprint exit
  - Given: Player is sprinting
  - When: `OnStaminaDepleted` event received
  - Then: Sprint disabled immediately; `effective_speed = base_move_speed`

- **AC-4**: Sprint blocked with 0 stamina
  - Given: Stamina = 0
  - When: Player holds Shift
  - Then: `is_sprinting = false`; speed unchanged

- **AC-5**: Sprint only in NightSurvival
  - Given: Phase = DayService
  - When: Player holds Shift
  - Then: No sprint activation; no `OnSprintChanged` event

---

## Test Evidence

**Story Type**: Logic
**Required evidence**: `tests/unit/player-controller/sprint_test.cs` — unit tests must exist and pass

**Status**: [x] Complete — 22/22 tests passing

- `Assets/_Project/Application/Editor/Tests/SprintTests.cs` (22 tests, AC-1 through AC-6 + no-duplicate-event guards)

---

## Dependencies

- Depends on: Story 002 (WASD Movement)
- Unlocks: Story 007 (Strike Warning Integration — uses sprint state)

