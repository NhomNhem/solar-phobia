# ADR-0011: Audio State Director with BroAudio

> **Status**: Accepted
> **Date**: 2026-05-11
> **Author**: opencode

## Context

Solar Phobia's audio system must deliver phase-gated day/night audio snapshots (warm/measured day vs urgent/primal night), ambient layer blending (ocean, wind, vocals, threat drone), and event-driven SFX (footsteps, cover, hazards, strike warnings). The GDD `design/gdd/audio-core-loop.md` lists this as an open question ("Unity Audio or FMOD?"). BroAudio (`Assets/Plugins/BroAudio/`) is already imported — a full-featured Unity audio middleware with SoundID-based playback, MusicPlayer with crossfade transitions, volume/pitch control per type, and effect chaining.

## Decision

Use **BroAudio** as the sole audio middleware. Build an `AudioMixDirector` service that:

1. **Subscribes to `IPhaseStateMachine.OnPhaseChanged` via R3** — triggers Day→Night audio snapshots (warm → cold mix, ambient layer crossfade, threat motif)
2. **Manages ambient layers** as BroAudio `SoundID` handles — ocean (base), wind, vocals, threat drone — each with independent volume control for blending
3. **Plays event-driven SFX** via `BroAudio.Play(SoundID)` — footsteps, cover enter/exit, strike warnings, hazard loops, shrine arrival
4. **Uses `MusicPlayer` for BGM** — day (sparse guitar), night (tension pulse), with `Transition.Crossfade` at phase boundaries

```csharp
using Ami.BroAudio;
using Ami.BroAudio.Runtime;
using R3;

public class AudioMixDirector : IInitializable, IDisposable
{
    private readonly IPhaseStateMachine _phaseState;
    private readonly CompositeDisposable _disposables = new();

    // SoundIDs assigned via BroAudio library manager
    private SoundID _ambientOcean;
    private SoundID _ambientWind;
    private SoundID _ambientVocals;
    private SoundID _ambientThreat;
    private SoundID _bgmDay;
    private SoundID _bgmNight;

    public void Initialize()
    {
        _phaseState.OnNightStart
            .Subscribe(_ => TransitionToNight())
            .AddTo(_disposables);

        _phaseState.OnDayStart
            .Subscribe(_ => TransitionToDay())
            .AddTo(_disposables);
    }

    private void TransitionToNight()
    {
        // Crossfade BGM
        BroAudio.Play(_bgmNight).AsMusicPlayer()
            .SetTransition(Transition.Crossfade, StopMode.FadeOut, 1.5f);

        // Blend ambient layers
        BroAudio.SetVolume(_ambientOcean, 1.0f, 2f);
        BroAudio.SetVolume(_ambientWind, 1.0f, 2f);
        BroAudio.SetVolume(_ambientVocals, 0.8f, 3f);
        BroAudio.SetVolume(_ambientThreat, 0.6f, 2f);
    }

    private void TransitionToDay()
    {
        BroAudio.Play(_bgmDay).AsMusicPlayer()
            .SetTransition(Transition.Crossfade, StopMode.FadeOut, 1.5f);

        BroAudio.SetVolume(_ambientOcean, 0.5f, 2f);
        BroAudio.SetVolume(_ambientWind, 0.3f, 2f);
        BroAudio.SetVolume(_ambientVocals, 0.0f, 3f);
        BroAudio.SetVolume(_ambientThreat, 0.0f, 2f);
    }
}
```

## Consequences

- Positive: BroAudio is already imported — no new package dependency, no middleware licensing
- Positive: `SoundID` system maps cleanly to the GDD's audio palette table (one ID per layer/event)
- Positive: `MusicPlayer.SetTransition(Transition.Crossfade)` directly implements the GDD's phase transition audio spec
- Negative: Audio assets (clips, mixer snapshots) must be configured in the BroAudio Library Manager editor — not data-driven from code
- Need: Story files must define which SoundIDs map to which GDD audio events before implementation

## GDD Requirements Addressed

- TR-audio-001: Day/night audio mix snapshots
- TR-audio-002: Phase-gated music transitions
- TR-audio-003: Ambient layer blending (ocean, wind, vocals, threat)
- TR-audio-004: Event-driven SFX (footsteps, cover, hazards, strike)

## Engine Compatibility

- Unity 6000.3.11f1
- BroAudio (in-project plugin, `Assets/Plugins/BroAudio/`)
- R3 for phase events, VContainer for DI
- No known post-cutoff API conflicts — BroAudio uses standard Unity AudioSource under the hood

## ADR Dependencies

- Depends on: ADR-0001 (Phase State Machine — provides phase events)
- Depends on: R3 reactive pattern (established in ADR-0001)
- Dependent ADRs: ADR-0010 (Day/Night Camera Transition — audio cues fire alongside camera transitions)
