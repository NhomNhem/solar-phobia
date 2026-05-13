# Solar Phobia — Target Architecture v2

## Document Status
- Version: 2.0
- Date: 2026-05-13
- Status: Official architecture standard for this repo
- Scope: Full project structure, DI scopes, package policy, scene architecture, migration phases
- Replaces as target: ad-hoc layer usage in current `Assets/_Project/*`
- Coexists with: [architecture.md](/I:/unityVers/Solar%20phobia/docs/architecture/architecture.md)

---

## Purpose

This document defines the official architecture standard for Solar Phobia.

The goal is not a cosmetic folder rename. The goal is to establish:

1. Strict dependency direction
2. Multi-scope VContainer composition
3. Clear separation between domain logic, orchestration, runtime adapters, and presentation
4. Feature-first folder ownership inside each layer
5. Package usage rules that scale with project size
6. A migration path from the current codebase to the target state

This standard assumes Solar Phobia will continue to scale in feature count, runtime complexity, tooling depth, and team size.

---

## Core Decisions

1. Use full six-layer project structure: `01_Domain` → `06_Shared`
2. Use VContainer as the only DI container
3. Use `VContainerSettings` root scope instead of ad-hoc bootstrap scene wiring
4. Use NhemDangFugBixs source-generator tooling for automatic registration and compile-time architecture diagnostics
5. Use R3 for local reactive state and view-model observation
6. Use MessagePipe for cross-bounded-context event bus only
7. Keep Domain pure C# with no Unity runtime dependency
8. Split runtime by scope, not by convenience singleton
9. Treat scenes as runtime composition units, not state containers
10. Organize feature-specific code under `Features/<Feature>/...` inside every layer

---

## Official Tech Stack

| Library | Package | Role |
|---|---|---|
| VContainer | `jp.hadashikick.vcontainer` | DI container |
| NhemDangFugBixs.Tooling | `com.nhemdangfugbixs.tooling` | Source generator, analyzers, scope registration |
| R3 + R3.Unity | `com.cysharp.r3` | Local reactive state |
| ObservableCollections | `com.cysharp.observablecollections` | Observable collection primitives for non-Domain runtime/view use only |
| MessagePipe | `com.cysharp.messagepipe` | Event bus |
| MessagePipe.VContainer | `com.cysharp.messagepipe.vcontainer` | MessagePipe + VContainer integration |
| UniTask | `com.cysharp.unitask` | Async orchestration |
| ZLogger | `com.cysharp.zlogger` | Structured logging |
| Unity Input System | `com.unity.inputsystem` | Input |

### Installed-But-Not-Automatically-Approved Packages

Installed packages may exist outside `manifest.json` or arrive from local package cache, Asset Store imports, or manual project integration.

That does not automatically make them part of the approved architecture surface.

Approved usage is controlled by this document, not only by package presence.

---

## Target Folder Structure

```text
Assets/
├── _Project/
│   ├── 01_Domain/
│   │   ├── Entities/
│   │   ├── ValueObjects/
│   │   ├── Events/
│   │   ├── Services/
│   │   ├── Repositories/
│   │   ├── Rules/
│   │   └── Features/                  ← feature-specific domain logic
│   │       └── <Feature>/
│   │
│   ├── 02_Application/
│   │   ├── Features/                  ← primary home for all feature code
│   │   │   ├── Combat/
│   │   │   ├── Consequences/
│   │   │   ├── Day/
│   │   │   ├── Flow/
│   │   │   ├── Hazards/
│   │   │   ├── MainMenu/
│   │   │   ├── Map/
│   │   │   ├── Phase/
│   │   │   ├── Player/
│   │   │   ├── Rituals/
│   │   │   ├── Shrines/
│   │   │   ├── Strike/
│   │   │   ├── Ward/
│   │   │   └── <Feature>/
│   │   ├── Shared/                    ← cross-cutting utilities (narrow scope)
│   │   ├── Contracts/                 ← cross-layer contracts only
│   │   ├── Messages/                  ← cross-feature events/commands/queries
│   │   ├── UseCases/
│   │   ├── Models/
│   │   ├── Repositories/
│   │   ├── Resources/
│   │   └── Editor/
│   │
│   ├── 03_Infrastructure/
│   │   ├── Features/                  ← feature-specific adapters
│   │   │   └── <Feature>/
│   │   ├── Audio/
│   │   ├── Camera/
│   │   ├── Dialogue/
│   │   ├── Hazards/
│   │   ├── Input/
│   │   ├── Logging/
│   │   ├── MainMenu/
│   │   ├── Network/
│   │   ├── Physics/
│   │   ├── State/
│   │   ├── VFX/
│   │   └── Shared/                    ← cross-cutting infrastructure utilities
│   │
│   ├── 04_Presentation/
│   │   ├── Features/                  ← feature-specific UI/presenters
│   │   │   └── <Feature>/
│   │   ├── HUD/
│   │   │   └── Toolkit/
│   │   ├── MainMenu/
│   │   │   ├── Scripts/
│   │   │   └── Toolkit/
│   │   └── Player/
│   │
│   ├── 05_Composition/
│   │   ├── Scopes/
│   │   └── Installers/
│   │
│   └── 06_Shared/
│       ├── Scopes/
│       ├── Logging/
│       ├── Extensions/
│       ├── Constants/
│       ├── Conventions/
│       └── Configuration/
│
├── _Scenes/
├── _Art/
├── _Audio/
└── Editor/
```

