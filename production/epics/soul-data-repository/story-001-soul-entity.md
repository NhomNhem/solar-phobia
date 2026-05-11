# Story 001: Soul Entity — Canonical Data Model (Linh, Van, Minh)

> **Epic**: soul-data-repository
> **Status**: Complete
> **Layer**: Foundation
> **Type**: Logic
> **Priority**: P0

## Context

**ADR**: ADR-0002: Soul Data Repository Pattern — Status: Accepted

## Acceptance Criteria

- [x] 3 canonical souls initialized: linh (Em Linh), van (Ông Văn), minh (Anh Minh).
- [x] SoulId is immutable lookup key.
- [x] DaySelectionState: Unselected / Saved / Abandoned.
- [x] NightOutcomeState: None / Drag / Block / FakeShrine.
- [x] LifeState: Alive / Lost.

## Test Evidence

**Status**: [x] Complete — covered by SoulRepositoryTests.cs (16 tests)
