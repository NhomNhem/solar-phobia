# Story 003: Query API — GetSavedSouls, GetAbandonedSoul, IsSelectionValid

> **Epic**: soul-data-repository
> **Status**: Complete
> **Layer**: Foundation
> **Type**: Logic
> **Priority**: P0

## Context

**ADR**: ADR-0002: Soul Data Repository Pattern — Status: Accepted

## Acceptance Criteria

- [x] GetSavedSouls() returns correct list.
- [x] GetAbandonedSoul() returns correct list.
- [x] IsSelectionValid(2) returns true when 2 saved, 1 abandoned.
- [x] GetSoul(id) is O(1) Dictionary lookup.

## Test Evidence

**Status**: [x] Complete — covered by SoulRepositoryTests.cs (16 tests)
