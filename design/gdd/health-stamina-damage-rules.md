# Health/Stamina & Damage Rules

> **Status**: Approved
> **Author**: User + opencode
> **Last Updated**: 2026-05-07
> **Implements Pillar**: Consequence-driven survival loop with day/night emotional contrast

## Overview

Health/Stamina & Damage Rules defines the unified survival resource system that merges traditional "health" and "stamina" into a single mechanic called **Spirit Ward** (Nước Mắm Cốt). This system tracks the player's humanity and survival time during the night phase, acting as both the timer and the health bar. Every action—sprinting, dashing, getting hit by hazards—consumes Ward time. When Ward reaches zero, the whale devours the player. This system exists because Solar Phobia needs a single, understandable resource that creates tension: every second counts, and every choice has a time cost.

## Player Fantasy

The player should feel **time pressure and vulnerability**—their Ward timer is always counting down, and there's never enough time to be safe. The fantasy is "running out of life"—each action must be weighed against survival time. When Ward drops to critical levels (≤25%), the screen should feel terrifying: chromatic aberration, audio distortion, and the sense that death is imminent. The emotional promise is "every moment matters"—the player never feels safe because the timer is always ticking.

## Detailed Design

### Core Rules

1. **Unified Resource**: All health and stamina functions merge into Spirit Ward (Ward Timer).
2. **Initialization**: At night start, Ward is computed from day choices: `Initial_Ward_Sec = Base_Ward_Sec + (Ghosts_Saved * Ward_Per_Ghost_Sec) - Day_Penalties_Sec`.
3. **Action Costs**: Each player action consumes Ward time directly.
4. **Time Drain**: Ward constantly drains during night at rate modified by Bone Relic pickups.
5. **Hazard Damage**: Traps and searchlight strikes apply instant Ward penalties.
6. **Death Condition**: Ward = 0 triggers lose state.

### Ward Initialization

| Parameter | Value | Source | Description |
|-----------|-------|--------|-------------|
| `Base_Ward_Sec` | 10 | Game State contract | Minimum survival time (intentionally short) |
| `Ward_Per_Ghost_Sec` | 30 | config | Bonus time per saved soul |
| `Day_Penalties_Sec` | 0-30 | Day Service | Penalties from failed minigames |
| Max Initial Ward | 70 | computed | 10 + (2 × 30) = 70s with 2 souls saved |

### Action Costs

| Action | Ward Cost | Notes |
|--------|-----------|-------|
| Sprint (Shift held) | -0.5s/sec | Continuous while held, in addition to base drain |
| Jump / Double Jump | 0 | Free movement |
| Swing (cloth throw) | -2.0s | One-time burst |
| Spirit Dash | -5.0s | Burst movement |
| Glide | -1.0s/sec | Continuous while airborne |

### Ward Drain Rate

```csharp
float effective_ward_drain = base_drain_rate * (1 + (bones_carried * hallucination_multiplier));
```

| Parameter | Default | Range | Owner |
|-----------|---------|-------|-------|
| `base_drain_rate` | 1.0/s | 0.1-5.0 | Map & Spawn Director |
| `hallucination_multiplier` | 1.0 | 1.0-3.0 | Map & Spawn Director |
| `bones_carried` | 0-3 | runtime | Player state |

### Hazard Penalties

| Hazard | Ward Penalty | Source |
|--------|--------------|--------|
| Nước Dâng (Water Trap) | -3.0s/sec | Consequence Resolver |
| Lưới Máu (Blood Net) | -5.0s instant + slow | Consequence Resolver |
| Ảo Ảnh (Illusion) | -15.0s instant | Consequence Resolver |
| Searchlight Strike | -30.0s (capped) | Map & Spawn Director (`StrikeTimePenaltySec`) |

**Strike Damage Cap**: To prevent unbounded difficulty spikes (e.g., 60+ seconds lost from strike + sprint + Bone Relic drain), the maximum Ward loss per strike is capped at `min(StrikeTimePenaltySec, Initial_Ward_Sec * 0.3)`. For example, with 70s initial Ward, max strike penalty is 21s (30%), not 30s.

**Strike Warning Priority**: The strike warning icon (from Map & Spawn Director) MUST have visual z-order priority above all Tier 4 (Panic) effects (chromatic aberration). This ensures players can see the warning even at critical Ward levels. Alternative: use audio cue (distinct strike alarm sound) as fallback when visual is obscured.

### Readability Thresholds

| Ward Percent | Tier | Visual/Audio Effects | Max Duration |
|--------------|------|---------------------|---------------|
| >75% | Tier 1: Stable | Normal | None |
| ≤75% | Tier 2: Creeping Dread | Vignette (α 0.3), low-pass audio | None |
| ≤50% | Tier 3: Heavy Burden | Heavy breathing, dash cooldown +0.1s | None |
| ≤25% | Tier 4: Panic | Chromatic aberration, whispering audio | 30 sec cap |
| ≤10s | Tier 5: Death Spiral | Tinnitus SFX, tunnel vision | None |

