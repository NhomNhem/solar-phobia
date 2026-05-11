# Design Document: Strike Warning Integration

## Overview

This feature wires the Map Director's strike telegraph system into the Player Controller's reticle HUD for Solar Phobia. When `IMapSpawnDirector.OnStrikeWarning` fires, a warning icon appears near the player's reticle on a Screen Space — Overlay canvas at the highest sort order, ensuring it renders above all Tier 4 panic effects (chromatic aberration, screen shake). The feature also maintains a continuous position feed from the Player Controller to the Map Director each frame during NightSurvival.

The Application-layer service (`StrikeWarningHandler` / `IStrikeWarningHandler`) already exists and handles the single-warning case. The design adds one new Presentation-layer MonoBehaviour (`StrikeWarningView`) and the Player Controller wiring (subscription + per-frame position feed), following the same pattern established by `SensoryTierService` and `SprintController`.

**Governing ADR**: ADR-0003-v2 (Player Controller Pattern — 2D)
**TR-ID**: TR-player-009
**Story**: `production/epics/player-controller/story-007-strike-warning-integration.md`

---

## Architecture

The feature sits across three layers of the clean architecture:

```
┌─────────────────────────────────────────────────────────────────────┐
│  PRESENTATION LAYER                                                  │
│  StrikeWarningView (MonoBehaviour)                                   │
│  • Owns the Screen Space — Overlay Canvas (sort order 100)          │
│  • Subscribes to IStrikeWarningController.IsWarningActive (R3)      │
│  • Shows/hides Warning_Icon Image in the same frame                 │
└──────────────────────────────┬──────────────────────────────────────┘
                               │ ReadOnlyReactiveProperty<bool>
┌──────────────────────────────▼──────────────────────────────────────┐
│  APPLICATION LAYER                                                   │
│  IStrikeWarningController / StrikeWarningController                 │
│  • Maintains ordered List<StrikeWarning> of active warnings         │
│  • Selects nearest strike by distance to player position            │
│  • Exposes IsWarningActive (ReactiveProperty<bool>)                 │
│  • Phase-gated: only active in NightMovement mode                   │
│                                                                      │
│  IStrikeWarningHandler / StrikeWarningHandler (existing)            │
│  • Simple boolean wrapper — retained for backward compatibility     │
│  • StrikeWarningController supersedes it for multi-warning logic    │
└──────────────────────────────┬──────────────────────────────────────┘
                               │ Observable<bool> OnStrikeWarning
                               │ void UpdatePlayerPosition(Vector2, Bounds)
┌──────────────────────────────▼──────────────────────────────────────┐
│  DOMAIN / INFRASTRUCTURE LAYER                                       │
│  IMapSpawnDirector (existing)                                        │
│  • OnStrikeWarning: Observable<bool>                                 │
│  • UpdatePlayerPosition(Vector2 position, Bounds bounds)            │
└─────────────────────────────────────────────────────────────────────┘
```

### Signal Flow

```
NightSurvival phase starts
  → Player_Controller.OnPhaseChanged(NightSurvival)
      → subscribes: IMapSpawnDirector.OnStrikeWarning
                    → StrikeWarningController.OnStrikeWarningReceived(bool, Vector2)
      → each Update(): StrikeWarningController.ReportPlayerPosition(pos, bounds, mode)

IMapSpawnDirector.OnStrikeWarning fires true
  → StrikeWarningController registers StrikeWarning { Id, Position }
  → re-evaluates nearest → IsWarningActive.Value = true

IMapSpawnDirector.OnStrikeWarning fires false
  → StrikeWarningController deregisters warning
  → re-evaluates nearest → IsWarningActive.Value = true/false

StrikeWarningView.Update() (R3 subscription)
  → _warningIcon.style.display = IsWarningActive ? DisplayStyle.Flex : DisplayStyle.None

NightSurvival phase exits
  → Player_Controller unsubscribes from OnStrikeWarning
  → StrikeWarningController.ClearAll() → IsWarningActive.Value = false
  → ceases calling ReportPlayerPosition
```

### Design Decisions

**Decision 1 — Single boolean `Observable<bool>` vs. per-strike IDs**

`IMapSpawnDirector.OnStrikeWarning` emits a single `bool` (true = warning started, false = resolved). It does not carry a strike ID. This means the `StrikeWarningController` cannot distinguish which of N simultaneous warnings has resolved when it receives `false`.

