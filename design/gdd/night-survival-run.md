# Night Survival Run

> **Status**: Approved
> **Author**: User + opencode
> **Last Updated**: 2026-05-07
> **Implements Pillar**: Consequence-driven survival loop with day/night emotional contrast

## Overview

Night Survival Run is the action phase of Solar Phobia's core loop—the frantic, timed run from StartShrine to EndShrine while the player's Ward Timer counts down. During this phase, the player navigates a dark, hazardous map using movement skills (sprint, dash, swing, glide) and cover mechanics while being pursued by the Cá Ông (whale) boss searchlight and haunted by karma hazards that spawn based on which soul was abandoned during Day Phase. The player must reach the EndShrine before their Ward Timer depletes; success means salvation, failure means the whale devours them. Without this system, the day/night loop has no resolution—choices made in daylight have no consequence.

## Player Fantasy

The player should feel **desperate hope** — running at full speed toward the shrine with the timer counting down, knowing each second brings them closer to salvation but also closer to the whale's jaws. The shrine glow on the horizon is both relief and desperation.

Simultaneously, the player should feel **haunted terror** — the specific curse that hunts them is *their fault*. If they abandoned Linh, water traps rise from the ground. If they abandoned Van, blood nets block the path. If they abandoned Minh, the ground betrays them. The horror is personal, not random.

The emotional promise: **"Your day choices made this nightmare. Now survive it."** Every hazard encountered at night reminds the player of who they left behind. The whale is judgment; the shrine is redemption.

## Detailed Design

### Core Rules

**Phase Entry**
1. Night Survival Run starts when Game State Machine transitions from `ChoiceLock` to `NightSurvival`, sending `OnNightStart(payload)` event.
2. Payload contains: `curse_type` (Drag/Block/FakeShrine), `initial_ward_sec`, `spawn_bundle`, `abandoned_soul_id`.
3. Player spawns at StartShrine location; camera transitions to night view (cold color grade, vignette).

**Objective**
4. Player must navigate from StartShrine to EndShrine (Am Tho Bai Thuyen) before Ward Timer depletes.
5. EndShrine is visible as a glowing beacon; direction indicator shows at screen edge.
6. When player presses E key within EndShrine trigger zone (radius 3.0 units), win triggers.

**Movement System (Player Controller integration)**
7. WASD movement with CharacterController physics.
8. Jump: Hold for higher jump; wall-slide against rock faces; wall-jump to climb.
9. Sprint (Shift): Increased movement speed, triggers FOV increase + motion blur.
10. Spirit Dash (Shift while airborne/grounded): Quick burst, costs -5s Ward, leaves trail effect.
11. Swing (Left Click on anchor points): Pendulum physics to swing across gaps.
12. Glide (Space while falling): Reduces fall speed to 30%, costs -1s Ward per second.
13. Cover (S + inside Mound collider): Player must be fully inside Mound collider to be "in cover".

**Hazard System**
14. **Boss Cá Ông Searchlight**: Skeleton whale in background sweeps a green cone. If player exposed (not in valid cover) during sweep, strike telegraph triggers, then -30s Ward penalty after delay.
15. **Karma Hazards** (based on `abandoned_soul_id`):
    - `Linh` abandoned → Vũng Nước Tử Thần (Water Trap): DoT -3s/s when standing in water zones.
    - `Van` abandoned → Lưới Máu (Blood Net): -5s + 50% slow for 3s when touched.
    - `Minh` abandoned → Bệ Đá Ảo Ảnh (Illusion Platform): Platform collapses 0.2s after stepped on.
16. **Ngọc Cốt Relics**: Optional pickups that increase Ward drain multiplicatively (hallucination effect).

**Survival Mechanics**
17. Ward Timer initializes: `Initial_Ward_Sec = 10 + (Ghosts_Saved * 30) - Day_Penalties`.
18. Passive Ward drain: 1.0s/sec base, multiplied by `(1 + bones_carried * hallucination_multiplier)`.
19. Sensory tier degradation triggers at 75%, 50%, 25%, and ≤10s remaining.

**Phase Exit**
20. Win: `OnShrineReached` sent to Game State Machine → transition to `Resolve` (success path).
21. Lose: Ward Timer = 0 → `OnWardTimerEmpty` sent to Game State Machine → transition to `Resolve` (failure path).

