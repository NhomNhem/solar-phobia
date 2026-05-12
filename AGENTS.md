# AGENTS.md - Solar Phobia Unity Project

## Project Overview
Unity 6000.3.11f1 (Unity 6) project using C# 9.0 targeting .NET 4.7.1.
Architecture follows clean layering with Assembly Definitions: Domain → Application → Infrastructure/Presentation → Composition.

## Build & Test Commands

### Unity Editor (Primary)
- Open project in Unity 6000.3.11f1 Editor
- **Build**: File → Build Profiles (Ctrl+Shift+B), then Build
- **Test Runner**: Window → General → Test Runner (or Ctrl+Alt+T)
  - EditMode tests: Assets/_Project/**/Editor/Tests/
  - PlayMode tests: Assets/_Project/**/Tests/

### Command Line (CI/CD)
```bash
# Run EditMode tests
Unity.exe -runTests -projectPath "I:\unityVers\Solar phobia" -testPlatform EditMode -testResults results.xml

# Run PlayMode tests
Unity.exe -runTests -projectPath "I:\unityVers\Solar phobia" -testPlatform PlayMode -testResults results.xml

# Build project (Windows standalone example)
Unity.exe -quit -batchmode -projectPath "I:\unityVers\Solar phobia" -buildTarget Win64 -executeMethod BuildPipeline.BuildPlayer
```

### Run Single Test (Unity Test Runner)
- Open Test Runner window
- Right-click specific test → Run Selected
- Or use NUnit's `Category` attribute to filter test runs

## Code Style Guidelines

### Layered Folder Rule
- Feature-specific application code should live under `Assets/_Project/Application/<Feature>/...`
- `Assets/_Project/Application/Services/...` is reserved for cross-cutting services only
- Keep folder and namespace aligned 1:1 with the feature or layer path
- Avoid using `Services` as a catch-all bucket for unrelated gameplay features

### Naming Conventions
- **Namespaces**: `SolarPhobia.Domain`, `SolarPhobia.Application.Combat`, `SolarPhobia.Application.Consequences` (PascalCase with dots)
- **Classes/Interfaces**: `PhaseStateMachine`, `ISoulRepository` (PascalCase, prefix I for interfaces)
- **Methods**: `TrySetSelection`, `AdvancePhase` (PascalCase)
- **Private fields**: `_mode`, `_subscriptions`, `_mapDirector` (underscore + camelCase)
- **Local variables**: `tempRoot`, `snapshot` (camelCase)
- **Constants**: `Rng` (static readonly), or UPPER_SNAKE_CASE for true constants
- **Assembly Definitions**: `SolarPhobia.Domain.asmdef` matching namespace

### File Structure
```csharp
using System;                    // System imports first
using System.Collections.Generic;
using SolarPhobia.Application.Combat; // Third-party/Project imports after
using UnityEditor;
using UnityEngine;

namespace SolarPhobia.Application.Systems {
    /// <summary>XML doc comments on public types.</summary>
    public class PhaseStateMachine {
        // ── Section Separators ──────────────────────────────
        private List<PhaseState> _phases;

        /// <summary>XML docs on public members.</summary>
        public void DoWork() {
            // Implementation
        }
    }
}
```

### Formatting
- **Braces**: Opening brace on new line (Allman style)
- **Indentation**: 4 spaces (no tabs)
- **Line length**: Aim for ~120 characters, but prioritize readability
- **Sections**: Use comment separators with box-drawing chars: `// ── Section Name ──────────────────────`
- **Regions**: Avoid `#region`; use section comments instead

