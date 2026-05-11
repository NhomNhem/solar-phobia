# Epic: Boss Cá Ông Searchlight

> **Layer**: Feature
> **GDD**: design/gdd/boss-ca-ong-searchlight.md
> **Architecture Module**: BossSearchlight (from architecture.md)
> **Status**: Ready
> **Stories**: Not yet created — run `/create-stories boss-ca-ong-searchlight`

## Overview

Boss Cá Ông (Whale) Searchlight is the persistent threat during Night Phase—a massive whale skeleton in the background that sweeps a searchlight cone across the playable area. If the player is exposed (not in valid cover) during a sweep, they receive a strike warning, then a -30s Ward penalty. This creates constant tension and forces tactical movement.

**Theme**: "Relentless pursuit" — The whale is always there, always watching, always sweeping. The searchlight should feel oppressive—a green cone of death that forces hiding like prey. Successful covers should feel narrow survival.

## Governing ADRs

| ADR | Decision Summary | Engine Risk |
|-----|-----------------|-------------|
| (None) | No ADR exists for this system | — |

## GDD Requirements

| TR-ID | Requirement | ADR Coverage |
|-------|-------------|--------------|
| (None tracked) | Searchlight sweep behavior, cover detection, strike warning, Ward penalty | — |

## Definition of Done

This epic is complete when:
- All stories are implemented, reviewed, and closed via `/story-done`
- All acceptance criteria from `design/gdd/boss-ca-ong-searchlight.md` are verified
- All Logic and Integration stories have passing test files in `tests/`
- All Visual/Feel and UI stories have evidence docs with sign-off in `production/qa/evidence/`

## Next Step

Run `/create-stories boss-ca-ong-searchlight` to break this epic into implementable stories.