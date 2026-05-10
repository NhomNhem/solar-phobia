# Unity 6 — Current Best Practices

> Last verified: 2026-05-07

This document outlines recommended patterns for Unity 6 that may differ from older versions or LLM training data.

---

## Dependency Injection

### VContainer (Recommended for this project)

```csharp
// Installer pattern
public class GameInstaller : MonoInstaller {
    public override void Install(IContainerBuilder builder) {
        builder.RegisterComponent(gameObject.GetComponent<PlayerController>());
        builder.Register<IInputService, InputService>();
    }
}

// Inject into MonoBehaviour
public class PlayerController : MonoBehaviour {
    [Inject] private IInputService _inputService;
}
```

---

## Reactive Programming

### R3 (Recommended for this project)

```csharp
// Observable streams
public class PhaseStateMachine : MonoBehaviour {
    private readonly ReactiveProperty<PhaseState> _currentPhase = new(PhaseState.DayService);
    
    public ReadOnlyReactiveProperty<PhaseState> CurrentPhase => _currentPhase;
    
    public void Transition(PhaseState newPhase) {
        _currentPhase.Value = newPhase;
    }
}

// Subscribe
_currentPhase
    .Where(p => p == PhaseState.NightSurvival)
    .Subscribe(_ => EnableNightSystems());
```

**Important**: `ReadOnlyReactiveProperty<T>` does NOT expose a `.Value` property directly. To access the current value:

```csharp
// Interface
public interface IMyService {
    MyState CurrentStateValue { get; }  // Wrapper property
    ReadOnlyReactiveProperty<MyState> CurrentState { get; }  // Observable
}

// Implementation
private readonly ReactiveProperty<MyState> _currentState = new(MyState.Idle);
public MyState CurrentStateValue => _currentState.Value;
public ReadOnlyReactiveProperty<MyState> CurrentState => _currentState;
```

---

## Input System

### New Input System (Required for Unity 6)

```csharp
// Enable in PlayerSettings > Active Input Handling > Both or New

// Action-based input
public class InputService : IInputService {
    private readonly PlayerInputActions _actions;
    
    public InputService() {
        _actions = new PlayerInputActions();
        _actions.Player.Enable();
    }
    
    public Vector2 GetMovement() => _actions.Player.Move.ReadValue<Vector2>();
    public bool IsSprintHeld() => _actions.Player.Sprint.IsPressed();
}
```

---

## UI Toolkit (vs Legacy uGUI)

### USS/UXML Pattern (Recommended)

```csharp
// Create UXML in Editor, load at runtime
var visualTree = Resources.Load<VisualTreeAsset>("UI/MainMenu");
var panel = new VisualElement();
visualTree.CloneTree(panel);
root.Add(panel);

// Or via UI Document component
public class MainMenu : MonoBehaviour {
    [SerializeField] private UIDocument _document;
    
    private void Awake() {
        var button = _document.rootVisualElement.Q<Button>("start-button");
        button.clicked += OnStartClicked;
    }
}
```

---

## Object Finding

### Preferred Patterns

```csharp
// Don't use FindObjectsOfType (obsolete)
var objects = Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None);

// Use singleton pattern with DI instead
// Or inject via VContainer

// For runtime finding, use tags carefully
var player = GameObject.FindWithTag("Player");
```

---

## Performance

### Frame Budget: 16.6ms (60 FPS)

- Use `IJobEntity` for batched processing
- Use Addressables for asset loading
- Use Object Pooling for frequently created/destroyed objects

---

## Project Structure (Clean Architecture)

```
Assets/
├── _Project/
│   ├── Domain/           # Entities, value objects
│   │   └── Entities/
│   ├── Application/      # Use cases, interfaces
│   │   ├── UseCases/
│   │   └── Interfaces/
│   ├── Infrastructure/   # External concerns
│   │   └── Persistence/
│   ├── Presentation/     # Unity MonoBehaviours
│   │   └── Components/
│   └── Composition/      # VContainer installers
├── _Data/                # ScriptableObjects, configs
└── _External/            # Third-party code
```

---

## Cross-Reference

- See `breaking-changes.md` for breaking changes to avoid
- See `deprecated-apis.md` for deprecated API list