# Shadow Spatial Management

> **Status**: Approved
> **Author**: User + opencode
> **Last Updated**: 2026-05-06
> **Implements Pillar**: Consequence-driven survival loop with day/night emotional contrast

## Overview$

Shadow Spatial Management controls the shrinking shadow polygon and thermal death zones during Solar Phobia's day phase. As the Mặt Trời Rỗng (Hollow Sun) rises, the safe zone (bóng râm xanh lam) shrinks via polygon contraction, forcing 3 souls and the player into tighter space. When a soul touches sunlight, it burns (thermal death) — this is the "moral pressure" moment where player must push one soul into the sun to save the other two. Without this system, the day phase lacks spatial consequence — choices feel meaningless because the sun isn't a real threat.

**Key facts:**
- Only active during `DayService` and `ChoiceLock` phases (Game State contract)
- Uses 2.5D top-down view; shadow is a polygon rendered on the ground plane
- Thermal death is instant: soul touches sunlight → burns, Ward Timer unaffected (only NPCs die)
- Player is "trapped" in shrinking zone — cannot move (Player Controller disabled during DayService)

## Player Fantasy$

The player should feel **"trapped and morally burdened"** as the shadow shrinks — the sun is an active threat judging their choices. The fantasy is **"the sun is watching, and it's hungry."** As the safe zone contracts, the player realizes they cannot save everyone — the moment of pushing a soul into sunlight should feel **genuinely uncomfortable**, like you're condemning someone to a terrible death.

Reference games: *Amnesia: The Dark Descent* (trapped, claustrophobic dread), *Papers, Please* (moral weight, burden of decisions). This is NOT a "fun" system — it's infrastructure that creates the feeling, not something the player "enjoys" interacting with directly.

The emotional promise: **"You can't save everyone, and the sun makes sure you know it."** The shrinking shadow is a timer, but a visible, spatial one — you see the safe space disappearing, and you know someone must go.

## Detailed Design$

### Core Rules$

1. **Active Phase**: Only during `DayService` and `ChoiceLock` (Game State contract per systems index).
2. **Shadow Polygon**: 2.5D top-down view; shadow is a convex polygon rendered on ground plane, color: xanh lam mát mẻ (cool blue).
3. **Shrink Mechanic**: Mặt Trời Rỗng rises; shadow polygon contracts linearly over `DayDurationSec` (from Game State formula: `phase_duration = base_phase_duration * pacing_multiplier`).
4. **Thermal Death**: Soul touches sunlight (outside polygon) → `NightOutcomeState = Lost` (not Ward Timer loss). Player is immune (cannot die during day).
5. **Crowding**: 3 souls + player have RigidBody colliders; they push each other. Soul hit-box: 1x1m, Player hit-box: 0.8x0.8m.
6. **Capacity**: At final second of DayService, shadow can hold max 3 entities (Tú + 2 souls). Exceeding causes nearest soul(s) to be pushed out (auto-push towards sunlight).

**Key constraints**:
- Player cannot move (Player Controller disabled during DayService — WASD ignored). 
- Only 2 souls can survive (saved) + 1 must die (abandoned → sunlight death). 
- Shrink rate is deterministic: `shrink_rate = initial_polygon_area / phase_duration`.

### States and Transitions$

| State | Entry Condition | Exit Condition | Behavior |
|-------|-----------------|----------------|----------|
| Active | `DayService` phase active | `ChoiceLock` begins | Shrink polygon, thermal death enabled, crowd physics active |
| Locked | `ChoiceLock` phase active | `NightSurvival` begins | Freeze shadow state, no changes allowed |
| Inactive | `NightSurvival` or `Reset` active | `DayService` restarts | No shadow rendering or logic |

### Interactions with Other Systems$

- **Game State / Phase State Machine -> Shadow Spatial**: 
  - Receives phase change events: `OnDayStart` enabling, `OnChoiceLock` freezing, `OnNightStart` disabling.
  - Provides phase contract: only active during `DayService`/`ChoiceLock`.
- **Player Controller <- Shadow Spatial**: 
  - No direct interaction — Player Controller is disabled (UI only) during day.
  - Shadow creates spatial pressure that influences crowd physics (souls push player).
- **Day Service & Selection <-> Shadow Spatial**: 
  - Souls crowd within shadow polygon; player sees shrinking space via 2.5D top-down view.
  - Pressure influences selection timing (player must choose quickly as space shrinks).
- **NPC/Soul Data Model <- Shadow Spatial**: 
  - Tracks `LifeState = Lost` for souls that touch sunlight (thermal death).
  - No direct write — shadow triggers thermal death event.

## Formulas$

### Shrink Rate$
```
shrink_rate = initial_polygon_area / phase_duration
```

| Variable | Type | Range | Source |
|----------|------|-------|--------|
| initial_polygon_area | float m² | 50-200 | config | Starting shadow size |
| phase_duration | float sec | 60-300 | Game State | Duration of DayService |

**Expected output range**: 0.1-3.0 m²/sec.  
**Edge case**: clamp to min 0.1 m²/sec if phase_duration > 2000s (never happens in vertical slice).

