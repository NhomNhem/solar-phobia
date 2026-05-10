# Story 004: Illusion Platform Effect (FakeShrine - Minh Abandoned)

> **Epic**: curse-effect-modules
> **Status**: Ready
> **Layer**: Feature
> **Type**: Logic
> **Manifest Version**: N/A

## Context

**GDD**: `design/gdd/curse-effect-modules.md`
**Requirement**: Illusion platform collapses 0.2s after stepped on

**ADR Governing Implementation**: None — no ADR exists for this system yet

**Engine**: Unity 6000.3.11f1 (Unity 6) | **Risk**: MEDIUM
**Engine Notes**: Uses VContainer for DI, R3 for reactive state

**Control Manifest Rules (this layer)**: N/A — manifest not yet created

---

## Acceptance Criteria

*From GDD `design/gdd/curse-effect-modules.md`, scoped to this story:*

- [ ] When curse_type = FakeShrine (Minh abandoned), spawn Illusion platforms (Bệ Đá Ảo Ảnh)
- [ ] Platform collapses 0.2 seconds after player steps on it
- [ ] Player falls through collapsed platform (no ground collision)
- [ ] Visual: Shimmer effect, ground cracks
- [ ] Audio: Ground rumble, deceptive whisper

---

## Implementation Notes

*Illusion Platform hazard implementation:*

- Use platform collider (Collider2D with PlatformEffector or separate trigger)
- OnTriggerEnter2D: start 0.2s timer, then disable ground collider
- After collapse: platform visual fades out, collider stays disabled
- Apply visual effect: shimmer shader + crack sprite appear
- Trigger audio: rumble on collapse, whisper loop while standing

---

## Out of Scope

*Handled by neighbouring stories — do not implement here:*

- Story 001: Core state machine (dependency)
- Story 002: Water Trap effects
- Story 003: Blood Net effects

---

## QA Test Cases

**[For Logic stories — automated test specs]:**

- **AC-1**: Platform collapse after 0.2s
  - Given: Player steps on Illusion Platform
  - When: 0.2 seconds elapse
  - Then: Platform collider disabled, player falls
  
- **AC-2**: Immediate collapse when standing at end
  - Given: Player stands on platform for 0.25s
  - When: Frame update
  - Then: Platform already collapsed
  
- **AC-3**: Platform stays collapsed
  - Given: Platform has collapsed
  - When: Player returns to area
  - Then: Platform remains disabled (no respawn)

---

## Test Evidence

**Story Type**: Logic
**Required evidence**: `Assets/_Project/Application/Editor/Tests/IllusionPlatformTests.cs` — must exist and pass

**Status**: [ ] Not yet created

---

## Dependencies

- Depends on: Story 001 (Curse Effect Core) must be DONE
- Unlocks: Story 005 (Visual/Audio for all curse types)