### States and Transitions

| State | Entry Condition | Exit Condition | Behavior |
|-------|-----------------|----------------|----------|
| NightStart | `OnNightStart(payload)` received | Initialization complete | Initialize Ward Timer, spawn hazards, enable movement systems |
| Navigating | Initialization complete | Win/lose condition met | Process player input, update Ward Timer, detect hazards |
| UnderAttack | Boss strike telegraph triggers | Strike resolves or player takes cover | Display warning VFX, apply knockback/damage after delay |
| ShrineApproach | Player within shrine_glow_start_distance (15 units) | Player reaches shrine or leaves range | Enable shrine glow VFX, show proximity indicator |
| Win | E key pressed in shrine zone | Event sent to Game State Machine | Play shrine bloom animation, transition to Resolve |
| Lose | Ward Timer = 0 | Event sent to Game State Machine | Play death animation (swallowed by whale), transition to Resolve |

### Interactions with Other Systems

- **Game State / Phase State Machine <- Night Survival Run**: Sends `OnShrineReached` (win) or `OnWardTimerEmpty` (lose). Receives `OnNightStart(payload)` on phase entry.
- **Player Controller -> Night Survival Run**: Sends movement input, receives movement constraints (cover state, dash cooldown).
- **Shrine Objective & Win/Lose Rules -> Night Survival Run**: EndShrine location and trigger zone owned by Shrine system. Night Run detects proximity and E key presses.
- **Health/Stamina & Damage Rules -> Night Survival Run**: Ward Timer logic owned by Health system. Night Run reads current Ward value and triggers lose when = 0.
- **Consequence Resolver -> Night Survival Run**: Receives `curse_type` payload that determines which karma hazards spawn.
- **Map & Spawn Director -> Night Survival Run**: Provides spawn bundle with hazard placements, StartShrine/EndShrine locations.
- **Boss Cá Ông Searchlight <- Night Survival Run**: Runs in parallel; Night Run detects when player is in/out of cover for searchlight logic.



## Formulas

### Ward Timer Initialization (Day → Night Translation)

```
Initial_Ward_Sec = Base_Ward_Sec + (Ghosts_Saved * Ward_Per_Ghost_Sec) - Day_Penalties_Sec
```

| Variable | Type | Range | Source | Description |
|----------|------|-------|--------|-------------|
| Base_Ward_Sec | float sec | 10 | config | Minimum survival time (intentionally short to prevent speedrunning) |
| Ghosts_Saved | int | 0-2 | Day Service | Number of souls saved (from 3 total) |
| Ward_Per_Ghost_Sec | float sec | 30 | config | Bonus time per saved soul |
| Day_Penalties_Sec | float sec | 0-30 | Day Service | Penalties from failed minigames/broken offerings |

**Expected output range**: 10-70 sec  
**Edge case**: clamp to minimum 10s to prevent trivial runs.

---

### Passive Ward Drain (Night)

```
effective_ward_drain = base_drain_rate * (1 + (bones_carried * hallucination_multiplier))
```

| Variable | Type | Range | Source | Description |
|----------|------|-------|--------|-------------|
| base_drain_rate | float sec | 1.0 | config | Default drain per second |
| bones_carried | int | 0-3 | runtime | Ngọc Cốt relics currently held |
| hallucination_multiplier | float | 1.0-3.0 | config | Extra drain per relic |

**Expected output range**: 1.0-20.0 sec/sec (passive drain always active)  
**Edge case**: clamp to max safe drain cap.

---

### Karma Hazard Damage

**Water Trap (Linh abandoned)**:
```
water_trap_damage = -3.0 * deltaTime  // per second while in zone
```

**Blood Net (Van abandoned)**:
```
blood_net_penalty = -5.0  // immediate on contact
blood_net_slow = 0.5  // 50% movement speed reduction
slow_duration = 3.0 sec
```

**Illusion Platform (Minh abandoned)**:
```
collapse_delay = 0.2 sec  // after player steps on
```

---

### Boss Searchlight Strike

```
strike_penalty = -30.0 sec Ward
telegraph_duration = 1.5 sec
strike_delay = 1.0 sec after telegraph
```

---

### Active Skill Costs