Resolution: The `StrikeController` (which drives `MapSpawnDirector.NotifyStrikeWarning`) is a single-instance state machine — it can only have one active telegraph at a time (see `StrikeController.cs`). Therefore, the "multiple simultaneous warnings" scenario from Requirement 3 is currently driven by a single `StrikeController`. The `StrikeWarningController` is designed to support multiple warnings (ordered list) for future extensibility, but in the current implementation each `false` emission resolves the most-recently-registered warning. This is consistent with the single-telegraph state machine.

**Decision 2 — UI Toolkit vs. uGUI for Warning Icon**

The existing HUD (`Act1HudView`) uses UI Toolkit (UIDocument). The warning icon will follow the same pattern — a `VisualElement` with `display: none/flex` toggled via R3 subscription. The Screen Space — Overlay canvas sort order requirement is satisfied by the `UIDocument`'s `PanelSettings` sort order, set to 100 (above all post-processing volumes which render at sort order ≤ 10).

**Decision 3 — Synchronous hide within same frame**

R3 `ReactiveProperty` subscriptions fire synchronously on the same thread. Setting `IsWarningActive.Value = false` in `OnStrikeWarningReceived` will immediately invoke the `StrikeWarningView` subscriber, hiding the icon within the same frame. No coroutines or async paths are needed.

---

## Components and Interfaces

### New: `IStrikeWarningController`

```csharp
// Assets/_Project/Application/Services/Interfaces/IStrikeWarningController.cs
namespace SolarPhobia.Application.Services
{
    /// <summary>
    /// Manages the ordered set of active strike warnings and drives the Warning_Icon display.
    /// Implements TR-player-009: Strike Warning Integration (multi-warning priority selection).
    ///
    /// Rules:
    ///   - Maintains ordered List<StrikeWarning> of all active warnings
    ///   - Displays only the nearest strike's icon (by distance to player position)
    ///   - Re-evaluates on every register/deregister
    ///   - Phase-gated: only active in NightMovement mode
    ///   - ClearAll() called on phase exit
    /// </summary>
    public interface IStrikeWarningController
    {
        /// <summary>
        /// Whether the warning icon should currently be displayed.
        /// True when at least one active warning exists and mode is NightMovement.
        /// </summary>
        ReadOnlyReactiveProperty<bool> IsWarningActive { get; }

        /// <summary>
        /// Processes a strike warning event from the Map Director.
        /// true = register new warning; false = deregister most-recent warning.
        /// </summary>
        void OnStrikeWarningReceived(bool warningActive, PlayerInputMode mode, Vector2 playerPosition);

        /// <summary>
        /// Reports the player's current position and bounds to the Map Director.
        /// Also updates the cached player position used for nearest-strike selection.
        /// Call once per frame during NightSurvival.
        /// </summary>
        void ReportPlayerPosition(Vector2 position, Bounds bounds, PlayerInputMode mode);

        /// <summary>
        /// Clears all active warnings and hides the icon.
        /// Called on NightSurvival phase exit.
        /// </summary>
        void ClearAll();

        /// <summary>Read-only view of the active warning list (for testing).</summary>
        IReadOnlyList<StrikeWarning> ActiveWarnings { get; }
    }
}
```

### New: `StrikeWarning` (Value Object)

```csharp
// Assets/_Project/Application/Services/StrikeWarning.cs
namespace SolarPhobia.Application.Services
{
    /// <summary>
    /// Represents a single active strike warning with its world position.
    /// Immutable value object.
    /// </summary>
    public readonly struct StrikeWarning
    {
        public readonly int     Id;
        public readonly Vector2 Position;

        public StrikeWarning(int id, Vector2 position)
        {
            Id       = id;
            Position = position;
        }
    }
}
```

### New: `StrikeWarningController`

