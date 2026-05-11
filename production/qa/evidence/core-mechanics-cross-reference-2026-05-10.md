# Core Mechanics Cross-Reference (GDD -> Implementation)

Date: 2026-05-10

## Systems mapped
- Phase State Machine
  - GDD: `design/gdd/game-state-phase-state-machine.md`
  - Code: `Assets/_Project/Application/Services/PhaseStateMachine.cs`

- Player State Machine
  - GDD: `design/gdd/player-controller.md`
  - Code: `Assets/_Project/Application/Services/PlayerStateMachine.cs`

- Day Phase Timeline
  - GDD: `design/gdd/day-service-and-selection.md`
  - Code: `Assets/_Project/Application/Services/DayPhaseTimelineService.cs`

- Day Phase Mechanics
  - GDD: `design/gdd/day-service-and-selection.md`
  - Code: `Assets/_Project/Application/Services/DayPhaseMechanicsService.cs`

- Map and Spawn Director
  - GDD: `design/gdd/map-and-spawn-director.md`
  - Code: `Assets/_Project/Application/Services/MapSpawnDirector.cs`

- Cover Detection
  - GDD: `design/gdd/shadow-spatial-management.md`
  - Code: `Assets/_Project/Application/Services/CoverDetector2D.cs`

- Karma Hazards
  - GDD: `design/gdd/curse-effect-modules.md`
  - Code: `Assets/_Project/Application/Services/KarmaHazardService.cs`

- Ward Timer
  - GDD: `design/gdd/night-survival-run.md`
  - Code: `Assets/_Project/Infrastructure/Services/WardTimerService.cs`

- Sensory Tiers
  - GDD: `design/gdd/sensory-feedback-system.md`
  - Code: `Assets/_Project/Infrastructure/Services/SensoryTierService.cs`

- Strike Warning Integration
  - GDD: `design/gdd/player-controller.md`
  - Code: `Assets/_Project/Application/Services/StrikeWarningController.cs`
  - Presentation: `Assets/_Project/Presentation/Player/PlayerController.cs`

- Phase-Gated Input
  - GDD: `design/gdd/player-controller.md`
  - Code: `Assets/_Project/Application/Services/PlayerInputHandler.cs`

## Test evidence coverage
- Application EditMode tests: 27 files under `Assets/_Project/Application/Editor/Tests`
- Additional test files under `_Project`: total 30 `*Tests.cs`

## Notes
- Implementation is organized under `Assets/_Project` (Unity-first layout), not `src/`.
- Gate policy should treat `Assets/_Project` as canonical runtime code location.
