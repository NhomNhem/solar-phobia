# ADR-0008: Night Survival Run Architecture

> **Status**: Accepted
> **Date**: 2026-05-07
> **Author**: opencode

## Context

Night Survival Run coordinates all night phase systems: movement, hazards, boss, shrine. Phase-gated container.

## Decision

Container system that enables/disables child systems based on phase:

```csharp
public class NightSurvivalRun : IInitializable {
    private readonly PhaseStateMachine _phaseState;
    private readonly PlayerController _player;
    private readonly BossSearchlight _boss;
    private readonly CurseEffects _curses;
    private readonly ShrineObjective _shrine;
    private readonly WardTimer _ward;
    
    public void Initialize() {
        _phaseState.CurrentPhase
            .Where(p => p == PhaseState.NightSurvival)
            .Subscribe(_ => Enable());
    }
    
    private void Enable() {
        _player.Enable();
        _boss.Enable();
        _curses.Enable(_phaseState.GetPayload<CursePayload>());
    }
}
```

## Consequences

- Positive: Centralized phase entry/exit
- Positive: Clean enable/disable of subsystems
- Need: All dependencies must be ready before Enable

## GDD Requirements Addressed

- TR-night-001: Night loop state machine
- TR-night-002: Phase entry/exit coordination