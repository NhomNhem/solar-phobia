---
paths:
  - "src/gameplay/**"
---

# Gameplay Code Rules

- ALL gameplay values MUST come from external config/data files, NEVER hardcoded
- Use delta time for ALL time-dependent calculations (frame-rate independence)
- NO direct references to UI code — use events/signals for cross-system communication
- Every gameplay system must implement a clear interface
- State machines must have explicit transition tables with documented states
- Write unit tests for all gameplay logic — separate logic from presentation
- Document which design doc each feature implements in code comments
- No static singletons for game state — use dependency injection
- Feature-specific application code should live under `Application/Features/<Feature>/...`; use `Application/Services` only for narrow cross-cutting services during migration
- Use **R3** for single-value reactive state and local streams, not for global event bus behavior
- Use **ObservableCollections** only when collection add/remove/move/replace deltas matter; never in Domain or public cross-layer contracts
- Use **MessagePipe** for one-way cross-context gameplay events with multiple decoupled consumers; do not use it as state storage or request/response
- Do NOT use **ZLinq** in normal gameplay orchestration; only allow it in profiled hot paths with a clear allocation/performance reason

## Examples

**Correct** (data-driven):

```gdscript
var damage: float = config.get_value("combat", "base_damage", 10.0)
var speed: float = stats_resource.movement_speed * delta
```

**Incorrect** (hardcoded):

```gdscript
var damage: float = 25.0   # VIOLATION: hardcoded gameplay value
var speed: float = 5.0      # VIOLATION: not from config, not using delta
```