> **Note on naming**: The numbered prefixes (`01_Domain` through `06_Shared`) are the long-term target. The current project uses unprefixed layer names (`Domain`, `Application`, etc.). Both layouts follow the same structure rules below; only the root folder name differs.

---

## Namespace Rules

Folder and namespace must match one-to-one.

### Layer Prefix Mapping

| Folder Prefix | Namespace Prefix |
|---|---|
| `01_Domain/...` or `Domain/...` | `SolarPhobia.Domain...` |
| `02_Application/...` or `Application/...` | `SolarPhobia.Application...` |
| `03_Infrastructure/...` or `Infrastructure/...` | `SolarPhobia.Infrastructure...` |
| `04_Presentation/...` or `Presentation/...` | `SolarPhobia.Presentation...` |
| `05_Composition/...` or `Composition/...` | `SolarPhobia.Composition...` |
| `06_Shared/...` or `Shared/...` | `SolarPhobia.Shared...` |

### Feature-First Namespace Examples

| Folder Path | Namespace |
|---|---|
| `Application/Features/Combat/...` | `SolarPhobia.Application.Features.Combat...` |
| `Application/Features/Phase/Day/...` | `SolarPhobia.Application.Features.Phase.Day...` |
| `Application/Features/Player/Movement/...` | `SolarPhobia.Application.Features.Player.Movement...` |
| `Application/Shared/...` | `SolarPhobia.Application.Shared...` |
| `Application/Contracts/...` | `SolarPhobia.Application.Contracts...` |
| `Application/Messages/...` | `SolarPhobia.Application.Messages...` |
| `Domain/Features/Combat/...` | `SolarPhobia.Domain.Features.Combat...` |
| `Infrastructure/Features/Combat/...` | `SolarPhobia.Infrastructure.Features.Combat...` |
| `Presentation/Features/Combat/...` | `SolarPhobia.Presentation.Features.Combat...` |

### Technical Subfolder Namespace Examples

| Folder Path | Namespace |
|---|---|
| `Domain/ValueObjects/...` | `SolarPhobia.Domain.ValueObjects...` |
| `Domain/Entities/...` | `SolarPhobia.Domain.Entities...` |
| `Domain/Repositories/...` | `SolarPhobia.Domain.Repositories...` |
| `Domain/Services/...` | `SolarPhobia.Domain.Services...` |
| `Domain/Rules/...` | `SolarPhobia.Domain.Rules...` |
| `Domain/Events/...` | `SolarPhobia.Domain.Events...` |
| `Application/UseCases/...` | `SolarPhobia.Application.UseCases...` |
| `Composition/Scopes/...` | `SolarPhobia.Composition.Scopes...` |
| `Composition/Installers/...` | `SolarPhobia.Composition.Installers...` |
| `Shared/Logging/...` | `SolarPhobia.Shared.Logging...` |
| `Shared/Extensions/...` | `SolarPhobia.Shared.Extensions...` |

### Hard Rules

1. No `SolarPhobia.Application` type may live under `Presentation` or `Infrastructure`
2. No `SolarPhobia.Domain` type may live outside `01_Domain` (or `Domain`)
3. No "temporary" namespace aliases as a long-term solution
4. Namespace migration must be done together with asmdef and scene/reference validation
5. Feature code MUST go under `Features/<Feature>/...`, not at the layer root as a peer of technical folders
6. `Shared`, `Contracts`, and `Messages` subfolders MUST contain only cross-cutting code — never feature-specific logic

