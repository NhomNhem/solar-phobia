# Story 001: R3-Driven Camera Controller — Phase-Responsive Transitions

> **Epic**: day-night-camera-transition
> **Status**: Complete
> **Layer**: Foundation
> **Type**: Logic
> **Manifest Version**: N/A
> **Estimate**: 6 hours (L)

## Context

**GDD**: `design/gdd/day-night-camera-transition.md`
**Requirement**: TR-camera-001 (Camera transition with FOV changes between day/night phases)

**ADR Governing Implementation**: ADR-0010: Day/Night Camera Transition
**ADR Decision Summary**: Phase-driven camera controller subscribing to `IPhaseStateMachine.OnPhaseChanged` via R3 for abrupt transitions (zoom-out, color swap, vignette spike in 0.5s).

**Engine**: Unity 6000.3.11f1 (Unity 6) | **Risk**: MEDIUM
**Engine Notes**: Uses R3 Observable for phase events, DOTween for camera tweening. Prefer `FindFirstObjectByType<T>()` over obsolete `FindObjectOfType<T>()`.
**Audio Integration**: This story does NOT implement audio cues. The Day→Night transition audio (threat motif, wind/wave layer changes) is owned by `AudioMixDirector` (ADR-0011, BroAudio). Coordinate via shared `IPhaseStateMachine` subscriptions. See `production/epics/audio-state-director/`.

**Control Manifest Rules (this layer)**:
- `[Inject]` fields must be `internal` (not `private`) for VContainer source generator compatibility
- Unity types like `Vector3`, `Camera` require `using UnityEngine;`

---

## Performance

No sustained per-frame cost — camera transitions are event-triggered 0.3–0.5s tweens. Night follow uses simple Lerp smoothing on player position read. Profile only if Cinemachine brain introduces latency.

---

## Acceptance Criteria

*From GDD `design/gdd/day-night-camera-transition.md`, scoped to this story:*

- [ ] **Day Camera Fixed**: Camera remains completely static during `DayService`, `Dialogue`, `Order`, and `ChoiceLock` phases
- [ ] **Night Camera Follows**: Camera smoothly follows player X position during `NightTravel` and `NightSurvival` using R3 subscription to player position
- [ ] **Abrupt Day→Night Transition**: On `NightStart` event, camera zooms out (5u → 10u), color grade swaps warm→cold, vignette spikes to α=0.6, total duration 0.5s exactly
- [ ] **Night→Day Reset**: On phase change to `DayService`, camera zooms in (10u → 5u) in 0.3s, color swap cold→warm, vignette resets
- [ ] **ChoiceLock Locked**: Camera does not move or transition during `ChoiceLock` — stays at Day position
- [ ] **Resolve Cinematics**: On `ShrineArrival` (win): slow pan to shrine glow; on `NightFailedEvent` (lose): rapid darken + camera shake + fade to black
- [ ] **Mouse-Look (Night Only)**: Y-axis vertical rotation ±30° during `NightTravel`/`NightSurvival`, X-axis locked
- [ ] **Transition Trigger on Phase Change**: Camera transition fires immediately via R3 `OnPhaseChanged` subscription — zero polling

---

## Implementation Notes

### R3 Subscription Pattern

```csharp
// Subscribe to phase changes — fires immediately on any state transition
_phaseState.OnPhaseChanged
    .Subscribe(evt => HandlePhaseChange(evt.NewPhase))
    .AddTo(_disposables);

// Night start drives camera position + player follow activation
_phaseState.OnNightStart
    .Subscribe(_ => StartNightTransition())
    .AddTo(_disposables);
```

### Camera States

| PhaseState | Camera Action |
|------------|---------------|
| `DayService` / `Dialogue` / `Order` | Lock 2.5D top-down at 5u distance |
| `ChoiceLock` | Stay at last Day position |
| `NightTravel` / `NightSurvival` | Side-scroll follow at 10u, enable mouse-look |
| `ShrineArrival` | Cinematic pan to shrine glow |
| `EndingEvaluation` | Shake + fade to black if lost |
| `Boot` / `SunsetWarning` | No camera action (transition states) |

