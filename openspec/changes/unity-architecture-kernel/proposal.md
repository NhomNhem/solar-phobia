## Why

Solar Phobia has already validated a strong Unity architecture pattern, but the conventions need to be captured in a stable, reusable project layout instead of living as scattered project-specific guidance. We want to formalize Option B as the standard architecture shape so future work stays consistent and the current repo stops drifting back to catch-all folders.

## What Changes

- Introduce a neutral `unity-architecture-kernel` capability that defines the reusable architecture contract for Unity projects.
- Codify Option B as the canonical project architecture: six asmdefs, VContainer root scope, `IScopeMarkers`, `AutoRegisterIn`, and clear layer boundaries.
- Keep feature code under `Application/Features`, while preserving the exact layer buckets for `Domain`, `Infrastructure`, `Presentation`, `Composition`, and `Shared`.
- Define enforcement conventions for namespaces, DI scope markers, logging, reactive state, message bus usage, and package policy.
- Keep Solar Phobia as the reference implementation of Option B.
- **BREAKING**: Replace ambiguous folder guidance with the stricter Option B layout and namespace rules.

## Capabilities

### New Capabilities
- `unity-architecture-kernel`: formalized Unity project architecture standard based on Option B, covering layer structure, scope wiring, package policy, and enforcement rules.

### Modified Capabilities
- None.

## Impact

- `openspec/changes/unity-architecture-kernel/*`
- architecture docs and future template scaffolding
- package usage conventions for VContainer, R3, MessagePipe, UniTask, ZLogger, and the Unity Input System
- future project bootstrapping, DI wiring, and layer naming conventions
- no gameplay mechanics or content changes
