# Epic: Ward Timer / Nước Mắm Cốt

> **Layer**: Foundation
> **GDD**: design/gdd/health-stamina-damage-rules.md
> **Architecture Module**: WardTimer (from architecture.md)
> **Status**: Ready
> **Stories**: Not yet created — run `/create-stories ward-timer`

## Overview

Health/Stamina & Damage Rules defines the unified survival resource system that merges traditional "health" and "stamina" into a single mechanic called **Spirit Ward** (Nước Mắm Cốt). This system tracks the player's humanity and survival time during the night phase, acting as both the timer and the health bar.

**Theme**: "Time pressure and vulnerability" — Every second counts, and every choice has a time cost. The player never feels safe because the timer is always ticking. When Ward drops to critical levels (≤25%), the screen should feel terrifying: chromatic aberration, audio distortion, and the sense that death is imminent.

## Governing ADRs

| ADR | Decision Summary | Engine Risk |
|-----|-----------------|-------------|
| ADR-0005: Ward Timer Implementation | **Not yet created** | — |

## GDD Requirements

| TR-ID | Requirement | ADR Coverage |
|-------|-------------|--------------|
| TR-ward-001 | Ward initialization from saved souls count + day penalties | ❌ No ADR — **BLOCKED** |

> ⚠️ **Warning**: Stories for this epic will be marked Blocked until ADR-0005 is created. Run `/architecture_decision` to create the ADR, then proceed.

## Definition of Done

This epic is complete when:
- All stories are implemented, reviewed, and closed via `/story-done`
- All acceptance criteria from `design/gdd/health-stamina-damage-rules.md` are verified
- All Logic and Integration stories have passing test files in `tests/`
- All Visual/Feel and UI stories have evidence docs with sign-off in `production/qa/evidence/`

## Next Step

1. Run `/architecture_decision` to create ADR-0005 (Ward Timer Implementation)
2. Then run `/create-stories ward-timer` to break this epic into implementable stories