# Story 001: Curse Effect Core — Hazard Spawning System

> **Epic**: curse-effect-modules
> **Status**: Ready
> **Layer**: Feature
> **Type**: Logic
> **Manifest Version**: N/A
> **Estimate**: 4 hours (M)

## Context

**GDD**: `design/gdd/curse-effect-modules.md`
**Requirement**: Correct hazard spawns based on abandoned soul

**ADR Governing Implementation**: None — no ADR exists for this system yet

**Engine**: Unity 6000.3.11f1 (Unity 6) | **Risk**: MEDIUM
**Engine Notes**: Uses VContainer for DI, R3 for reactive state

**Control Manifest Rules (this layer)**: N/A — manifest not yet created

---

## Performance

No performance impact expected — curse state machine is event-driven with minimal per-frame logic. Hazard zone detection uses Unity physics triggers (OnTriggerEnter/Exit) which are optimized by the engine.

---

## Acceptance Criteria

*From GDD `design/gdd/curse-effect-modules.md`, scoped to this story:*

- [ ] Receives curse_type (Drag/Block/FakeShrine) from Consequence Resolver at night start
- [ ] Idle state when not NightSurvival — no hazards spawned
- [ ] CurseActive state when NightSurvival + curse received — spawn and manage hazards
- [ ] HazardTriggered state when player enters hazard zone
- [ ] HazardCleared state when player exits hazard zone

---

## Implementation Notes

*Core state machine for curse effect management:*

- Subscribe to ConsequenceResolver for curse_type payload
- Use R3 ReactiveProperty for current curse state (Idle/CurseActive)
- Use R3 Subject for hazard triggered/cleared events
- Coordinate with MapSpawnDirector for hazard placement
- Coordinate with SensoryFeedbackSystem for visual/audio activation

---

## Out of Scope

*Handled by neighbouring stories — do not implement here:*

- Story 002: Water Trap specific effects (DoT)
- Story 003: Blood Net specific effects (slow + penalty)
- Story 004: Illusion Platform specific effects (collapse)
- Story 005: Visual/Audio per curse

---

## QA Test Cases

**[For Logic stories — automated test specs]:**

- **AC-1**: Idle state when not night phase
  - Given: Game phase is DayService
  - When: CurseEffectManager initializes
  - Then: Current state is Idle, no hazards spawned
  
- **AC-2**: CurseActive state receives curse
  - Given: Game phase is NightSurvival, ConsequenceResolver sends curse_type
  - When: OnCurseReceived event fires
  - Then: Current state is CurseActive

- **AC-3**: HazardTriggered on player entry
  - Given: CurseActive state, hazard zone exists
  - When: Player collider enters hazard trigger
  - Then: HazardTriggered state, effect begins

- **AC-4**: HazardCleared on player exit
  - Given: HazardTriggered state
  - When: Player collider exits hazard trigger
  - Then: HazardCleared state, effect ends

---

## Test Evidence

**Story Type**: Logic
**Required evidence**: `Assets/_Project/Application/Editor/Tests/CurseEffectCoreTests.cs` — must exist and pass

**Status**: [ ] Not yet created

---

## Dependencies

- Depends on: Consequence Resolver epic (must send curse_type)
- Unlocks: Stories 002-005 (specific hazard effects)