---

## Layer Responsibilities

### 01_Domain

Contains:
- Entities
- Value objects
- Domain events
- Domain rules
- Domain services
- Repository interfaces when they are part of business truth

Must not contain:
- `MonoBehaviour`
- `UnityEngine`
- `R3`
- `MessagePipe`
- `PlayerPrefs`
- file I/O
- scene object references

Domain is the business truth layer.

**Organization**: Feature-specific domain logic SHOULD be placed under `Domain/Features/<Feature>/...` when it improves ownership clarity. Purely technical subfolders (`Entities/`, `ValueObjects/`, `Events/`, `Services/`, `Repositories/`, `Rules/`) remain valid for cross-cutting or foundational domain concepts.

### 02_Application

Contains:
- Use cases
- Coordinators
- Command/query handlers
- Cross-domain orchestration
- Message contracts
- Application service interfaces and ports

**Organization**: Feature-specific code MUST reside under `Application/Features/<Feature>/...`. The `Application/Shared/`, `Application/Contracts/`, and `Application/Messages/` subfolders are reserved for cross-cutting concerns only. `Application/Services/...` MUST NOT be used as a catch-all bucket.

May depend on:
- `Domain`
- approved abstractions from `Shared`
- MessagePipe contracts if event contracts must cross bounded contexts
- UniTask for orchestration

Must not contain:
- `MonoBehaviour`
- direct Unity settings APIs
- direct scene references
- runtime-only engine adapters

Application owns flow, not rendering, not persistence, not scene objects.

### 03_Infrastructure

Contains:
- Config loading
- Persistence implementations
- Runtime adapters
- Audio implementation
- Input implementation
- Scene loading implementation
- External system implementations
- Networking implementation when networking is added

Infrastructure is allowed to know Unity and external packages.

**Organization**: Feature-specific infrastructure adapters SHOULD be placed under `Infrastructure/Features/<Feature>/...`. Truly shared adapters (config, logging, input) remain valid in `Infrastructure/Shared/...` or top-level technical folders.

### 04_Presentation

Contains:
- `MonoBehaviour`
- UI Toolkit views
- HUD bindings
- presenter/adapters from application state to visuals
- player-facing visual behavior
- camera presentation behavior

**Organization**: Feature-specific presentation code SHOULD be placed under `Presentation/Features/<Feature>/...`. Shell-level UI code (HUD, MainMenu) remains valid in dedicated top-level folders.

Presentation does not own authoritative game state.

### 05_Composition

Contains:
- `LifetimeScope` classes
- installers
- registration orchestration
- root bootstrap configuration

Composition must not contain gameplay logic.

### 06_Shared

Contains only truly cross-cutting, non-feature-specific concerns:
- scope markers
- logging wrappers
- small general-purpose extensions
- constants that are not game-balance data
- utility types used by multiple layers

Shared must not become a dumping ground.

---

## Assembly Definition Target

| Assembly | Depends on | Notes |
|---|---|---|
| `SolarPhobia.Domain` | none | Purest and fastest assembly |
| `SolarPhobia.Application` | Domain, Shared, UniTask, MessagePipe contracts only when justified | No Unity runtime ownership |
| `SolarPhobia.Infrastructure` | Domain, Application, Shared, Unity packages, external adapters | Only layer that knows engine/runtime integrations broadly |
| `SolarPhobia.Presentation` | Domain, Application, Shared, R3, Unity UI/Input | No persistence/network ownership |
| `SolarPhobia.Composition` | all project assemblies, VContainer, tooling | Wiring only |
| `SolarPhobia.Shared` | limited cross-cutting packages only | No upstream project assembly dependency |

### Dependency Graph

```text
Domain <- Application <- Infrastructure
   ^           ^
   |           |
   +-- Shared -+
   ^
   +-- Presentation
   ^
   +-- Composition
```

### Prohibited Dependencies

1. Domain -> anything
2. Application -> Presentation
3. Application -> Infrastructure concrete implementation
4. Presentation -> Infrastructure concrete implementation, unless via explicit composition-approved adapter boundary
5. Shared -> Domain/Application/Infrastructure/Presentation/Composition

---

## Scope Architecture

Solar Phobia adopts full scoped composition because the project is expected to scale significantly.

### Scope Marker Interfaces

These live in `06_Shared/Scopes/`.

