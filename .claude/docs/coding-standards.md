# Coding Standards

- Feature-specific application code belongs under `Assets/_Project/Application/Features/<Feature>/...`
- Feature-specific code in other layers belongs under `Assets/_Project/<Layer>/Features/<Feature>/...`
- `Assets/_Project/Application/Services/...` is reserved for narrow cross-cutting services only during migration
- Folder and namespace should match the feature or layer path 1:1
- Do not use `Services` as a catch-all for unrelated gameplay code

- All game code must include doc comments on public APIs
- Every system must have a corresponding architecture decision record in `docs/architecture/`
- Gameplay values must be data-driven (external config), never hardcoded
- All public methods must be unit-testable (dependency injection over singletons)
- Commits must reference the relevant design document or task ID
- **Verification-driven development**: Write tests first when adding gameplay systems.
  For UI changes, verify with screenshots. Compare expected output to actual output
  before marking work complete. Every implementation should have a way to prove it works.

# Reactive and Messaging Standards

- Use **R3** for single-value state and local reactive streams with a clear owner.
- Use **ObservableCollections** only when observers need collection delta events such as add/remove/move/replace/range changes.
- Use **MessagePipe** for one-way events that cross modules, layers, or DI scopes and may have multiple consumers.
- Use **ZLinq** only in profiled or obviously hot paths where allocations matter.
- Use **INhemLogger** for project logging in DI-managed classes. Direct `NhemUnityLogger` construction is allowed only in static loaders or MonoBehaviours that are not created by DI.

## Package Selection Rules

- If the problem is "one value changed", use **R3**.
- If the problem is "this list/dictionary changed", use **ObservableCollections**.
- If the problem is "multiple independent modules should be notified", use **MessagePipe**.
- If the problem is "this query runs in a hot path and allocates too much", consider **ZLinq** after profiling.

## Package Restrictions

- **R3** is not a replacement for a global event bus and should stay out of `Domain`.
- **ObservableCollections** must not appear in `Domain` or in public contracts that cross layers.
- **MessagePipe** must not become a second state store and should not be used for request/response.
- **ZLinq** is disallowed in tests, most UI/presentation code, and ordinary application orchestration.
- **UnityEngine.Debug** is disallowed in `Assets/_Project` production code. Use `INhemLogger` or `NhemUnityLogger` instead.
- Do not mix **ZLinq** and `System.Linq` in the same hot path without an explicit reason.

# Design Document Standards

- All design docs use Markdown
- Each mechanic has a dedicated document in `design/gdd/`
- Documents must include these 8 required sections:
  1. **Overview** -- one-paragraph summary
  2. **Player Fantasy** -- intended feeling and experience
  3. **Detailed Rules** -- unambiguous mechanics
  4. **Formulas** -- all math defined with variables
  5. **Edge Cases** -- unusual situations handled
  6. **Dependencies** -- other systems listed
  7. **Tuning Knobs** -- configurable values identified
  8. **Acceptance Criteria** -- testable success conditions
- Balance values must link to their source formula or rationale

# Testing Standards

## Test Evidence by Story Type

All stories must have appropriate test evidence before they can be marked Done:

| Story Type | Required Evidence | Location | Gate Level |
|---|---|---|---|
| **Logic** (formulas, AI, state machines) | Automated unit test — must pass | `tests/unit/[system]/` | BLOCKING |
| **Integration** (multi-system) | Integration test OR documented playtest | `tests/integration/[system]/` | BLOCKING |
| **Visual/Feel** (animation, VFX, feel) | Screenshot + lead sign-off | `production/qa/evidence/` | ADVISORY |
| **UI** (menus, HUD, screens) | Manual walkthrough doc OR interaction test | `production/qa/evidence/` | ADVISORY |
| **Config/Data** (balance tuning) | Smoke check pass | `production/qa/smoke-[date].md` | ADVISORY |

## Automated Test Rules

- **Naming**: `[system]_[feature]_test.[ext]` for files; `test_[scenario]_[expected]` for functions
- **Determinism**: Tests must produce the same result every run — no random seeds, no time-dependent assertions
- **Isolation**: Each test sets up and tears down its own state; tests must not depend on execution order
- **No hardcoded data**: Test fixtures use constant files or factory functions, not inline magic numbers
  (exception: boundary value tests where the exact number IS the point)
- **Independence**: Unit tests do not call external APIs, databases, or file I/O — use dependency injection

## What NOT to Automate

- Visual fidelity (shader output, VFX appearance, animation curves)
- "Feel" qualities (input responsiveness, perceived weight, timing)
- Platform-specific rendering (test on target hardware, not headlessly)
- Full gameplay sessions (covered by playtesting, not automation)

## CI/CD Rules

- Automated test suite runs on every push to main and every PR
- No merge if tests fail — tests are a blocking gate in CI
- Never disable or skip failing tests to make CI pass — fix the underlying issue
- Engine-specific CI commands:
  - **Godot**: `godot --headless --script tests/gdunit4_runner.gd`
  - **Unity**: `game-ci/unity-test-runner@v4` (GitHub Actions)
  - **Unreal**: headless runner with `-nullrhi` flag
