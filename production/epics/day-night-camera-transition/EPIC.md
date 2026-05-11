# Epic: Day/Night Camera Transition

> **Layer**: Foundation
> **GDD**: design/gdd/day-night-camera-transition.md
> **Architecture Module**: CameraDirector (from architecture.md)
> **Status**: Ready
> **Stories**: Not yet created — run `/create-stories day-night-camera-transition`

## Overview

Day/Night Camera Transition controls the visual perspective shift between the day and night phases. The day phase uses a fixed 2.5D top-down macro view (close-up on the market stall), while the night phase uses a side-scrolling camera that follows the player through the horizontal lane. The transition itself is abrupt and jarring—a deliberate design choice that reinforces the emotional contrast between calm deliberation and panic survival.

**Theme**: "Visual disorientation and emotional whiplash" — The close day camera creates a false sense of safety; the pulled-back night camera shatters that illusion. What was contained is now exposed.

## Governing ADRs

| ADR | Decision Summary | Engine Risk |
|-----|-----------------|-------------|
| ADR: Day/Night Camera Transition | **Not yet created** | — |

## GDD Requirements

| TR-ID | Requirement | ADR Coverage |
|-------|-------------|--------------|
| TR-camera-001 | Camera transition with FOV changes between day/night phases | ❌ No ADR — **BLOCKED** |

> ⚠️ **Warning**: Stories for this epic will be marked Blocked until the ADR is created. Run `/architecture_decision` to create the ADR, then proceed.

## Definition of Done

This epic is complete when:
- All stories are implemented, reviewed, and closed via `/story-done`
- All acceptance criteria from `design/gdd/day-night-camera-transition.md` are verified
- All Logic and Integration stories have passing test files in `tests/`
- All Visual/Feel and UI stories have evidence docs with sign-off in `production/qa/evidence/`

## Next Step

1. Run `/architecture_decision` to create ADR for Day/Night Camera Transition
2. Then run `/create-stories day-night-camera-transition` to break this epic into implementable stories