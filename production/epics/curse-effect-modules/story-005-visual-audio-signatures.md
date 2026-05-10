# Story 005: Visual/Audio Signatures Per Curse

> **Epic**: curse-effect-modules
> **Status**: Ready
> **Layer**: Feature
> **Type**: Visual/Feel
> **Manifest Version**: N/A

## Context

**GDD**: `design/gdd/curse-effect-modules.md`
**Requirement**: Distinct visual and audio per curse

**ADR Governing Implementation**: None — no ADR exists for this system yet

**Engine**: Unity 6000.3.11f1 (Unity 6) | **Risk**: MEDIUM
**Engine Notes**: Uses VContainer for DI, R3 for reactive state

**Control Manifest Rules (this layer)**: N/A — manifest not yet created

---

## Acceptance Criteria

*From GDD `design/gdd/curse-effect-modules.md`, scoped to this story:*

- [ ] **Drag (Linh)**: Blue water glow, ripple VFX + Bubbling, drowning sounds
- [ ] **Block (Van)**: Red blood net overlay, chains + Chain rattling, fabric tear
- [ ] **FakeShrine (Minh)**: Shimmer effect, ground cracks + Ground rumble, deceptive whisper
- [ ] VFX and audio trigger correctly based on active curse_type

---

## Implementation Notes

*Visual and audio mapping for each curse:*

- **Drag**: Blue emission material + ParticleSystem (ripple) + AudioSource (looping bubble)
- **Block**: Red sprite overlay + chain particle + AudioSource (chain rattle + tear)
- **FakeShrine**: Custom shader (shimmer) + crack sprite + AudioSource (rumble + whisper)
- Coordinate with SensoryFeedbackSystem for tier integration

---

## Out of Scope

*Handled by neighbouring stories — do not implement here:*

- Story 001: Core state machine (dependency)
- Story 002-004: Specific hazard mechanics (which trigger these visuals)

---

## QA Test Cases

**[For Visual/Feel stories — manual verification steps]:**

- **AC-1**: Drag visual and audio
  - Setup: Start NightSurvival with Linh abandoned (curse_type = Drag)
  - Verify: Blue water glow visible, ripple particles playing, bubbling audio heard
  - Pass condition: All three present simultaneously

- **AC-2**: Block visual and audio
  - Setup: Start NightSurvival with Van abandoned (curse_type = Block)
  - Verify: Red overlay visible, chains particle playing, rattle + tear audio heard
  - Pass condition: All elements present simultaneously

- **AC-3**: FakeShrine visual and audio
  - Setup: Start NightSurvival with Minh abandoned (curse_type = FakeShrine)
  - Verify: Shimmer effect on platforms, crack sprites visible, rumble + whisper audio
  - Pass condition: All elements present simultaneously

---

## Test Evidence

**Story Type**: Visual/Feel
**Required evidence**: `production/qa/evidence/curse-visual-audio-evidence.md` — must exist with sign-off

**Status**: [ ] Not yet created

---

## Dependencies

- Depends on: Stories 001-004 must be DONE
- Unlocks: None (leaf story)