# Physical Crowding & Push

> **Status**: Approved
> **Author**: User + opencode
> **Last Updated**: 2026-05-07
> **Implements Pillar**: Consequence-driven survival loop with day/night emotional contrast

## Overview

Physical Crowding & Push controls the spatial dynamics between Tú and the 3 NPCs (souls) during Day Phase. As the shadow shrinks, space becomes insufficient for all 4 characters, creating physical挤压 (crowding) that forces the player to make difficult positional decisions. The Push mechanic at phase end forces one soul out of the safe zone, writing the `sacrificed_ghost_id`. Without this system, day phase has no spatial tension—players could save everyone without sacrifice.

## Player Fantasy

The player should feel **claustrophobic pressure**—the shadow walls closing in, souls pressed against each other, no room to breathe. The Push mechanic should feel **guilt-inducing**—forcibly shoving someone out, screen shake, audio impact, the soul burning with a scream. This is the moment choices become real.

## Detailed Design

### Core Rules

1. **RigidBody Collision**: Tú and all 3 souls have RigidBody2D with hard collision hitboxes. Characters cannot pass through each other.
2. **Swap Mechanic**: Player cannot jump over souls. To move a soul from edge to safety, approach and press Swap (auto-trigger on proximity) → 0.5s animation swaps positions.
3. **Shove Mechanic**: At Day Phase end (4:30-5:00), player MUST push one soul out. Screen shake, audio impact, soul burns with scream. This writes `sacrificed_ghost_id`.
4. **Push Trigger**: Auto-triggers when shadow polygon shrinks below capacity threshold OR when timer expires with unsaved souls at edge.
5. **Abandoned State**: Pushed soul transitions to Abandoned → Consequence Resolver receives `abandoned_soul_id`.

### States and Transitions

| State | Entry Condition | Exit Condition | Behavior |
|-------|-----------------|----------------|----------|
| Comfortable | Shadow width > character width × 4 | Shadow width ≤ character width × 4 | Normal movement, no crowding |
| Crowded | Shadow width ≤ character width × 4 | Shadow width ≤ character width × 3 | Movement restricted, souls bump |
| Crisis | Shadow width ≤ character width × 3 | Phase end or push triggered | Must swap/shove to survive |
| PushAction | Push triggered | Animation complete | Force one soul out, write sacrificed_id |
| Burned | Soul abandoned | Night start | Soul removed from play, marked for curse |

### Interactions with Other Systems

- **NPC/Soul Data Model -> Physical Crowding**: Reads soul positions and states. Writes `sacrificed_ghost_id` on push.
- **Shadow Spatial Management <- Physical Crowding**: Receives crowding pressure events. Reads soul positions for shadow shrink behavior.
- **Game State / Phase State Machine -> Physical Crowding**: Phase-gated (DayService only). Receives push trigger when timer expires.
- **Day Service & Selection <- Physical Crowding**: Soul positions affect which can be selected for saving.
- **Consequence Resolver <- Physical Crowding**: Sends `abandoned_soul_id` after push completes.

## Formulas

### Crowding Threshold
```
is_crowded = shadow_width <= (character_width * crowd_multiplier)
```

| Variable | Type | Range | Source | Description |
|----------|------|-------|--------|-------------|
| shadow_width | float | 0-100 | Shadow Manager | Current safe zone width |
| character_width | float | 1.0 | config | Width of one character hitbox |
| crowd_multiplier | float | 3.0-4.0 | config | Threshold for crowding state |

---

### Swap Animation Duration
```
swap_duration_sec = 0.5
```

---

### Push Force (for screen shake)
```
push_shake_intensity = 0.8
push_shake_duration = 0.3
```

## Edge Cases

| Scenario | Expected Behavior | Rationale |
|----------|-------------------|-----------|
| Player tries to walk through soul | Blocked by collision; must swap | No jump-over allowed |
| Two souls at edge, only one pushed | Pushed = soul closest to edge | Deterministic selection |
| Push during swap animation | Swap completes, then push triggers | Animation priority |
| All souls already safe (impossible per design) | Last soul at edge auto-pushed | Must have sacrifice |
| Soul tries to move into shadow during push | Blocked; push completes | No rescue during push |

## Dependencies

| System | Direction | Nature of Dependency |
|--------|-----------|---------------------|
| NPC/Soul Data Model | This depends on it | **Hard**: reads soul positions, writes sacrificed_id |
| Shadow Spatial Management | This depends on it | **Hard**: receives crowding events, reads soul positions |
| Game State / Phase State Machine | This depends on it | **Hard**: phase-gated, push trigger signal |
| Day Service & Selection | Depends on this | **Soft**: soul positions affect selection UI |
| Consequence Resolver | Depends on this | **Hard**: receives abandoned_soul_id for curse mapping |

## Tuning Knobs

| Parameter | Current Value | Safe Range | Effect of Increase | Effect of Decrease |
|-----------|---------------|------------|-------------------|-------------------|
| `crowd_multiplier` | 3.5 | 3.0-4.5 | Earlier crowding detection | More comfortable spacing |
| `swap_duration_sec` | 0.5 | 0.3-1.0 | Slower swap feel | Snappier swap |
| `push_shake_intensity` | 0.8 | 0.5-1.2 | More violent push | Subtle push |
| `crisis_threshold_multiplier` | 3.0 | 2.5-4.0 | Earlier crisis state | Longer comfortable phase |

## Visual/Audio Requirements

| Event | Visual Feedback | Audio Feedback | Priority |
|-------|-----------------|----------------|----------|
| Crowding Begins | Souls start bumping, tighter formation | Cloth rustle, grunting | High |
| Swap Animation | Smooth position exchange, brief glow | Whoosh, fabric movement | High |
| Push Trigger | Screen shake, camera jolt | Deep impact thud | Critical |
| Soul Burns | Soul bursts into flame, flies out of screen | Screaming, crackling fire | Critical |
| Soul Abandoned | Red ember particle trail | Whispering, distant crying | High |

## UI Requirements

| Information | Display Location | Update Frequency | Condition |
|-------------|------------------|------------------|-----------|
| Soul position indicators | Near each soul | Real-time | Day phase |
| Crowding warning | Edge of screen (red pulse) | When threshold hit | Day phase |
| Push prompt | Center screen | When forced push triggers | Day phase end |

## Acceptance Criteria

- [ ] Souls have RigidBody2D with collision hitboxes
- [ ] Player cannot walk through souls (collision blocks)
- [ ] Swap mechanic moves soul to safety with 0.5s animation
- [ ] Push mechanic forces one soul out at phase end
- [ ] `sacrificed_ghost_id` written correctly
- [ ] Crowding states trigger at correct thresholds
- [ ] Screen shake on push is noticeable but not nausea-inducing

## Open Questions

| Question | Owner | Deadline | Resolution |
|----------|-------|----------|------------|
| Should souls have their own AI to move? | Game Designer | Before implementation | Open - currently player-driven |
| How to visualize safe zone boundary? | UX Designer | Before implementation | Open