# Epic: HUD-less Design & Sensory Feedback

> **Layer**: Presentation
> **GDD**: (referenced in systems-index.md — no dedicated GDD)
> **Architecture Module**: SensoryFeedbackManager (from architecture.md)
> **Status**: Ready
> **Stories**: Not yet created — run `/create-stories hud-less-design`

## Overview

HUD-less Design & Sensory Feedback provides diegetic, non-HUD information to the player about their survival state. Instead of watching progress bars, the player feels danger through progressive visual and audio degradation. This layer coordinates the Ward Timer's sensory tiers with the Sensory Feedback System to create unconscious awareness of danger without explicit numbers.

**Theme**: "Unconscious awareness" — The player knows they're in danger without seeing numbers. Each sensory tier builds dread, creating "Hopeless Hope" where the player knows death is coming but fights anyway.

## Governing ADRs

| ADR | Decision Summary | Engine Risk |
|-----|-----------------|-------------|
| (None) | No ADR exists for this system | — |

## GDD Requirements

| TR-ID | Requirement | ADR Coverage |
|-------|-------------|--------------|
| (None tracked) | Visual degradation, audio degradation, diegetic feedback, HUD-free design | — |

## Definition of Done

This epic is complete when:
- All stories are implemented, reviewed, and closed via `/story-done`
- All acceptance criteria from `design/ux/` (if any UX specs exist) are verified
- All Visual/Feel and UI stories have evidence docs with sign-off in `production/qa/evidence/`

## Next Step

Run `/create-stories hud-less-design` to break this epic into implementable stories.