### Thermal Death Check$
```
is_touching_sunlight = NOT (soul_position WITHIN polygon_bounds)
```

**Expected output**: boolean.  
**Edge case**: if polygon area < 1.0 m², auto-trigger thermal death for all souls.

### Crowd Density$
```
crowd_density = entity_count / polygon_area
```

**Expected output range**: 0.01-0.5 souls/m².  
**Edge case**: if density > 0.5, trigger auto-push (nearest soul outward).

## Edge Cases$

| Scenario | Expected Behavior | Rationale |
|----------|-------------------|-----------|
| Polygon area < 1.0 m² | Auto-trigger thermal death for all souls | Prevents impossible survival |
| Soul pushes player into sunlight | Player immune (can't die day), soul burns → `LifeState = Lost` | Sun only kills souls |
| Crowd density > 0.5 souls/m² | Auto-push nearest soul outward (towards sunlight) | Enforces "1 must die" rule |
| Player tries WASD movement | Ignored (Player Controller disabled) | Consistent with phase rules |
| DayService timeout (auto-commit) | Shadow freezes at final size, no more shrinking | Preserves moral choice moment |
| Two souls touch sunlight simultaneously | Both burn → `LifeState = Lost` for both | Consistent thermal death rule |

## Dependencies$

### Upstream (This depends on):
| System | Direction | Nature of Dependency |
|--------|-----------|---------------------|
| Game State / Phase State Machine | This depends on it | **Hard**: provides phase events (`OnDayStart`, `OnChoiceLock`, `OnNightStart`). Only active during `DayService`/`ChoiceLock`. |

### Downstream (Depends on this):
| System | Direction | Nature of Dependency |
|--------|-----------|---------------------|
| None listed in systems index yet |

**Interface ownership:**
- **Shadow Spatial owns** shadow polygon rendering, shrink rate, thermal death detection.
- **Game State owns** phase contract (active only during day).

## Tuning Knobs$

| Parameter | Current Value | Safe Range | Effect of Increase | Effect of Decrease |
|-----------|---------------|------------|-------------------|-------------------|
| `initial_polygon_area` | 100 m² | 50-200 m² | Larger safe zone, less pressure | Smaller zone, faster death |
| `shrink_rate` | depends on phase_duration | 0.1-3.0 m²/sec | Faster shrink, higher pressure | Slower shrink, more time |
| `polygon_color` | xanh lam mát mẻ | — | N/A | N/A |

**Interacting Knobs:**
- `shrink_rate` depends on `initial_polygon_area` and `DayDurationSec` (from Game State).
- Increasing `DayDurationSec` decreases `shrink_rate` (more time, slower shrink).

## Visual/Audio Requirements$

| Event | Visual Feedback | Audio Feedback | Priority |
|-------|-----------------|----------------|----------|
| Shadow Active (DayService) | Convex blue polygon, cool tint | Soft wind, distant waves | High |
| Polygon Shrink | Edge glow brightens, subtle hiss | Tension rise, whispers increase | High |
| Soul Touches Sunlight | Body contorts, heat distortion, ash fade | Sharp sizzle, soul scream | Critical |
| Crowd Density Up | Souls overlap, hit-box push VFX | Whispers → urgent murmurs | Medium |
| Day End (ChoiceLock) | Polygon freeze, edge pulse | Tension drop, low drone | Medium |

## UI Requirements$

| Information | Display Location | Update Frequency | Condition |
|-------------|------------------|------------------|-----------|
| Shadow size (m²) | Top bar, cool blue tint | Real-time | Active phase only |
| Souls remaining (alive) | Decision panel footer | On thermal death | DayService |
| Crowd density warning | Screen edge pulse (red) | On density >0.5 | Active phase |
| No direct UI — player uses Day Service | N/A | N/A | Always |

## Acceptance Criteria$

- [ ] **Polygon Shrink**: Linear contraction over `DayDurationSec` (verified via render debug).
- [ ] **Thermal Death**: Soul touches sunlight → `LifeState = Lost` (player immune).
- [ ] **Crowd Density**: Density >0.5 souls/m² → auto-push nearest soul outward.
- [ ] **Player Immunity**: WASD/mouse ignored during DayService (Player Controller disabled).
- [ ] **Performance**: Shadow polygon update completes within 16ms/frame (60fps).
- [ ] **Cross-System**: Game State phase events trigger correctly (OnDayStart/OnChoiceLock).
- [ ] **UI Feedback**: Shadow size visible via 2.5D top-down view, no direct UI controls.

## Open Questions$

| Question | Owner | Deadline | Resolution |
|----------|-------|----------|------------|
| Should polygon shrink be linear or exponential? | Game Designer | Before vertical slice lock | Open |
| Should thermal death min polygon size be configable? | Systems Designer | Before MVP lock | Open |
| Should crowd push force scale with density? | Systems Designer | Before MVP lock | Open |
| Should player ever move during DayService? | UX-Designer | Before playtest | Open |
