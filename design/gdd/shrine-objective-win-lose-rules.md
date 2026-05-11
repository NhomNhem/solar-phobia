# Shrine Objective & Win/Lose Rules

> **Status**: Approved
> **Author**: User + opencode
> **Last Updated**: 2026-05-06
> **Implements Pillar**: Consequence-driven survival loop with day/night emotional contrast

## Overview

Shrine Objective & Win/Lose Rules defines the conditions that end the night survival phase—specifically, what constitutes a "win" (reaching the EndShrine) and a "lose" (Ward Timer reaching zero). The system owns the win/lose detection logic, the shrine arrival event flow, and the failure/death sequence. During NightSurvival phase, the player must navigate from StartShrine to EndShrine (Am Tho Bai Thuyen) while their Ward Timer (Nuoc Mam Cot) counts down from the initial value set during Day Phase. Reaching the shrine triggers the win condition, sending a shrine arrival event to Game State Machine for transition to Resolve. If the Ward Timer depletes before shrine arrival, the player dies and the whale devours them—triggering the lose condition and transition to Resolve (failure path). Without this system, the night survival loop has no goal and no failure state—the player would run forever with no consequence.

## Player Fantasy

The player should feel **desperate hope** as they run toward the EndShrine—each step closer is a step away from the whale's jaws. Reaching the shrine should bring an overwhelming sense of **relief and salvation**—the nightmare ends, the whale won't get you, your choices mattered. The shrine glow (visual feedback) and relief stinger (audio) should make this feel like a genuine escape from death.

Conversely, the losing condition—Ward Timer reaching zero—should feel like **inescapable consequence**. The player isn't killed by a bug or bad design; they're devoured because their day choices left them with too little Ward Timer to survive the night. The fantasy is "the whale I abandoned has finally caught me"—a direct result of the soul I left behind during Day Phase. The rapid darken, camera fade to black, and distorted impact sound should feel like being swallowed whole—final, absolute, and entirely deserved.

The emotional promise: **Your day choices determine if you escape or get eaten.** The shrine is salvation earned through sacrifice; the whale is judgment for cowardice.

## Detailed Design

### Core Rules

1. **Win Detection**: When player presses E key within EndShrine trigger zone during `NightSurvival`, send `OnShrineReached` event to Game State Machine.
2. **Lose Detection**: When Ward Timer (Nuoc Mam Cot) reaches 0, send `OnWardTimerEmpty` event to Game State Machine.
3. **Phase Validation**: Shrine can only be activated during `NightSurvival` phase. E key press during other phases is ignored.
4. **Single Shrine**: Only one EndShrine exists per night (single lane from StartShrine to EndShrine).
5. **Win Outcome**: `OnShrineReached` triggers Game State Machine to transition to `Resolve` (success path).
6. **Lose Outcome**: `OnWardTimerEmpty` triggers Game State Machine to transition to `Resolve` (failure path—whale devours player).
7. **Shrine Visual**: EndShrine glows when player is within approach distance (visual cue for win condition).

### States and Transitions

| State | Entry Condition | Exit Condition | Behavior |
|-------|-----------------|----------------|----------|
| NightActive | `NightSurvival` phase starts | Win or Lose condition met | Monitor Ward Timer; detect shrine arrival via Player Controller E key |
| Win (Success) | E key pressed in EndShrine zone | Game State Machine transitions to `Resolve` | Send `OnShrineReached` event; play shrine glow bloom animation |
| Lose (Failure) | Ward Timer = 0 | Game State Machine transitions to `Resolve` | Send `OnWardTimerEmpty` event; play death animation (swallowed by whale) |

### Interactions with Other Systems

- **Game State / Phase State Machine <- Shrine Objective**: Receives `OnShrineReached` (win) or `OnWardTimerEmpty` (lose) events. Transitions to `Resolve` state for outcome processing.

- **Player Controller -> Shrine Objective**: Sends E key press event when player is near EndShrine. Shrine Objective validates phase and proximity, then triggers win.

- **Health/Stamina & Damage Rules -> Shrine Objective**: Sends `OnWardTimerEmpty` event when Ward Timer reaches 0. Shrine Objective forwards to Game State Machine as lose condition.

