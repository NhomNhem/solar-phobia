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
- [None configured yet — add as architectural decisions are made]

## Allowed Libraries / Addons

<!-- Add approved third-party dependencies here -->
- [None configured yet — add as dependencies are approved]

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
