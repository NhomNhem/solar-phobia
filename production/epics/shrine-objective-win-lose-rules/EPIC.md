# Epic: Shrine Objective & Win/Lose Rules

> **Layer**: Core
> **GDD**: design/gdd/shrine-objective-win-lose-rules.md
> **Architecture Module**: ShrineObjective (from architecture.md)
> **Status**: Ready
> **Stories**: Not yet created — run `/create-stories shrine-objective-win-lose-rules`

## Overview

Shrine Objective & Win/Lose Rules defines the conditions that end the night survival phase—specifically, what constitutes a "win" (reaching the EndShrine) and a "lose" (Ward Timer reaching zero). The system owns the win/lose detection logic, the shrine arrival event flow, and the failure/death sequence.

**Theme**: "Desperate hope" — Each step closer to the shrine is a step away from the whale's jaws. Reaching the shrine should bring overwhelming relief and salvation—the nightmare ends, the whale won't get you, your choices mattered.

## Governing ADRs

| ADR | Decision Summary | Engine Risk |
|-----|-----------------|-------------|
| (None) | No ADR exists for this system | — |

## GDD Requirements

| TR-ID | Requirement | ADR Coverage |
|-------|-------------|--------------|
| (None tracked) | Win condition (reach EndShrine), lose condition (Ward = 0), shrine arrival event, death sequence | — |

## Definition of Done

This epic is complete when:
- All stories are implemented, reviewed, and closed via `/story-done`
- All acceptance criteria from `design/gdd/shrine-objective-win-lose-rules.md` are verified
- All Logic and Integration stories have passing test files in `tests/`
- All Visual/Feel and UI stories have evidence docs with sign-off in `production/qa/evidence/`

## Next Step

Run `/create-stories shrine-objective-win-lose-rules` to break this epic into implementable stories.