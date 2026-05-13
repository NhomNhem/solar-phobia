## Context

Solar Phobia has already moved away from a flat `Application/Services` model in several places, but the codebase still risks regressing into a new catch-all structure as the project scales. The current goal is to make feature boundaries explicit inside every architectural layer so folder structure, namespaces, and dependency rules reinforce the same model.

The target state is a feature-first layered layout:

- `01_Domain/Features/...`
- `02_Application/Features/...`
- `03_Infrastructure/Features/...`
- `04_Presentation/Features/...`
- `05_Composition/...`
- `06_Shared/...`

This design must preserve clean architecture boundaries, keep DI wiring separate, and avoid breaking Unity serialization or test discovery during migration.

## Goals / Non-Goals

**Goals:**

- Make feature boundaries the default organizing principle inside each layer.
- Remove `Application/Services` as a general-purpose bucket.
- Align namespaces 1:1 with folders so code discovery is predictable.
- Keep `Shared` narrow and truly cross-cutting.
- Preserve compile stability while migrating incrementally.

**Non-Goals:**

- Rewriting gameplay behavior or tuning game balance.
- Introducing new runtime dependencies.
- Changing the visible feature set of the game.
- Forcing every existing utility into a feature folder if it is genuinely cross-cutting.

## Decisions

1. **Use `Features` as the primary substructure inside each layer.**
   This is preferable to keeping technical buckets like `Services` because the repo has already shown that technical buckets become dumping grounds over time. Feature-first structure keeps related use cases, ports, and adapters close together.
   Alternatives considered:
   - Keep `Application/<Feature>` at the root. Rejected because the layer root becomes a long flat list as the project grows.
   - Keep `Application/Services/<Feature>`. Rejected because `Services` remains a misleading technical bucket and still invites catch-all usage.

2. **Keep `Shared` narrow and explicit.**
   `Shared` is necessary, but only for cross-cutting code that is truly reused across multiple features. This avoids recreating a global dumping ground under a new name.
   Alternatives considered:
   - Remove `Shared` entirely. Rejected because some contracts and helpers are genuinely cross-cutting.
   - Allow feature code in `Shared`. Rejected because it weakens boundaries and reintroduces ambiguity.

3. **Align namespaces to folder boundaries.**
   Namespaces should mirror the folder tree so imports and searches remain obvious. This makes refactors mechanical and reduces accidental coupling.
   Alternatives considered:
   - Keep legacy namespaces after file moves. Rejected because stale namespaces hide ownership and make code harder to review.

4. **Keep `Composition` wiring-only.**
   Composition should remain the place for installers, scope setup, and bootstrap orchestration. It should not become another feature bucket.
   Alternatives considered:
   - Mix bootstrap and feature logic together. Rejected because it blurs runtime ownership and makes DI harder to reason about.

5. **Migrate incrementally with compile checkpoints.**
   The current project is already mid-refactor, so a big-bang rename is too risky. Each migration batch must preserve compile health and update tests at the same time.
   Alternatives considered:
   - Full repo rename in one shot. Rejected because of Unity serialization risk and source-generator/asmdef churn.

## Risks / Trade-offs

- [Risk] Large namespace and path churn may break references in Unity assets and tests. → [Mitigation] Migrate in small batches and verify compile after every batch.
- [Risk] `Shared` may become a new dumping ground. → [Mitigation] Keep `Shared` review rules strict and require feature placement whenever a boundary exists.
- [Risk] Some legacy code will not fit neatly into a feature on the first pass. → [Mitigation] Allow temporary compatibility placements, then move them again once the owning feature is identified.
- [Risk] Folder depth becomes longer. → [Mitigation] Keep paths descriptive but consistent; use feature names to improve discoverability instead of flattening everything.
- [Risk] Existing docs may diverge from the new structure during migration. → [Mitigation] Update architecture docs and coding rules in the same change set whenever a layer rule changes.

## Migration Plan

1. Define the target folder tree and namespace rules for every layer.
2. Move Application code from `Services` buckets into `Application/Features/...` and `Application/Shared/...`.
3. Move matching feature-specific code in Domain, Infrastructure, and Presentation into aligned feature folders where it improves clarity.
4. Update namespace declarations, asmdef references, and DI registrations in the same batch.
5. Update tests to mirror the new layout.
6. Keep compile verification after each migration batch and stop if a batch introduces unresolved references.

Rollback strategy:

- Revert the last migration batch if compile breaks cannot be resolved quickly.
- Restore namespace imports and asmdef references together with the file move.
- Keep the previous layout only as long as needed to unblock the current batch, not as a permanent fallback.

## Open Questions

- Which exact top-level feature groups should be canonical for each layer: `Phase`, `Player`, `Combat`, `Consequences`, `Ward`, `MainMenu`, `Shared`, `Contracts`, `Messages`, or a slightly different final taxonomy?
- Should `Domain` also be converted fully to `Domain/Features/...`, or should only feature-heavy domain areas use that structure?
- Should `Presentation` use `Presentation/Features/...` exclusively, or keep a small number of technical folders for UI shell concerns?