| Skill | Ward Cost | Cooldown |
|-------|-----------|----------|
| Spirit Dash | -5.0s | 2.0 sec |
| Swing | -2.0s | 1.5 sec |
| Glide | -1.0s/sec | continuous while held |

---

### Shrine Proximity Check

```
is_near_shrine = distance(player_position, shrine_center) <= shrine_trigger_radius
```

| Variable | Type | Range | Source | Description |
|----------|------|-------|--------|-------------|
| shrine_trigger_radius | float | 3.0 | config | Radius for E key activation |

---

### Sensory Tier Thresholds

| Tier | Ward % | Name |
|------|--------|------|
| 1 | >75% | Stable |
| 2 | ≤75% | Creeping Dread |
| 3 | ≤50% | Heavy Burden |
| 4 | ≤25% | Panic |
| 5 | ≤10s | Death Spiral |

## Edge Cases

| Scenario | Expected Behavior | Rationale |
|----------|-------------------|-----------|
| Player reaches shrine + Ward = 0 simultaneously | Lose triggers (Ward = 0 = death); win is ignored | Death is absolute; whale devours you |
| Player dies (Ward = 0) while in shrine zone | Lose triggers; win is ignored even if E was pressed | Death is final; whale gets you |
| Boss strike while player in cover | No strike; player is protected by Mound | Valid cover state prevents damage |
| Player exits cover during strike telegraph | Strike resolves; player takes full penalty | Timing matters; must stay in cover |
| Multiple Ngọc Cốt picked up simultaneously | Each triggers drain multiplier additively | Stacking creates extreme difficulty |
| Illusion platform collapses with player on it | Player falls; no fall damage but time lost | Platforms are deceptive hazards |
| Player at 0 Ward but has active dash in progress | Dash completes, then death triggers | Prevent ability to "dodge" death |
| Ward timer reaches 0 during sensory tier transition | Lose takes priority; transition interrupted | Death is absolute |
| Boss searchlight sweep covers entire safe path | Player must wait for sweep to pass or find alternate route | Tactical pacing; cannot brute force |
| Player tries to enter EndShrine during wrong phase | Shrine Objective ignores; no event generated | Phase gating prevents invalid actions |
| Map fails to load (spawn bundle invalid) | Game State Machine enters FatalError | Cannot start night without valid map |

## Dependencies

### Upstream (This depends on)

| System | Direction | Nature of Dependency |
|--------|-----------|---------------------|
| Game State / Phase State Machine | This depends on it | **Hard**: receives `OnNightStart(payload)` with curse_type, initial_ward_sec. Sends win/lose events back. |
| Shrine Objective & Win/Lose Rules | This depends on it | **Hard**: reads EndShrine location and trigger zone; sends `OnShrineReached` when E pressed in zone. |
| Consequence Resolver | This depends on it | **Hard**: receives `curse_type` (Drag/Block/FakeShrine) that determines karma hazard behavior. |
| Health/Stamina & Damage Rules | This depends on it | **Hard**: reads Ward Timer value; triggers `OnWardTimerEmpty` when = 0. |
| Map & Spawn Director | This depends on it | **Hard**: receives spawn_bundle with hazard placements, StartShrine/EndShrine positions. |
| Player Controller | This depends on it | **Hard**: receives movement input; sends movement constraints (cover, dash costs). |

### Downstream (Depends on this)

| System | Direction | Nature of Dependency |
|--------|-----------|---------------------|
| HUD-less Design & Sensory Feedback | Depends on this | **Soft**: subscribes to Ward tier changes for sensory degradation. |
| Audio State Director | Depends on this | **Soft**: subscribes to night start/end, strike events for audio mix. |
| Save Seed / Run Reset | Depends on this | **Hard**: needs night outcome (win/lose) for run snapshot. |
| Boss Cá Ông Searchlight | Depends on this | **Soft**: operates in parallel; Night Run provides cover state for searchlight logic. |
| Curse Effect Modules | Depends on this | **Soft**: receives curse_type for VFX/audio based on karma hazards. |

### Provisional Dependencies (Not Started)

The following dependencies are not yet designed. This GDD assumes their interfaces:

| System | Expected Interface |
|--------|-------------------|
| Boss Cá Ông Searchlight | Sweep pattern data, strike telegraph events, -30s penalty on hit |
| Curse Effect Modules | curse_type → VFX mapping (blue tint for Drag, red for Block, shimmer for FakeShrine) |
| Ngọc Cốt / Relic System | Relic pickup events, bone count, drain multiplier |
| Resource Effects & Hương Hỏa | Resource consumption during night (if any) |