**Tier 4 Duration Cap**: To prevent feedback loop (low Ward → panic → sprint → faster drain → more panic), Tier 4 effects have a maximum duration of 30 seconds. After 30 seconds at ≤25% Ward, Tier 4 effects fade to Tier 3 intensity even if Ward remains ≤25%.

## Formulas

### Initialization Formula

```
Initial_Ward_Sec = Base_Ward_Sec + (Ghosts_Saved * Ward_Per_Ghost_Sec) - Day_Penalties_Sec
```

| Variable | Type | Range | Source | Description |
|----------|------|-------|--------|-------------|
| `Base_Ward_Sec` | float | 10 | config | Minimum survival time |
| `Ghosts_Saved` | int | 0-2 | NPC Model | Number of souls saved |
| `Ward_Per_Ghost_Sec` | float | 30 | config | Bonus per saved soul |
| `Day_Penalties_Sec` | float | 0-30 | Day Service | Accumulated penalties |

**Expected output range**: 0-70 seconds

### Effective Drain Rate

```
effective_ward_drain = base_drain_rate * (1 + (bones_carried * hallucination_multiplier))
```

| Variable | Type | Range | Source | Description |
|----------|------|-------|--------|-------------|
| `base_drain_rate` | float | 0.1-5.0 | Map Director config | Base time loss per second |
| `bones_carried` | int | 0-3 | runtime | Whale bones collected |
| `hallucination_multiplier` | float | 1.0-3.0 | Map Director config | Multiplier per bone |

**Expected output range**: 0.1-20.0 units/second

### Ward Percentage

```
Ward_Percent = (Current_Ward / Max_Initial_Ward) * 100
```

| Variable | Type | Range | Source | Description |
|----------|------|-------|--------|-------------|
| `Current_Ward` | float | 0-Max | runtime | Current Ward value |
| `Max_Initial_Ward` | float | 70 | config | Maximum possible initial Ward |

**Expected output range**: 0-100%

## Edge Cases

| Scenario | Expected Behavior | Rationale |
|----------|-------------------|-----------|
| Ward goes negative | Clamp to 0, trigger death | No negative time allowed |
| All souls abandoned (0 saved) | Base_Ward_Sec = 10s, no bonus | Minimum challenge |
| Strike during drift-dash | Apply full penalty | No invulnerability frames |
| Multiple traps hit same frame | Sum all penalties | Cumulative consequence |
| Ward at 5s, picks up bone | Drain jumps from 1x to 2x | Instant tension spike |

## Dependencies

| System | Direction | Nature |
|--------|-----------|--------|
| Game State / Phase State Machine | Depends on this | Hard: manages Ward lifecycle, triggers death |
| Map & Spawn Director | This depends on it | Hard: provides drain rates, strike penalty |
| NPC/Soul Data Model | Soft | Provides Ghosts_Saved count |
| Day Service & Selection | Soft | Provides Day_Penalties_Sec |

**Interface Ownership**:
- Map & Spawn Director owns: `base_drain_rate`, `hallucination_multiplier`, `StrikeTimePenaltySec`
- Health/Stamina references but does not own these values

## Tuning Knobs

| Parameter | Current | Safe Range | Effect of Increase | Effect of Decrease |
|-----------|---------|------------|---------------------|---------------------|
| `Base_Ward_Sec` | 10s | 5-30s | Harder base survival | Easier base survival |
| `Ward_Per_Ghost_Sec` | 30s | 15-60s | More reward for saving | Less incentive to save |
| `base_drain_rate` | 1.0/s | 0.1-5.0 | Faster time pressure | Slower time pressure |
| `StrikeTimePenaltySec` | 30s | 5-60s | Harsh punishment | Forgiving punishment |
| `hallucination_multiplier` | 1.0 | 1.0-3.0 | More bone risk | Less bone risk |

## Acceptance Criteria

1. **Initialization**: Ward starts at correct value based on day choices (10-70s range)
2. **Action Costs**: Each action subtracts exact Ward amount specified
3. **Time Drain**: Drain rate correctly scales with bones carried
4. **Hazard Damage**: Each trap applies correct penalty
5. **Death Trigger**: Ward = 0 triggers lose event to Game State
6. **Visual Feedback**: Each threshold (75%, 50%, 25%, 10s) triggers correct VFX
7. **Formula Accuracy**: All computed values match specification exactly
8. **Phase Lock**: Ward only depletes during NightSurvival phase

---

*End of Health/Stamina & Damage Rules GDD.*