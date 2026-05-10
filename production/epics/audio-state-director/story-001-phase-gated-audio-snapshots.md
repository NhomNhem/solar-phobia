# Story 001: Phase-Gated Audio Snapshots — Day/Night BGM + Ambient Transitions

> **Epic**: audio-state-director
> **Status**: Ready
> **Layer**: Presentation
> **Type**: Integration
> **Manifest Version**: N/A
> **Estimate**: 6 hours (M)

## Context

**GDD**: `design/gdd/audio-core-loop.md`
**Requirement**: TR-audio-001 (Day/night audio mix snapshots), TR-audio-002 (Phase-gated music transitions)

**ADR Governing Implementation**: ADR-0011: Audio State Director with BroAudio
**ADR Decision Summary**: `AudioMixDirector` subscribes to `IPhaseStateMachine.OnNightStart`/`OnDayStart` via R3. Uses `BroAudio.Play()` for BGM with `MusicPlayer.SetTransition(Transition.Crossfade)`. Ambient layer volumes adjusted per phase.

**Engine**: Unity 6000.3.11f1 (Unity 6) | **Risk**: MEDIUM
**Engine Notes**: BroAudio namespace `Ami.BroAudio`. `SoundID` values assigned via BroAudio Library Manager editor. `AsMusicPlayer()` extension on `IAudioPlayer` for BGM control.

**Control Manifest Rules (this layer)**:
- `[Inject]` fields must be `internal` for VContainer source generator
- R3 subscriptions must be managed via `CompositeDisposable` — never leak observers

---

## Performance

Audio transitions are event-triggered (not per-frame). BroAudio's mixer pooling handles instance management. No sustained per-frame cost. Budget: <0.01ms/frame for the controller tick.

---

## Acceptance Criteria

*From GDD `design/gdd/audio-core-loop.md`, scoped to this story:*

- [ ] **Day Audio Snapshot**: During `DayService`/`Dialogue`/`Order`, BGM plays sparse guitar, ambient layers at day volume (ocean 0.5, wind 0.3, vocals 0.0, threat 0.0)
- [ ] **Night Audio Snapshot**: During `NightTravel`/`NightSurvival`, BGM crossfades to tension pulse, ambient layers at night volume (ocean 1.0, wind 1.0, vocals 0.8, threat 0.6)
- [ ] **Day→Night Transition**: On `OnNightStart` R3 event, BGM crossfades in 1.5s via `Transition.Crossfade`, ambient layers blend over 2–3s
- [ ] **Night→Day Transition**: On `OnDayStart` R3 event, BGM crossfades back, ambient layers return to day volume
- [ ] **ChoiceLock Silence**: No audio changes during `ChoiceLock` — snapshot locked at last state
- [ ] **Resolve Stinger**: On `ShrineArrival` (win): relief stinger plays; on `NightFailedEvent` (lose): distorted hit + low drone
- [ ] **Reset Fade**: On phase change back to `DayService`, all audio fades out over 0.5s then restarts day snapshot
- [ ] **All subscriptions disposed** on `AudioMixDirector.Dispose()` — no leaked observers

---

## Implementation Notes

### Core Service

```csharp
public class AudioMixDirector : IInitializable, IDisposable
{
    private readonly IPhaseStateMachine _phaseState;
    private readonly CompositeDisposable _disposables = new();

    // SoundIDs — assigned via BroAudio Library Manager, stored in config
    private SoundID _bgmDay;
    private SoundID _bgmNight;
    private SoundID _sfxReliefStinger;
    private SoundID _sfxDeathHit;
    private SoundID _sfxDeathDrone;
    private SoundID _ambientOcean;
    private SoundID _ambientWind;
    private SoundID _ambientVocals;
    private SoundID _ambientThreat;

    public void Initialize()
    {
        _phaseState.OnNightStart
            .Subscribe(_ => TransitionToNight())
            .AddTo(_disposables);

        _phaseState.OnDayStart
            .Subscribe(_ => TransitionToDay())
            .AddTo(_disposables);

        // Additional subscriptions per phase
    }
}
```

### Phase → Audio Mapping

| PhaseState | BGM | Ocean | Wind | Vocals | Threat |
|------------|-----|-------|------|--------|--------|
| DayService | Day guitar | 0.5 | 0.3 | 0.0 | 0.0 |
| Dialogue | Day guitar | 0.5 | 0.3 | 0.0 | 0.0 |
| Order | Day guitar | 0.5 | 0.3 | 0.0 | 0.0 |
| ChoiceLock | Day guitar (frozen) | frozen | frozen | frozen | frozen |
| NightTravel | Night pulse | 1.0 | 1.0 | 0.8 | 0.6 |
| NightSurvival | Night pulse | 1.0 | 1.0 | 0.8 | 0.6 |
| ShrineArrival | Relief stinger | 0.5 | 0.3 | 0.0 | 0.0 |
| EndingEvaluation | Death hit + drone | 0.0 | 0.0 | 0.0 | 0.0 |

### Test Evidence

- **Test file**: `Assets/_Project/Infrastructure/Tests/Editor/AudioMixDirectorTests.cs`
- Coverage: Phase subscription mapping, transition timing, dispose cleanup, edge cases (ChoiceLock freeze, Reset fade)
