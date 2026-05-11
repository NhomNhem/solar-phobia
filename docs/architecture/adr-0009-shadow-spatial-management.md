# ADR-0009: Shadow Spatial Management

> **Status**: Accepted
> **Date**: 2026-05-07
> **Author**: opencode

## Context

Shadow Spatial Management controls the shrinking safe zone polygon during Day Phase.

## Decision

Polygon-based shadow with shrink events:

```csharp
public class ShadowManager : IInitializable, ITickable {
    private Polygon2D _shadowPolygon;
    private float _shrinkRate;
    
    public void Tick() {
        if (PhaseStateMachine.CurrentPhase == PhaseState.dayService) {
            _shadowPolygon.Shrink(_shrinkRate * Time.deltaTime);
        }
    }
    
    public bool IsInShadow(Vector2 position) {
        return _shadowPolygon.Contains(position);
    }
}
```

## Consequences

- Positive: Precise polygon collision for souls
- Positive: Shrink rate controllable via config
- Need: Must trigger crowd events as it shrinks

## GDD Requirements Addressed

- TR-shadow-001: Polygon-based shadow
- TR-shadow-002: Shrink events to Physical Crowding