- **Map & Spawn Director <-> Shrine Objective**: Map Director owns EndShrine placement and trigger zone geometry. Shrine Objective reads player proximity to shrine zone for win detection.

- **HUD & Diegetic Feedback <- Shrine Objective**: Receives win/lose state changes for display updates (shrine direction cue, success/failure VFX).

## Formulas$

### Shrine Proximity Check

```
is_near_shrine = distance(player_position, shrine_center) <= shrine_trigger_radius
```

| Variable | Type | Range | Source | Description |
|----------|------|-------|--------|-------------|
| player_position | Vector2 | — | Player Controller | Current player world position |
| shrine_center | Vector2 | — | Map & Spawn Director | EndShrine trigger zone center |
| shrine_trigger_radius | float | 2.0-5.0 | config | Radius for E key activation |

**Expected output range**: boolean  
**Edge case**: if shrine zone not loaded (Map Director failed), return false.

---

### Win Condition Check

```
win_triggered = is_near_shrine AND E_key_pressed AND (current_phase == NightSurvival)
```

| Variable | Type | Range | Source | Description |
|----------|------|-------|--------|-------------|
| is_near_shrine | bool | {0,1} | Shrine Proximity Check | Player within trigger zone |
| E_key_pressed | bool | {0,1} | Player Controller | E key down event |
| current_phase | enum | {NightSurvival} | Game State Machine | Must be NightSurvival |

**Expected output range**: boolean  
**Edge case**: ignore E key if phase != NightSurvival (handled by Player Controller gating).

---

*Note: Ward Timer initialization (`Initial_Ward_Sec = Base_Ward_Sec + (Ghosts_Saved * 30s) - Day_Penalties_Sec`) is defined in **Health/Stamina & Damage Rules**. Ward Timer depletion (`Ward Timer = 0`) triggers lose condition—Shrine Objective only detects the threshold.*



## Edge Cases

| Scenario | Expected Behavior | Rationale |
|----------|-------------------|-----------|
| Player presses E outside shrine zone | Silently ignore; no event sent | Prevents false win triggers |
| Ward Timer = 0 exactly | Immediate lose; send `OnWardTimerEmpty` to Game State Machine | Death takes priority over win |
| Player reaches shrine + Ward = 0 simultaneously | Lose triggers (Ward = 0 = death); win is ignored | Death is absolute; whale devours you |
| Shrine zone not loaded (Map Director failed) | Win impossible; log error; continue night until Ward = 0 triggers lose | Graceful degradation; player gets alternate outcome |
| Multiple E presses near shrine | Debounce: ignore repeats within 0.5s window | Prevents duplicate `OnShrineReached` events |
| Player dies (Ward = 0) while in shrine zone | Lose triggers; win is ignored even if E was pressed | Death is final; whale gets you |
| E key pressed during wrong phase | Ignored by Player Controller phase gating; no event generated | Consistent with Phase behavioral rules |
| Shrine glow VFX plays, player doesn't press E | Shrine stays active; player can press E anytime while in zone | No timeout; player chooses when to win |
| Win triggers but Game State Machine not in NightSurvival | Log error; don't send event (shouldn't happen) | Defensive programming; validates state integrity |

## Dependencies

| System | Direction | Nature of Dependency |
|--------|-----------|---------------------|
| Game State / Phase State Machine | This depends on it | **Hard**: sends `OnShrineReached` (win) or `OnWardTimerEmpty` (lose) events. Game State Machine transitions to `Resolve` state. |
| Player Controller | This depends on it | **Hard**: receives E key press event near EndShrine. Validates phase and proximity before triggering win. |
| Health/Stamina & Damage Rules | This depends on it | **Hard**: receives `OnWardTimerEmpty` event when Ward Timer = 0. Forwards lose condition to Game State Machine. |
| Map & Spawn Director | This depends on it | **Soft**: reads EndShrine location and trigger zone geometry for win detection. |
| HUD & Diegetic Feedback | Depends on this | **Soft**: subscribes to win/lose state changes for shrine glow VFX, success/failure audio and visual feedback. |
| Curse Effect Modules | Depends on this | **Soft**: win/lose outcome may affect curse resolution in `Resolve` state. |
| Night Survival Run | Depends on this | **Hard**: win/lose conditions are central to night survival loop completion. |

