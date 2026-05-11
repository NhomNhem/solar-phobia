# ADR-0006: Map Generation Strategy

> **Status**: Accepted
> **Date**: 2026-05-07
> **Author**: opencode

## Context

Map & Spawn Director must generate deterministic, reproducible night maps using seeded procedural generation.

## Decision

Use deterministic seed-based chunk spawning:
- Seed stored in run snapshot for reproducibility
- Chunks spawn ahead of player, despawn behind
- Curse bias applied at generation time (hazard placement)

```csharp
public class MapSpawnDirector : IInitializable {
    private int _seed;
    private System.Random _rng;
    
    public void Initialize(int seed) {
        _seed = seed;
        _rng = new System.Random(seed);
    }
    
    public Chunk GenerateChunk(int index) {
        // Deterministic based on index + seed
        var chunkSeed = _seed + index;
        var chunkRng = new System.Random(chunkSeed);
        // Generate terrain, spawns, hazards
    }
}
```

## Consequences

- Positive: Reproducible runs from seed
- Positive: Curse bias affects hazard placement
- Need: Ensure route viability check passes

## GDD Requirements Addressed

- TR-map-001: Procedural generation with seed
- TR-map-002: Curse-based hazard spawning

## Engine Compatibility

- Unity 6.3.11f1
- VContainer