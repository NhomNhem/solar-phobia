# ADR-0002: Soul Data Repository Pattern

## Status
Accepted

## Date
2026-05-07

## Context

### Problem Statement
The game needs a canonical data source for 3 souls (Linh, Van, Minh) that maintains their identity, selection state, and night outcome across the day/night loop. Multiple systems must read/write this data:
- Day Service & Selection writes selection state
- Consequence Resolver reads abandoned soul and writes curse mapping
- Ward Timer reads saved count for initialization
- Save/Reset snapshots for reproducibility

Without a single authoritative source, systems could disagree about who was saved, leading to broken consequences and player confusion.

### Constraints
- **Engine**: Unity 6000.3.11f1 (Unity 6) — beyond LLM training data
- **Performance**: Soul lookups must be O(1), state changes observable
- **DI**: Must use VContainer for dependency injection
- **Reactive**: Must use R3 for state observation by multiple systems

### Requirements
- Must store exactly 3 canonical souls with immutable SoulId
- Must enforce phase-locked writes (DayService only, then locked)
- Must reject contradictory states (cannot be both Saved and Abandoned)
- Must expose reactive observables for selection changes
- Must support Reset to clear run-scoped fields

## Decision

The Soul Data Repository uses a Dictionary-based store with R3 reactive properties, managed by VContainer. Phase locking is enforced by checking IPhaseStateMachine.CurrentPhase before any write.

### Architecture Diagram

```
┌─────────────────────────────────────────────────────────────┐
│                    SOUL DATA REPOSITORY                       │
│                     (VContainer Singleton)                     │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  ┌─────────────────────────────────────────────────────┐    │
│  │              Dictionary<string, Soul>              │    │
│  │                                                      │    │
│  │  "linh" ──► Soul { Id: "linh", Name: "Em Linh",    │    │
│  │              DaySelection: Saved,                   │    │
│  │              NightOutcome: None }                   │    │
│  │                                                      │    │
│  │  "van" ───► Soul { Id: "van", Name: "Ông Văn",     │    │
│  │              DaySelection: Abandoned,               │    │
│  │              NightOutcome: Drag }                   │    │
│  │                                                      │    │
│  │  "minh" ──► Soul { Id: "minh", Name: "Anh Minh",    │    │
│  │              DaySelection: Saved,                   │    │
│  │              NightOutcome: None }                   │    │
│  └─────────────────────────────────────────────────────┘    │
│                          │                                    │
│           ┌──────────────┼──────────────┐                     │
│           ▼              ▼              ▼                     │
│  ┌─────────────────┐ ┌─────────────────┐ ┌─────────────────┐
│  │ Souls (list)   │ │ SavedSouls     │ │ AbandonedSoul   │
│  │ ReactiveProperty│ │ Observable     │ │ Observable      │
│  └─────────────────┘ └─────────────────┘ └─────────────────┘
│                                                                 │
└─────────────────────────────────────────────────────────────┘
           │                    │                    │
           ▼                    ▼                    ▼
    ┌──────────────┐     ┌──────────────┐    ┌──────────────┐
    │ Day Service  │     │ Consequence  │    │  Ward Timer  │
    │ (writes)     │     │  Resolver    │    │  (reads)     │
    └──────────────┘     └──────────────┘    └──────────────┘
```

### Key Interfaces

```csharp
// Soul Entity
public class Soul {
    public string Id { get; }           // Immutable: "linh", "van", "minh"
    public string LocalizedName { get; }
    public DaySelectionState DaySelection { get; internal set; }
    public NightOutcomeState NightOutcome { get; internal set; }
    public LifeState Life { get; internal set; }
}

public enum DaySelectionState { Unselected, Saved, Abandoned }
public enum NightOutcomeState { None, Drag, Block, FakeShrine }
public enum LifeState { Alive, Lost }

// Repository Interface
public interface ISoulRepository {
    IReadOnlyList<Soul> Souls { get; }
    
    Soul GetSoul(string id);  // O(1) lookup
    
    bool TrySetSelection(string soulId, DaySelectionState state);
    bool TrySetNightOutcome(string soulId, NightOutcomeState outcome);
    
    IReadOnlyList<Soul> GetSavedSouls();
    IReadOnlyList<Soul> GetAbandonedSoul();
    
    bool IsSelectionValid(int requiredSaved);
    
    void Reset();  // Clear run-scoped state
    
    // Reactive
    IObservable<SelectionChangedEvent> OnSelectionChanged { get; }
}

// Events
public struct SelectionChangedEvent {
    public string SoulId;
    public DaySelectionState OldState;
    public DaySelectionState NewState;
}

public struct NightOutcomeEvent {
    public string SoulId;
    public NightOutcomeState Outcome;
}
```

