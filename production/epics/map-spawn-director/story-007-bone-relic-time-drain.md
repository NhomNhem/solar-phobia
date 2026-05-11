# Story 007: Bone Relic Time Drain Event — Cross-System Integration

> **Epic**: map-spawn-director
> **Status**: Complete
> **Layer**: Foundation
> **Type**: Integration
> **Priority**: P1
> **Estimate**: 4 hours

## Context

**GDD**: `design/gdd/map-and-spawn-director.md` — Time Drain Under Bone Relic formula
**TR-ID**: `TR-map-008`
**ADR**: ADR-0006

**Formula**: `effective_drain = base_drain_rate * (1 + bones_carried * hallucination_multiplier)`

## Acceptance Criteria

- [x] TimeDrainCalculator.CalculateEffectiveDrain formula verified.
- [x] base_drain_rate configurable (default 1.0).
- [x] hallucination_multiplier configurable (default 0.5).
- [x] Effective drain clamped to MaxSafeDrainRate (20.0).
- [x] Formula: base * (1 + bones * multiplier) verified for 0-3 bones.

## Dependencies

- Depends on: Story 001 (seed), Story 006 (interface)
- Unblocks: player-controller story-008 (Relic Pickup Integration)

## Test Evidence

**Story Type**: Integration
**Required**: `Assets/_Project/Application/Editor/Tests/TimeDrainIntegrationTests.cs`

**Status**: [x] Complete — 12/12 tests passing

- `Assets/_Project/Application/Editor/Tests/TimeDrainTests.cs` (12 tests)

