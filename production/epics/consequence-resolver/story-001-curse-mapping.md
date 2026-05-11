# Story 001: Curse Mapping — Abandoned Soul to NightOutcomeState

> **Epic**: consequence-resolver
> **Status**: Complete
> **Layer**: Feature
> **Type**: Logic
> **Manifest Version**: N/A
> **Estimate**: 3 hours (S)

## Context

**GDD**: `design/gdd/consequence-resolver.md`
**Requirement**: TR-consequence-001 (Curse payload generation from sacrifice)

**ADR Governing Implementation**: ADR-0007: Consequence Resolver Pattern
**ADR Decision Summary**: Deterministic Dictionary lookup table mapping SoulId to CurseType. One-write rule enforced by boolean guard. Payload sent downstream via R3 Subject.

**Engine**: Unity 6000.3.11f1 (Unity 6) | **Risk**: LOW
**Engine Notes**: Pure logic class — no Unity API dependency except for test assertions. VContainer register as `ILifetimeScope` singleton.

**Control Manifest Rules (this layer)**:
- `[Inject]` fields must be `internal` for VContainer source generator
- Use `readonly` for injected dependencies where possible

---

## Performance

Curse mapping is a single Dictionary lookup (O(1)). No per-frame impact. Acceptable threshold: <0.1ms per invocation.

---

## Acceptance Criteria

*From GDD `design/gdd/consequence-resolver.md`, scoped to this story:*

- [ ] **Curse Mapping**: Deterministic lookup — abandoned soul `"Linh"` → `NightOutcomeState.Drag`, `"Van"` → `NightOutcomeState.Block`, `"Minh"` → `NightOutcomeState.FakeShrine`. No randomness.
- [ ] **One-Write Rule**: `Resolve()` writes `NightOutcomeState` exactly once per run. Second call throws `InvalidOperationException`.
- [ ] **Invalid Soul ID**: Soul IDs not in `{"Linh", "Van", "Minh"}` are rejected, emits `InvalidSoulId` error, defaults to `NightOutcomeState.Drag`.
- [ ] **Duplicate Write**: Second write attempt throws `InvalidOperationException("Already resolved")`.
- [ ] **Payload Delivery**: Returns `CursePayload` with `CurseType`, `Intensity = 1.0f`, and `SpawnBias = abandonedSoulId`. Downstream systems (CurseEffectManager, MapSpawnDirector) receive via R3 Subject or direct method call.
- [ ] **Integration with Ghost Model**: After resolution, writes `NightOutcomeState` on the abandoned `Ghost` entity (or equivalent soul data).

---

## Implementation Notes

### Core Mapping (from ADR-0007)

```csharp
public class ConsequenceResolver
{
    private static readonly Dictionary<string, NightOutcomeState> CurseMap = new()
    {
        { "Linh", NightOutcomeState.Drag },
        { "Van", NightOutcomeState.Block },
        { "Minh", NightOutcomeState.FakeShrine }
    };

    private bool _hasResolved = false;

    public CursePayload Resolve(string abandonedSoulId)
    {
        if (_hasResolved)
            throw new InvalidOperationException("Already resolved");

        _hasResolved = true;

        if (!CurseMap.TryGetValue(abandonedSoulId, out var curseType))
        {
            Debug.LogWarning($"InvalidSoulId: {abandonedSoulId} — defaulting to Drag");
            curseType = NightOutcomeState.Drag;
        }

        return new CursePayload
        {
            CurseType = curseType,
            Intensity = 1.0f,
            SpawnBias = abandonedSoulId
        };
    }
}
```

### Payload Model

```csharp
public class CursePayload
{
    public NightOutcomeState CurseType { get; set; }
    public float Intensity { get; set; }
    public string SpawnBias { get; set; }
}
```

### Trigger Sequence

1. `IPhaseStateMachine` enters `ChoiceLock` after Day Service confirms selection
2. `GameFlowCoordinator` calls `ConsequenceResolver.Resolve(abandonedSoulId)`
3. Resulting `CursePayload` is passed to:
   - `CurseEffectManager` — activates the correct hazard type for Night
   - `MapSpawnDirector` — biases hazard placement

### Test Evidence

- **Test file**: `Assets/_Project/Application/Editor/Tests/ConsequenceResolverTests.cs`
- Coverage: All three mappings, duplicate write rejection, invalid ID fallback, payload structure

---
## Completion Notes
**Completed**: 2026-05-11
**Criteria**: 6/6 passing (14 automated tests, all passing)
**Deviations**: Advisory — lowercase mapping keys vs GDD capitalized names; NightOutcomeState vs ADR's CurseType
**Test Evidence**: Logic — `Assets/_Project/Application/Editor/Tests/ConsequenceResolverTests.cs` (14/14 passing)
**Code Review**: Skipped (Lean mode)
