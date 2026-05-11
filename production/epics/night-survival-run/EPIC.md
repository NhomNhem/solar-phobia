# Epic: Night Survival Run

> **Layer**: Feature
> **GDD**: design/gdd/night-survival-run.md
> **Architecture Module**: NightSurvivalRun (from architecture.md)
> **Status**: Ready
> **Stories**: Not yet created — run `/create-stories night-survival-run`

## Overview

Night Survival Run is the action phase of Solar Phobia's core loop—the frantic, timed run from StartShrine to EndShrine while the player's Ward Timer counts down. During this phase, the player navigates a dark, hazardous map using movement skills (sprint, dash, swing, glide) and cover mechanics while being pursued by the Cá Ông (whale) boss searchlight and haunted by karma hazards that spawn based on which soul was abandoned during Day Phase.

**Theme**: "Desperate hope" — Running at full speed toward the shrine with the timer counting down, knowing each second brings them closer to salvation but also closer to the whale's jaws.

## Governing ADRs

| ADR | Decision Summary | Engine Risk |
|-----|-----------------|-------------|
| ADR-0008: Night Survival Run Architecture | **Not yet created** | — |

## GDD Requirements

| TR-ID | Requirement | ADR Coverage |
|-------|-------------|--------------|
| TR-night-001 | Night loop with hazards, chase, objective timing | ❌ No ADR — **BLOCKED** |

> ⚠️ **Warning**: Stories for this epic will be marked Blocked until ADR-0008 is created. Run `/architecture_decision` to create the ADR, then proceed.

## Definition of Done

This epic is complete when:
- All stories are implemented, reviewed, and closed via `/story-done`
- All acceptance criteria from `design/gdd/night-survival-run.md` are verified
- All Logic and Integration stories have passing test files in `tests/`
- All Visual/Feel and UI stories have evidence docs with sign-off in `production/qa/evidence/`

## Next Step

1. Run `/architecture_decision` to create ADR-0008 (Night Survival Run Architecture)
2. Then run `/create-stories night-survival-run` to break this epic into implementable stories