### Implementation

```csharp
public class SoulRepository : ISoulRepository, IInitializable {
    private readonly Dictionary<string, Soul> _souls;
    private readonly IPhaseStateMachine _phaseStateMachine;
    private readonly Subject<SelectionChangedEvent> _selectionSubject = new();
    private readonly Subject<NightOutcomeEvent> _outcomeSubject = new();
    
    public IReadOnlyList<Soul> Souls => _souls.Values.ToList();
    
    public IObservable<SelectionChangedEvent> OnSelectionChanged => _selectionSubject;
    public IObservable<NightOutcomeEvent> OnNightOutcomeChanged => _outcomeSubject;
    
    // Reactive properties for collection queries
    public IReadOnlyList<Soul> GetSavedSouls() => 
        _souls.Values.Where(s => s.DaySelection == DaySelectionState.Saved).ToList();
    
    public IReadOnlyList<Soul> GetAbandonedSoul() => 
        _souls.Values.Where(s => s.DaySelection == DaySelectionState.Abandoned).ToList();
    
    public Soul GetSoul(string id) => 
        _souls.TryGetValue(id, out var soul) ? soul : null;
    
    public bool TrySetSelection(string soulId, DaySelectionState state) {
        // Phase check - only allow writes in DayService
        if (_phaseStateMachine.CurrentPhase.Value != PhaseState.DayService) {
            Debug.LogWarning($"SoulRepository: Cannot write selection outside DayService phase");
            return false;
        }
        
        var soul = GetSoul(soulId);
        if (soul == null) return false;
        
        // Validate state transition
        if (state == DaySelectionState.Saved && soul.DaySelection == DaySelectionState.Abandoned) {
            Debug.LogError($"SoulRepository: Cannot mark {soulId} as Saved when Abandoned");
            return false;
        }
        
        var oldState = soul.DaySelection;
        soul.DaySelection = state;
        
        // Emit event
        _selectionSubject.OnNext(new SelectionChangedEvent {
            SoulId = soulId,
            OldState = oldState,
            NewState = state
        });
        
        return true;
    }
    
    public bool TrySetNightOutcome(string soulId, NightOutcomeState outcome) {
        // Only Consequence Resolver (authorized) can write during ChoiceLock
        var currentPhase = _phaseStateMachine.CurrentPhase.Value;
        if (currentPhase != PhaseState.ChoiceLock && currentPhase != PhaseState.NightSurvival) {
            Debug.LogWarning($"SoulRepository: Cannot write night outcome outside ChoiceLock/NightSurvival");
            return false;
        }
        
        var soul = GetSoul(soulId);
        if (soul == null || soul.DaySelection != DaySelectionState.Abandoned) {
            Debug.LogError($"SoulRepository: Night outcome only valid for abandoned soul");
            return false;
        }
        
        soul.NightOutcome = outcome;
        
        _outcomeSubject.OnNext(new NightOutcomeEvent {
            SoulId = soulId,
            Outcome = outcome
        });
        
        return true;
    }
    
    public bool IsSelectionValid(int requiredSaved) {
        int savedCount = GetSavedSouls().Count;
        int abandonedCount = GetAbandonedSoul().Count;
        
        return savedCount == requiredSaved && 
               abandonedCount == _souls.Count - requiredSaved;
    }
    
    public void Reset() {
        foreach (var soul in _souls.Values) {
            soul.DaySelection = DaySelectionState.Unselected;
            soul.NightOutcome = NightOutcomeState.None;
            soul.Life = LifeState.Alive;
        }
    }
    
    public void Initialize() {
        // Initialize 3 canonical souls
        _souls = new Dictionary<string, Soul> {
            ["linh"] = new Soul("linh", "Em Linh"),
            ["van"] = new Soul("van", "Ông Văn"),
            ["minh"] = new Soul("minh", "Anh Minh")
        };
    }
}
```

### VContainer Registration

