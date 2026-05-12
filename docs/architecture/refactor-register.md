# Refactor Register

Updated: 2026-05-12

## Purpose

This register tracks structural cleanup work across `Assets/_Project` so that refactoring stays aligned with:

- clean architecture boundaries
- folder and namespace consistency
- SOLID responsibilities
- Unity-safe incremental migration

This is not a wishlist. Each item should become a concrete migration batch with compile verification in Unity after every slice.

## Current Findings

### 1. Generic `Application/Services` is still overloaded

The project has started to split `Application/Map/*` and `Application/Player/*`, but `Assets/_Project/Application/Services/` still contains multiple unrelated responsibilities:

- phase flow and lifecycle
- menu/application state
- cursor policy
- hazards and curse systems
- objective logic
- combat effects
- input orchestration
- repositories and state-like logic nearby

This makes the layer readable only by file-name memory, not by structure.

### 2. Interface placement is still inconsistent

Many interfaces still live under `Assets/_Project/Application/Services/Interfaces/` while their implementations have already moved into feature folders.

Examples:

- movement contracts moved conceptually to `Application/Player/Movement`, but many other contracts still remain in a flat `Interfaces` folder
- `IKarmaHazardRuntime`, `IMainMenuSettingsStore`, `IMainMenuPlatformService`, `ICursorController`, `ICurseEffectManager` still follow old placement

This weakens cohesion and makes the dependency graph harder to read.

### 3. Folder-to-namespace mapping is inconsistent

Examples found during scan:

- `Presentation/HUD/Toolkit/*` classes use `SolarPhobia.Presentation.HUD` instead of a more specific `SolarPhobia.Presentation.HUD.Toolkit`
- `Composition/Scopes/*` classes use `SolarPhobia.Composition` instead of `SolarPhobia.Composition.Scopes`
- many moved files still keep old source comments pointing at legacy paths
- test namespaces mix `SolarPhobia.Application.Tests` and `SolarPhobia.Application.Editor.Tests`

This is small at compile-time but expensive for maintainability.

### 4. Duplicate folder tax exists in Infrastructure

`Infrastructure` currently has both:

- top-level feature folders like `Audio`, `Camera`, `Dialogue`, `Hazards`, `VFX`
- and `Infrastructure/Services/*` subtrees like `Services/Input`, `Services/Dialogue`, `Services/Logging`, `Services/Network`, `Services/Physics`

That means two competing organizational schemes are active at once.

### 5. Clean architecture boundaries are improved but not yet clean

Known remaining structural debts:

- `Application/Repositories/SoulRepository.cs` is still an implementation in the Application layer
- stateful runtime logic is still mixed with contracts in some feature areas
- some application services are still grouped by technical type (`Services`) instead of business capability

### 6. SOLID drift still exists

Common patterns still present:

- classes named `*Controller` or `*Service` that own multiple responsibilities inside one feature flow
- infrastructure or presentation concerns partially hidden behind application naming
- some contracts are still too broad or technically named instead of capability-driven

This is not yet a severe rewrite problem, but it should be addressed while restructuring folders so we do not preserve bad object boundaries in a cleaner directory tree.

## Target Rules

### Folder and namespace rules

- Folder and namespace must match.
- Avoid flat catch-all folders like `Services`, `Interfaces`, `Messages` when a feature or bounded-context folder is more precise.
- Prefer feature-first organization inside a layer.

Examples:

- `SolarPhobia.Application.Player.Movement`
- `SolarPhobia.Application.Player.State`
- `SolarPhobia.Application.Phase.Flow`
- `SolarPhobia.Application.UI.Menu`
- `SolarPhobia.Infrastructure.Audio`
- `SolarPhobia.Presentation.HUD.Toolkit`
- `SolarPhobia.Composition.Scopes`

### Clean architecture rules

- `Domain`: no Unity, no R3, no infrastructure/plugin runtime types
- `Application`: orchestration, use cases, policies, contracts
- `Infrastructure`: implementations for runtime, IO, config, engine, adapter behavior
- `Presentation`: MonoBehaviours, UI Toolkit views, scene bindings, visual adapters
- `Composition`: DI wiring only

### SOLID rules used in refactor review

- Single Responsibility: one class should own one policy or one adapter role
- Open/Closed: prefer adding new feature-specific services instead of widening catch-all services
- Liskov: contracts should describe behavior, not leak implementation detail
- Interface Segregation: avoid flat kitchen-sink interface folders and fat contracts
- Dependency Inversion: contracts stay in Application or Domain, implementations live in Infrastructure or Presentation

## Batch Backlog

### Batch A: Phase domain cleanup

Target:

- move phase-related classes from `Application/Services` and `Application/Services/Phase`
- normalize under a dedicated phase subtree

Candidate target structure:

- `Application/Phase/Flow`
- `Application/Phase/Timeline`
- `Application/Phase/Reset`

Candidate files:

- `PhaseStateMachine`
- `DayPhaseTimelineService`
- `DayPhaseMechanicsService`
- `NightToDayResetService`
- related interfaces and events

### Batch B: Menu/UI application cleanup

Target:

- move menu application logic out of generic `Services`
- align contracts with `Application/UI/Menu`

Candidate files:

- `MainMenuApplicationService`
- `IMainMenuApplicationService`
- `IMainMenuSettingsStore`
- `IMainMenuPlatformService`

### Batch C: Hazard and curse systems

Target:

- split gameplay policy from runtime adapters more explicitly
- stop using flat `Services` placement for curse/hazard files

Candidate structure:

- `Application/Gameplay/Hazards`
- `Application/Gameplay/Curses`
- `Infrastructure/Hazards`

Candidate files:

- `KarmaHazardService`
- `KarmaHazardData`
- `IKarmaHazardService`
- `IKarmaHazardRuntime`
- `CurseEffectManager`
- `ICurseEffectManager`
- `WaterTrapEffectService`
- combat-adjacent effect services if they belong in the same bounded context

### Batch D: Objective and progression

Target:

- group shrine, ward death, ritual, ngoc cot, resource effects into clearer progression/objective subtrees

Candidate structure:

- `Application/Objective/*`
- `Application/Progression/*`

### Batch E: Repositories and runtime state

Target:

- move implementation repositories out of `Application`
- keep only contracts where appropriate

Known high-priority item:

- `Application/Repositories/SoulRepository.cs`

Likely target:

- `Infrastructure/State/SoulRepository.cs`

### Batch F: Namespace normalization

Target:

- normalize namespaces after structural batches, not before
- remove mixed test namespaces
- remove stale file header comments with legacy paths

Known items:

- `Presentation/HUD/Toolkit/*`
- `Composition/Scopes/*`
- Application editor tests namespace inconsistency

### Batch G: Infrastructure folder normalization

Target:

- pick one scheme for Infrastructure

Recommended direction:

- top-level feature folders stay authoritative
- nested `Infrastructure/Services/*` should be flattened into domain-relevant folders or renamed into a clearer scheme

## Execution Rule

Every batch must:

1. move files and matching `.meta`
2. update namespace and imports
3. update DI registrations
4. update tests
5. run Unity compile verification
6. only then proceed to the next batch

## Current Status

Completed slices:

- `Application/Map/*`
- `Application/Player/Warnings`
- `Application/Player/Movement`
- logger policy and architecture hooks
- several application services already de-Unity or moved to Infrastructure

Next recommended slice:

- `Batch A: Phase domain cleanup`
