# Epic: Sensory Feedback System

> **Layer**: Core
> **GDD**: design/gdd/sensory-feedback-system.md
> **Architecture Module**: (Core layer — no dedicated module)
> **Status**: Ready
> **Stories**: Not yet created — run `/create-stories sensory-feedback-system`

## Overview

Sensory Feedback System provides diegetic, non-HUD information to the player about their survival state. Instead of watching a progress bar, the player feels danger approaching through progressive visual and audio degradation. The system maps Ward Timer thresholds to sensory tiers that alter the player's perception without explicit numbers.

**Theme**: "Unconscious awareness" — Knowing they're in danger without seeing numbers. The degradation should create dread escalation—each tier feels worse than the last, building to "Hopeless Hope" where the player knows death is coming but fights anyway.

## Governing ADRs

| ADR | Decision Summary | Engine Risk |
|-----|-----------------|-------------|
| (None) | No ADR exists for this system | — |

## GDD Requirements

| TR-ID | Requirement | ADR Coverage |
|-------|-------------|--------------|
| (None tracked) | Ward timer thresholds → sensory tiers, visual degradation, audio degradation, diegetic feedback | — |

## Definition of Done

This epic is complete when:
- All stories are implemented, reviewed, and closed via `/story-done`
- All acceptance criteria from `design/gdd/sensory-feedback-system.md` are verified
- All Logic and Integration stories have passing test files in `tests/`
- All Visual/Feel and UI stories have evidence docs with sign-off in `production/qa/evidence/`

## Next Step

Run `/create-stories sensory-feedback-system` to break this epic into implementable stories.