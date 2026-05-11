# Epic: Phase State Machine

> **Layer**: Foundation
> **GDD**: design/gdd/game-state-phase-state-machine.md
> **Architecture Module**: PhaseStateMachine (from architecture.md)
> **Status**: Complete
> **Stories**: 9 stories created

## Overview

The Phase State Machine is the authoritative runtime controller for Solar Phobia's core loop. It transitions the run through Day Service, Choice Lock, Night Survival, and Resolve/Reset phases, enforcing which systems are active in each phase so rules cannot overlap incorrectly.

**Theme**: "Opposites Attract" — The static, tactile deliberation of Day (hy vọng giả tạo) versus the frantic, physical panic of Night (tốc độ cuồng nộ). The game tests "Hopeless Hope" — the unbearable tension between wanting to save everyone and the mathematical impossibility of doing so.

## Governing ADRs

| ADR | Decision Summary | Engine Risk |
|-----|-----------------|-------------|
| ADR-0001: Phase State Machine Architecture | R3 ReactiveProperty-based state machine with 7 states (Bootstrapping, DayService, ChoiceLock, NightSurvival, Resolve, Reset, FatalError) and phase contracts for action gating | HIGH |

## GDD Requirements

| TR-ID | Requirement | ADR Coverage |
|-------|-------------|--------------|
| TR-state-001 | Phase state machine with day/night cycle transitions | ADR-0001 ✅ |
| TR-state-002 | Day/Night phase transitions with phase-gated system activation | ADR-0001 ✅ |

## Definition of Done

This epic is complete when:
- All stories are implemented, reviewed, and closed via `/story-done`
- All acceptance criteria from `design/gdd/game-state-phase-state-machine.md` are verified
- All Logic and Integration stories have passing test files in `tests/`
- All Visual/Feel and UI stories have evidence docs with sign-off in `production/qa/evidence/`

## Stories

| # | Title | Layer | Type | Status |
|---|-------|-------|------|--------|
| 001 | Day Phase Timeline — 4 Pressure Phases | Foundation | Logic | ✅ Complete |
| 002 | Day Phase Mechanics (Light/Shadows) | Core | Logic | ✅ Complete |
| 003 | Night Phase Movement (Speed Reduction) | Core | Logic | ✅ Complete |
| 004 | Cover Detection (Mound Colliders) | Feature | Logic | ✅ Complete |
| 005 | Boss Searchlight (Detection Sweep) | Feature | Integration | ✅ Complete |
| 006 | Karma Hazards (Curse Spawning) | Feature | Logic + Integration | ✅ Complete |
| 007 | Ngọc Cốt Relic Pickups | Feature | Logic | ✅ Complete |
| 008 | Ward Timer Initialization | Feature | Logic | ✅ Complete |
| 009 | Sensory Tiles (Detection Thresholds) | Presentation | Logic + UI | ✅ Complete |

## Next Step

Start implementing Story 001 (foundational — Day Phase Timeline).