*Note: These are marked "Not Started" in systems-index. Interfaces assumed from master GDD description.*

## Tuning Knobs

| Parameter | Current Value | Safe Range | Effect of Increase | Effect of Decrease |
|-----------|---------------|------------|-------------------|-------------------|
| `base_ward_sec` | 10 | 5-30 | Longer minimum survival time | Faster death, higher tension |
| `ward_per_ghost_sec` | 30 | 15-60 | More reward for saving souls | Less incentive to save |
| `passive_drain_rate` | 1.0 | 0.5-2.0 | Faster time pressure | More forgiving |
| `dash_cost_sec` | 5.0 | 2-10 | Dash is more expensive | Dash spammable |
| `swing_cost_sec` | 2.0 | 1-5 | Swing is more expensive | Swing spammable |
| `glide_cost_per_sec` | 1.0 | 0.5-2.0 | Glide drains faster | Glide more sustainable |
| `boss_strike_penalty` | 30 | 15-60 | Boss more punishing | Boss more forgiving |
| `strike_telegraph_sec` | 1.5 | 0.5-3.0 | More warning time | Sudden strikes |
| `shrine_trigger_radius` | 3.0 | 2.0-5.0 | Easier to trigger win | Harder to trigger |
| `shrine_glow_distance` | 15.0 | 10.0-30.0 | Shrine visible earlier | Surprise shrine |
| `hallucination_multiplier` | 1.0 | 0.5-2.0 | Ngọc Cốt more punishing | Ngọc Cốt forgiving |
| `water_trap_dps` | 3.0 | 1-5 | Water trap more deadly | Water trap forgiving |
| `blood_net_slow` | 0.5 | 0.3-0.8 | Blood net more debilitating | Blood net less slow |
| `illusion_collapse_delay` | 0.2 | 0.1-0.5 | Platform collapses faster | More reaction time |

**Interacting Knobs:**
- `base_ward_sec` + `ward_per_ghost_sec` directly control night difficulty via day choices.
- `passive_drain_rate` + `hallucination_multiplier` interact: more Ngọc Cốt = exponential drain increase.
- `boss_strike_penalty` + `strike_telegraph_sec` control boss difficulty vs. player fairness.
- Dash/swing/glide costs should be balanced against `base_ward_sec` to ensure at least one use is viable.

## Visual/Audio Requirements

| Event | Visual Feedback | Audio Feedback | Priority |
|-------|-----------------|----------------|----------|
| Night Start (Day→Night transition) | Camera zoom-out abrupt, cold color grade, vignette increase | Threat motif + wind/wave layer swells | Critical |
| Movement - Sprint | FOV increase, motion blur, dust particles | Heavy breathing, footstep frequency increases | Medium |
| Movement - Dash | Trail renderer, dash VFX | Whoosh sound | High |
| Movement - Swing | Pendulum arc visual, rope/chain effect | Rope snap + wind rush | High |
| Movement - Glide | Khăn tang expands like parachute | Fabric flapping + wind | High |
| Enter Cover | Player crouches, Mound glow highlight | Muffled exterior sounds | High |
| Exit Cover | Player stands, glow fades | Exterior sounds return | High |
| Boss Searchlight Sweep | Green cone sweeps across background | Low rumbling drone | High |
| Boss Strike Telegraph | Ground shimmer, warning icon near reticle | Rising crack sound | Critical |
| Boss Strike Hit | Screen shake, red flash, knockback | Loud impact + distortion | Critical |
| Karma Hazard - Water Trap | Blue glow, water ripple VFX | Bubbling, distant drowning | High |
| Karma Hazard - Blood Net | Red tint, net overlay | Ripping fabric, chains rattling | High |
| Karma Hazard - Illusion | Shimmer, ground crack before collapse | Ground rumble, deceptive whisper | High |
| Ngọc Cốt Pickup | Vignette tighten, chromatic aberration | Breath crack, chant layer | High |
| Ward Tier Changes | Progressive sensory degradation (vignette→lowpass→breathing→tunnel vision) | Layered audio changes per tier | Critical |
| Shrine Approach | Shrine glow builds, soft light halo | Distant bell chime, low drone builds | High |
| Shrine Enter (trigger zone) | Bright glow bloom, path lights up | Relief stinger, tension drops | Critical |
| Win (E key pressed) | White fade to shrine interior | Success chime, whale song fades | Critical |
| Lose (Ward Timer = 0) | Rapid darken, camera shake, swallowed by whale | Distorted impact, whale roar | Critical |

