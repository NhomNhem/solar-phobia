# Epic: Physical Crowding & Push

> **Layer**: Core
> **GDD**: design/gdd/physical-crowding-and-push.md
> **Architecture Module**: (Core layer — no dedicated module)
> **Status**: Ready
> **Stories**: Not yet created — run `/create-stories physical-crowding-and-push`

## Overview

Physical Crowding & Push controls the spatial dynamics between Tú and the 3 NPCs (souls) during Day Phase. As the shadow shrinks, space becomes insufficient for all 4 characters, creating physical挤压 (crowding) that forces the player to make difficult positional decisions. The Push mechanic at phase end forces one soul out of the safe zone, writing the `sacrificed_ghost_id`.

**Theme**: "Claustrophobic pressure" — The shadow walls closing in, souls pressed against each other, no room to breathe. The Push mechanic should feel guilt-inducing—forcibly shoving someone out, screen shake, audio impact, the soul burning with a scream.

## Governing ADRs

| ADR | Decision Summary | Engine Risk |
|-----|-----------------|-------------|
| (None) | No ADR exists for this system | — |

## GDD Requirements

| TR-ID | Requirement | ADR Coverage |
|-------|-------------|--------------|
| (None tracked) | RigidBody collision, crowding physics, push mechanic | — |

## Definition of Done

This epic is complete when:
- All stories are implemented, reviewed, and closed via `/story-done`
- All acceptance criteria from `design/gdd/physical-crowding-and-push.md` are verified
- All Logic and Integration stories have passing test files in `tests/`
- All Visual/Feel and UI stories have evidence docs with sign-off in `production/qa/evidence/`

## Next Step

Run `/create-stories physical-crowding-and-push` to break this epic into implementable stories.