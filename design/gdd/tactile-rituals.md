# Tactile Rituals

> **Status**: Approved
> **Author**: User + opencode
> **Last Updated**: 2026-05-07
> **Implements Pillar**: Consequence-driven survival loop with day/night emotional contrast

## Overview

Tactile Rituals are the three minigames (diêm, rót, vay) that the player performs during Day Phase to earn Hương Hỏa (Spirit Essence). Each ritual is a physical interaction that feels grounded in Vietnamese cultural tradition—striking matches, pouring tea, fanning coals. Success earns resources; failure incurs penalties. Without this system, day phase has no player action—only UI clicking—with no tactile satisfaction.

## Player Fantasy

The player should feel **deliberate craftsmanship**—each action requires attention and physical timing, not just clicking. The rituals should feel **intimate and ritualistic**—like actually performing the traditions. The minigames create the "tactile discomfort" of day phase before the physical panic of night.

## Detailed Design

### Core Rules

1. **Diêm (Match Lighting)**: Click and drag to strike match against box. Requires proper friction/duration to ignite. Success → +Hương Hỏa. Fail (too short/fumble) → broken match, no points.
2. **Rót (Tea Pouring)**: Hold and tilt to pour tea from pot to bowl. Requires steady hand—tilting too fast spills, too slow gets cold. Success → +Hương Hỏa. Spill → -Hương Hỏa penalty.
3. **Vay (Fan Fanning)**: Click rhythmically to fan charcoal. Must maintain heat without blowing ash. Success → +Hương Hỏa. Too hard → ash flies, penalty.
4. **Ritual Selection**: Player chooses which ritual to attempt per cycle (3 rituals in rotation).
5. **Difficulty Scaling**: Early day (0-1:30) = easy rituals. Mid day (1:30-3:00) = slightly harder. Late day (3:00-5:00) = harder with time pressure.
6. **Penalties**: Failed rituals reduce Hương Hỏa and add to `Day_Penalties_Sec` which subtracts from initial Ward at night.

### States and Transitions

| State | Entry Condition | Exit Condition | Behavior |
|-------|-----------------|----------------|----------|
| RitualAvailable | DayService active, previous ritual cooldown done | Player selects ritual | Show ritual options |
| DiêmActive | Player selects diêm | Match lit or broken | Track drag motion, detect strike |
| RótActive | Player selects rót | Tea poured or spilled | Track tilt angle, detect spill |
| VayActive | Player selects vay | Heat maintained or ash flies | Track click rhythm |
| RitualComplete | Ritual succeeds or fails | Cooldown timer starts | Award/penalty, update Hương Hỏa |
| Cooldown | Ritual complete | Timer expires | Prevent spam, create pacing |

### Interactions with Other Systems

- **Game State / Phase State Machine -> Tactile Rituals**: Phase-gated (DayService only). Sends timer pressure signals.
- **NPC/Soul Data Model <- Tactile Rituals**: Souls react to ritual success/fail (happy on success, anxious on fail).
- **Day Service & Selection <- Tactile Rituals**: Hương Hỏa earned determines how many souls can receive resources.
- **Health/Stamina & Damage Rules <- Tactile Rituals**: Failed rituals add to `Day_Penalties_Sec`.

## Formulas

### Hương Hỏa Award
```
successful_ritual = base_huong_hoa + difficulty_bonus
```

| Variable | Type | Range | Source | Description |
|----------|------|-------|--------|-------------|
| base_huong_hoa | int | 10 | config | Base Hương Hỏa per success |
| difficulty_bonus | int | 0-10 | config | Extra based on time of day |

**Expected output range**: 10-20 per ritual

---

### Day Penalty Calculation
```
day_penalty_sec = sum(failed_rituals * penalty_per_fail)
```

| Variable | Type | Range | Source | Description |
|----------|------|-------|--------|-------------|
| penalty_per_fail | int | 5-10 | config | Seconds subtracted from initial Ward |
| failed_rituals | int | 0-N | runtime | Count of failed rituals in day |

**Expected output range**: 0-30 sec penalty

## Edge Cases

| Scenario | Expected Behavior | Rationale |
|----------|-------------------|-----------|
| Player spams rituals (no cooldown) | Blocked by cooldown timer | Prevents exploit |
| All three rituals fail in a row | Heavy penalty, souls enter Panic | Creates desperation |
| Ritual fails but player persists | Penalty accumulates, timer still running | No escape from failure |
| Player ignores rituals entirely | Cannot earn Hương Hỏa, minimal Ward at night | Forces engagement |

## Dependencies

| System | Direction | Nature of Dependency |
|--------|-----------|---------------------|
| Game State / Phase State Machine | This depends on it | **Hard**: phase-gated, timer signals |
| NPC/Soul Data Model | This depends on it | **Soft**: soul reactions to ritual outcomes |
| Day Service & Selection | This depends on it | **Hard**: Hương Hỏa determines resource assignment |
| Health/Stamina & Damage Rules | Depends on this | **Hard**: penalty seconds reduce initial Ward |

## Tuning Knobs

| Parameter | Current Value | Safe Range | Effect of Increase | Effect of Decrease |
|-----------|---------------|------------|-------------------|-------------------|
| `base_huong_hoa` | 10 | 5-20 | More resources per ritual | Fewer resources |
| `penalty_per_fail` | 5 | 3-10 | Harsher penalties | More forgiving |
| `ritual_cooldown_sec` | 3.0 | 1.0-5.0 | More time between rituals | Faster ritual spam |
| `difficulty_scaling` | 1.0-2.0x | 1.0-3.0x | Steeper difficulty curve | Gentler curve |

## Visual/Audio Requirements

| Event | Visual Feedback | Audio Feedback | Priority |
|-------|-----------------|----------------|----------|
| Match strike | Spark particles, smoke, flame ignites | Scratch sound, flame whoosh | High |
| Tea pour | Liquid stream, steam rising | Liquid pouring, steam hiss | High |
| Fan click | Embers glow, ash particle burst | Fan swish, crackling | High |
| Ritual fail | Dim feedback, smoke/fizzle | Failure sound, disappointed sigh | High |
| Hương Hỏa awarded | Bright particle burst, soul glows | Success chime, achievement tone | Critical |

## UI Requirements

| Information | Display Location | Update Frequency | Condition |
|-------------|------------------|------------------|-----------|
| Hương Hỏa count | Top-left (collectible icons) | After each ritual | Day phase |
| Ritual selection | Center (3 icons) | On ritual available | Day phase |
| Current ritual feedback | Near player | Real-time during ritual | Day phase |
| Penalty warning | Red flash when fail | On ritual fail | Day phase |

## Acceptance Criteria

- [ ] Diêm: drag to strike, success/fail detection works
- [ ] Rót: tilt to pour, spill detection works
- [ ] Vay: click to fan, ash detection works
- [ ] Cooldown prevents spam
- [ ] Hương Hỏa awarded correctly
- [ ] Penalties applied to Ward calculation
- [ ] Difficulty scales with time of day

## Open Questions

| Question | Owner | Deadline | Resolution |
|----------|-------|----------|------------|
| Should rituals have hidden skill ceiling? | Game Designer | Before implementation | Open |
| Can multiple players do rituals simultaneously? | Game Designer | Before implementation | Open - Single player only |