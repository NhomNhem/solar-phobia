# Epic: Consequence Resolver

> **Layer**: Feature
> **GDD**: design/gdd/consequence-resolver.md
> **Architecture Module**: ConsequenceResolver (from architecture.md)
> **Status**: Ready
> **Stories**: Not yet created — run `/create-stories consequence-resolver`

## Overview

Consequence Resolver is the moral arithmetic of Solar Phobia's night phase—it reads the abandoned soul from Day Service's selection payload and translates that choice into a physical curse that hunts the player. The system owns the curse mapping logic, the NightOutcomeState write, and the deterministic assignment that ensures "the person you left behind becomes your nightmare."

**Theme**: "This is my fault" — The curse that hunts them at night is a direct manifestation of the person they abandoned. There's no randomness here; the water trap, blood net, or collapsing platform exists *because* you left that specific soul behind. The fantasy is ownership of horror.

## Governing ADRs

| ADR | Decision Summary | Engine Risk |
|-----|-----------------|-------------|
| ADR-0007: Consequence Resolver Pattern | Curse mapping from abandoned soul ID to NightOutcomeState (Drag, Block, FakeShrine) | MEDIUM |

## GDD Requirements

| TR-ID | Requirement | ADR Coverage |
|-------|-------------|--------------|
| TR-consequence-001 | Curse payload generation from sacrifice | ADR-0007 ✅ |

## Definition of Done

This epic is complete when:
- All stories are implemented, reviewed, and closed via `/story-done`
- All acceptance criteria from `design/gdd/consequence-resolver.md` are verified
- All Logic and Integration stories have passing test files in `tests/`
- All Visual/Feel and UI stories have evidence docs with sign-off in `production/qa/evidence/`

## Next Step

Run `/create-stories consequence-resolver` to break this epic into implementable stories.