```csharp
namespace SolarPhobia.Shared.Scopes
{
    public interface IProjectScope { }
    public interface IRunScope { }
    public interface IUIScope { }
    public interface IEncounterScope { }
}
```

### Scope Semantics

| Scope | Lifetime | Owns |
|---|---|---|
| `IProjectScope` | entire app lifetime | logging, config, audio root, global services |
| `IRunScope` | one gameplay loop/run | phase state, soul repository, ward timer, consequence state |
| `IUIScope` | UI scene lifetime | UI presenters, menu state presenters, HUD bindings |
| `IEncounterScope` | temporary gameplay segment if needed later | encounter-local systems, hazards, short-lived controllers |

### Scope Policy

1. Parent scope cannot depend on child scope services
2. Child scope may depend on parent scope services
3. Runtime state must live in the narrowest valid scope
4. A service with reset semantics should generally not live in project scope

---

## VContainer Root Setup

Use `VContainerSettings` as the app root.

### Required Setup

1. Create `ProjectScope.prefab`
2. Attach `ProjectLifetimeScope`
3. Create `VContainerSettings`
4. Assign `ProjectScope.prefab` as Root Lifetime Scope
5. Ensure `VContainerSettings` is in preload assets as required by the project setup

No ad-hoc bootstrap scene should replace this root composition model.

---

## NhemDangFugBixs.Tooling Policy

This tooling is part of the target architecture, not optional sugar.

### Mandatory Patterns

1. `LifetimeScopeFor` attribute on every concrete scope
2. scope marker interface implemented by every concrete scope
3. `[AutoRegisterIn(typeof(...))]` on services intended for generated registration
4. installer ordering via `[InstallerOrder(...)]` for complex setup

### Example

```csharp
[LifetimeScopeFor(typeof(IRunScope))]
public class RunLifetimeScope : LifetimeScope, IRunScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        VContainerRegistration.RegisterAll(builder);
    }
}
```

### Diagnostics Policy

The following categories are architecture blockers:

- double registration
- cross-scope injection violation
- missing event broker registration
- missing root logging setup
- missing view registration contract

Warnings from this tooling must be treated as architecture debt, not ignored noise.

---

## Reactive and Event Policy

### R3

Use R3 for:
- local observable state
- presenter/view-model bindings
- phase/state subscriptions
- short-path runtime reactive coordination

Do not use R3 in Domain.

### ObservableCollections

Use ObservableCollections for:
- presentation-facing observable lists/dictionaries
- infrastructure/runtime collection synchronization where collection-delta observation is actually needed
- local runtime state adapters that benefit from add/remove/move notifications

Do not use ObservableCollections:
- in Domain
- in public contracts that cross major layer boundaries
- as a default replacement for normal collections

If only current-value observation is needed, prefer normal collections plus R3 state around the owning service instead.

### MessagePipe

Use MessagePipe for:
- one-way cross-bounded-context event dispatch
- events that must decouple producers from many consumers
- events that should not imply authoritative state ownership

Do not use MessagePipe:
- as a second state store
- as a substitute for direct method calls inside a tight local use case
- for every single interaction by default

### Rule of Thumb

- Local state observation: R3
- Cross-context event bus: MessagePipe
- Deterministic business decision in same use case: direct call

---

## Package Usage Standard

### Approved Core Packages

| Package | Allowed Layers | Policy |
|---|---|---|
| VContainer | Composition, Infrastructure, Presentation bridge points | Required |
| NhemDangFugBixs.Tooling | Composition + compile-time tooling | Required |
| R3 | Application, Infrastructure, Presentation | Approved |
| ObservableCollections | Infrastructure, Presentation, narrowly-scoped Application adapters only | Approved with strong restrictions |
| MessagePipe | Application contracts, Infrastructure, Composition | Approved with restrictions |
| UniTask | Application, Infrastructure | Approved |
| ZLogger | Shared, Infrastructure, Composition | Approved |
| Input System | Infrastructure, Presentation | Approved |

### Restricted Packages

| Package | Restriction |
|---|---|
| ObservableCollections | Must not appear in Domain or public cross-layer contracts; use only when collection-delta observation is required |
| ZLinq | Use only in profiled hot paths with explicit justification |
| DOTween | Presentation only |

### Installed But Not Approved For New Code

Any installed package not listed as approved must be treated as unavailable for new architecture decisions until explicitly added to this document.

---

## Scene Architecture Target

Solar Phobia should move toward additive scene composition with scope-aware runtime ownership.

