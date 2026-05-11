# Implementation Plan: Strike Warning Integration

## Overview

Five targeted gaps remain after the existing `StrikeWarningHandler`, `IStrikeWarningHandler`, and integration test suite were implemented. Tasks are ordered so each builds on the previous: fix the null-guard first (smallest, self-contained), then introduce the new `StrikeWarningController` service and its value object, wire the Player Controller MonoBehaviour, create the `StrikeWarningView` Presentation MonoBehaviour, and finally add property-based tests and the canvas smoke test.

The design document (`design.md`) is the authoritative reference for all interfaces, pseudocode, and correctness properties cited below.

---

## Tasks

- [x] 1. Add null-guard to `StrikeWarningHandler.ReportPlayerPosition`
  - Open `Assets/_Project/Application/Services/StrikeWarningHandler.cs`
  - In `ReportPlayerPosition`, before calling `_mapDirector.UpdatePlayerPosition(position, bounds)`, add:
    ```csharp
    if (_mapDirector == null)
    {
        Debug.LogWarning("[StrikeWarningHandler] MapDirector is null — skipping UpdatePlayerPosition.");
        return;
    }
    ```
  - The guard must sit inside the `mode == NightMovement` branch, after the early-return for non-Night modes
  - No other logic changes — the existing phase gate and call site remain unchanged
  - _Requirements: 5.4_

  - [ ]* 1.1 Add unit test for null Map Director guard
    - Extend `StrikeWarningIntegrationTests.cs` with a test that constructs `StrikeWarningHandler(null)` and asserts `ReportPlayerPosition` does not throw and does not call `UpdatePlayerPosition`
    - Use `Assert.DoesNotThrow` consistent with existing AC-5 tests
    - _Requirements: 5.4_

- [x] 2. Create `StrikeWarning` value object and `IStrikeWarningController` interface
  - Create `Assets/_Project/Application/Services/StrikeWarning.cs`
    - `readonly struct` with fields `int Id` and `Vector2 Position`
    - Allman braces, 4-space indent, `SolarPhobia.Application.Services` namespace
  - Create `Assets/_Project/Application/Services/Interfaces/IStrikeWarningController.cs`
    - Members: `ReadOnlyReactiveProperty<bool> IsWarningActive`, `IReadOnlyList<StrikeWarning> ActiveWarnings`, `void OnStrikeWarningReceived(bool, PlayerInputMode, Vector2)`, `void ReportPlayerPosition(Vector2, Bounds, PlayerInputMode)`, `void ClearAll()`
    - Copy XML doc comments verbatim from `design.md` § "New: `IStrikeWarningController`"
  - _Requirements: 1.3, 1.4, 3.1, 5.1_

