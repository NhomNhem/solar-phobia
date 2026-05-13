# Design

## Overview

This change formalizes Option B as the canonical Unity project architecture for Solar Phobia and any future project that wants the same structure. The goal is to lock the project layout, layer boundaries, scope model, and package policy into one clear standard without introducing a separate meta-framework.

## Goals

- Keep the project architecture consistent and reusable across projects.
- Preserve the clean layering model that has already worked in Solar Phobia.
- Make folder placement and namespace choice obvious and enforceable.
- Avoid duplicated top-level taxonomy such as multiple competing `Shared` or `Services` buckets.

## Non-Goals

- No gameplay feature design.
- No Solar Phobia content migration in this change.
- No attempt to create a new kernel layer above Option B.
- No engine/package upgrade work beyond documenting the architecture standard.

## Canonical Project Structure

```text
Assets/_Project/
├── Domain/
│   ├── Entities/
│   ├── ValueObjects/
│   ├── Events/
│   ├── Contracts/
│   ├── Services/
│   └── Rules/
├── Application/
│   ├── Features/
│   │   ├── Phase/
│   │   ├── Player/
│   │   ├── Combat/
│   │   ├── Ward/
│   │   ├── Consequences/
│   │   └── MainMenu/
│   ├── Messages/
│   ├── Contracts/
│   └── Shared/
├── Infrastructure/
│   ├── Features/
│   ├── Runtime/
│   ├── Input/
│   ├── Audio/
│   ├── Persistence/
│   └── Shared/
├── Presentation/
│   ├── Features/
│   ├── Views/
│   ├── Controllers/
│   ├── UI/
│   └── Shared/
├── Composition/
│   ├── Scopes/
│   ├── Installers/
│   └── Bootstrap/
└── Shared/
    ├── Constants/
    ├── Extensions/
    ├── Logging/
    ├── Rules/
    └── Utilities/
```

This layout is the standard. The intent is to keep the project familiar, easy to review, and resistant to drift back into catch-all folders.

## Assembly Model

The architecture keeps the six-assembly model:

```text
Domain
Application -> Domain
Infrastructure -> Domain, Application, Shared
Presentation -> Domain, Application, Shared
Composition -> all
Shared -> none
```

This dependency graph is the core of the design. Folder naming only matters insofar as it keeps dependencies obvious.

## Package Policy

The architecture standard documents package usage as policy rather than implementation detail.

- `VContainer`: required for composition and scope wiring.
- `R3`: required for reactive state and local streams.
- `MessagePipe`: supported as the cross-context one-way event bus.
- `UniTask`: supported for async orchestration.
- `ZLogger` / `NhemLogger`: supported for diagnostics.
- `ObservableCollections`: supported for collection deltas, but not in Domain or public cross-layer contracts.
- `ZLinq`: allowed only in measured hot paths.

Networking, DOTween, FishNet, and other external systems are optional adapters and do not belong to the architecture baseline.

## Scope and Bootstrap

The standard includes:

- root bootstrap through `VContainerSettings`
- `ProjectLifetimeScope` as the composition root
- feature-specific lifetime scopes via interface markers
- source-generator registration via `AutoRegisterIn`
- explicit diagnostics for registration and scope violations

The standard does not hardcode game-specific scope names. A project may use `Run`, `Match`, `Player`, or any other feature scopes as long as dependency direction remains valid.

## Enforcement

Option B only works if it is enforced.

The architecture standard should include:

- rules for namespace and folder alignment
- analyzer or hook checks for layer drift
- tests that assert forbidden dependencies in `Domain`
- composition checks for duplicate or missing registrations

## Risks

- Too many subfolders can recreate the same clutter the architecture is trying to prevent.
- `Shared` can become a dumping ground if rules are weak.
- Optional package support can drift into implicit hard dependencies unless the docs are explicit.

## Decision

The recommended shape is Option B as the canonical architecture for the project. The standard should stay close to the concrete folder and asmdef layout above, not drift into a separate meta-framework or an extra reusable kernel layer.
