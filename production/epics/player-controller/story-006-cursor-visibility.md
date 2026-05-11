# Story 006: Cursor Visibility — Phase-Driven Show/Hide

> **Epic**: player-controller
> **Status**: Complete
> **Layer**: Core
> **Type**: UI
> **Priority**: P1
> **Estimate**: 2 hours
> **Manifest Version**: N/A

## Context

**GDD**: `design/gdd/player-controller.md`
**TR-ID**: `TR-player-007`
**ADR Governing Implementation**: ADR-0003: Player Controller Pattern
**ADR Decision Summary**: Cursor visible in DayService (UI mode), hidden and locked in NightSurvival (mouselook mode). Handled synchronously in `OnPhaseChanged()`.

**Engine**: Unity 6000.3.11f1 (Unity 6) | **Risk**: LOW

---

## Acceptance Criteria

- [x] On `DayService` entry: `Cursor.visible = true`, `Cursor.lockState = CursorLockMode.None`.
- [x] On `NightSurvival` entry: `Cursor.visible = false`, `Cursor.lockState = CursorLockMode.Locked`.
- [x] On `ChoiceLock`, `Resolve`, `Reset` entry: cursor shown (`CursorLockMode.None`) for UI prompts.
- [x] Cursor state set synchronously on phase change — no frame delay.

---

## Implementation Notes

*Derived from ADR-0003 Implementation Guidelines:*

- Handle in `OnPhaseChanged()` alongside `PlayerInputMode` switch
- `Cursor.lockState = CursorLockMode.Locked` required for mouselook to work correctly in Unity
- Edge case: game loses focus (Alt+Tab) → Unity auto-unlocks cursor; re-lock on `OnApplicationFocus(true)` if phase is NightSurvival

---

## Out of Scope

*Handled by neighbouring stories — do not implement here:*

- Mouselook camera rotation (separate camera system story)

---

## QA Test Cases

**Story Type**: UI — manual walkthrough required.

- **AC-1**: DayService shows cursor
  - Given: Game starts in DayService
  - When: Player observes screen
  - Then: OS cursor visible; can click UI elements

- **AC-2**: NightSurvival hides cursor
  - Given: Phase transitions to NightSurvival
  - When: Player observes screen
  - Then: Cursor hidden; mouse movement controls camera look

- **AC-3**: ChoiceLock restores cursor
  - Given: Phase transitions from NightSurvival to EndingEvaluation
  - When: Player observes screen
  - Then: Cursor visible; can interact with outcome prompts

---

## Test Evidence

**Story Type**: UI
**Required evidence**: Manual walkthrough doc — `production/qa/evidence/cursor-visibility-walkthrough.md`

**Status**: [x] Logic verified — 10/10 automated tests passing. Manual walkthrough pending sign-off.

- `Assets/_Project/Application/Editor/Tests/CursorVisibilityTests.cs` (10 automated tests — phase mapping logic)
- `production/qa/evidence/cursor-visibility-walkthrough.md` (manual walkthrough — pending tester sign-off)

---

## Dependencies

- Depends on: Story 001 (Phase-Gated Input)
- Unlocks: None (leaf story)
