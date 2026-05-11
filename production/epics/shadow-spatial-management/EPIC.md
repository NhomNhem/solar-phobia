# Epic: Shadow Spatial Management

> **Layer**: Foundation
> **GDD**: design/gdd/shadow-spatial-management.md
> **Architecture Module**: ShadowManager (from architecture.md)
> **Status**: Ready
> **Stories**: Not yet created — run `/create-stories shadow-spatial-management`

## Overview

Shadow Spatial Management controls the shrinking shadow polygon and thermal death zones during Solar Phobia's day phase. As the Mặt Trời Rỗng (Hollow Sun) rises, the safe zone (bóng râm xanh lam) shrinks via polygon contraction, forcing 3 souls and the player into tighter space.

**Theme**: "Trapped and morally burdened" — The sun is an active threat judging their choices. As the safe zone contracts, the player realizes they cannot save everyone. The moment of pushing a soul into sunlight should feel genuinely uncomfortable, like condemning someone to a terrible death.

## Governing ADRs

| ADR | Decision Summary | Engine Risk |
|-----|-----------------|-------------|
| ADR: Shadow Spatial Management | **Not yet created** | — |

## GDD Requirements

| TR-ID | Requirement | ADR Coverage |
|-------|-------------|--------------|
| TR-shadow-001 | Shadow polygon with shrink mechanics and thermal death zones | ❌ No ADR — **BLOCKED** |

> ⚠️ **Warning**: Stories for this epic will be marked Blocked until the ADR is created. Run `/architecture_decision` to create the ADR, then proceed.

## Definition of Done

This epic is complete when:
- All stories are implemented, reviewed, and closed via `/story-done`
- All acceptance criteria from `design/gdd/shadow-spatial-management.md` are verified
- All Logic and Integration stories have passing test files in `tests/`
- All Visual/Feel and UI stories have evidence docs with sign-off in `production/qa/evidence/`

## Next Step

1. Run `/architecture_decision` to create ADR for Shadow Spatial Management
2. Then run `/create-stories shadow-spatial-management` to break this epic into implementable stories