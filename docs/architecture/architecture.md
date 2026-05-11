# Solar Phobia — Master Architecture

## Document Status
- Version: 1.2
- Last Updated: 2026-05-07
- Engine: Unity 6000.3.11f1 (Unity 6)
- GDDs Covered: 10 approved system GDDs (game-state, npc-soul, map-spawn, health-stamina, shadow, day-night-camera, player-controller, day-service, shrine-objective, night-survival-run)
- Cross-System Issues: RESOLVED (4 blockers fixed 2026-05-07)
- ADRs Referenced: 4 (ADR-0001, ADR-0002, ADR-0003, ADR-0004)

---

## Engine Knowledge Gap Summary

**Risk Level**: HIGH — Unity 6 is beyond LLM training data (~2023 cutoff)

### HIGH RISK Domains
- **VContainer** — New DI package, post-cutoff
- **R3** — New reactive system, post-cutoff  
- **UI Toolkit** — Significant changes from uGUI
- **New Input System** — Required for Unity 6, not in training data

### Systems in HIGH RISK
- Phase State Machine → uses VContainer + R3
- NPC/Soul Data Model → uses R3 observables
- Player Controller → New Input System
- All other systems depend on these

⚠️ **Flagged**: All ADRs created from this architecture must include Engine Compatibility sections.

---

## System Layer Map

```
┌─────────────────────────────────────────────┐
│  PRESENTATION LAYER                         │  ← UI, HUD, audio, VFX
├─────────────────────────────────────────────┤
│  FEATURE LAYER                              │  ← gameplay systems, AI
├─────────────────────────────────────────────┤
│  CORE LAYER                                 │  ← physics, input, movement
├─────────────────────────────────────────────┤
│  FOUNDATION LAYER                           │  ← engine integration, state
├─────────────────────────────────────────────┤
│  PLATFORM LAYER                             │  ← Unity engine APIs
└─────────────────────────────────────────────┘
```

### Layer Assignments

| Layer | Systems |
|-------|---------|
| **Foundation** | Phase State Machine, Soul Data Repository, Map Spawn Director, Ward Timer, Shadow Manager, Camera Director, Save/Reset |
| **Core** | Player Controller, Day Selection UI, Shrine Objective, Physical Crowding, Tactile Rituals, Sensory Feedback |
| **Feature** | Consequence Resolver, Curse Effects, Boss Searchlight, Relic Collector, Night Survival Run |
| **Presentation** | HUD-less Design, Sensory Feedback Manager, Audio Mix Director |

---

## Module Ownership

### Foundation Layer

| Module | Owns | Exposes | Consumes |
|--------|------|---------|----------|
| **PhaseStateMachine** | Phase state, transitions, allowed-action masks | CurrentPhase (R3), OnPhaseChanged events | Game state data |
| **SoulDataRepository** | Soul entities (3 souls), saved/abandoned state | Souls, OnSoulStateChanged | None |
| **MapSpawnDirector** | Map chunks, spawn points, hazard placement, seed | GetSpawnPoints(), GenerateChunk() | Phase state |
| **WardTimer** | Ward value, drain rate, sensory tiers | CurrentWard (R3), OnWardDepleted | Phase state, SoulData |
| **ShadowManager** | Shadow polygon geometry, shrink state | ShadowBounds, OnShrinkComplete | Phase state |
| **CameraDirector** | Camera position, FOV, transitions | CameraTransform, OnTransitionComplete | Phase state |
| **SaveResetManager** | Run snapshot, deterministic seed | Save(), Load(), Reset() | All modules |

### Core Layer

| Module | Owns | Exposes | Consumes |
|--------|------|---------|----------|
| **PlayerController** | Player position, input, movement | Position, IsGrounded, OnInteract | PhaseStateMachine, InputSystem |
| **DaySelectionUI** | Selection state, ritual assignments | OnSelectionConfirmed | SoulDataRepository |
| **ShrineObjective** | Shrine zone, win/lose detection | OnShrineReached, OnPlayerDied | WardTimer, PhaseStateMachine |
| **TactileRituals** | Minigame state (diêm/rót/vay) | OnRitualComplete, HươngHỏaAwarded | None |

### Feature Layer

