# Story 005: Cover Density Validation — Spawn Pass Guard

> **Epic**: map-spawn-director
> **Status**: Complete
> **Layer**: Foundation
> **Type**: Logic
> **Priority**: P1
> **Estimate**: 3 hours

## Context

**GDD**: `design/gdd/map-and-spawn-director.md` — Cover Density Validation formula
**TR-ID**: `TR-map-006`
**ADR**: ADR-0006

**Formula**: `cover_density = mo_thuong_count / lane_length` (target: 0.02-0.08 per unit)

## Acceptance Criteria

- [x] CoverDensityValidator.IsValid(int moThuongCount, float laneLength) returns true in range.
- [x] Below minimum → IsValid returns false (caller regenerates).
- [x] MinCountForLane(laneLength) returns minimum required count.
- [x] DefaultMoThuongCount = 14.

## Test Evidence

**Story Type**: Logic
**Required**: `Assets/_Project/Application/Editor/Tests/CoverDensityTests.cs`

**Status**: [x] Complete — 11/11 tests passing

- `Assets/_Project/Application/Editor/Tests/CoverDensityTests.cs` (11 tests)

