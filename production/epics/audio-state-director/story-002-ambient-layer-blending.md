# Story 002: Ambient Layer Blending — Independent Volume Control per Layer

> **Epic**: audio-state-director
> **Status**: Ready
> **Layer**: Presentation
> **Type**: Integration
> **Manifest Version**: N/A
> **Estimate**: 4 hours (M)

## Context

**GDD**: `design/gdd/audio-core-loop.md`
**Requirement**: TR-audio-003 (Ambient layer blending)

**ADR Governing Implementation**: ADR-0011: Audio State Director with BroAudio
**ADR Decision Summary**: Each ambient layer (ocean, wind, vocals, threat) has its own `SoundID`. `BroAudio.SetVolume(BroAudioType, float, fadeTime)` controls individual layer volume. Layers loop seamlessly via BroAudio's `LoopType` setting.

**Engine**: Unity 6000.3.11f1 (Unity 6) | **Risk**: LOW
**Engine Notes**: Ambient loops should use `LoopType.Loop` in BroAudio Library Manager. Layers should be configured as `BroAudioType.Ambient` for grouped volume control.

---

## Performance

4 looping audio sources, negligible per-frame cost. Budget: <0.005ms/frame for volume updates.

---

## Acceptance Criteria

- [ ] **Ocean Base**: Continuous ocean wave loop, always present at phase-appropriate volume (day 0.5, night 1.0)
- [ ] **Wind Layer**: Wind loop blends from light breeze (day 0.3) to strong gusts (night 1.0) over 2s on phase change
- [ ] **Vocals Layer**: Soft village ambient during day (0.0 — silent), whispers/distant calls during night (0.8)
- [ ] **Threat Drone**: None during day (0.0), low whale drone during night (0.6), spikes to 1.0 during strike warning
- [ ] **Independent Volume**: Each layer responds to `BroAudio.SetVolume(SoundID, float, fadeTime)` independently — changing ocean does not affect wind
- [ ] **Seamless Loop**: All ambient layers loop without audible pop/click at loop boundaries
- [ ] **Graceful Degradation**: If a layer's SoundID is not assigned (invalid 0), layer is silently skipped — no errors

---

## Implementation Notes

```csharp
public void SetAmbientLayer(SoundID layerId, float targetVolume, float fadeTime = 2f)
{
    if (layerId.IsValid)
    {
        BroAudio.SetVolume(layerId, targetVolume, fadeTime);
    }
}
```

### SoundID Validity Check

BroAudio uses `SoundID` struct — check `.IsValid` or compare to default before calling API.

### Test Evidence

- **Test file**: `Assets/_Project/Infrastructure/Tests/Editor/AmbientLayerTests.cs`
- Coverage: All 4 layers respond to volume changes independently, crossfade timing, loop behavior, invalid SoundID handling