```text
ProjectScope (persistent)
    └── ProjectLifetimeScope

SceneManager.unity
    └── GameFlow entry
    └── additive scene orchestration

SceneUI.unity
    └── UI shells / swap sub-scenes

SceneMap.unity
    └── map content and static scene objects

SceneGameplay.unity
    └── players, runtime simulation, hazards, reactive gameplay
```

### Current Mapping Recommendation

Solar Phobia does not need to fully split all scenes immediately.

But all new scene architecture should move toward:
- persistent project root
- additive UI/runtime split
- run-scoped gameplay ownership
- presentation and runtime content separated where feasible

---

## Current Codebase Gaps Against Target

These are the largest known gaps as of 2026-05-13:

1. `Application` still contains Unity-facing runtime services
2. `Domain` and public APIs recently leaked package-specific collection types
3. current composition root is still largely flat rather than truly scoped
4. package usage is inconsistent: R3 is dominant, MessagePipe is mostly absent, ZLinq usage is sporadic
5. documentation and runtime code are not yet aligned around full scope architecture
6. `Application/Services/` still exists in a few migration seams but is no longer the preferred home for feature code
7. `Features/` subfolders exist in the target and partially in the current repo; some remaining code still needs to be migrated into them

---

## Migration Strategy

Migration is mandatory to reach this target. It must be phased.

### Phase 1 — Establish Structure

Deliverables:
- create `01_Domain` to `06_Shared`
- create new asmdefs
- create scope marker interfaces
- create root `ProjectLifetimeScope`
- create `target-architecture-v2.md`

Exit criteria:
- folder, namespace, asmdef skeleton exists
- no runtime behavior change required yet

### Phase 2 — Domain Purity

Deliverables:
- move pure entities, value objects, rules, domain interfaces into `01_Domain`
- remove Unity/package leak from domain
- convert package-specific public contracts to neutral contracts

Exit criteria:
- `SolarPhobia.Domain` has no Unity dependency
- Domain no longer depends on R3, MessagePipe, ObservableCollections

### Phase 3 — Application Purity

Deliverables:
- move Unity-facing application services out to Infrastructure/Presentation
- keep only orchestration and use-case logic in Application
- formalize ports for persistence, input, scene loading, logging, settings

Exit criteria:
- no `MonoBehaviour`, `PlayerPrefs`, `Camera`, `QualitySettings`, `AudioListener`, `Debug.Log` in Application runtime code

### Phase 4 — Infrastructure and Presentation Split

Deliverables:
- move runtime adapters to `03_Infrastructure`
- move MonoBehaviours and UI binders to `04_Presentation`
- ensure gameplay state is not owned in Presentation

Exit criteria:
- Presentation becomes view/binding layer
- Infrastructure owns engine/external integration

### Phase 5 — Scoped Composition

Deliverables:
- introduce `IRunScope`, `IUIScope`, optional `IEncounterScope`
- split registrations by scope
- add NhemDangFugBixs auto-registration patterns
- define scope ownership per runtime system

Exit criteria:
- root/project/run/ui boundaries are explicit
- no child-to-parent violation

### Phase 6 — Event and Package Standardization

Deliverables:
- standardize R3 vs MessagePipe usage
- remove opportunistic package usage outside policy
- document approved package-by-layer matrix

Exit criteria:
- package usage follows this document
- new code review can reject violations mechanically

### Phase 7 — Enforcement

Deliverables:
- architecture validation tests or analyzer gates
- CI check for forbidden namespace/package/layer references
- code review checklist based on target architecture

Exit criteria:
- architecture drift becomes a failing signal, not tribal knowledge

---

## Non-Goals

This target architecture does not require:

1. immediate networking adoption
2. immediate full additive-scene decomposition
3. immediate rewrite of every existing service before continuing product work

But it does require that all new architecture decisions move toward the target rather than away from it.

---

## Immediate Next Actions

1. Approve this target architecture document as the canonical target-state reference
2. Create new folder and asmdef skeleton without moving all code yet
3. Produce a file-by-file migration register for current `Assets/_Project`
4. Start migration with Domain purity and Application de-Unity slices

---

## Decision Summary

Solar Phobia will adopt the full long-scale architecture model:

- six layers
- scoped VContainer composition
- NhemDangFugBixs source-generator registration
- R3 for local reactive state
- MessagePipe for cross-context bus
- strict package policy
- explicit migration phases

This is the target architecture baseline for future refactor and growth.
