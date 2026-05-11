# ADR-0003-v2: Player Controller Pattern (2D)

## Status
Accepted

## Date
2026-05-09

## Supersedes
ADR-0003 (2026-05-07) — original 3D CharacterController design, now archived.

## Context

Master GDD V5.0 confirmed Solar Phobia is a **2D side-scrolling precision platformer**.

- **Day phase**: 1D X-axis movement (A/D keys, slow walk). Space=Swap, F=Shove.
- **Night phase**: 2D platformer (A/D sprint). Shift=Spirit Dash, Click=Swing, hold=Glide.

## Decision

Use **Rigidbody2D** for Night phase. Day phase uses Transform.position on X-axis only.

## Input Mapping (Master GDD V5.0)

| Action | Day | Night |
|--------|-----|-------|
| Move | A/D slow | A/D sprint |
| Interact | Left Click (Ritual) | Left Click (Swing -2s) |
| Special 1 | Space (Swap) | Shift (Spirit Dash -5s) |
| Special 2 | F (Shove) | S (Crouch) |

## Night Skills

| Skill | Ward Cost |
|-------|-----------|
| Spirit Dash | -5.0s |
| Swing | -2.0s |
| Glide | -1.0s/sec |

## Platformer Feel

- Coyote Time: 0.1s
- Jump Buffering: 0.15s
- Cover: 2D trigger overlap (Mộ Gió tombstones)

## Reused from ADR-0003 v1

- PlayerInputHandler + PlayerInputMode enum (phase gating unchanged)
- InteractHandler (tag dispatch platform-agnostic)
- CursorController (phase to cursor mapping unchanged)
- 42 existing tests still valid

## Archived from ADR-0003 v1

- MovementCalculator (3D) → replaced by Movement2DCalculator
- SprintController → replaced by DashController
- CoverDetector (3D bounds) → replaced by CoverDetector2D
- SolarPhobiaInputActions bindings need update

## Stories Needed

- Story 002-v2: A/D Movement (2D Rigidbody)
- Story 003-v2: Spirit Dash (-5s Ward cost)
- Story 004-v2: Swing + Glide skills
- Story 005-v2: Coyote Time + Jump Buffering
- Story 006-v2: Cover Detection 2D (trigger overlap)
- Story 007-v2: Day Phase Swap + Shove
