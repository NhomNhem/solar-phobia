# Epic: Day Service & Selection

> **Layer**: Core
> **GDD**: design/gdd/day-service-and-selection.md
> **Architecture Module**: DaySelectionUI (from architecture.md)
> **Status**: Ready
> **Stories**: Not yet created — run `/create-stories day-service-and-selection`

## Overview

Day Service & Selection is the moral core of Solar Phobia's day phase—the moment where the player must choose which 2 of 3 souls to save and which one to abandon to the sun. The system owns the selection UI, validation logic, and confirmation flow that locks the player's choice before night begins.

**Theme**: "Moral weight and visceral discomfort" — The day phase isn't about "completing tasks"—it's about choosing who lives and who burns. The moment of abandonment—physically shoving a soul out of the shade—should feel genuinely uncomfortable, like becoming the monster.

## Governing ADRs

| ADR | Decision Summary | Engine Risk |
|-----|-----------------|-------------|
| (None) | No ADR exists for this system | — |

## GDD Requirements

| TR-ID | Requirement | ADR Coverage |
|-------|-------------|--------------|
| TR-day-service-001 (unregistered) | Selection UI, soul cards, validation, confirmation flow, "2 saved, 1 abandoned" rule | — |

## Stories

| # | Story | Type | Status | ADR |
|---|-------|------|--------|-----|
| 001 | Selection Logic & Validation | Logic | Complete | None |
| 002 | Selection UI & Confirm Flow | Integration | Ready | None |
| 003 | Ritual Assignment | Integration | Ready | None |

## Definition of Done

This epic is complete when:
- All stories are implemented, reviewed, and closed via `/story-done`
- All acceptance criteria from `design/gdd/day-service-and-selection.md` are verified
- All Logic and Integration stories have passing test files in `tests/`
- All Visual/Feel and UI stories have evidence docs with sign-off in `production/qa/evidence/`

## Next Step

Run `/create-stories day-service-and-selection` to break this epic into implementable stories.