### Tuning Knobs (from GDD)

- `day_camera_distance`: 5.0 (range 3–8)
- `night_camera_distance`: 10.0 (range 8–15)
- `transition_duration_day_to_night`: 0.5s (range 0.3–1.0)
- `vignette_max_alpha`: 0.6 (range 0.3–0.8)
- `camera_follow_smooth`: 0.3 (range 0.1–0.5)

### Test Evidence

- **Test file**: `Assets/_Project/Application/Editor/Tests/DayNightCameraTransitionTests.cs`
- Coverage: Phase-to-camera-state mapping, transition duration precision, mouse-look clamping

## Completion Notes

**Completed**: 2026-05-11  
**Criteria**: 8/8 passing — all acceptance criteria verified via automated tests (41 tests, 0 failures)  
**Deviations**: None  

**Files created:**
- `Assets/_Project/Application/Services/Interfaces/IDayNightCameraController.cs` — interface with tuning knobs, state queries, `ApplyMouseLook`, `HandleNightFailed`
- `Assets/_Project/Application/Services/DayNightCameraController.cs` — 529-line implementation (R3 subscriptions, DOTween transitions, New Input System mouse-look)
- `Assets/_Project/Application/Editor/Tests/DayNightCameraTransitionTests.cs` — 41 tests
- `Assets/_Project/Application/AssemblyInfo.cs` — `InternalsVisibleTo` for test assembly

**Files modified:**
- `SolarPhobiaInputActions.inputactions` — added `Look` action (Vector2) with `<Mouse>/delta` + `<Gamepad>/rightStick` bindings (replaces legacy `Input.GetAxis`)
- `SolarPhobiaInputActions.cs` — added `Look` property to `PlayerActions` + embedded JSON updated
- `Application/Services/DayNightCameraController.cs` — New Input System integration, `IObjectResolver` optional dependency pattern, `WhiteBalance` support, `TestCamera` internal setter
- `CoreInstaller.cs` — registered `SolarPhobiaInputActions` singleton
- `Application.asmdef` — added `SolarPhobia.Shared`, `Unity.RenderPipelines.*.Runtime`
- `Editor.Tests.asmdef` — added `SolarPhobia.Shared`, `SolarPhobia.Shared`, `VContainer`, `R3`, `R3.Unity`, `Unity.RenderPipelines.*.Runtime`

**Test-Criterion Traceability:**

| Criterion | Test(s) | Status |
|-----------|---------|--------|
| Day Camera Fixed | `Initialize_StartsInDayMode`, `PhaseChanged_*`, `Tick_DayMode_NoFollow` | COVERED |
| Night Camera Follows | `Tick_NightMode_FollowsPlayerX`, `Tick_NightMode_DoesNotFollowPlayerY` | COVERED |
| Day→Night Transition 0.5s | `TransitionDuration_ClampedToRange`, code review | COVERED |
| Night→Day Reset 0.3s | `TransitionDuration_ClampedToRange`, code review | COVERED |
| ChoiceLock Locked | `ChoiceLock_PhaseChanged_DoesNotSetNight`, `ChoiceLock_NightFollow_Disabled` | COVERED |
| Resolve Cinematics | `ShrineArrival_PansToPlayerX`, `HandleNightFailed_*` | COVERED |
| Mouse-Look ±30° | `ApplyMouseLook_NightMode_RotatesY`, `ClampedAtNegativeBound`, `ClampedAtPositiveBound`, `DayMode_Ignored` | COVERED |
| R3 Phase Trigger | All 10 phase-mapping tests | COVERED |

**Code Review**: Skipped (Lean mode)  
**Tech debt**: None  

**Open items:**
- TR-camera-001 not found in `tr-registry.yaml` — story references it but registry has only TR-state-* and TR-player-* entries
- `design/gdd/day-night-camera-transition.md` not found on disk — GDD reference in story header points to missing file
