# Story 001: Deterministic Seed — Chunk Generation + Run Reproducibility

> **Epic**: map-spawn-director
> **Status**: Complete
> **Layer**: Foundation
> **Type**: Logic
> **Priority**: P0
> **Estimate**: 5 hours

## Context

**GDD**: `design/gdd/map-and-spawn-director.md`
**TR-ID**: `TR-map-001`
**ADR**: ADR-0006: Map Generation Strategy — Status: Accepted

**ADR Decision Summary**: Seed stored in run snapshot. Chunks spawn ahead of player, despawn behind. Same seed always produces identical layout.

## Acceptance Criteria

- [x] Initialize(seed) sets deterministic RNG state.
- [x] GenerateChunk(index) produces identical output for same seed + index.
- [x] Different seeds produce different layouts.
- [x] Seed is stored and retrievable for run snapshot/replay.
- [x] Chunk generation is pure math — no Unity scene state required.

## Implementation Notes

- `MapSpawnDirector.Initialize(int seed)` → `_rng = new System.Random(seed)`
- `GenerateChunk(int index)` → `new System.Random(_seed + index)`
- Returns `ChunkData` struct (mound positions, type, hazard flags)
- No MonoBehaviour dependency — fully testable in Editor

## Test Evidence

**Story Type**: Logic
**Required**: `Assets/_Project/Application/Editor/Tests/MapSeedTests.cs`

**Status**: [x] Complete — 16/16 tests passing

- `Assets/_Project/Application/Editor/Tests/MapSeedTests.cs` (16 tests, AC-1 through AC-5 + multi-chunk determinism)
