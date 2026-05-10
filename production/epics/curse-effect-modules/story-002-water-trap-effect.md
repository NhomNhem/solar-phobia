# Story 002: Water Trap Effect (Drag - Linh Abandoned)

> **Epic**: curse-effect-modules
> **Status**: Ready
> **Layer**: Feature
> **Type**: Logic
> **Manifest Version**: N/A

## Context

**GDD**: `design/gdd/curse-effect-modules.md`
**Requirement**: Water trap applies -3s/s DoT

**ADR Governing Implementation**: None — no ADR exists for this system yet

**Engine**: Unity 6000.3.11f1 (Unity 6) | **Risk**: MEDIUM
**Engine Notes**: Uses VContainer for DI, R3 for reactive state

**Control Manifest Rules (this layer)**: N/A — manifest not yet created

---

## Acceptance Criteria

*From GDD `design/gdd/curse-effect-modules.md`, scoped to this story:*

- [ ] When curse_type = Drag (Linh abandoned), spawn Water Trap zones (Vũng Nước Tử Thần)
- [ ] Player standing in water takes -3.0 * deltaTime damage per second (DoT)
- [ ] Visual: Blue water glow, ripple VFX
- [ ] Audio: Bubbling, drowning sounds

---

## Implementation Notes

*Water Trap hazard implementation:*

- Use physics trigger zone (Collider2D) for water areas
- OnTriggerStay2D: apply continuous damage (-3.0 * Time.deltaTime)
- Apply visual effect: blue particle system + ripple shader
- Trigger audio: looping bubbling sound when player in zone

---

## Out of Scope

*Handled by neighbouring stories — do not implement here:*

- Story 001: Core state machine (dependency)
- Story 003: Blood Net effects
- Story 004: Illusion Platform effects

---

## QA Test Cases

**[For Logic stories — automated test specs]:**

- **AC-1**: Water trap damage per second
  - Given: Player in Water Trap zone
  - When: 1 second elapses
  - Then: Ward reduced by 3.0
  
- **AC-2**: DoT applies continuously
  - Given: Player in Water Trap zone for 3 seconds
  - When: Damage calculated each frame
  - Then: Ward reduced by 9.0 total

---

## Test Evidence

**Story Type**: Logic
**Required evidence**: `Assets/_Project/Application/Editor/Tests/WaterTrapEffectTests.cs` — must exist and pass

**Status**: [ ] Not yet created

---

## Dependencies

- Depends on: Story 001 (Curse Effect Core) must be DONE
- Unlocks: Story 005 (Visual/Audio)