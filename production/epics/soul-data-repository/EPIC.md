# Epic: Soul Data Repository

> **Layer**: Foundation
> **GDD**: design/gdd/npc-soul-data-model.md
> **Architecture Module**: SoulDataRepository (from architecture.md)
> **Status**: Complete
> **Stories**: Not yet created — run `/create-stories soul-data-repository`

## Overview

The Soul Data Repository is the canonical source of truth for every soul used in Solar Phobia's day/night loop. It defines identity, daily selection state, abandonment outcome, and persistent run-scoped metadata so Day Service, Consequence Resolver, and downstream survival systems consume consistent data contracts.

**Theme**: Clarity with guilt — "I know exactly who I left behind, and the night reflects that choice." When this model is working, players trust that outcomes are consistent and fair, which reinforces ownership rather than randomness.

## Governing ADRs

| ADR | Decision Summary | Engine Risk |
|-----|-----------------|-------------|
| ADR-0002: Soul Data Repository Pattern | Dictionary-based O(1) lookup for soul entities with R3 observables for state changes | MEDIUM |

## GDD Requirements

| TR-ID | Requirement | ADR Coverage |
|-------|-------------|--------------|
| TR-npc-001 | Soul entity storage with unique ID, selection state, and night outcome | ADR-0002 ✅ |

## Definition of Done

This epic is complete when:
- All stories are implemented, reviewed, and closed via `/story-done`
- All acceptance criteria from `design/gdd/npc-soul-data-model.md` are verified
- All Logic and Integration stories have passing test files in `tests/`
- All Visual/Feel and UI stories have evidence docs with sign-off in `production/qa/evidence/`

## Next Step

Run `/create-stories soul-data-repository` to break this epic into implementable stories.
## Stories

| Story | Title | Status |
|-------|-------|--------|
| story-001 | Soul Entity — Canonical Data Model | ✅ Complete |
| story-002 | Phase-Locked Writes | ✅ Complete |
| story-003 | Query API | ✅ Complete |

## Test Evidence

- Assets/_Project/Application/Repositories/SoulRepository.cs — full implementation
- Assets/_Project/Application/Editor/Tests/SoulRepositoryTests.cs — 16 tests, all passing