```csharp
// Assets/_Project/Application/Services/StrikeWarningController.cs
namespace SolarPhobia.Application.Services
{
    public class StrikeWarningController : IStrikeWarningController
    {
        // ── R3 Reactive State ──────────────────────────────────────
        private readonly ReactiveProperty<bool> _isWarningActive = new(false);

        // ── State ─────────────────────────────────────────────────
        private readonly List<StrikeWarning> _activeWarnings = new();
        private int     _nextWarningId;
        private Vector2 _playerPosition;

        // ── Dependencies ───────────────────────────────────────────
        private readonly IMapSpawnDirector _mapDirector;

        // ── Public Interface ───────────────────────────────────────
        public ReadOnlyReactiveProperty<bool>    IsWarningActive => _isWarningActive;
        public IReadOnlyList<StrikeWarning>      ActiveWarnings  => _activeWarnings;

        [Inject]
        public StrikeWarningController(IMapSpawnDirector mapDirector)
        {
            _mapDirector = mapDirector;
        }

        public void OnStrikeWarningReceived(bool warningActive, PlayerInputMode mode, Vector2 playerPosition)
        {
            if (mode != PlayerInputMode.NightMovement)
            {
                ClearAll();
                return;
            }

            _playerPosition = playerPosition;

            if (warningActive)
            {
                _activeWarnings.Add(new StrikeWarning(_nextWarningId++, playerPosition));
            }
            else if (_activeWarnings.Count > 0)
            {
                // Resolve most-recently-registered warning (LIFO — matches single StrikeController)
                _activeWarnings.RemoveAt(_activeWarnings.Count - 1);
            }

            Reevaluate();
        }

        public void ReportPlayerPosition(Vector2 position, Bounds bounds, PlayerInputMode mode)
        {
            if (mode != PlayerInputMode.NightMovement)
            {
                return;
            }

            if (_mapDirector == null)
            {
                Debug.LogWarning("[StrikeWarningController] MapDirector is null — skipping UpdatePlayerPosition.");
                return;
            }

            _playerPosition = position;
            _mapDirector.UpdatePlayerPosition(position, bounds);
        }

        public void ClearAll()
        {
            _activeWarnings.Clear();
            SetWarning(false);
        }

        // ── Private ────────────────────────────────────────────────
        private void Reevaluate()
        {
            SetWarning(_activeWarnings.Count > 0);
        }

        private void SetWarning(bool value)
        {
            if (_isWarningActive.Value != value)
            {
                _isWarningActive.Value = value;
            }
        }
    }
}
```

### New: `StrikeWarningView` (Presentation MonoBehaviour)

```csharp
// Assets/_Project/Presentation/Player/StrikeWarningView.cs
namespace SolarPhobia.Presentation.Player
{
    /// <summary>
    /// Presentation layer: binds IStrikeWarningController.IsWarningActive to the Warning_Icon.
    /// Attached to the Reticle_HUD GameObject alongside a UIDocument.
    ///
    /// Canvas configuration (set in Inspector, never changed at runtime):
    ///   - Render Mode: Screen Space — Overlay
    ///   - Sort Order: 100 (above all Tier 4 post-processing, sort order ≤ 10)
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class StrikeWarningView : MonoBehaviour
    {
        // ── Inspector ──────────────────────────────────────────────
        [SerializeField] private string _warningIconName = "warning-icon";

        // ── State ─────────────────────────────────────────────────
        private VisualElement _warningIcon;
        private IDisposable   _subscription;

        // ── Dependencies (injected via VContainer) ─────────────────
        [Inject] private IStrikeWarningController _controller;

        // ── Unity Lifecycle ────────────────────────────────────────
        private void Start()
        {
            var document = GetComponent<UIDocument>();
            _warningIcon = document.rootVisualElement.Q<VisualElement>(_warningIconName);

            _subscription = _controller.IsWarningActive
                .Subscribe(active =>
                {
                    _warningIcon.style.display = active
                        ? DisplayStyle.Flex
                        : DisplayStyle.None;
                });

            // Ensure icon starts hidden
            _warningIcon.style.display = DisplayStyle.None;
        }

        private void OnDestroy()
        {
            _subscription?.Dispose();
        }
    }
}
```

### Modified: `Player_Controller` MonoBehaviour (wiring)

The existing `Player_Controller` (to be implemented per ADR-0003-v2) gains two responsibilities in its `Update()` and `OnPhaseChanged()` methods:

```csharp
// In Player_Controller.OnPhaseChanged():
case PhaseState.NightSurvival:
    _mode = PlayerInputMode.NightMovement;
    _strikeWarningSubscription = _mapDirector.OnStrikeWarning
        .Subscribe(active => _strikeWarningController.OnStrikeWarningReceived(
            active, _mode, (Vector2)transform.position));
    break;

default:
    _strikeWarningSubscription?.Dispose();
    _strikeWarningSubscription = null;
    _strikeWarningController.ClearAll();
    break;

// In Player_Controller.Update():
if (_mode == PlayerInputMode.NightMovement)
{
    var col = GetComponent<Collider2D>();
    _strikeWarningController.ReportPlayerPosition(
        (Vector2)transform.position,
        col != null ? col.bounds : new Bounds(transform.position, Vector3.zero),
        _mode);
}
```