## Tuning Knobs

| Parameter | Current Value | Safe Range | Effect of Increase | Effect of Decrease |
|-----------|---------------|------------|-------------------|-------------------|
| `shrine_trigger_radius` | 3.0 | 2.0-5.0 | Easier to trigger win (forgiving) | Miss E key presses; frustration |
| `shrine_glow_start_distance` | 15.0 | 10.0-30.0 | Player sees shrine earlier; less tension | Player surprised by shrine; anticlimactic |
| `debounce_window_sec` | 0.5 | 0.3-1.0 | Prevents duplicate win events reliably | May miss rapid re-presses |

**Interacting Knobs:**
- `shrine_trigger_radius` + `shrine_glow_start_distance` control player experience near shrine. Smaller radius with earlier glow gives player time to react.
- These knobs interact with Player Controller's E key sensitivity and Map & Spawn Director's shrine placement.

## Visual/Audio Requirements$

| Event | Visual Feedback | Audio Feedback | Priority |
|-------|-----------------|----------------|----------|
| Shrine Approach (within glow distance) | Shrine glow builds; soft light halo | Distant bell chime, low drone builds | High |
| Shrine Enter (trigger zone) | Bright glow bloom; path lights up | Relief stinger; tension drops | Critical |
| Win (E key pressed) | White fade to shrine interior; soft light | Success chime; whale song fades | Critical |
| Lose (Ward Timer = 0) | Rapid darken; camera shakes; swallowed whole | Distorted impact; low drone; whale roar | Critical |
| Shrine Glow Idle | Pulsing soft glow in night | Low ambient hum near shrine | Medium |

## UI Requirements$

| Information | Display Location | Update Frequency | Condition |
|-------------|------------------|------------------|-----------|
| Shrine direction cue (arrow) | Edge of screen (points to shrine) | Real-time during NightSurvival | Night phase, shrine not reached |
| Shrine proximity indicator | Near player reticle (fills as approach) | Real-time when within glow distance | Night phase, near shrine |
| Win/Lose status icon | Center-top (shrine icon) | On win/lose event | Night phase |
| Shrine trigger zone visual | Ground marker at shrine base | Static | Night phase, shrine loaded |

## Acceptance Criteria

- [ ] **Win Detection**: E key pressed within EndShrine trigger zone during `NightSurvival` sends `OnShrineReached` event to Game State Machine.
- [ ] **Win Validation**: E key press during other phases (`DayService`, `ChoiceLock`, `Resolve`, `Reset`) is ignored; no event generated.
- [ ] **Lose Detection**: Ward Timer reaching 0 sends `OnWardTimerEmpty` event to Game State Machine.
- [ ] **Lose Priority**: If Ward Timer = 0 and E key pressed simultaneously, lose triggers (whale devours player); win is ignored.
- [ ] **Shrine Proximity**: `is_near_shrine` returns true when `distance(player, shrine_center) <= shrine_trigger_radius`.
- [ ] **Shrine Glow**: Shrine glow VFX activates when player distance <= `shrine_glow_start_distance`.
- [ ] **Debounce**: Multiple E presses within `debounce_window_sec` window generate only one `OnShrineReached` event.
- [ ] **Phase Gating**: Win/lose checks only active during `NightSurvival`; idle during other phases.
- [ ] **Performance**: Win/lose check processing completes within 1ms average per frame on target PC.
- [ ] **Cross-System Events**: Verify `OnShrineReached` and `OnWardTimerEmpty` delivered reliably to Game State Machine.

## Open Questions

| Question | Owner | Deadline | Resolution |
|----------|-------|----------|------------|
| Should shrine glow start distance be linked to `shrine_trigger_radius`? | UX-Designer | Before vertical slice lock | Open |
| Should win trigger automatically on proximity (no E key)? | Game Designer | Before prototype test #1 | Resolved: E key required (player choice matters) |
| What happens if shrine zone fails to load (Map Director error)? | Systems Designer | Before night implementation | Open |
