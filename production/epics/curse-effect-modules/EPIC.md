# Epic: Curse Effect Modules

> **Layer**: Feature
> **GDD**: design/gdd/curse-effect-modules.md
> **Architecture Module**: CurseEffectManager (from architecture.md)
> **Status**: Ready
> **Stories**: 5 stories created — see below

## Overview

Curse Effect Modules control the karma-based hazards that spawn during Night Phase based on which soul was abandoned during Day Phase. Each curse (Drag/Block/FakeShrine) manifests as specific environmental hazards with unique visual and audio signatures.

**Theme**: "Haunted specificity" — Not generic danger, but *this particular* horror because of *this particular* choice. The water trap feels like Linh's drowning. The blood net feels like Van's capture. The illusion platform feels like Minh's betrayal. The horror is personal and deserved.

## Governing ADRs

| ADR | Decision Summary | Engine Risk |
|-----|-----------------|-------------|
| (None) | No ADR exists for this system | — |

## GDD Requirements

| TR-ID | Requirement | ADR Coverage |
|-------|-------------|--------------|
| (None tracked) | Three curse types (Drag, Block, FakeShrine), hazard spawning, visual/audio signatures | — |

## Definition of Done

This epic is complete when:
- All stories are implemented, reviewed, and closed via `/story-done`
- All acceptance criteria from `design/gdd/curse-effect-modules.md` are verified
- All Logic and Integration stories have passing test files in `tests/`
- All Visual/Feel and UI stories have evidence docs with sign-off in `production/qa/evidence/`

## Next Step

Run `/story-readiness` on Story 001 to begin implementation.

## Stories

| # | Story | Type | Status |
|---|-------|------|--------|
| 001 | Curse Effect Core — Hazard Spawning System | Logic | Ready |
| 002 | Water Trap Effect (Drag - Linh) | Logic | Ready |
| 003 | Blood Net Effect (Block - Van) | Logic | Ready |
| 004 | Illusion Platform Effect (FakeShrine - Minh) | Logic | Ready |
| 005 | Visual/Audio Signatures Per Curse | Visual/Feel | Ready |