### Types & Null Handling
- Use **nullable reference types** where applicable (C# 9.0)
- **String checks**: Use `string.IsNullOrEmpty()` or `string.IsNullOrWhiteSpace()`
- **Collections**: Prefer `List<T>` for mutable lists, `T[]` for fixed-size, `IEnumerable<T>` for returns
- **var keyword**: Use when type is obvious: `var list = new List<string>();`

### Error Handling
- Use **try-finally** for cleanup
- Avoid empty catch blocks; log or rethrow with `throw;`
- Unity-specific: Prefer `NhemDangFugBixs.NhemLogging.INhemLogger` for diagnostics in DI-managed classes. Direct `NhemUnityLogger` construction is only for static loaders or non-DI runtime components.
- Test assertions: NUnit `Assert.That()`, `Assert.AreEqual()`, `StringAssert.Contains()`

### Imports Organization
1. System namespaces (`System`, `System.Collections.Generic`, etc.)
2. Third-party namespaces (`NhemBootstrap`, `Cysharp`, etc.)
3. Unity namespaces (`UnityEngine`, `UnityEditor`, etc.)
4. Project namespaces (`SolarPhobia.Domain`, etc.)

### Testing Patterns
- **Framework**: NUnit (Unity Test Framework 1.6.0)
- **Property-based testing**: Use fixed seed RNG for reproducibility (see `new System.Random(42)`)
- **Test naming**: `MethodName_Scenario_ExpectedResult`
- **XML docs on tests**: Include `<summary>` with "Validates: Requirements X.X" referencing specs
- **Test file location**: Mirror source structure in `Editor/Tests/` or `Tests/` folders

## Assembly Definitions (.asmdef)
- Place one `.asmdef` per layer/folder
- Set `autoReferenced: false` for explicit dependency management
- Reference other assemblies via `references` array
- Use `rootNamespace` matching assembly name

## Key Packages
- **VContainer**: Dependency injection (jp.hadashikick.vcontainer) — see [Dependency Injection Rules](#dependency-injection-rules)
- **NhemDangFugBixs.Logging**: Project logging abstraction — use `INhemLogger` in DI-managed classes; keep `NhemUnityLogger` construction limited to static loaders and non-DI runtime components
- **R3**: Reactive programming (com.cysharp.r3) — replaces reactive patterns
- **ObservableCollections**: Collection-level change tracking (Cysharp) — use for add/remove/move/replace deltas, never in Domain or public cross-layer contracts
- **UniTask**: Async/await for Unity (com.cysharp.unitask)
- **ZLinq**: LINQ extensions (com.cysharp.zlinq)
- **DOTween**: Animation tweens (Demigiant)
- **Odin Inspector**: Editor enhancements (Sirenix)
- **MessagePipe**: Event/message bus (com.cysharp.messagepipe)

## Dependency Injection Rules

- **No `[Inject]` on public fields.** All injected dependencies must be private fields (underscore + camelCase naming).
- **Injected fields**: If field injection is used in a legacy or exceptional case, the field must be `internal` for source generator compatibility.
- **MonoBehaviour injection**: Use `[Inject] public void Construct(...)` method — never field injection.
  - Example: `[Inject] public void Construct(IPhaseStateMachine phaseMachine) { _phaseMachine = phaseMachine; }`
- **Plain C# class injection**: Use constructor injection — never field injection.
  - Example: `public MyService(IPhaseStateMachine phaseMachine) { _phaseMachine = phaseMachine; }`
- **Too many constructor parameters**: Split the class or group dependencies via facade/service pattern (aim for ≤4 parameters).
- **INhemLogger in constructors**: Add `INhemLogger logger` as a constructor parameter; the DI container resolves it. Never assign `new NhemUnityLogger()` as a default.
- **Parameterless constructors**: Do NOT add `[Inject]` to parameterless constructors. VContainer resolves via constructor automatically.

## Package Usage Policy

- **R3**: Use for single-value state or local reactive streams with a clear owner. Typical cases: current phase, ward timer, flags, local UI state.
- **ObservableCollections**: Use only when collection deltas matter: add/remove/move/replace/range updates, usually for UI/list binding or runtime collection sync.
- **MessagePipe**: Use for one-way events that cross bounded contexts or scopes and may have multiple decoupled consumers.
- **ZLinq**: Use only in measured or clearly justified hot paths. Default to plain code or `System.Linq` elsewhere.

### R3 Rules

- Prefer `ReactiveProperty<T>` or `ReadOnlyReactiveProperty<T>` for a single observable value.
- Keep R3 out of `Domain`; use it in `Application`, `Infrastructure`, or `Presentation` only.
- Do not use R3 as a global event bus.
- Do not use R3 to model collection deltas when `ObservableCollections` is the real need.

### ObservableCollections Rules

- Allowed in `Infrastructure` and `Presentation`; only use in `Application` when a collection-change stream is genuinely required.
- Forbidden in `Domain`.
- Do not expose `ObservableList<T>`, `ObservableDictionary<TKey, TValue>`, or other package-specific collection types in public cross-layer contracts.
- If only a current snapshot or count is needed, prefer normal collections plus an owning service/property.

### MessagePipe Rules

- Prefer direct method calls when there is one clear consumer and synchronous flow is simpler.
- Use MessagePipe for cross-module notifications such as lifecycle broadcasts, telemetry hooks, audio triggers, and decoupled UI reactions.
- Do not use MessagePipe as authoritative state storage.
- Do not use MessagePipe for request/response flows.

### ZLinq Rules

- Forbidden in tests, normal orchestration code, most UI/presentation code, and small one-off queries.
- Only introduce ZLinq after profiling or when the code is an obvious hot path with allocation sensitivity.
- Do not mix `ZLinq` and `System.Linq` in the same hot path unless the reason is explicit and reviewed.

## Scene Folder Structure

**Use only** `Assets/_Project/_Scenes/` for project scenes:

| Folder | Purpose |
|--------|---------|
| `_Scenes/Dev/` | Development, testing, and prototype scenes |
| `_Scenes/Dev/Prototype/` | Prototype test scenes |
| `_Scenes/Dev/Dialogue/` | Dialogue system development scenes |
| `_Scenes/Gameplay/` | Main gameplay and level scenes |
| `_Project/Settings/Scenes/` | URP scene templates only |

**Never use**: `Assets/Scenes/` (deprecated - delete if empty)

## Coding Standards Namespace

Use `SolarPhobia.Rules` namespace for codifiable standards:

```csharp
using SolarPhobia.Rules;

// Layer attribute for architectural enforcement
[Layer(NamingConventions.Layers.Domain)]
public class MyEntity { }

// Scene path constants
string devScenes = ScenePaths.Scenes.Dev;
```

See `Assets/_Project/Domain/Rules/` for:
- `NamingConventions.cs` - Namespace and naming rules
- `LayerAttributes.cs` - Architectural layer attributes
- `ScenePaths.cs` - Scene folder path constants

## Unity 6 API Conventions

- **`FindObjectOfType<T>()` / `FindObjectsOfType<T>()`**: **Obsolete** in Unity 6000+. Use `FindFirstObjectByType<T>()` or `FindObjectsByType<T>(FindObjectsSortMode.None)` instead. See `docs/engine-reference/unity/breaking-changes.md`.
- **Duplicate `using` directives**: Avoid declaring the same `using` namespace multiple times in a file. Unity compiler warns CS0105.
- **R3 reactive patterns**: Use `ReactiveProperty<T>` and `ReadOnlyReactiveProperty<T>` for observable single-value state. Subscribe via `.Subscribe()` and dispose via `CancellationToken` or `Dispose()`.
- **ObservableCollections**: Use when observers must react to collection add/remove/move/replace deltas; do not put these types in Domain or public cross-layer APIs.
- **MessagePipe**: Use for cross-context one-way events, not as a second state store.
- **ZLinq**: Restrict to profiled hot paths; prefer plain code or `System.Linq` for normal queries.
- **VContainer `[Inject]` fields**: Must be `internal` visibility, not `private`. Private fields cannot be set by the source generator (VCON0007).
- **R3 ReadOnlyReactiveProperty.Value**: Does NOT have a `.Value` accessor directly. Add a wrapper property like `public T CurrentStateValue => _currentState.Value` to expose the value.
- **Unity types in services**: If using `Vector3`, `Vector2`, `Quaternion`, etc., add `using UnityEngine;` to the file.
- **Test event types**: When mocking IPhaseStateMachine in tests, include `using SolarPhobia.Application.Messages;` for PhaseChangedEvent, DayStartEvent, NightStartEvent, ResolveEvent.
- **PhaseState enum**: Always check `Assets/_Project/Domain/ValueObjects/PhaseState.cs` for valid values. Common valid states: `Boot`, `DayService`, `Dialogue`, `Order`, `SunsetWarning`, `NightTravel`, `ShrineArrival`, `EndingEvaluation`, `NightSurvival`, `ChoiceLock`.

## Production & Design Docs

Game design and production artifacts live under `production/`:

| Path | Purpose |
|------|---------|
| `production/epics/` | Epic folders with story files per feature area |
| `production/milestones/` | Milestone plans and scope definitions |
| `production/sprints/` | Sprint plans and sprint retrospectives |
| `production/qa/` | Smoke tests, QA evidence, playtest reports |
| `production/session-state/` | Active session notes |
| `docs/architecture/` | Architecture Decision Records (ADRs) and master architecture |
| `design/gdd/` | Game Design Documents per system |

See `docs/architecture/` for ADRs that document technical decisions. Stories reference their governing ADRs and GDD requirements.

## Git Workflow
- **Ignore**: Library/, Temp/, Obj/, Build/, Logs/, UserSettings/, *.csproj, *.sln
- **Track**: Assets/, Packages/, ProjectSettings/
- See `.gitignore` for complete list

---

## Engine Version Reference

**Unity 6000.3.11f1** — this version is beyond the LLM's training data. Before using engine APIs in HIGH RISK areas:

1. Check `docs/engine-reference/unity/breaking-changes.md` for migration issues
2. Check `docs/engine-reference/unity/deprecated-apis.md` for APIs to avoid
3. Check `docs/engine-reference/unity/current-best-practices.md` for recommended patterns

Key risk areas: **VContainer** source generation, **R3** reactive patterns, **UI Toolkit** changes.

@docs/engine-reference/unity/VERSION.md
