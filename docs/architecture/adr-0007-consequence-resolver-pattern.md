# ADR-0007: Consequence Resolver Implementation

> **Status**: Accepted
> **Date**: 2026-05-07
> **Author**: opencode

## Context

Consequence Resolver maps abandoned soul → curse type deterministically. Must run after Day selection, before Night spawn.

## Decision

Deterministic lookup table, writes once:

```csharp
public class ConsequenceResolver {
    private static readonly Dictionary<string, CurseType> CurseMap = new() {
        { "Linh", CurseType.Drag },    // Water Trap
        { "Van", CurseType.Block },    // Blood Net
        { "Minh", CurseType.FakeShrine } // Illusion
    };
    
    private bool _hasResolved = false;
    
    public CursePayload Resolve(string abandonedSoulId) {
        if (_hasResolved) throw new InvalidOperationException("Already resolved");
        _hasResolved = true;
        
        var curseType = CurseMap.GetValueOrDefault(abandonedSoulId, CurseType.Drag);
        return new CursePayload { Type = curseType, Intensity = 1.0f };
    }
}
```

## Consequences

- Positive: Deterministic, no randomness
- Positive: One-write prevents double resolution
- Need: Payload must reach Map Director before Night starts

## GDD Requirements Addressed

- TR-consequence-001: Curse mapping from sacrifice
- TR-consequence-002: One-write rule enforcement