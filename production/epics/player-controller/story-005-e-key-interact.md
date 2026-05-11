# Story 005: E-Key Contextual Interact — Relic Pickup + Shrine Trigger

> **Epic**: player-controller
> **Status**: Complete
> **Layer**: Core
> **Type**: Logic
> **Priority**: P1
> **Estimate**: 4 hours
> **Manifest Version**: N/A

## Context

**GDD**: `design/gdd/player-controller.md`
**TR-ID**: `TR-player-005`
**ADR Governing Implementation**: ADR-0003: Player Controller Pattern
**ADR Decision Summary**: E-key raycast forward 2m. Tag `"CursedMound"` → fires `OnInteract("relic")`. Tag `"EndShrine"` → fires `OnInteract("shrine")`. No interactable nearby → silently ignored.

**Engine**: Unity 6000.3.11f1 (Unity 6) | **Risk**: HIGH

---

## Acceptance Criteria

- [x] Near CursedMound (Mo Oan), E key fires `OnInteract("relic")` event.
- [x] Near EndShrine, E key fires `OnInteract("shrine")` event.
- [x] E key with no interactable in range → silently ignored (no error popup, no log spam).
- [x] E-interact only active during `NightSurvival` phase.
- [x] Bone Relic pickup during active strike telegraph applies Time Drain immediately — does not cancel the strike.

---

## Implementation Notes

*Derived from ADR-0003 Implementation Guidelines:*

- `Physics.Raycast(transform.position, transform.forward, 2f)` on E press callback
- Tag checks: `hit.collider.CompareTag("CursedMound")` → `"relic"`, `CompareTag("EndShrine")` → `"shrine"`
- Guard: `if (_currentMode != NightMovement) return;`
- FalseSafeMound near-E: warning tell plays; no `OnInteract` event fired
- `OnInteract` is `event Action<string>` — subscribers handle downstream effects

---

## Out of Scope

*Handled by neighbouring stories — do not implement here:*

- Time Drain modifier calculation (owned by Resource Effects system)
- Shrine win condition evaluation (owned by Shrine Objective system)
- Relic pickup cross-system integration test (Story 008)

---

## QA Test Cases

- **AC-1**: E near CursedMound fires relic event
  - Given: Phase = NightSurvival; CursedMound within 2m forward
  - When: Player presses E
  - Then: `OnInteract("relic")` fires once

- **AC-2**: E near EndShrine fires shrine event
  - Given: Phase = NightSurvival; EndShrine within 2m forward
  - When: Player presses E
  - Then: `OnInteract("shrine")` fires once

- **AC-3**: E with nothing nearby → silent
  - Given: Phase = NightSurvival; no tagged object within 2m
  - When: Player presses E
  - Then: No event fired; no log output

- **AC-4**: E blocked outside NightSurvival
  - Given: Phase = DayService; CursedMound within 2m
  - When: Player presses E
  - Then: No event fired

- **AC-5**: Pickup during strike telegraph
  - Given: Strike telegraph active; CursedMound within 2m
  - When: Player presses E
  - Then: `OnInteract("relic")` fires; strike is NOT cancelled

---

## Test Evidence

**Story Type**: Logic
**Required evidence**: `tests/unit/player-controller/e-key-interact_test.cs` — unit tests must exist and pass

**Status**: [x] Complete — 15/15 tests passing

- `Assets/_Project/Application/Editor/Tests/EKeyInteractTests.cs` (15 tests, AC-1 through AC-5 + multi-press + tag/payload constants)

---

## Dependencies

- Depends on: Story 001 (Phase-Gated Input)
- Unlocks: Story 008 (Relic Pickup Integration)
