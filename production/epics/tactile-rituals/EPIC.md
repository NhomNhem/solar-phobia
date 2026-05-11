# Epic: Tactile Rituals

> **Layer**: Core
> **GDD**: design/gdd/tactile-rituals.md
> **Architecture Module**: TactileRituals (from architecture.md)
> **Status**: Ready
> **Stories**: Not yet created — run `/create-stories tactile-rituals`

## Overview

Tactile Rituals are the three minigames (diêm, rót, vay) that the player performs during Day Phase to earn Hương Hỏa (Spirit Essence). Each ritual is a physical interaction that feels grounded in Vietnamese cultural tradition—striking matches, pouring tea, fanning coals. Success earns resources; failure incurs penalties.

**Theme**: "Deliberate craftsmanship" — Each action requires attention and physical timing, not just clicking. The rituals should feel intimate and ritualistic—like actually performing the traditions. The minigames create the "tactile discomfort" of day phase before the physical panic of night.

## Governing ADRs

| ADR | Decision Summary | Engine Risk |
|-----|-----------------|-------------|
| (None) | No ADR exists for this system | — |

## GDD Requirements

| TR-ID | Requirement | ADR Coverage |
|-------|-------------|--------------|
| (None tracked) | Three minigames: diêm (match), rót (pour), vay (fan) with timing mechanics | — |

## Definition of Done

This epic is complete when:
- All stories are implemented, reviewed, and closed via `/story-done`
- All acceptance criteria from `design/gdd/tactile-rituals.md` are verified
- All Logic and Integration stories have passing test files in `tests/`
- All Visual/Feel and UI stories have evidence docs with sign-off in `production/qa/evidence/`

## Next Step

Run `/create-stories tactile-rituals` to break this epic into implementable stories.