- [x] 3. Implement `StrikeWarningController`
  - Create `Assets/_Project/Application/Services/StrikeWarningController.cs`
  - Implement `IStrikeWarningController` exactly as specified in `design.md` § "New: `StrikeWarningController`":
    - `ReactiveProperty<bool> _isWarningActive` initialised to `false`
    - `List<StrikeWarning> _activeWarnings` — newest at end (LIFO resolution)
    - `int _nextWarningId` — monotonically increasing
    - `Vector2 _playerPosition` — cached, updated in `ReportPlayerPosition` and `OnStrikeWarningReceived`
    - `[Inject]` constructor taking `IMapSpawnDirector`
    - `OnStrikeWarningReceived`: phase gate → `ClearAll()` if not NightMovement; else register (true) or LIFO-remove (false); call `Reevaluate()`
    - `ReportPlayerPosition`: phase gate; null-guard with `Debug.LogWarning`; update `_playerPosition`; call `_mapDirector.UpdatePlayerPosition`
    - `ClearAll()`: `_activeWarnings.Clear()` then `SetWarning(false)`
    - `Reevaluate()`: `SetWarning(_activeWarnings.Count > 0)`
    - `SetWarning(bool)`: only assign if value differs (no duplicate reactive events)
  - _Requirements: 1.3, 1.4, 1.5, 3.1, 3.2, 3.3, 3.4, 4.1, 4.2, 4.3, 4.4, 5.1, 5.2, 5.4_

  - [ ]* 3.1 Write unit tests for `StrikeWarningController` — phase gate and round trip
    - Create `Assets/_Project/Application/Editor/Tests/StrikeWarningControllerTests.cs`
    - Test: non-NightMovement mode → `IsWarningActive` stays false, `ActiveWarnings` stays empty
    - Test: single true then single false → `ActiveWarnings.Count == 0`, `IsWarningActive == false`
    - Test: unbalanced false (empty list) → no exception, no state change
    - Test: `ClearAll()` with active warnings → list empty, `IsWarningActive == false`
    - Test: null Map Director → `ReportPlayerPosition` does not throw
    - _Requirements: 1.5, 4.3, 4.4, 5.4_

  - [ ]* 3.2 Write property test — P1: Phase gate suppresses all warnings
    - Tag: `// Feature: strike-warning-integration, Property 1: non-NightMovement suppresses warnings`
    - Fixed seed `new System.Random(42)`, 100 iterations
    - Each iteration: pick random `PlayerInputMode` that is not `NightMovement`, random `bool` warning value; call `OnStrikeWarningReceived`; assert `IsWarningActive.CurrentValue == false` and `ActiveWarnings.Count == 0`
    - _Requirements: 1.5_

  - [ ]* 3.3 Write property test — P2: Register/deregister round trip
    - Tag: `// Feature: strike-warning-integration, Property 2: register then deregister is a round trip`
    - Fixed seed `new System.Random(42)`, 100 iterations
    - Each iteration: pick random N (1–10); send N `true` events then N `false` events in NightMovement; assert `ActiveWarnings.Count == 0` and `IsWarningActive.CurrentValue == false`
    - _Requirements: 1.3, 1.4, 4.1, 4.4_

  - [ ]* 3.4 Write property test — P3: Active list count tracks registrations
    - Tag: `// Feature: strike-warning-integration, Property 3: active list count tracks registrations`
    - Fixed seed `new System.Random(42)`, 100 iterations
    - Each iteration: generate random sequence of `true`/`false` events in NightMovement; track expected count as `max(0, trueCount - falseCount)`; assert `ActiveWarnings.Count == expectedCount`
    - _Requirements: 3.1, 3.4_

  - [ ]* 3.5 Write property test — P6: Phase exit clears all warnings
    - Tag: `// Feature: strike-warning-integration, Property 6: phase exit clears all warnings`
    - Fixed seed `new System.Random(42)`, 100 iterations
    - Each iteration: register random N (1–20) warnings in NightMovement; call `ClearAll()`; assert `ActiveWarnings.Count == 0` and `IsWarningActive.CurrentValue == false`
    - _Requirements: 4.3_

  - [ ]* 3.6 Write property test — P7: Position pass-through
    - Tag: `// Feature: strike-warning-integration, Property 7: position pass-through`
    - Create a `MockMapSpawnDirector` that records the last `(Vector2, Bounds)` passed to `UpdatePlayerPosition`
    - Fixed seed `new System.Random(42)`, 100 iterations
    - Each iteration: generate random `Vector2` and `Bounds`; call `ReportPlayerPosition` in NightMovement; assert mock received exact values
    - _Requirements: 5.2_

- [x] 4. Checkpoint — Ensure all `StrikeWarningController` tests pass
  - Run the EditMode test suite via Window → General → Test Runner
  - All tests in `StrikeWarningControllerTests.cs` must be green before proceeding
  - Ask the user if any test failures are unclear

- [x] 5. Wire `PlayerController` MonoBehaviour — strike warning subscription and position feed
  - Locate or create the `PlayerController` MonoBehaviour at `Assets/_Project/Presentation/Player/PlayerController.cs` (or the path established by the player-controller epic)
  - Add a private `IDisposable _strikeWarningSubscription` field
  - Inject `IStrikeWarningController _strikeWarningController` and `IMapSpawnDirector _mapDirector` via `[Inject]`
  - In the `NightSurvival` branch of `OnPhaseChanged()` (or equivalent phase-entry callback):
    - Set `_mode = PlayerInputMode.NightMovement`
    - Subscribe: `_strikeWarningSubscription = _mapDirector.OnStrikeWarning.Subscribe(active => _strikeWarningController.OnStrikeWarningReceived(active, _mode, (Vector2)transform.position));`
  - In the default/exit branch of `OnPhaseChanged()`:
    - Dispose and null the subscription
    - Call `_strikeWarningController.ClearAll()`
  - In `Update()`, inside a `_mode == PlayerInputMode.NightMovement` guard:
    - Retrieve `Collider2D col = GetComponent<Collider2D>()`
    - Call `_strikeWarningController.ReportPlayerPosition((Vector2)transform.position, col != null ? col.bounds : new Bounds(transform.position, Vector3.zero), _mode)`
  - If `PlayerController` does not yet exist, create a minimal stub with `[Inject]` fields, `OnPhaseChanged(PhaseState)`, and `Update()` — no movement logic required for this task
  - _Requirements: 1.1, 1.2, 5.1, 5.2, 5.3_

  - [ ]* 5.1 Write unit tests for `PlayerController` wiring
    - Test: entering NightSurvival phase creates a subscription (verify `OnStrikeWarning` subscriber count > 0, or use a mock that records subscription)
    - Test: exiting NightSurvival disposes subscription and calls `ClearAll()`
    - Test: `Update()` in NightMovement mode calls `ReportPlayerPosition` once per tick
    - Test: `Update()` outside NightMovement mode does NOT call `ReportPlayerPosition`
    - _Requirements: 1.1, 1.2, 5.1, 5.3_

