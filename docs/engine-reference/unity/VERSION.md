# Unity — Version Reference

| Field | Value |
|-------|-------|
| **Engine Version** | Unity 6000.3.11f1 (Unity 6) |
| **Project Pinned** | 2026-05-07 |
| **LLM Knowledge Cutoff** | ~2023 (Unity 2022.x / early 6000.x) |
| **Risk Level** | HIGH — version is beyond LLM training data |

## Knowledge Gap Analysis

Unity 6 is a major release beyond the LLM's training data. Key areas requiring verification:

- **VContainer** — DI framework used by this project (verify API + lifetime scopes on Unity 6)
- **R3** — Reactive system used for state observation (verify operators + disposal patterns)
- **ObservableCollections (Cysharp)** — High-performance observable collections + views, often paired with R3 (verify package + Unity integration)
- **UI Toolkit** — Significant changes vs uGUI (verify runtime + binding workflow)
- **Entities/DOTS** — Major API changes (verify package versions + authoring flow)
- **Scriptable Render Pipeline** — New features and defaults (verify URP/HDRP behaviors, renderer features)
- **ScriptableObject** — Safe for static config, risky for runtime mutable state (see Notes)
- **MessagePipe** — Event/messaging pipeline (verify VContainer integration + allocations + disposal)
- **DOTween (Demigiant)** — Tween/animation utility (verify Unity 6 compatibility + setup workflow)
- **ZLinq (Cysharp)** — “Zero-allocation” LINQ replacement (verify package + conventions)

## Pinned Packages (Project Baseline)

> Keep this table updated whenever you change package versions or install sources. This is the baseline to prevent machine-to-machine drift.

| Package | Source | Version | Notes / Verify |
|--------|--------|---------|----------------|
| Unity Editor | Unity Hub | 6000.3.11f1 | Engine pinned by Project Pinned date |
| VContainer | UPM / Git | (pin here) | Verify lifetime scopes + Unity 6 compatibility |
| R3 | OpenUPM / UPM | (pin here) | Verify operators + disposal/disposables |
| ObservableCollections (Cysharp) | OpenUPM | (pin here) | Prefer `ObservableCollections.R3` when observing collection changes |
| MessagePipe | UPM / Git | (pin here) | Verify VContainer integration + allocation behavior |
| DOTween (Demigiant) | Asset Store | (pin here) | Run DOTween Utility Panel setup after updates |
| ZLinq (Cysharp) | OpenUPM | (pin here) | Avoid mixing with `System.Linq` in hot paths |

## Notes by Risk Area

### ScriptableObject (SO)
**Recommended use (LOW–MED risk):**
- Static configuration/data assets: tuning values, tables, prefab references, design constants.
- “Definition” data (immutable at runtime): Soul definitions, Curse definitions, Audio ID lists.

**Avoid / be careful (MED–HIGH risk):**
- Do not store **runtime mutable state** in ScriptableObjects (especially if you want deterministic runs + safe reset).
- Editing SO fields at runtime can persist in the Editor and cause state bleed across play sessions.

**Project guidance:**
- Runtime state should live in **DI-owned Repositories/Services** and be resettable/deterministic.
- Treat ScriptableObjects as **read-only inputs** to services, not the state store.

### MessagePipe
**What it is:**
- High-performance in-memory pub/sub and mediator-style messaging designed for DI usage in Unity/.NET.

**Project guidance:**
- Use MessagePipe for **one-way event dispatching** (phase lifecycle events, SFX triggers, warnings…).
- Prefer **struct events** and keep payloads small to reduce allocations.
- Subscriptions must be **disposed** (tie to lifetime scope / CompositeDisposable pattern).

**Integration note:**
- Treat MessagePipe as the “event bus,” **not** a second state store (authoritative state lives in repositories/services).

### ObservableCollections (Cysharp)
**What it is:**
- High-performance observable collections (`ObservableList<T>`, `ObservableDictionary<TKey,TValue>`, etc.) and derived views/synchronization helpers.

**R3 integration:**
- `ObservableCollections.R3` enables R3-style observation of collection changes (a modern replacement path for old UniRx `ReactiveCollection` patterns).

**Project guidance:**
- Use it when you truly need **collection-level change streams** (add/remove/move/range ops) and/or efficient derived views.
- For simple single-value state (ward time, current phase, flags), keep using **R3 ReactiveProperty**.
- Treat observable collections as **runtime state** (DI-owned, resettable), not ScriptableObject state.

**Pitfalls:**
- Manage subscription/disposal the same way as R3 (CompositeDisposable / scope lifetimes).
- Prefer range/batched APIs (when available) to avoid per-item notification spam.

### DOTween (Demigiant)
**What it is:**
- A widely used tween engine (move/scale/fade/sequence…), good for UI/camera/VFX polish.

**Install / update note (important):**
- DOTween is typically imported as an asset and requires running the DOTween Utility Panel setup after updates to generate/enable modules.

**Project guidance:**
- Prefer DOTween in the **Presentation layer** (UI transitions, camera jolt, vignette hit, etc.).
- Avoid using DOTween as the authoritative gameplay state machine (keep determinism/state in services/controllers).
- If a tween impacts gameplay timing, treat the tween as presentation; keep logic/state explicit and replay-safe.

### ZLinq (Cysharp)
**What it is:**
- A “zero-allocation” LINQ replacement intended for heavy workloads (game loops).

**Project guidance:**
- Convention: **do not mix** ZLinq and `System.Linq` in the same hot path (easy to accidentally allocate).
- Prefer ZLinq for: filtering/sorting in update loops, temporary queries, large collections.
- For deterministic/reproducible logic: make ordering explicit where results must be stable.

## Usage

Before making architectural decisions that touch HIGH RISK areas:
1. Check `breaking-changes.md` for migration issues
2. Check `deprecated-apis.md` for APIs to avoid
3. Check `current-best-practices.md` for recommended patterns
4. Use WebSearch to verify uncertain APIs/behaviors against official docs, release notes, and package docs

## Verification Checklist (HIGH RISK)

Re-verify (WebSearch / official docs) when:
- Unity Editor minor/patch changes (6000.3.x → 6000.3.y)
- Any pinned package changes version/source
- Build pipeline changes (SRP, UI Toolkit runtime, Entities)
- New warnings/obsoletes appear after upgrading

Minimum verification sources:
- Unity Manual / Scripting API for Unity 6
- Package release notes (Cysharp/Demigiant/OpenUPM)
- Project docs: `breaking-changes.md`, `deprecated-apis.md`, `current-best-practices.md`

## Last Verified

2026-05-07 — via /setup-enginea

---

## Engine Version Reference

@docs/engine-reference/unity/VERSION.md