## ADDED Requirements

### Requirement: Canonical Option B Architecture
The architecture standard SHALL define Option B as the canonical Unity project structure for Solar Phobia and future projects that adopt the same layout.

#### Scenario: Standard Is Project-Neutral
- **WHEN** a new project adopts the standard
- **THEN** the documented folder names, namespace rules, and assembly layout SHALL not depend on SolarPhobia-specific nouns or scene names

### Requirement: Six-Assembly Layer Model
The standard SHALL define a six-assembly model covering `Domain`, `Application`, `Infrastructure`, `Presentation`, `Composition`, and `Shared`.

#### Scenario: Dependency Direction Is Explicit
- **WHEN** a project uses the kernel
- **THEN** `Domain` SHALL remain dependency-free
- **AND** `Application` SHALL depend only on `Domain` plus approved support packages
- **AND** `Infrastructure` and `Presentation` SHALL remain above `Domain` and `Application`
- **AND** `Composition` SHALL be allowed to wire all layers without containing gameplay logic

### Requirement: Feature-First Project Layout
The standard SHALL define feature-first organization inside the runtime layers so feature-specific code is grouped by bounded context instead of a flat services bucket.

#### Scenario: Feature Code Is Not Flat
- **WHEN** a feature-specific class is added
- **THEN** it SHALL live under a feature folder such as `Features/<Feature>/...`
- **AND** it SHALL not be added to a catch-all `Services` root

### Requirement: Bootstrap and Scope Model
The standard SHALL define a bootstrap model based on VContainer settings, a root lifetime scope, and explicit scope markers for feature/module lifetimes.

#### Scenario: New Project Boots Without Bootstrap Scene Logic
- **WHEN** a project starts
- **THEN** the root scope SHALL be loaded through VContainer settings
- **AND** feature scopes SHALL be registered through marker interfaces or equivalent reusable scope contracts
- **AND** the bootstrap model SHALL not rely on SolarPhobia-specific scene naming

### Requirement: Package Usage Policy
The standard SHALL define package usage policy for reactive, messaging, logging, async, and collection-change tooling so the same rules can be reused across projects.

#### Scenario: Reactive State Uses the Right Tool
- **WHEN** a project models single-value local state
- **THEN** it SHALL use `R3`-style reactive primitives
- **AND** when it models collection deltas it SHALL use `ObservableCollections`-style observables
- **AND** cross-context one-way events SHALL use a message-bus style integration

### Requirement: Architecture Enforcement
The standard SHALL include enforcement guidance or tooling hooks that detect layer drift, namespace drift, and invalid package usage.

#### Scenario: Domain Remains Pure
- **WHEN** a project adds a new `Domain` type
- **THEN** enforcement SHALL flag Unity, reactive, or messaging dependencies that violate the domain boundary

#### Scenario: Duplicate Registrations Are Flagged
- **WHEN** a service is both auto-registered and manually registered in composition
- **THEN** the kernel SHALL surface the duplication as a diagnostic issue

### Requirement: Option B Reference Coverage
The standard SHALL provide enough reference material to make Option B directly implementable in a new project.

#### Scenario: Fresh Project Has a Valid Skeleton
- **WHEN** a new project is generated from the reference material
- **THEN** it SHALL include at least one sample feature slice, one sample test, one composition root, and one bootstrap flow example