| Module | Owns | Exposes | Consumes |
|--------|------|---------|----------|
| **ConsequenceResolver** | Curse payload generation | GetCursePayload(sacrificedId) | DaySelectionUI, SoulData |
| **CurseEffectManager** | Active curse, hazard spawning | ApplyCurse(), RemoveCurse() | ConsequenceResolver, MapSpawn |
| **BossSearchlight** | Boss position, sweep, strike | OnSweepStart, OnStrikeHit | PlayerController |
| **RelicCollector** | Ngọc Cốt pickups | OnRelicPicked, CurrentMultiplier | WardTimer |
| **NightSurvivalRun** | Night loop state | OnNightStart, OnNightEnd | All Core systems |

### Presentation Layer

| Module | Owns | Exposes | Consumes |
|--------|------|---------|----------|
| **SensoryFeedbackManager** | Visual/audio tiers | UpdateTier(wardPercent) | WardTimer |
| **AudioMixDirector** | Audio snapshots | SetPhaseSnapshot() | PhaseStateMachine |

---

## Data Flow

### 1. Frame Update Path
```
Input System (New Input)
    → PlayerController.ProcessInput()
    → CharacterController.Move()
    → Player Position Updated
    → CameraDirector.Update()
    → Render
```

### 2. Event/Signal Path (R3 Reactive)
```
PhaseStateMachine._currentPhase
    .Subscribe(phase => {
        if (phase == PhaseState.NightSurvival)
            NightSurvivalRun.Enable();
    });

SoulDataRepository._souls
    .Where(s => s.IsSaved)
    .Subscribe(savedCount => 
        WardTimer.Initialize(savedCount));
```

### 3. Save/Load Path
```
PhaseStateMachine.GetSnapshot()
    → { Phase, Seed, SoulsSaved, SacrificeId, WardValue }

SaveResetManager.Save(snapshot)
    → PlayerPrefs.SetString("run_seed", JsonUtility.ToJson(snapshot))

Load → Restore all module states from snapshot
```

### 4. Initialisation Order
```
1. PhaseStateMachine (bootstrap)
2. SoulDataRepository (load persisted)
3. MapSpawnDirector (generate initial chunk)
4. CameraDirector (set initial position)
5. [Wait for player input] → Enable DayService
```

---

## API Boundaries

### IPhaseStateMachine
```csharp
public interface IPhaseStateMachine {
    ReadOnlyReactiveProperty<PhaseState> CurrentPhase { get; }
    IObservable<PhaseTransition> OnPhaseChanged { get; }
    
    void TransitionTo(PhaseState newPhase);
    bool TryTransition(PhaseState newPhase);
    bool IsActionAllowed(GameAction action);
}
```

### ISoulRepository
```csharp
public interface ISoulRepository {
    IReadOnlyList<Soul> Souls { get; }
    Soul GetSoul(string id);
    void SetSoulState(string id, SoulState state);
    IReadOnlyList<Soul> GetSavedSouls();
}
```

### IWardTimer
```csharp
public interface IWardTimer {
    ReactiveProperty<float> CurrentWard { get; }
    ReadOnlyReactiveProperty<SensoryTier> CurrentTier { get; }
    IObservable<Unit> OnDepleted { get; }
    
    void Initialize(int ghostsSaved, int dayPenalties);
    void ApplyPenalty(float seconds);
    void SetDrainMultiplier(float multiplier);
}
```

### IPlayerController
```csharp
public interface IPlayerController {
    Vector3 Position { get; }
    bool IsGrounded { get; }
    bool IsSprinting { get; }
    
    void Enable();  // Phase-gated
    void Disable(); // Phase-gated
    void ApplyForce(Vector3 force);
    event Action<string> OnInteract; // "swap", "shove", "shrine", "interact"
}
```

---

## ADR Audit

### Existing ADRs

| ADR | Title | Layer | GDD Linkage | Status |
|-----|-------|-------|-------------|--------|
| ADR-0001 | Phase State Machine Architecture | Foundation | game-state-phase-state-machine.md | ✅ Valid |
| ADR-0002 | Soul Data Repository Pattern | Foundation | npc-soul-data-model.md | ✅ Valid |
| ADR-0003 | Player Controller Pattern | Core | player-controller.md | ✅ Valid |
| ADR-0004 | Coding Standards & Scene Folder Structure | Foundation | AGENTS.md, Naming Conventions | ✅ Valid |

### Traceability Coverage

