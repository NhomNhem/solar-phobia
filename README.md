# Solar Phobia

Solar Phobia is a Unity 6 survival game prototype built around a day/night loop: make morally costly decisions during the day, then survive the consequences at night.

The project is currently focused on the first vertical slice: a playable foundation with phase transitions, soul selection, movement, cover, ward timer pressure, and consequence-driven hazards.

## Project Status

- **Engine**: Unity 6000.3.11f1
- **Milestone**: Vertical Slice 01 - Foundation Core
- **Target**: Complete playable day/night loop
- **Architecture**: Clean layered C# assemblies
- **Primary docs**: `design/`, `docs/architecture/`, `production/`

## Requirements

- Unity 6000.3.11f1
- JetBrains Rider or Visual Studio with Unity support
- Git

Unity-generated files such as `Library/`, `Temp/`, `Obj/`, `Logs/`, `.csproj`, and `.sln` files are ignored or treated as local/generated artifacts.

## Getting Started

1. Clone the repository.
2. Open the repository root in Unity Hub.
3. Use Unity 6000.3.11f1.
4. Let Unity restore packages from `Packages/manifest.json`.
5. Open a scene from `Assets/_Project/_Scenes/`.

Current prototype scenes live in:

```text
Assets/_Project/_Scenes/Dev/Prototype/
```

Do not create project scenes under `Assets/Scenes/`; that folder is deprecated for this project.

## Project Layout

```text
Assets/_Project/
  _Scenes/          Unity scenes for dev and gameplay
  Application/      Use cases, orchestration, application services
  Assets/           Project-owned art, textures, and media
  Composition/      Dependency injection and bootstrap wiring
  Domain/           Pure rules, entities, value objects, interfaces
  Infrastructure/   Unity/external integrations and persistence
  Prefabs/          Project prefabs
  Presentation/     UI, view controllers, feedback, and scene-facing code
  Settings/         Project settings assets and templates
  Shared/           Shared cross-layer utilities

design/             GDDs, UX specs, visual specs, system design
docs/architecture/  Architecture docs and ADRs
production/         Epics, stories, milestones, QA, sprint state
prototypes/         Throwaway or exploratory prototypes
```

## Architecture

Code follows clean layering through assembly definitions:

```text
Domain -> Application -> Infrastructure / Presentation -> Composition
```

Core principles:

- Domain code stays independent of Unity scene concerns.
- Application services orchestrate use cases and gameplay rules.
- Infrastructure owns persistence and external/engine integration.
- Presentation owns UI, visual feedback, and scene-facing controllers.
- Composition wires systems together with VContainer.
- Shared gameplay state uses reactive/event-driven flow where appropriate.

See `docs/architecture/architecture.md` and the ADRs in `docs/architecture/` for the current technical decisions.

## Key Packages

- VContainer for dependency injection
- R3 and MessagePipe for reactive/event-driven flows
- UniTask for async Unity workflows
- ZLinq for allocation-conscious query helpers
- Unity Input System
- UI Toolkit
- URP
- DOTween
- Odin Inspector

Package versions are pinned in `Packages/manifest.json`.

## Build and Test

### Unity Editor

- Open project in Unity 6000.3.11f1.
- Build from **File -> Build Profiles**.
- Run tests from **Window -> General -> Test Runner**.

### Command Line

```powershell
Unity.exe -runTests -projectPath "I:\unityVers\Solar phobia" -testPlatform EditMode -testResults results.xml
Unity.exe -runTests -projectPath "I:\unityVers\Solar phobia" -testPlatform PlayMode -testResults results.xml
```

Windows standalone build example:

```powershell
Unity.exe -quit -batchmode -projectPath "I:\unityVers\Solar phobia" -buildTarget Win64 -executeMethod BuildPipeline.BuildPlayer
```

## Coding Standards

- C# 9.0 targeting .NET 4.7.1.
- Allman braces.
- 4-space indentation.
- Public types and members should have XML docs.
- Private fields use `_camelCase`.
- Test names use `MethodName_Scenario_ExpectedResult`.
- Prefer NUnit `Assert.That()` style in tests.
- Use `SolarPhobia.Rules` for codifiable standards such as layer and scene path rules.

Project rules and agent-facing workflow details are captured in `AGENTS.md`.

## Current Vertical Slice Scope

Must-have systems for the first slice:

- Phase State Machine with day/night state flow
- Soul repository with initial souls
- Basic day selection flow
- Night movement
- Day-to-night transition
- Ward Timer pressure
- Cover detection
- Consequence-driven hazards

Milestone details are in `production/milestones/vertical-slice-01.md`.

## Useful Docs

- `AGENTS.md` - project conventions and automation instructions
- `docs/engine-reference/unity/VERSION.md` - pinned Unity version and risk notes
- `docs/architecture/architecture.md` - master architecture
- `production/epics/index.md` - epic map and implementation status
- `production/session-state/active.md` - latest production/session notes
- `production/qa/smoke-2026-05-08.md` - latest smoke test report

## License

See `LICENSE`.
