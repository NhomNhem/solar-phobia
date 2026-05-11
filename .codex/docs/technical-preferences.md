# Technical Preferences

> **Status**: BASELINE (Gate Evidence Ready)
> **Purpose**: Define performance budgets and technical constraints for the project
> **Reference**: AGENTS.md Build & Test Commands

---

## Performance Budgets

### Frame Rate
| Platform | Target FPS | Acceptable Range |
|----------|------------|------------------|
| Windows | 60 | 55-60 |
| macOS | 60 | 55-60 |
| Console (target) | 60 | 55-60 |

### Load Times
| Metric | Target | Maximum |
|--------|--------|---------|
| Cold start to main menu | < 5s | 10s |
| Scene transition (day→night) | < 2s | 4s |
| Save/load game | < 1s | 3s |

### Memory
| Metric | Target | Maximum |
|--------|--------|---------|
| Runtime memory (PC) | < 2GB | 3GB |
| Asset memory budget | < 1.2GB | 1.8GB |

---

## Rendering

- **Pipeline**: Universal Render Pipeline (URP)
- **Resolution**: Native with dynamic resolution scaling if needed
- **Post-processing**: On (Bloom, Vignette for sensory feedback)

---

## Input

- **Input System**: New Input System (InputActions)
- **Keyboard/Mouse**: Primary
- **Controller**: Future consideration

---

## Code Quality

- **Architecture**: Clean Architecture (Domain → Application → Infrastructure → Presentation)
- **Dependency Injection**: VContainer
- **Testing**: NUnit via Unity Test Framework

---

## Naming Conventions

Reference: `Assets/_Project/Domain/Rules/NamingConventions.cs`

---

## Build Targets

- Windows (primary)
- macOS (secondary)
- Future: Console platforms

---

## Evidence Locations

- Core mechanics mapping: `production/qa/evidence/core-mechanics-cross-reference-2026-05-10.md`
- Test location index: `tests/README.md`
- Performance artifact drop location: `tests/performance/`

## Remaining Manual Validation

- Run Unity profiler and export captures to `tests/performance/`
- Run EditMode + PlayMode suites on machine with Unity CLI and archive output in `production/qa/`