| Requirement ID | GDD | Requirement | ADR Coverage | Status |
|---------------|-----|-------------|--------------|--------|
| TR-state-001 | game-state-phase-state-machine.md | Phase state machine | ADR-0001 | ✅ |
| TR-state-002 | game-state-phase-state-machine.md | Day/Night transitions | ADR-0001 | ✅ |
| TR-player-001 | player-controller.md | WASD movement | ADR-0003 | ✅ |
| TR-ward-001 | health-stamina-damage-rules.md | Ward initialization | — | ❌ GAP |
| TR-npc-001 | npc-soul-data-model.md | Soul entity storage | ADR-0002 | ✅ |
| TR-map-001 | map-and-spawn-director.md | Procedural generation | — | ❌ GAP |
| TR-consequence-001 | consequence-resolver.md | Curse mapping | — | ❌ GAP |
| TR-shrine-001 | shrine-objective-win-lose-rules.md | Win/Lose detection | — | ❌ GAP |
| TR-night-001 | night-survival-run.md | Night loop | — | ❌ GAP |
| TR-shadow-001 | shadow-spatial-management.md | Shadow polygon | — | ❌ GAP |
| TR-camera-001 | day-night-camera-transition.md | Camera transition | — | ❌ GAP |

**Coverage**: 4/11 requirements covered. **7 gaps** requiring new ADRs.

---

## Required ADRs (Priority Order)

### Already Created ✅
1. **ADR-0001** — Phase State Machine Architecture
2. **ADR-0002** — Soul Data Repository Pattern
3. **ADR-0003** — Player Controller Pattern
4. **ADR-0004** — Coding Standards & Scene Folder Structure

### Still Needed (7 gaps)
5. **Ward Timer Implementation** — ReactiveProperty with sensory tiers
   - Covers: TR-ward-001
6. **Map Generation Strategy** — Deterministic seed + chunk spawning
   - Covers: TR-map-001
7. **Shrine Objective Logic** — Win/lose detection with phase gating
   - Covers: TR-shrine-001
8. **Consequence Resolver Pattern** — Curse payload generation from sacrifice
   - Covers: TR-consequence-001
9. **Night Survival Run Architecture** — Night loop integration
   - Covers: TR-night-001
10. **Shadow Spatial Management** — Shadow polygon + shrink mechanics
    - Covers: TR-shadow-001
11. **Day/Night Camera Transition** — Camera behavior + transition effects
    - Covers: TR-camera-001

---

## Architecture Principles

1. **Phase-Gated Activation** — Only systems required by current phase are active. PlayerController disabled during DayService, DaySelectionUI disabled during NightSurvival.

2. **Deterministic Reproducibility** — All randomness seeded. Map generation, spawn placement, and run outcomes must be reproducible from seed.

3. **Single Lethal Resource** — Ward Timer is the only death condition. No health, no other counters. All feedback (sensory tiers) derives from this one source.

4. **Reactive Data Flow** — R3 ReactiveProperty for all state that multiple systems observe. No polling, no direct coupling.

5. **Consequence Immediacy** — Day choice → Night consequence is direct and visible. Karma hazards spawn based on exact sacrifice ID, not abstract "bad choices."

---

## Open Questions

| Question | Owner | Deadline | Notes |
|-----------|-------|----------|-------|
| Rendering pipeline (URP/HDRP) | Art Director | Before Feature ADRs | Affects lighting/shader patterns |
| Physics 2D vs 3D | Gameplay Programmer | Before Player Controller ADR | Game uses 2D side-scrolling but Unity 3D physics |
| Multiplayer consideration | Not MVP | Deferred | Single-player only for vertical slice |

---

## Next Steps

1. **ADRs already created**: ADR-0001 (Phase State Machine), ADR-0002 (Soul Repository), ADR-0003 (Player Controller), ADR-0004 (Coding Standards)
2. **Create remaining ADRs** for the 7 coverage gaps (Ward Timer, Map, Shrine, Consequence, Night Run, Shadow, Camera)
3. Run `/create-control-manifest` to produce the layer rules manifest
4. Run `/gate-check pre-production` when all required ADRs are written

**Priority order for remaining ADRs**:
- Ward Timer Implementation (survival core)
- Map Generation Strategy (procedural foundation)
- Consequence Resolver Pattern (game identity)