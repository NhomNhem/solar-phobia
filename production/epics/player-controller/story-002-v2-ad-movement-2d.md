# Story 002-v2: A/D Movement — 2D Rigidbody Night Traversal + Day Walk

> **Epic**: player-controller
> **Status**: Complete
> **Layer**: Core
> **Type**: Logic
> **Priority**: P0
> **Replaces**: story-002-wasd-movement.md (3D WASD — archived)

## Context

**GDD**: Master GDD V5.0 — Phase 2 (Day) + Phase 3 (Night)
**ADR**: ADR-0003-v2: Player Controller Pattern (2D)

Day phase: A/D slow walk on X-axis only (Transform.position, no physics).
Night phase: A/D sprint with Rigidbody2D horizontal velocity.

## Acceptance Criteria

- [x] Night: A/D keys drive horizontal velocity via Rigidbody2D.
- [x] Night: effective_speed = base_move_speed (default 5.0).
- [x] Night: Movement blocked when PlayerInputMode != NightMovement.
- [x] Day: A/D keys move player slowly on X-axis only (no Y movement).
- [x] Day: base_day_speed is configurable (default 2.0, slower than night).
- [x] base_move_speed is a configurable tuning knob — not hardcoded.

## Implementation Notes

- Night: Rigidbody2D.velocity = new Vector2(inputX * speed, rb.velocity.y)
- Day: transform.position += Vector3(inputX * daySpeed * deltaTime, 0, 0)
- Gravity handled by Rigidbody2D in Night phase
- Day phase: Rigidbody2D.simulated = false (disable physics during Day)

## Test Evidence

**Story Type**: Logic
**Required**: unit tests in Editor/Tests/

**Status**: [x] Complete — 22/22 tests passing

- `Assets/_Project/Application/Editor/Tests/Movement2DTests.cs` (22 tests, AC-1 through AC-6 + disabled mode + deltaTime scaling)

