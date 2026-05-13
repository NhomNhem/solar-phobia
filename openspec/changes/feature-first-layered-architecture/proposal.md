## Why

The current codebase has improved, but it still drifts toward catch-all folders and mixed responsibilities as the project scales. We need a single source of truth for a feature-first layout so future refactors stop reintroducing `Services`-style buckets and namespace mismatches.

## What Changes

- Reorganize the repo around feature-first folders inside each architectural layer.
- Replace broad root-level buckets like `Application/Services` with `Application/Features` plus a narrow `Application/Shared` for true cross-cutting code.
- Apply the same pattern consistently to `Domain`, `Infrastructure`, `Presentation`, and `Composition` where it improves scale and discoverability.
- Align namespaces 1:1 with folder boundaries so type discovery, imports, and reviews stay predictable.
- Keep `Shared` folders narrowly scoped to reusable primitives, contracts, and helpers that genuinely cross feature boundaries.
- **BREAKING** Update file paths and namespaces for moved types across the project.
- **BREAKING** Update tests, installers, and any serialized references affected by namespace or file moves.

## Capabilities

### New Capabilities
- `feature-first-layered-architecture`: Defines the target folder tree, namespace rules, and layer boundaries for a feature-first Solar Phobia codebase across Domain, Application, Infrastructure, Presentation, Composition, and Shared.

### Modified Capabilities
- None.

## Impact

- `Assets/_Project/01_Domain`, `02_Application`, `03_Infrastructure`, `04_Presentation`, `05_Composition`, `06_Shared` folder layout and their namespaces.
- VContainer installers, source-generator registrations, and assembly definition references.
- Unit tests and test namespaces that mirror the source layout.
- Existing documentation in `docs/architecture`, `AGENTS.md`, and `.claude/*` rules files.
- Any code currently depending on `Application/Services` as a catch-all bucket.
