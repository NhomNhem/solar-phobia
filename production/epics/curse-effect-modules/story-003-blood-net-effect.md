# Story 003: Blood Net Effect (Block - Van Abandoned)

> **Epic**: curse-effect-modules
> **Status**: Ready
> **Layer**: Feature
> **Type**: Logic
> **Manifest Version**: N/A

## Context

**GDD**: `design/gdd/curse-effect-modules.md`
**Requirement**: Blood net applies -5s + 50% slow for 3s

**ADR Governing Implementation**: None — no ADR exists for this system yet

**Engine**: Unity 6000.3.11f1 (Unity 6) | **Risk**: MEDIUM
**Engine Notes**: Uses VContainer for DI, R3 for reactive state

**Control Manifest Rules (this layer)**: N/A — manifest not yet created

---

## Acceptance Criteria

*From GDD `design/gdd/curse-effect-modules.md`, scoped to this story:*

- [ ] When curse_type = Block (Van abandoned), spawn Blood Net obstacles (Lưới Máu)
- [ ] Player touching Blood Net takes -5.0 immediate Ward penalty
- [ ] Player touching Blood Net receives 50% movement slow for 3 seconds
- [ ] Visual: Red blood net overlay, chains
- [ ] Audio: Chain rattling, fabric tear

---

## Implementation Notes

*Blood Net hazard implementation:*

- Use physics trigger zone (Collider2D) for blood net areas
- OnTriggerEnter2D: apply immediate -5.0 Ward damage + start slow Coroutine
- Slow Coroutine: modify movement speed to 50% for 3 seconds, then restore
- Apply visual effect: red overlay sprite + chain particle effect
- Trigger audio: chain rattle + fabric tear sound

---

## Out of Scope

*Handled by neighbouring stories — do not implement here:*

- Story 001: Core state machine (dependency)
- Story 002: Water Trap effects
- Story 004: Illusion Platform effects

---

## QA Test Cases

**[For Logic stories — automated test specs]:**

- **AC-1**: Immediate penalty on contact
  - Given: Player touches Blood Net
  - When: OnTriggerEnter2D fires
  - Then: Ward reduced by 5.0 immediately
  
- **AC-2**: Slow applied for 3 seconds
  - Given: Player touches Blood Net
  - When: Slow starts
  - Then: Movement speed at 50% for 3 seconds
  
- **AC-3**: Slow restores after 3 seconds
  - Given: 3 seconds have passed since Blood Net contact
  - When: Timer completes
  - Then: Movement speed returns to 100%

---

## Test Evidence

**Story Type**: Logic
**Required evidence**: `Assets/_Project/Application/Editor/Tests/BloodNetEffectTests.cs` — must exist and pass

**Status**: [ ] Not yet created

---

## Dependencies

- Depends on: Story 001 (Curse Effect Core) must be DONE
- Unlocks: Story 005 (Visual/Audio)