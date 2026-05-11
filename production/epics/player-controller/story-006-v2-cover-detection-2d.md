# Story 006-v2: Cover Detection 2D — Mộ Gió Trigger Overlap

> **Epic**: player-controller
> **Status**: Complete
> **Layer**: Core
> **Type**: Logic
> **Priority**: P1
> **Replaces**: story-004-cover-detection.md (3D bounds — archived)

## Context

**GDD**: Master GDD V5.0 — Section 3.2
**ADR**: ADR-0003-v2

Player hides behind Mộ Gió (tombstones). Cover = 2D trigger overlap, not 3D bounds containment.

## Acceptance Criteria

- [x] Player overlapping Mộ Gió trigger → IsInCover = true.
- [x] Player exits trigger → IsInCover = false.
- [x] IsInCover published as ReactiveProperty<bool>.
- [x] Cover check only active during NightSurvival phase.
- [x] FalseSafeMound: IsInCover = true but fires separate warning event.

## Implementation Notes

- ICoverDetector2D.NotifyOverlapEnter(tag) / NotifyOverlapExit(tag)
- MonoBehaviour calls these from OnTriggerEnter2D / OnTriggerExit2D
- No bounds math needed — Unity handles overlap detection

## Test Evidence

**Story Type**: Logic
**Required**: unit tests in Editor/Tests/

**Status**: [x] Complete — 15/15 tests passing

- `Assets/_Project/Application/Editor/Tests/CoverDetection2DTests.cs` (15 tests)

