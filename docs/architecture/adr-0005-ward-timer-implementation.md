# ADR-0005: Ward Timer Implementation

> **Status**: Accepted
> **Date**: 2026-05-07
> **Author**: opencode

## Context

The Ward Timer is the single lethal resource in Solar Phobia—counts down during Night Phase, death when it reaches 0. Must integrate with R3 reactive system and support sensory tier transitions.

## Decision

Use R3 `ReactiveProperty<float>` for Ward value with tier state machine:

```csharp
public class WardTimer : IInitializable, ITickable {
    private readonly ReactiveProperty<float> _currentWard = new(0f);
    public ReadOnlyReactiveProperty<float> CurrentWard => _currentWard;
    public ReadOnlyReactiveProperty<SensoryTier> CurrentTier { get; }
    
    private float _baseDrainRate = 1.0f;
    private float _drainMultiplier = 1.0f;
    
    public void Initialize(int ghostsSaved, int dayPenalties) {
        _currentWard.Value = 10f + (ghostsSaved * 30f) - dayPenalties;
    }
    
    public void Tick() {
        _currentWard.Value -= _baseDrainRate * _drainMultiplier * Time.deltaTime;
        if (_currentWard.Value <= 0) {
            _onDepleted.OnNext(Unit.Default);
        }
    }
}
```

## Consequences

- Positive: ReactiveProperty enables systems to subscribe to Ward changes
- Positive: Single source of truth for all damage/penalty calculations
- Need: Ensure tier transitions are smooth, not jarring

## GDD Requirements Addressed

- TR-ward-001: Ward initialization from day choices
- TR-ward-002: Passive drain with multiplier
- TR-ward-003: Sensory tier transitions

## Engine Compatibility

- Unity 6.3.11f1
- R3 (new reactive system, post-cutoff from LLM training)
- VContainer for DI