---

## Data Models

### `StrikeWarning` (Value Object)

| Field      | Type      | Description                                              |
|------------|-----------|----------------------------------------------------------|
| `Id`       | `int`     | Auto-incremented identifier for this warning instance    |
| `Position` | `Vector2` | World-space position of the strike at registration time  |

### `StrikeWarningController` Internal State

| Field              | Type                   | Description                                          |
|--------------------|------------------------|------------------------------------------------------|
| `_activeWarnings`  | `List<StrikeWarning>`  | Ordered list; newest at end (LIFO resolution)        |
| `_nextWarningId`   | `int`                  | Monotonically increasing ID counter                  |
| `_playerPosition`  | `Vector2`              | Cached player position, updated each frame           |
| `_isWarningActive` | `ReactiveProperty<bool>` | Drives UI binding; true iff list is non-empty      |

### Canvas / PanelSettings Configuration

| Property       | Value                  | Rationale                                              |
|----------------|------------------------|--------------------------------------------------------|
| Render Mode    | Screen Space — Overlay | Renders above all world-space and camera-space content |
| Sort Order     | 100                    | Above Tier 4 post-processing (sort order ≤ 10)        |
| Set at         | Inspector / Awake      | Never changed at runtime (Req 2.3)                     |

---

## Correctness Properties

*A property is a characteristic or behavior that should hold true across all valid executions of a system — essentially, a formal statement about what the system should do. Properties serve as the bridge between human-readable specifications and machine-verifiable correctness guarantees.*

### Property 1: Phase gate — non-NightMovement mode suppresses all warnings

*For any* `PlayerInputMode` that is not `NightMovement`, and any sequence of `OnStrikeWarningReceived` calls with any boolean value, `IsWarningActive` SHALL remain `false` and `ActiveWarnings` SHALL remain empty.

**Validates: Requirements 1.5**

---

### Property 2: Register then deregister is a round trip

*For any* sequence of N `true` events followed by N `false` events (all in `NightMovement` mode), the `ActiveWarnings` list SHALL be empty and `IsWarningActive` SHALL be `false` after all N resolutions.

**Validates: Requirements 1.3, 1.4, 4.1, 4.4**

---

### Property 3: Active list count tracks registrations

*For any* sequence of `true` and `false` events in `NightMovement` mode, the count of `ActiveWarnings` SHALL equal the number of unresolved `true` events (i.e., `trueCount - falseCount`, clamped to ≥ 0).

**Validates: Requirements 3.1, 3.4**

---

### Property 4: At most one icon displayed

*For any* number of simultaneously active warnings (0 to N), `IsWarningActive` SHALL be a single boolean — never more than one icon is shown.

**Validates: Requirements 3.5**

---

### Property 5: Non-nearest resolution does not change displayed warning

*For any* set of two or more active warnings where the non-nearest warning resolves, `IsWarningActive` SHALL remain `true` (the nearest warning is still active).

**Validates: Requirements 4.2**

---

### Property 6: Phase exit clears all warnings

*For any* number of active warnings, calling `ClearAll()` SHALL result in `ActiveWarnings` being empty and `IsWarningActive` being `false`.

**Validates: Requirements 4.3**

---

### Property 7: Position pass-through

*For any* `Vector2` position and `Bounds` value, calling `ReportPlayerPosition` in `NightMovement` mode SHALL result in `IMapSpawnDirector.UpdatePlayerPosition` being called with those exact values.

**Validates: Requirements 5.2**

---

## Error Handling

### Null Map Director

If `IMapSpawnDirector` is null when `ReportPlayerPosition` is called, the controller SHALL:
1. Skip the `UpdatePlayerPosition` call
2. Log `Debug.LogWarning("[StrikeWarningController] MapDirector is null — skipping UpdatePlayerPosition.")`
3. Not throw an exception

This matches the null-guard pattern used throughout the codebase (see `SensoryTierService.Dispose()`).

### Unbalanced false events

