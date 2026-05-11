# Epic: Audio State Director

> **Layer**: Presentation
> **GDD**: design/gdd/audio-core-loop.md
> **Architecture Module**: AudioMixDirector (from architecture.md)
> **Status**: Ready
> **Stories**: 3 stories created — see below

## Overview

Audio State Director controls the sonic identity of Solar Phobia's core loop. It manages day/night audio snapshots, phase-gated music transitions, and threat escalation cues. The audio should feel like "watercolour on paper" — organic, textured, intimate, sound ON the page not OF the world.

**Theme**: "Opposites in sound" — Day: warm, measured, contemplative (gentle waves on a quiet shore). Night: urgent, desperate, primal (being hunted by something ancient).

## Governing ADRs

| ADR | Decision Summary | Engine Risk |
|-----|-----------------|-------------|
| ADR-0011: Audio State Director with BroAudio | BroAudio-based AudioMixDirector with R3 phase subscriptions, ambient layer blending, MusicPlayer crossfade | MEDIUM — BroAudio is in-project but post-cutoff |

## GDD Requirements

| TR-ID | Requirement | ADR Coverage |
|-------|-------------|--------------|
| TR-audio-001 | Day/night audio mix snapshots | ADR-0011 ✅ |
| TR-audio-002 | Phase-gated music transitions | ADR-0011 ✅ |
| TR-audio-003 | Ambient layer blending (ocean, wind, vocals, threat) | ADR-0011 ✅ |
| TR-audio-004 | Event-driven SFX (footsteps, cover, hazards, strike) | ADR-0011 ✅ |

## Definition of Done

This epic is complete when:
- All stories are implemented, reviewed, and closed via `/story-done`
- All acceptance criteria from `design/gdd/audio-core-loop.md` are verified
- All Visual/Feel stories have evidence docs with sign-off in `production/qa/evidence/`

## Stories

| Story | Type | Status | Acceptance Criteria |
|-------|------|--------|-------------------|
| Story 001: Phase-Gated Audio Snapshots | Integration | Ready | Day/Night BGM crossfade via R3, ambient layer blend, ChoiceLock freeze, Resolve stingers |
| Story 002: Ambient Layer Blending | Integration | Ready | 4 layers (ocean/wind/vocals/threat) with independent volume control |
| Story 003: Event-Driven SFX | Visual/Feel | Ready | Footsteps, cover, hazards, strike warning, relic pickup, death |

## Next Step

Run `/dev-story production/epics/audio-state-director/story-001-phase-gated-audio-snapshots.md`
to begin implementation.