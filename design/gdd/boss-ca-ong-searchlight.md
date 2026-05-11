# Boss Cá Ông Searchlight

> **Status**: Approved
> **Author**: User + opencode
> **Last Updated**: 2026-05-07
> **Implements Pillar**: Consequence-driven survival loop with day/night emotional contrast

## Overview

Boss Cá Ông (Whale) Searchlight is the persistent threat during Night Phase—a massive whale skeleton in the background that sweeps a searchlight cone across the playable area. If the player is exposed (not in valid cover) during a sweep, they receive a strike warning, then a -30s Ward penalty. This creates constant tension and forces tactical movement. Without this system, night phase has no dynamic threat—the player could simply run without needing to seek cover.

## Player Fantasy

The player should feel **relentless pursuit**—the whale is always there, always watching, always sweeping. The searchlight should feel **oppressive**—a green cone of death that forces hiding like prey. Successful covers should feel **narrow survival**—the player barely escaped being seen.

## Detailed Design

### Core Rules

1. **Skeleton Background**: Cá Ông skeleton visible in background layer, not interactable, provides narrative presence.
2. **Searchlight Sweep**: Cone sweeps across the lane in predictable pattern. Player can anticipate timing.
3. **Cover Detection**: Player must be fully inside Mound collider to be "in cover" and avoid detection.
4. **Telegraph Phase**: 1.5s warning before strike (ground shimmer + audio cue).
5. **Strike Resolution**: If player exits cover during telegraph or is exposed at strike time, -30s Ward penalty (capped at 30% of initial Ward).
6. **Strike Debris**: Strike destroys the targeted Mound, creating fallen rock debris at impact location. Debris becomes a NEW cover volume that players can use for subsequent sweeps. The debris persists for the remainder of the night run.

### States and Transitions

| State | Entry Condition | Exit Condition | Behavior |
|-------|-----------------|----------------|----------|
| Idle | Not NightSurvival | NightSurvival starts | No sweep active |
| Sweeping | NightSurvival active | Player detected or sweep ends | Move cone across play area |
| Targeting | Player enters cone, not in cover | Telegraph ends or player enters cover | Lock onto player |
| Telegraphing | Targeting locked | Strike resolves or player enters cover | Warning VFX + audio |
| Striking | Telegraph complete, player exposed | Impact resolves | Apply penalty, create debris |
| Cooldown | Strike resolved | Next sweep cycle | Brief pause before next sweep |

### Interactions with Other Systems

- **Night Survival Run <- Boss Searchlight**: Operates during NightSurvival. Sends strike penalty events.
- **Map & Spawn Director <- Boss Searchlight**: Coordinates sweep pattern with map layout.
- **Player Controller -> Boss Searchlight**: Provides player position and cover state.
- **Ward Timer <- Boss Searchlight**: Strike reduces Ward.

## Formulas

### Sweep Pattern
```
sweep_interval_sec = 6.0
sweep_width_units = 5.0
sweep_speed_units_per_sec = sweep_width / sweep_interval
```

### Strike Penalty (capped)
```
strike_penalty_sec = min(configured_penalty, initial_ward * 0.3)
```
With default 30s penalty and 70s max initial Ward, capped at 21s.

### Cover Detection
```
is_in_cover = player_position inside Mound_collider
```

## Edge Cases

| Scenario | Expected Behavior | Rationale |
|----------|-------------------|-----------|
| Player enters cover during telegraph | No strike, player safe | Valid timing escape |
| Strike hits player already at low Ward | Penalty capped, no instant death | Prevents unfair death |
| Player hit during strike, exits cover immediately | Next sweep can hit again | No invulnerability |
| Multiple strikes in rapid succession | Each strike has cooldown, no spam | Fair pacing |

## Dependencies

| System | Direction | Nature of Dependency |
|--------|-----------|---------------------|
| Night Survival Run | This depends on it | **Hard**: operates during night phase |
| Map & Spawn Director | This depends on it | **Hard**: sweep pattern affects map layout |
| Player Controller | This depends on it | **Hard**: player position and cover state |
| Ward Timer | This depends on it | **Hard**: strike applies penalty |

## Tuning Knobs

| Parameter | Current Value | Safe Range | Effect of Increase | Effect of Decrease |
|-----------|---------------|------------|-------------------|-------------------|
| `sweep_interval_sec` | 6.0 | 3.0-10.0 | More time between sweeps | More frequent pressure |
| `sweep_width` | 5.0 | 3.0-8.0 | Harder to avoid | Easier to avoid |
| `telegraph_duration_sec` | 1.5 | 0.8-2.5 | More reaction time | Less reaction time |
| `strike_penalty_sec` | 30 | 15-45 | Harsher penalty | More forgiving |
| `strike_damage_cap_pct` | 30 | 20-40 | Lower cap (more forgiving) | Higher cap |

## Visual/Audio Requirements

| Event | Visual | Audio | Priority |
|-------|--------|-------|----------|
| Sweep movement | Green cone scans background | Low rumbling drone | High |
| Player targeted | Cone color intensifies | Rising tension sound | High |
| Telegraph | Ground shimmer, warning icon | Crackling alarm | Critical |
| Strike hit | Screen shake, red flash, knockback | Loud impact | Critical |
| Debris created | Rock falls at impact, becomes cover | Crash, rubble settling | High |

## Acceptance Criteria

- [ ] Skeleton visible in background
- [ ] Green cone sweeps predictably across lane
- [ ] Cover detection works (full collider inside Mound)
- [ ] Telegraph shows 1.5s warning
- [ ] Strike applies -30s (capped) Ward penalty
- [ ] Debris created becomes new cover
- [ ] Player can time covers to avoid strikes

## Open Questions

| Question | Owner | Deadline | Resolution |
|----------|-------|----------|------------|
| Should sweep pattern be random or fixed? | Game Designer | Before implementation | Open - Fixed is more learnable |
| Should there be blind spots in sweep? | Level Designer | Before map design | Open - Creates variety |