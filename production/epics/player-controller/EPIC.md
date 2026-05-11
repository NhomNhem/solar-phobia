# Epic: Player Controller & Skills

> **Layer**: Core
> **GDD**: design/gdd/player-controller.md
> **Architecture Module**: PlayerController (from architecture.md)
> **Status**: In Progress (2D Refactor + FSM Gap Fix — ADR-0003-v2)
> **Stories**: 14 stories total — 2 new stories added to address FSM gap

## Overview

Player Controller is the input and movement system that translates player actions into game responses, enforcing what the player can and cannot do based on the current phase. During Day phase, the player is restricted to UI-only interactions—selecting NPCs, assigning resources, and confirming choices—with no physical movement allowed. During Night phase, the system enables WASD movement, sprint, and contextual interaction as the player navigates the lane from start shrine to end shrine.

**Theme**: "Ownership through contrast" — Daytime calm makes nighttime panic feel deserved, not random. When the player survives or fails, they should connect it to their earlier choices, not to unfair mechanics.

## Governing ADRs

| ADR | Decision Summary | Engine Risk |
|-----|-----------------|-------------|
| ADR-0003: Player Controller Pattern | New Input System + CharacterController with phase-gated enable/disable | HIGH |

## GDD Requirements

| TR-ID | Requirement | ADR Coverage |
|-------|-------------|--------------|
| TR-player-001 | WASD movement with sprint, dash, glide, swing actions (phase-gated) | ADR-0003 ✅ |
| TR-player-009 | PlayerStateMachine FSM with ReactiveProperty<EPlayerState> (Idle, Moving, Sprinting, etc.) | ADR-0003-v2 ✅ |
| TR-player-010 | Phase integration: subscribe to IPhaseStateMachine for Day/Night behavior changes | ADR-0003-v2 ✅ |

## Stories

| Story | Title | Type | Priority | Status |
|-------|-------|------|----------|--------|
| story-001 | Phase-Gated Input | Logic | P0 | ✅ Complete (reused) |
| story-002 | WASD Movement (3D) | Logic | P0 | 📦 Superseded |
| story-003 | Sprint (3D) | Logic | P1 | 📦 Superseded |
| story-004 | Cover Detection (3D) | Logic | P1 | 📦 Superseded |
| story-005 | E-Key Interact | Logic | P1 | ✅ Complete (reused) |
| story-006 | Cursor Visibility | UI | P1 | ✅ Complete (reused) |
| story-002-v2 | A/D Movement 2D | Logic | P0 | ✅ Complete |
| story-003-v2 | Spirit Dash (-5s Ward) | Logic | P1 | ✅ Complete |
| story-004-v2 | Swing + Glide Skills | Logic | P1 | ✅ Complete |
| story-005-v2 | Coyote Time + Jump Buffer | Logic | P1 | ✅ Complete |
| story-006-v2 | Cover Detection 2D (Mộ Gió) | Logic | P1 | ✅ Complete |
| story-007-v2 | Day Phase Swap + Shove | Logic | P1 | ✅ Complete |
| story-007 | Strike Warning Integration | Integration | P1 | ✅ Complete |
| story-008 | Relic Pickup Integration | Integration | P2 | ✅ Complete |
| **story-009** | **PlayerStateMachine Core (FSM)** | **Logic** | **P0** | **✅ Complete** |
| **story-010** | **PlayerStateMachine Phase Integration** | **Integration** | **P0** | **✅ Complete** |

**Implementation order**: 009 → 010 → 002-v2 → 003-v2 → 004-v2 → 005-v2 → 006-v2 → 007-v2 → 007 → 008

> **Critical Gap Fixed**: Stories 009-010 address the missing PlayerStateMachine FSM that was not covered in the original story list.

## Definition of Done

This epic is complete when:
- All stories are implemented, reviewed, and closed via `/story-done`
- All acceptance criteria from `design/gdd/player-controller.md` are verified
- All Logic and Integration stories have passing test files in `tests/`
- All Visual/Feel and UI stories have evidence docs with sign-off in `production/qa/evidence/`

## Next Step

Run `/dev-story production/epics/player-controller/story-001-phase-gated-input.md` to begin implementation.