## UI Requirements

| Information | Display Location | Update Frequency | Condition |
|-------------|------------------|------------------|-----------|
| Shrine direction cue | Screen edge (arrow points to shrine) | Real-time during NightSurvival | Night phase, shrine not reached |
| Shrine proximity indicator | Near player reticle (fills as approaching) | Real-time when within glow distance | Night phase, near shrine |
| Curse icon (Linh/Van/Minh) | Top-right corner | On night start | Night phase |
| Ward sensory tier indicator | Implicit (visual degradation) | Real-time per tier thresholds | Night phase |
| Strike warning | Near player reticle + ground shimmer | During telegraph window | Boss targeting player |
| Win/Lose status | Center-top (shrine icon for win) | On event trigger | Night phase |

**HUD-less Design Philosophy**:
- No numeric Ward Timer display
- No minimap
- No skill cooldown indicators (cooldowns felt through game feel)
- All information conveyed through sensory degradation, not numbers

## Acceptance Criteria

### Phase Entry
- [ ] `OnNightStart(payload)` received and parsed correctly
- [ ] Ward Timer initialized to correct value based on day choices
- [ ] Hazards spawn according to curse_type payload
- [ ] Camera transitions to night view

### Movement
- [ ] WASD movement works with CharacterController physics
- [ ] Jump with variable height, wall-slide, wall-jump functional
- [ ] Sprint (Shift) increases speed, triggers FOV change
- [ ] Spirit Dash (-5s Ward) works with cooldown
- [ ] Swing (Left Click) creates pendulum motion
- [ ] Glide (-1s/sec while falling) reduces fall speed
- [ ] Cover detection requires full collider inside Mound

### Hazards
- [ ] Boss searchlight sweeps background with telegraph before strike
- [ ] Strike applies -30s Ward penalty when player exposed
- [ ] Karma hazard (Water Trap) applies DoT when standing in zone
- [ ] Karma hazard (Blood Net) applies -5s + 50% slow for 3s
- [ ] Karma hazard (Illusion Platform) collapses 0.2s after stepped on
- [ ] Ngọc Cốt pickup increases Ward drain multiplicatively

### Survival
- [ ] Ward Timer counts down at base rate + multiplier
- [ ] Sensory tier changes trigger at correct thresholds (75%, 50%, 25%, ≤10s)
- [ ] Ward = 0 triggers death and `OnWardTimerEmpty` to Game State

### Objective
- [ ] EndShrine visible with glow at 15 units distance
- [ ] Shrine direction arrow shows at screen edge
- [ ] E key in shrine zone triggers `OnShrineReached`
- [ ] Win/Lose events sent to Game State Machine

### Performance
- [ ] Movement processing completes within 2ms average per frame
- [ ] Hazard detection processing completes within 1ms average per frame
- [ ] No memory leaks during 5-minute continuous play

### Cross-System Integration
- [ ] Game State Machine receives correct events (OnNightStart → win/lose)
- [ ] Shrine Objective receives E key events
- [ ] Health System Ward Timer integrated with Night Run
- [ ] Map Director spawn data correctly interpreted

## Open Questions

| Question | Owner | Deadline | Resolution |
|----------|-------|----------|------------|
| Should Ngọc Cốt be required for True Ending or optional? | Game Designer | Before vertical slice | Open |
| Should boss searchlight have blind spots or full coverage? | Game Designer | Before night implementation | Open |
| How many Ngọc Cốt should spawn per night run? | Level Designer | Before map design | Open |
| Should karma hazards be skippable or mandatory path? | Level Designer | Before map design | Open |
| Should dash/swing/glide have cooldowns or only Ward cost? | Systems Designer | Before implementation | Open |
| Does player need to press E to win, or auto-trigger on proximity? | UX Designer | Before first playtest | Open (Shrine GDD: E key required) |
| Should curse icon be visible to player before night starts? | UX Designer | Before first playtest | Open |