# Resource Effects & Hương Hỏa Epic

> **Status**: Not Started
> **Layer**: Economy
> **Priority**: Vertical Slice
> **Estimate**: 16 hours

## Overview
Manages the economic systems related to resource consumption during nighttime survival, including Time Drain effects from Bone Relic pickup and Hương Hỏa point accumulation from ritual completions.

## Systems in Epic
- Resource Effects & Hương Hỏa (main system)
- Time Drain calculation subsystem
- Hương Hỏa point tracking subsystem

## Stories
- story-001-resource-effects-core.md: Core resource effects infrastructure
- story-002-time-drain-calculator.md: Time Drain calculation implementation
- story-003-hương-hỏa-tracking.md: Hương Hỏa point accumulation system
- story-004-resource-effects-integration.md: Cross-system integration tests

## Dependencies
- Depends on: NPC/Soul Data Model, Game State / Phase State Machine, Day Service & Selection
- Unlocks: Night Survival Run

## Progress
- [ ] Story 001: Core resource effects infrastructure
- [ ] Story 002: Time Drain calculation implementation  
- [ ] Story 003: Hương Hỏa point accumulation system
- [ ] Story 004: Resource effects integration tests