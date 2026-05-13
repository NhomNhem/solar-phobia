## ADDED Requirements

### Requirement: Feature-first application layout
The Application layer SHALL organize feature-specific code under `Application/Features/<Feature>/...`.
The Application layer SHALL reserve `Application/Shared/...`, `Application/Contracts/...`, and `Application/Messages/...` for cross-cutting code only.
The Application layer SHALL NOT use `Application/Services/...` as a catch-all bucket for unrelated feature code.

#### Scenario: New gameplay feature is added
- **WHEN** a new gameplay use case is introduced
- **THEN** its use cases, services, and feature-local contracts SHALL be placed under a dedicated `Application/Features/<Feature>/...` folder

### Requirement: Layer-specific feature folders
The Domain, Infrastructure, and Presentation layers SHALL also support feature-first subfolders when a feature boundary is clearer than a technical bucket.
Each layer SHALL keep shared utilities in a narrowly scoped `Shared` area rather than a broad technical catch-all.

#### Scenario: A combat adapter is implemented
- **WHEN** a combat-specific implementation is created in Infrastructure
- **THEN** it SHALL live under `Infrastructure/Features/Combat/...` or another feature-aligned folder rather than a generic root service bucket

### Requirement: Namespace alignment
Namespaces SHALL match folder boundaries one-to-one for all new and migrated code.
Types moved into feature folders SHALL be renamed to reflect the feature namespace and SHALL NOT keep stale root-level namespaces.

#### Scenario: A service is moved into a feature folder
- **WHEN** `DayPhaseMechanicsService` is moved into `Application/Features/Phase/Day/...`
- **THEN** its namespace SHALL become a feature-aligned namespace under `SolarPhobia.Application.Features.Phase.Day`

### Requirement: Shared code boundaries
Shared code SHALL only contain cross-cutting primitives, contracts, helpers, and adapters that are genuinely reused across multiple features.
Shared code SHALL NOT absorb feature-specific behavior simply because no folder currently exists.

#### Scenario: A reusable math helper is created
- **WHEN** a helper is used by multiple unrelated features
- **THEN** it MAY be placed in a narrow `Shared` area
- **AND** it SHALL NOT be promoted to a feature-specific service or use case

### Requirement: Composition and bootstrap separation
Composition code SHALL remain limited to wiring, installers, and bootstrap concerns.
Composition code SHALL NOT contain feature business logic and SHALL reference feature namespaces only for registration and orchestration.

#### Scenario: A new installer is added
- **WHEN** a new VContainer installer is created
- **THEN** it SHALL register feature types without embedding gameplay rules or UI behavior

### Requirement: Migration safety
Moves between folders and namespaces SHALL preserve behavior, tests, and serialization-friendly references where applicable.
Refactoring SHALL update test namespaces and importer references together with source moves.

#### Scenario: A feature is migrated to the new layout
- **WHEN** a feature folder is renamed or moved
- **THEN** the corresponding tests, namespace imports, and DI registrations SHALL be updated in the same change
- **AND** the project SHALL continue to compile after the migration
