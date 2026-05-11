# Epic: Map & Spawn Director

> **Layer**: Foundation
> **GDD**: design/gdd/map-and-spawn-director.md
> **Architecture Module**: MapSpawnDirector (from architecture.md)
> **Status**: In Progress
> **Stories**: 7 stories created — see below

## Overview

Map & Spawn Director controls night-phase spatial pressure in the Act 1 vertical slice. It owns route layout, fog-limited visibility, grave mound placement, shrine endpoints, and boss searchlight strike targeting windows. The player experiences this as a desperate traversal from Am Tho Bai Thuyen to the next safe shrine through a beach lane where cover, cursed pickups, and scanning threat must be read in real time.

**Theme**: "Hunted and morally cornered" — The map is not an arena to dominate; it is a haunted corridor where every detour for Bone Relics (`Ngoc Cot`) trades safety for guilt and time loss.

## Governing ADRs

| ADR | Decision Summary | Engine Risk |
|-----|-----------------|-------------|
| ADR-0006: Map Generation Strategy | **Not yet created** | — |

## GDD Requirements

| TR-ID | Requirement | ADR Coverage |
|-------|-------------|--------------|
| TR-map-001 | Procedural generation with deterministic seed, chunk spawning | ❌ No ADR — **BLOCKED** |

> ⚠️ **Warning**: Stories for this epic will be marked Blocked until ADR-0006 is created. Run `/architecture_decision` to create the ADR, then proceed.

## Definition of Done

This epic is complete when:
- All stories are implemented, reviewed, and closed via `/story-done`
- All acceptance criteria from `design/gdd/map-and-spawn-director.md` are verified
- All Logic and Integration stories have passing test files in `tests/`
- All Visual/Feel and UI stories have evidence docs with sign-off in `production/qa/evidence/`

## Next Step

1. Run `/architecture_decision` to create ADR-0006 (Map Generation Strategy)
2. Then run `/create-stories map-spawn-director` to break this epic into implementable stories
## Stories

| Story | Title | Type | Priority | Status |
|-------|-------|------|----------|--------|
| story-001 | Deterministic Seed + Chunk Generation | Logic | P0 | Ready |
| story-002 | Sweep Exposure Check | Logic | P0 | Ready |
| story-003 | Strike Telegraph + Penalty | Logic | P0 | Ready |
| story-004 | Route Viability Check | Logic | P1 | Ready |
| story-005 | Cover Density Validation | Logic | P1 | Ready |
| story-006 | IMapSpawnDirector Interface (Player Controller signals) | Logic | P0 | Ready |
| story-007 | Bone Relic Time Drain Event | Integration | P1 | Ready |

**Implementation order**: 001 → 002 → 003 → 006 → 004 → 005 → 007

## Stories

| Story | Title | Type | Priority | Status |
|-------|-------|------|----------|--------|
| story-001 | Deterministic Seed + Chunk Generation | Logic | P0 | Ready |
| story-002 | Sweep Exposure Check | Logic | P0 | Ready |
| story-003 | Strike Telegraph + Penalty | Logic | P0 | Ready |
| story-004 | Route Viability Check | Logic | P1 | Ready |
| story-005 | Cover Density Validation | Logic | P1 | Ready |
| story-006 | IMapSpawnDirector Interface | Logic | P0 | Ready |
| story-007 | Bone Relic Time Drain Event | Integration | P1 | Ready |

**Implementation order**: 001 → 002 → 003 → 006 → 004 → 005 → 007
