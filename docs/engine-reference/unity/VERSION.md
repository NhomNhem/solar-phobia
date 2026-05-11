# Unity — Version Reference

| Field | Value |
|-------|-------|
| **Engine Version** | Unity 6000.3.11f1 (Unity 6) |
| **Project Pinned** | 2026-05-07 |
| **LLM Knowledge Cutoff** | ~2023 (Unity 2022.x / early 6000.x) |
| **Risk Level** | HIGH — version is beyond LLM training data |

## Knowledge Gap Analysis

Unity 6 is a major release beyond the LLM's training data. Key areas requiring verification:

- **VContainer** — New DI package, not in training data
- **R3** — New reactive system, not in training data
- **UI Toolkit** — Significant changes from uGUI
- **Entities/DOTS** — Major API changes
- **Scriptable Render Pipeline** — New features and defaults

## Usage

Before making architectural decisions that touch HIGH RISK areas:
1. Check `breaking-changes.md` for migration issues
2. Check `deprecated-apis.md` for APIs to avoid
3. Check `current-best-practices.md` for recommended patterns
4. Use WebSearch to verify uncertain APIs against official Unity docs

## Last Verified

2026-05-07 — via /setup-engine

---

## Engine Version Reference

@docs/engine-reference/unity/VERSION.md