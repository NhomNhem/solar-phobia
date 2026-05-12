# Technical Preferences

<!-- Populated by /setup-engine. Updated as the user makes decisions throughout development. -->
<!-- All agents reference this file for project-specific standards and conventions. -->

## Engine & Language

- **Engine**: Unity 6000.3.11f1 (Unity 6)
- **Language**: C# 9.0
- **Rendering**: [TO BE CONFIGURED — URP/HDRP/Built-in]
- **Physics**: Unity Physics (3D)

## Input & Platform

- **Target Platforms**: [TO BE CONFIGURED — e.g., PC, Console, Mobile, Web]
- **Input Methods**: New Input System (Keyboard/Mouse + Gamepad)
- **Primary Input**: Keyboard/Mouse
- **Gamepad Support**: [TO BE CONFIGURED — Full / Partial / None]
- **Touch Support**: None (PC-focused)
- **Platform Notes**: Windows primary, consider WebGL later

## Naming Conventions (Unity/C#)

- **Classes**: PascalCase (e.g., `PlayerController`)
- **Public fields/properties**: PascalCase (e.g., `MoveSpeed`)
- **Private fields**: _camelCase (e.g., `_moveSpeed`)
- **Methods**: PascalCase (e.g., `TakeDamage()`)
- **Files**: PascalCase matching class (e.g., `PlayerController.cs`)
- **Constants**: PascalCase or UPPER_SNAKE_CASE

## Performance Budgets

- **Target Framerate**: 60 FPS
- **Frame Budget**: 16.6ms
- **Draw Calls**: [TO BE CONFIGURED]
- **Memory Ceiling**: [TO BE CONFIGURED]

## Testing

- **Framework**: NUnit (Unity Test Framework 1.6.0)
- **Minimum Coverage**: 80% for core systems
- **Required Tests**: Balance formulas, gameplay systems, phase state machine

## Forbidden Patterns

<!-- Add patterns that should never appear in this project's codebase -->
- Feature-specific application code living in `Application/Services` instead of `Application/<Feature>/...`
- Hardcoded gameplay values in code when the value can live in data/config
- `ObservableCollections` types in `Domain` or public contracts that cross architectural layers
- Using `MessagePipe` as authoritative state storage or for request/response flows
- Using `ZLinq` in normal orchestration, UI code, tests, or non-profiled code paths
- Treating `R3` as a global event bus instead of local/stateful reactivity

## Allowed Libraries / Addons

<!-- Add approved third-party dependencies here -->
- **VContainer** — dependency injection and scope composition
- **R3** — single-value reactive state and local reactive streams
- **ObservableCollections** — collection delta observation for runtime/UI binding only
- **MessagePipe** — one-way cross-context event dispatch with decoupled subscribers
- **UniTask** — async/await orchestration in Unity
- **ZLogger** — structured logging
- **ZLinq** — hot-path query optimization only, after profiling

## Package Usage Notes

- Prefer **R3** when a single value, flag, mode, or phase must be observed.
- Prefer **ObservableCollections** when collection add/remove/move/replace changes must be observed efficiently.
- Prefer **MessagePipe** when producers should not know who consumes an event and multiple modules may subscribe.
- Prefer plain loops or `System.Linq` by default; only bring in **ZLinq** for justified hot paths.
- Use `Application/Services` only for cross-cutting services; feature-specific application code belongs under `Application/<Feature>/...`.

## Architecture Decisions Log

<!-- Quick reference linking to full ADRs in docs/architecture/ -->
- [No ADRs yet — use /architecture-decision to create one]

## Engine Specialists

- **Primary**: unity-specialist (C# + Unity engine)
- **Language/Code Specialist**: unity-specialist
- **Shader Specialist**: unity-specialist (with shader focus)
- **UI Specialist**: unity-specialist (UI Toolkit)
- **Routing Notes**: All C# code routes to unity-specialist

### File Extension Routing

| File Extension / Type | Specialist to Spawn |
|-----------------------|---------------------|
| *.cs | unity-specialist |
| *.uxml, *.uss | unity-specialist |
| *.shader | unity-specialist |
| *.asmdef | unity-specialist |
| Scene / prefab files | Default (fallback) | |
