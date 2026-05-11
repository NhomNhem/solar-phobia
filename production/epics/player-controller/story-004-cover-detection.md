# Story 004: Cover Detection — Full Containment Validation

> **Epic**: player-controller
> **Superseded-by**: story-004-v2-cover-detection-2d.md
> **Reason**: 3D bounds containment replaced by 2D trigger overlap (Mộ Gió tombstones).
> **Status**: Superseded (see story-004-v2-cover-detection-2d.md)
> **Layer**: Core
> **Type**: Logic
> **Priority**: P1
> **Estimate**: 5 hours
> **Manifest Version**: N/A

## Context

**GDD**: `design/gdd/player-controller.md`
**TR-ID**: `TR-player-004`
**ADR Governing Implementation**: ADR-0003: Player Controller Pattern
**ADR Decision Summary**: Player collider must be fully inside cover volume (Mo Thuong, Mo Oan, FalseSafeMound) for cover to register. Partial overlap = exposed. `cover_enter_threshold` is configurable (default 1.0).

**Engine**: Unity 6000.3.11f1 (Unity 6) | **Risk**: HIGH

---

## Acceptance Criteria

- [x] Player collider fully inside Mo Thuong cover volume → `IsInCover = true`.
- [x] Partial overlap with any cover volume → `IsInCover = false` (exposed).
- [x] `IsInCover` state published as `ReactiveProperty<bool>` for HUD and Map Director subscription.
- [x] Cover check runs only during `NightSurvival` phase.
- [x] `cover_enter_threshold` is configurable (default 1.0, range 0.8–1.0).

---

## Implementation Notes

*Derived from ADR-0003 Implementation Guidelines:*

- Bounds check: player `CharacterController.bounds` fully contained within cover volume `Collider.bounds`
- `CheckCover()` called in `Update()` when `_currentMode == NightMovement`
- Expose `IsInCover` as `ReactiveProperty<bool>` — fire only on state change
- FalseSafeMound: cover registers (`IsInCover = true`) but also fires a separate warning tell event
- `cover_enter_threshold`: fraction of player bounds that must be inside (1.0 = fully inside)

---

## Out of Scope

*Handled by neighbouring stories — do not implement here:*

- Strike damage negation (owned by Map & Spawn Director)
- FalseSafeMound collapse penalty (owned by Map & Spawn Director)
- Cover visual feedback / glow (Visual/Feel story)

---

## QA Test Cases

- **AC-1**: Full containment → IsInCover = true
  - Given: Player bounds fully inside Mo Thuong volume
  - When: `CheckCover()` runs
  - Then: `IsInCover = true`

- **AC-2**: Partial overlap → IsInCover = false
  - Given: Player bounds 50% inside cover volume
  - When: `CheckCover()` runs
  - Then: `IsInCover = false`

- **AC-3**: Cover check disabled outside NightSurvival
  - Given: Phase = DayService
  - When: Player position overlaps cover volume
  - Then: `IsInCover` unchanged (check not running)

- **AC-4**: ReactiveProperty fires on state change only
  - Given: `IsInCover = false`
  - When: Player enters cover (full containment)
  - Then: `IsInCover` fires once with `true`; no repeat fires while staying inside

---

## Test Evidence

**Story Type**: Logic
**Required evidence**: `tests/unit/player-controller/cover-detection_test.cs` — unit tests must exist and pass

**Status**: [x] Complete — 20/20 tests passing

- `Assets/_Project/Application/Editor/Tests/CoverDetectionTests.cs` (20 tests, AC-1 through AC-4 + threshold + static helper)

---

## Dependencies

- Depends on: Story 002 (WASD Movement)
- Unlocks: Story 007 (Strike Warning Integration)