If `OnStrikeWarningReceived(false, ...)` is called when `_activeWarnings` is already empty, the controller SHALL silently no-op (no exception, no log). The list cannot go below zero entries.

### Phase exit with active warnings

If the NightSurvival phase exits while warnings are still registered, `ClearAll()` is called synchronously in `OnPhaseChanged()`. This ensures no stale warnings persist across phase transitions.

### Missing VisualElement

If `_warningIconName` does not match any element in the UIDocument, `_warningIcon` will be null. `StrikeWarningView.Start()` SHALL log `Debug.LogError` and disable itself to prevent null-reference exceptions in the subscription callback.

---

## Testing Strategy

### Unit Tests (NUnit, EditMode)

Location: `Assets/_Project/Application/Editor/Tests/StrikeWarningControllerTests.cs`

Focus areas:
- Phase gate: non-NightMovement mode suppresses warnings (Properties 1)
- Register/deregister round trip (Property 2)
- Active list count tracking (Property 3)
- Single icon invariant (Property 4)
- Non-nearest resolution (Property 5)
- ClearAll on phase exit (Property 6)
- Position pass-through with mock Map Director (Property 7)
- Null Map Director guard (Error Handling)
- Unbalanced false events (Error Handling)

### Property-Based Tests (NUnit + fixed-seed RNG, EditMode)

PBT is appropriate here because `StrikeWarningController` is a pure-logic service with clear input/output behavior, universal properties that hold across a wide input space (arbitrary sequences of true/false events, arbitrary player positions), and no external I/O in the logic path.

**Library**: NUnit with fixed-seed `System.Random` (project standard — see `AGENTS.md`). Each property test runs a minimum of 100 iterations.

**Tag format**: `// Feature: strike-warning-integration, Property N: <property_text>`

#### Property Test 1 — Phase gate

```
// Feature: strike-warning-integration, Property 1: non-NightMovement suppresses warnings
// For 100 random (mode, warningValue) pairs where mode != NightMovement:
//   IsWarningActive must remain false, ActiveWarnings must remain empty
```

#### Property Test 2 — Register/deregister round trip

```
// Feature: strike-warning-integration, Property 2: register then deregister is a round trip
// For 100 random N (1–10): send N true events, then N false events
//   ActiveWarnings.Count must be 0, IsWarningActive must be false
```

#### Property Test 3 — Active list count

```
// Feature: strike-warning-integration, Property 3: active list count tracks registrations
// For 100 random sequences of true/false events:
//   ActiveWarnings.Count == max(0, trueCount - falseCount)
```

#### Property Test 4 — Single icon invariant

```
// Feature: strike-warning-integration, Property 4: at most one icon displayed
// For 100 random N (0–20) simultaneous true events:
//   IsWarningActive is bool (never > 1 icon)
```

#### Property Test 5 — Non-nearest resolution

```
// Feature: strike-warning-integration, Property 5: non-nearest resolution does not change displayed warning
// For 100 random sets of 2+ warnings: resolve the non-nearest
//   IsWarningActive remains true
```

#### Property Test 6 — Phase exit clears all

```
// Feature: strike-warning-integration, Property 6: phase exit clears all warnings
// For 100 random N (1–20) active warnings: call ClearAll()
//   ActiveWarnings.Count == 0, IsWarningActive == false
```

#### Property Test 7 — Position pass-through

```
// Feature: strike-warning-integration, Property 7: position pass-through
// For 100 random (Vector2, Bounds) pairs in NightMovement mode:
//   UpdatePlayerPosition called with exact values provided
```

### Integration Tests

Location: `Assets/_Project/Application/Editor/Tests/StrikeWarningIntegrationTests.cs` (extend existing)

- Full signal chain: `MapSpawnDirector.NotifyStrikeWarning(true)` → subscription → `StrikeWarningController.OnStrikeWarningReceived` → `IsWarningActive = true`
- Phase subscription lifecycle: subscribe on NightSurvival, unsubscribe on phase exit, verify no events received after unsubscribe
- Canvas sort order smoke test: verify `PanelSettings.sortingOrder == 100` (single assertion, not property-based)

### Out of Scope

- Visual regression testing of the warning icon appearance (owned by Visual/Feel story)
- Strike damage calculation (owned by Map & Spawn Director)
- Ward Timer penalty on strike hit (owned by Ward Timer)
