## 1. Architecture Foundation

- [x] 1.1 Finalize the canonical folder tree for `01_Domain` through `06_Shared`, including `Features`, `Shared`, `Contracts`, and `Messages` locations.
- [x] 1.2 Define the namespace mapping rules for each layer and feature folder so new files can be placed without ambiguity.
- [x] 1.3 Update the architecture docs and AI rules to reflect the feature-first layout as the project standard.

## 2. Application Layer Migration

- [ ] 2.1 Move feature-specific code out of `Application/Services` into `Application/Features/<Feature>/...`.
- [ ] 2.2 Move feature-local interfaces and contracts next to their owning feature folders.
- [ ] 2.3 Create or update `Application/Shared`, `Application/Contracts`, and `Application/Messages` only for cross-cutting code.
- [ ] 2.4 Update Application namespaces, imports, and tests to match the new folder structure.

## 3. Domain, Infrastructure, and Presentation Alignment

- [ ] 3.1 Move feature-specific Domain code into `Domain/Features/...` where it improves clarity and ownership.
- [ ] 3.2 Move feature-specific Infrastructure adapters into `Infrastructure/Features/...` and keep only true shared adapters in `Infrastructure/Shared`.
- [ ] 3.3 Move feature-specific Presentation code into `Presentation/Features/...` and keep shell-level UI code isolated.
- [ ] 3.4 Update namespace declarations and references for all moved layer code.

## 4. Composition, Tests, and Enforcement

- [ ] 4.1 Update VContainer installers, registrations, and scope wiring to the new feature namespaces.
- [ ] 4.2 Update test namespaces, test fixtures, and fake implementations to mirror the new folder layout.
- [ ] 4.3 Update asmdef references if any assemblies depend on the old `Services` or root-level feature layout.
- [x] 4.4 Add or update architecture guardrails so `Application/Services` cannot reappear as a catch-all bucket.
- [ ] 4.5 Run Unity compile and test verification after each migration batch and fix any regressions before continuing.
