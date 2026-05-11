# Story 003: Event-Driven SFX — Footsteps, Cover, Hazards, Strike

> **Epic**: audio-state-director
> **Status**: Ready
> **Layer**: Presentation
> **Type**: Visual/Feel
> **Manifest Version**: N/A
> **Estimate**: 5 hours (M)

## Context

**GDD**: `design/gdd/audio-core-loop.md`
**Requirement**: TR-audio-004 (Event-driven SFX)

**ADR Governing Implementation**: ADR-0011: Audio State Director with BroAudio
**ADR Decision Summary**: SFX played via `BroAudio.Play(SoundID)` with optional positional 3D. Priority system: Critical (strike warning/hit), High (sprint, relic, cover), Medium (footsteps, death, shrine), Low (ambient, UI).

**Engine**: Unity 6000.3.11f1 (Unity 6) | **Risk**: LOW
**Engine Notes**: Use `BroAudio.Play(SoundID)` for 2D UI sounds, `BroAudio.Play(SoundID, Vector3)` for positional hazards. BroAudio handles voice limiting internally.

---

## Performance

Event-driven — no per-frame cost. BroAudio pools AudioSource instances. Budget: burst allocation <0.1ms per SFX call.

---

## Acceptance Criteria

- [ ] **Footsteps**: Footstep SFX plays while player moves — day variant (-20dB, pitch ±5%) vs night variant (-15dB, pitch ±10%)
- [ ] **Sprint Breath**: Heavy breathing loop plays while Shift held, stops on release
- [ ] **Cover Enter**: Muffled transition SFX plays on entering cover volume, 500ms crossfade
- [ ] **Cover Exit**: Unmuffled transition SFX plays on exiting cover volume, 300ms crossfade
- [ ] **Strike Warning**: Rising crack + heartbeat plays during sweep telegraph (Priority: Critical)
- [ ] **Strike Hit**: Loud impact + distortion plays when strike resolves
- [ ] **Bone Relic Pickup**: Breath + crackle + chant layer plays on E-key at CursedMound
- [ ] **Death**: Distorted hit + low drone plays when Ward = 0
- [ ] **Shrine Arrival**: Relief stinger + tension drop on reaching EndShrine

---

## Implementation Notes

### Integration Points

| SFX Event | Trigger | BroAudio API |
|-----------|---------|-------------|
| Footstep (day/night) | Player movement + phase state | `BroAudio.Play(_sfxFootstepDay/Night)` |
| Sprint breath | Shift held | `BroAudio.Play(_sfxSprintBreath).AsLooping()` |
| Cover enter/exit | CoverDetector2D events | `BroAudio.Play(_sfxCoverEnter/Exit)` |
| Strike warning | StrikeWarningController | `BroAudio.Play(_sfxStrikeWarning)` |
| Strike hit | StrikeController | `BroAudio.Play(_sfxStrikeHit, position)` |
| Relic pickup | NgocCotService | `BroAudio.Play(_sfxRelicPickup)` |
| Death | WardTimer depleted | `BroAudio.Play(_sfxDeathHit)` |
| Shrine arrival | GameFlowCoordinator | `BroAudio.Play(_sfxReliefStinger)` |

### Visual/Feel Note

SFX timing and mixing cannot be auto-verified by unit tests. Acceptance criteria are manually confirmed via playtest. Evidence doc required.

### Test Evidence

- **No automated test** — Visual/Feel story
- **Evidence doc**: `production/qa/evidence/audio-sfx-walkthrough.md` — manual sign-off required
