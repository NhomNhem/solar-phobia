## Code Review: Project-wide (SOLID, VContainer/DI, UniTask/Awaitable, R3, MessagePipe, ObserverCollections)

Date: 2026-05-09
Scope: `Assets/_Project` first-party runtime/application/infrastructure/presentation code

### Standards Compliance: 3/6 passing
- Fails:
  - Complete DI registration coverage in composition root.
  - Lifecycle disposal consistency for R3 subscriptions.
  - Domain encapsulation around observable collections.
- Passes:
  - Interface-first service design is broadly applied.
  - Layering intent is clear across Domain/Application/Infrastructure/Presentation.
  - Many public services include XML doc comments.

### Architecture: VIOLATIONS FOUND
- Incomplete VContainer composition root means many injected dependencies are not resolvable at runtime.
- Some services depend on reactive primitives directly instead of domain/application abstractions.
- Domain entity exposes mutable observable collection directly, leaking internal invariants.

### SOLID: ISSUES FOUND
- DIP violations in runtime wiring where concrete reactive types are injected instead of service abstractions.
- SRP/encapsulation issues where entity internals are externally mutable.
- OCP is generally good in state-machine patterns, but runtime bootstrapping is not extension-safe due to missing registrations.

### Game-Specific Concerns
- Duplicate reactive subscription risk can duplicate gameplay signal handling.
- Runtime hazard cleanup uses `DestroyImmediate`, which is risky in play-mode loops.
- Subscription lifecycle not uniformly disposed can leak behavior across scene transitions.

### Positive Observations
- R3 usage patterns (ReactiveProperty + event streams) are mostly consistent and readable.
- Phase/state service boundaries are generally test-friendly.
- Presentation classes mostly keep gameplay state ownership in application services.

### Required Changes
1. Complete VContainer registrations for all runtime `[Inject]` dependencies.
2. Refactor `KarmaHazardService` to consume `IPhaseStateMachine` (or dedicated phase abstraction), not `ReadOnlyReactiveProperty<PhaseState>`.
3. Make `PlayerInputHandler` dispose `_phaseSubscription` (implement `IDisposable` lifecycle).
4. Prevent duplicate subscriptions in `PlayerController.SubscribeToStrikeWarning()` via dispose-before-resubscribe guard.
5. Replace runtime `DestroyImmediate` cleanup with runtime-safe destruction flow.
6. Encapsulate `Order.ItemsNeeded` mutation API; do not expose mutable `ObservableDictionary` directly.

### Suggestions
1. Add a DI container validation test that resolves scene-entry MonoBehaviours and core services.
2. Introduce MessagePipe only for cross-bounded-context messaging; keep R3 for local reactive state.
3. If async workflows expand, standardize on UniTask with explicit cancellation/lifecycle conventions.

### Verdict: CHANGES REQUIRED

---

## Detailed Findings (ordered by severity)

### 1) Incomplete DI graph in VContainer composition root (High)
- `CoreLifetimeScope` currently registers only three services:
  - `IPhaseStateMachine`
  - `ISoulRepository`
  - `IDayPhaseTimelineService`
- Runtime classes in Presentation/Application/Infrastructure request many additional dependencies via `[Inject]` that are not registered here.

References:
- `Assets/_Project/Application/Installers/CoreInstaller.cs:18`
- `Assets/_Project/Application/Installers/CoreInstaller.cs:21`
- `Assets/_Project/Application/Installers/CoreInstaller.cs:24`
- Example consumer: `Assets/_Project/Presentation/Player/PlayerController.cs:26`

Impact:
- Runtime resolve failures, partial startup, and fragile scene behavior.

---

### 2) DI contract fragility: injecting `ReadOnlyReactiveProperty<PhaseState>` directly (High)
- `KarmaHazardService` constructor injects a reactive primitive directly.
- This creates tight coupling to one producer detail and weakens Dependency Inversion.

Reference:
- `Assets/_Project/Application/Services/KarmaHazardService.cs:36`

Impact:
- Harder test setup, brittle container configuration, weaker architectural boundaries.

---

### 3) Duplicate strike-warning subscription risk (High)
- `PlayerController` subscribes to mode stream in `Start()`, and also conditionally subscribes immediately when starting already in `NightMovement`.
- If mode subscription emits current value immediately, this can result in duplicate `OnStrikeWarning` subscriptions.
- `SubscribeToStrikeWarning()` does not dispose existing subscription before replacing.

References:
- `Assets/_Project/Presentation/Player/PlayerController.cs:44`
- `Assets/_Project/Presentation/Player/PlayerController.cs:49`
- `Assets/_Project/Presentation/Player/PlayerController.cs:90`
- `Assets/_Project/Presentation/Player/PlayerController.cs:106`

Impact:
- Duplicate event handling and inconsistent warning behavior.

---

### 4) Missing disposal for `PlayerInputHandler` subscription (Medium)
- `_phaseSubscription` is assigned during initialize but never disposed.
- Class is not `IDisposable`.

References:
- `Assets/_Project/Application/Services/PlayerInputHandler.cs:33`
- `Assets/_Project/Application/Services/PlayerInputHandler.cs:62`

Impact:
- Subscription leaks across lifetime boundaries.

---

### 5) Domain encapsulation leak with `ObservableDictionary` exposure (Medium)
- `Order.ItemsNeeded` is publicly mutable and observable.
- External code can mutate entity state without invariant guards.

Reference:
- `Assets/_Project/Domain/Entities/Order.cs:7`

Impact:
- Domain rule bypass and harder reasoning about entity correctness.

---

### 6) Runtime use of `DestroyImmediate` in hazard cleanup (Medium)
- `ClearHazards()` uses `GameObject.DestroyImmediate(..., true)`.

Reference:
- `Assets/_Project/Application/Services/KarmaHazardService.cs:77`

Impact:
- Potential runtime instability and editor/runtime behavior mismatch.

---

## Focus-Area Status

### VContainer / DI
- **Status:** Issues found.
- Main gaps: incomplete registration coverage, fragile primitive-type injection.

### R3
- **Status:** Mostly good, with lifecycle issues.
- Main gaps: inconsistent disposal and one duplicate-subscription risk.

### MessagePipe
- **Status:** Not in active use in first-party code under `Assets/_Project`.
- No direct defects found; this is a capability gap if cross-context pub/sub is intended.

### UniTask / Awaitable
- **Status:** Not in active use in first-party code under `Assets/_Project`.
- No direct defects found; no async lifecycle conventions established yet.

### ObserverCollections
- **Status:** In use; encapsulation concerns found.
- Main gap: domain mutable observable dictionary exposure.

---

## Residual Testing Gaps
- Container bootstrap/resolve smoke test absent for full runtime graph.
- No explicit regression test proving single subscription behavior for strike-warning flow.
- No guard test around hazard cleanup behavior in play mode.