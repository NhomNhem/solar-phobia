## Tasks

- [x] Define Option B as the canonical project shape
  - [x] Finalize the exact folder tree and namespace rules for `Domain`, `Application`, `Infrastructure`, `Presentation`, `Composition`, and `Shared`
  - [x] Lock the six-asmdef dependency matrix and the VContainer bootstrap flow
  - [x] Confirm which packages are core, optional, or feature-level adapters

- [x] Write the reference layout and rules
  - [x] Document the exact Option B folder structure for Domain, Application, Infrastructure, Presentation, Composition, and Shared
  - [x] Document namespace rules, feature-first placement rules, and the no catch-all Services bucket rule
  - [x] Capture package usage policies for R3, ObservableCollections, MessagePipe, UniTask, ZLogger, and ZLinq

- [x] Add enforcement guidance
  - [x] Define the analyzer/hook checks that protect layer boundaries and registration rules
  - [x] Document the diagnostics expected for duplicate registrations, forbidden dependencies, and namespace drift
  - [x] Specify the minimum test harness required to prove the standard stays valid

- [x] Add starter examples
  - [x] Provide a minimal feature slice that shows the folder layout and dependency direction
  - [x] Provide a minimal composition root and bootstrap flow example
  - [x] Provide a minimal test example that validates the architecture rules

- [x] Align Solar Phobia as the reference implementation
  - [x] Map the current Solar Phobia architecture docs to the Option B vocabulary
  - [x] Remove ambiguous guidance where it conflicts with the canonical layout
  - [x] Keep Solar Phobia as the reference project for the standard