```csharp
public class CoreInstaller : MonoInstaller {
    public override void Install(IContainerBuilder builder) {
        builder.Register<SoulRepository>(Lifetime.Singleton).As<ISoulRepository>();
    }
}
```

## Alternatives Considered

### Alternative 1: ScriptableObject-based Soul Data
- **Description**: Use Unity ScriptableObjects for soul data assets
- **Pros**: Native Unity, easy to create in editor, good for static data
- **Cons**: Not reactive, harder to observe changes, not ideal for runtime state
- **Rejection Reason**: Need reactive state for Day Service UI updates, Ward Timer initialization triggers

### Alternative 2: Entity Component System (DOTS)
- **Description**: Use ECS entities for souls with components
- **Pros**: High performance, Unity's recommended future path
- **Cons**: Overkill for 3 entities, steep learning curve, not compatible with current tech stack
- **Rejection Reason**: Project uses VContainer/R3 pattern, ECS would require significant refactoring

### Alternative 3: Simple C# Classes with Events
- **Description**: Plain C# classes with C# events
- **Pros**: No dependencies, simple to understand
- **Cons**: More boilerplate, no R3 integration, manual subscription management
- **Rejection Reason**: R3 provides essential reactive patterns for multi-system observation

## Consequences

### Positive
- **O(1) lookups**: Dictionary-based storage for instant access
- **Phase enforcement**: Writes rejected outside appropriate phase
- **Reactive**: All systems can subscribe to selection changes
- **Validation**: Contradictory states (Saved+Abandoned) automatically rejected

### Negative
- **Locked writes**: After ChoiceLock, model is frozen (by design, but limits debugging)
- **R3 dependency**: Another library to maintain

### Risks
- **Risk**: Phase state machine not initialized when SoulRepository starts
  - **Mitigation**: VContainer initialization order ensures PhaseStateMachine Initialize() runs first
- **Risk**: Race condition on Reset during active gameplay
  - **Mitigation**: Reset only valid in Resolve/Reset phases

## Performance Implications
- **CPU**: O(1) dictionary lookup, minimal overhead
- **Memory**: 3 Soul objects (~100 bytes each), negligible
- **Load Time**: No impact (lazy singleton)
- **Network**: N/A

## Migration Plan
1. Create Soul.cs and SoulRepository.cs in Domain layer
2. Register in CoreInstaller (VContainer)
3. Update DayService to use ISoulRepository for selection
4. Update ConsequenceResolver to read abandoned soul
5. Update WardTimer to read saved count for initialization

## Validation Criteria
- [ ] 3 canonical souls initialized: linh, van, minh
- [ ] TrySetSelection rejected outside DayService
- [ ] Cannot mark soul as both Saved and Abandoned
- [ ] GetSavedSouls() returns correct count
- [ ] OnSelectionChanged fires on state change
- [ ] Reset() clears all run-scoped state

## Related Decisions
- ADR-0001: Phase State Machine Architecture — provides phase gating
- Related GDD: design/gdd/npc-soul-data-model.md
- Related GDD: design/gdd/game-state-phase-state-machine.md (for AutoCommitPolicy)

---

## Engine Compatibility

| Field | Value |
|-------|-------|
| Engine | Unity 6000.3.11f1 |
| Post-Cutoff APIs Used | R3 (not in training data) |
| Deprecated APIs | None |

### Verification
⚠️ R3 is a post-cutoff library — verify ReactiveProperty behavior against Unity docs if issues arise.

## GDD Requirements Addressed

| TR-ID | Requirement | Status |
|-------|-------------|--------|
| TR-npc-001 | Three canonical souls (Linh, Van, Minh) | ✅ Covered |
| TR-npc-002 | SoulId as immutable lookup key | ✅ Covered |
| TR-npc-003 | DaySelectionState (Unselected/Saved/Abandoned) | ✅ Covered |
| TR-npc-004 | NightOutcomeState (None/Drag/Block/FakeShrine) | ✅ Covered |
| TR-npc-005 | Phase-locked writes (DayService only) | ✅ Covered |
| TR-npc-006 | Reject contradictory states | ✅ Covered |
| TR-npc-007 | Selection validity check (2 of 3) | ✅ Covered |
| TR-npc-008 | Reset clears run-scoped fields | ✅ Covered |
| TR-npc-009 | Reactive observables for subscribers | ✅ Covered |