- [x] 6. Create `StrikeWarningView` Presentation MonoBehaviour
  - Create `Assets/_Project/Presentation/Player/StrikeWarningView.cs`
  - Class must be `[RequireComponent(typeof(UIDocument))]`
  - Inspector field: `[SerializeField] private string _warningIconName = "warning-icon";`
  - Private fields: `VisualElement _warningIcon`, `IDisposable _subscription`
  - `[Inject]` field: `IStrikeWarningController _controller`
  - `Start()`:
    - Retrieve `UIDocument` via `GetComponent<UIDocument>()`
    - Query `_warningIcon = document.rootVisualElement.Q<VisualElement>(_warningIconName)`
    - If `_warningIcon == null`: log `Debug.LogError("[StrikeWarningView] VisualElement '{_warningIconName}' not found — disabling.")` and call `enabled = false; return;`
    - Subscribe: `_subscription = _controller.IsWarningActive.Subscribe(active => _warningIcon.style.display = active ? DisplayStyle.Flex : DisplayStyle.None);`
    - Set initial state: `_warningIcon.style.display = DisplayStyle.None;`
  - `OnDestroy()`: `_subscription?.Dispose()`
  - Namespace: `SolarPhobia.Presentation.Player`
  - _Requirements: 1.3, 2.1, 2.2, 2.3, 4.1, 4.4_

  - [ ]* 6.1 Write unit tests for `StrikeWarningView`
    - Test: `IsWarningActive` emitting `true` sets `DisplayStyle.Flex` on the icon element
    - Test: `IsWarningActive` emitting `false` sets `DisplayStyle.None` on the icon element
    - Test: missing element name → `Debug.LogError` called and component disabled (use a mock or test-only subclass)
    - _Requirements: 1.3, 4.1, 4.4_

- [x] 7. Add canvas sort order smoke test
  - Extend `StrikeWarningIntegrationTests.cs` with a single test `CanvasSortOrder_ReticleHUD_Is100`
  - Load the `PanelSettings` asset for the Reticle_HUD UIDocument (use `AssetDatabase.LoadAssetAtPath` or `Resources.Load`)
  - Assert `panelSettings.sortingOrder == 100`
  - This is a single example-based assertion, not property-based
  - _Requirements: 2.1, 2.3_

- [x] 8. Final checkpoint — Ensure all tests pass
  - Run the full EditMode test suite via Window → General → Test Runner
  - All tests in `StrikeWarningIntegrationTests.cs` and `StrikeWarningControllerTests.cs` must be green
  - Verify no compiler errors or warnings in the Unity console
  - Ask the user if any failures are unclear

---

## Notes

- Tasks marked with `*` are optional and can be skipped for a faster MVP
- Each task references specific requirements from `requirements.md` for traceability
- Property tests use fixed seed `new System.Random(42)` and 100 iterations per the project standard (see `AGENTS.md`)
- Property test tags follow the format: `// Feature: strike-warning-integration, Property N: <text>`
- The existing `StrikeWarningHandler` and its tests are **not modified** except for the null-guard in Task 1 — `StrikeWarningController` is the new multi-warning service
- `StrikeWarningView` uses UI Toolkit (`UIDocument` + `VisualElement`) consistent with `Act1HudView` — no uGUI Canvas component is created
- The `PanelSettings` sort order of 100 is set in the Inspector and never changed at runtime (Requirement 2.3)
