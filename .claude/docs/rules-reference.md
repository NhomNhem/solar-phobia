# Path-Specific Rules

Rules in `.claude/rules/` are automatically enforced when editing files in matching paths:

| Rule File | Path Pattern | Enforces |
| ---- | ---- | ---- |
| `gameplay-code.md` | `src/gameplay/**` | Data-driven values, delta time, no UI references |
| `engine-code.md` | `src/core/**` | Zero allocs in hot paths, thread safety, API stability, package performance discipline |
| `ai-code.md` | `src/ai/**` | Performance budgets, debuggability, data-driven params |
| `network-code.md` | `src/networking/**` | Server-authoritative, versioned messages, security |
| `ui-code.md` | `src/ui/**` | No game state ownership, localization-ready, accessibility |
| `design-docs.md` | `design/gdd/**` | Required 8 sections, formula format, edge cases |
| `narrative.md` | `design/narrative/**` | Lore consistency, character voice, canon levels |
| `data-files.md` | `assets/data/**` | JSON validity, naming conventions, schema rules |
| `test-standards.md` | `tests/**` | Test naming, coverage requirements, fixture patterns |
| `prototype-code.md` | `prototypes/**` | Relaxed standards, README required, hypothesis documented |
| `shader-code.md` | `assets/shaders/**` | Naming conventions, performance targets, cross-platform rules |

## Cross-Cutting Package Policy

- **R3**: single-value state and local reactive streams.
- **ObservableCollections**: collection delta observation only; not allowed in Domain or public cross-layer contracts.
- **MessagePipe**: one-way cross-context events only; not state storage and not request/response.
- **ZLinq**: hot-path optimization tool only; avoid in ordinary code and tests unless profiling justifies it.

## Solar Phobia Folder Rule

- Feature-specific application code lives under `Assets/_Project/Application/<Feature>/...`
- `Assets/_Project/Application/Services/...` is reserved for cross-cutting services only
- Namespace must match the folder path and the feature boundary
- Do not use `Services` as a default bucket for unrelated gameplay modules
