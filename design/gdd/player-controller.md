# Player Controller

> **Status**: Approved
> **Author**: User + opencode
> **Last Updated**: 2026-05-06
> **Implements Pillar**: Consequence-driven survival loop with day/night emotional contrast

## Overview

Player Controller is the input and movement system that translates player actions into game responses, enforcing what the player can and cannot do based on the current phase. During Day phase, the player is restricted to UI-only interactions—selecting NPCs, assigning resources (tea/incense/offering), and confirming choices—with no physical movement allowed. During Night phase, the system enables WASD/arrow movement, sprint, and contextual interaction (E key) as the player navigates the lane from start shrine to end shrine, using cover volumes (Mo Thuong, Mo Oan, FalseSafeMound) to avoid boss sweeps and collecting optional Bone Relics. The system exists to make phase transitions feel distinct: daytime is calm and deliberate (menu-driven), nighttime is urgent and physical (movement-driven). Without it, the game's day/night emotional contrast breaks because players could move during day (breaking the "choose who to save" tension) or be unable to traverse at night (breaking survival pressure).

## Player Fantasy

During Day phase, the player should feel **calm and deliberate**—a sense of thoughtful weight as they decide who to save and who to leave behind. Like *Papers, Please* or a strategy game, the UI-only interaction creates distance and reflection; the player is an observer making moral choices, not a physical participant. The fantasy is "burden of leadership"—every click carries consequence, but there's no physical danger yet.

During Night phase, the fantasy shifts dramatically to **panic and tension**—heart-pounding urgency where the player constantly looks over their shoulder. Drawing from *Amnesia: The Dark Descent* and *Outlast*, the player feels physically vulnerable and hunted, with only their reflexes and cover usage standing between survival and failure. The WASD movement and cover mechanics should reinforce "I am being chased and I cannot fight back"—pure survival flight, not combat.

The emotional promise is **ownership through contrast**: daytime calm makes the nighttime panic feel deserved, not random. When the player survives or fails, they should connect it to their earlier choices, not to unfair mechanics.

## Detailed Design

### Core Rules

1. **Phase-Gated Input**: Input processing is determined by current phase from Game State Machine:
   - `DayService`: Only UI clicks (NPC selection, resource assignment, confirm button). WASD/mouse movement is ignored.
   - `NightSurvival`: WASD movement, Shift (sprint), E (contextual interact), mouse-look. UI selection clicks are ignored.
   - `ChoiceLock`, `Resolve`, `Reset`: All input disabled except skip/continue prompts.

2. **Movement (Night Only)**: WASD keys drive horizontal movement relative to camera facing. Player speed is `base_move_speed` (configurable tuning knob).

3. **Sprint**: Holding Shift activates sprint, consuming stamina per second from Health/Stamina system. Sprint speed = `base_move_speed * sprint_multiplier`. Releasing Shift or stamina depletion returns to base speed.

4. **Cover System**: Player collider must be **fully inside** a cover volume (Mo Thuong, Mo Oan, FalseSafeMound) for cover to register. Partial overlap does NOT count as "in cover" for sweep exposure checks.

5. **Contextual Interact (E key)**:
   - Near CursedMound (Mo Oan): Picks up Bone Relic (`Ngoc Cot`), triggering Time Drain modifier and hallucination package.
   - Near Shrine (EndShrine): Triggers shrine arrival event to Game State Machine.
   - Near FalseSafeMound: Warning tell plays; player can choose to hide (risky) or avoid.

6. **Camera**: Mouse-look controls camera rotation (Y-axis only for vertical lane). No manual camera zoom. Camera pans smoothly behind player during Night phase.

7. **No Combat Input**: Player has no attack, block, or counter inputs. Survival is flight-only, matching the "helpless flight" fantasy.

### States and Transitions

| State | Entry Condition | Exit Condition | Behavior |
|-------|-----------------|----------------|----------|
| DayUI | `DayService` phase active | Phase exits `DayService` | Ignore WASD/mouse movement; process only UI clicks on NPCs/resources; show cursor |
| NightMovement | `NightSurvival` phase active | Phase exits `NightSurvival` | Process WASD/mouse-look; enable sprint; enable cover detection; enable E-interact; hide cursor |
| Disabled | `ChoiceLock`, `Resolve`, `Reset` phases | Phase exits to `DayService` or `NightSurvival` | Ignore all input except continue/skip prompts |

### Interactions with Other Systems

- **Game State / Phase State Machine -> Player Controller**: Receives phase change events (`OnDayStart`, `OnNightStart`, `OnChoiceLock`, `OnResolve`, `OnReset`). Controller switches input mode and cursor visibility based on phase.

- **Map & Spawn Director <-> Player Controller**: 
  - Controller sends player position + collider bounds to Map Director for cover validation.
  - Map Director sends `OnEnterCover`, `OnExitCover`, `OnStrikeWarning` events to Controller.
  - Controller displays strike warning icon near reticle when `OnStrikeWarning` received.

- **Health/Stamina & Damage Rules <-> Player Controller**:
  - Controller sends sprint state (active/inactive) to stamina system for drain calculation.
  - Stamina system sends `OnStaminaDepleted` to Controller; Controller forces exit from sprint.
  - Ward Timer events (`OnWardTimerCritical`, `OnWardTimerEmpty`) are received; Controller can trigger death animation/failure.

- **Resource Effects <- Player Controller**: Bone Relic pickup (E key on CursedMound) triggers `OnResourcePickedUp(NgocCot)` event to Resource Effects system for Time Drain activation.

- **HUD & Diegetic Feedback <- Player Controller**: Controller sends current state (phase, sprint active, in cover, strike warning) to HUD for real-time display updates.

## Formulas

### Effective Move Speed

```
effective_speed = base_move_speed * (is_sprinting ? sprint_multiplier : 1.0)
```

| Variable | Type | Range | Source | Description |
|----------|------|-------|--------|-------------|
| base_move_speed | float | 2.0-8.0 | config | Base WASD movement speed |
| is_sprinting | bool | {0,1} | runtime input | Shift key held and stamina > 0 |
| sprint_multiplier | float | 1.5-3.0 | config | Speed multiplier when sprinting |

**Expected output range**: 2.0-24.0 units/sec  
**Edge case**: clamp to max speed cap; if stamina depletes mid-sprint, immediately drop to base speed.

---

### Phase Input Validation

```
input_allowed = (current_phase in allowed_phases[input_type])
```

| Variable | Type | Range | Source | Description |
|----------|------|-------|--------|-------------|
| current_phase | enum | {DayService, ChoiceLock, NightSurvival, Resolve, Reset} | Game State Machine | Active phase |
| allowed_phases | map[input_type] → phase_set | — | config | Maps input types to allowed phases |
| input_type | enum | {UI_Click, WASD_Move, Sprint, Interact} | runtime | Type of input received |

**Expected output range**: boolean  
**Edge case**: if `input_allowed = false`, silently consume input (no error popup) to avoid breaking immersion.

---

*Note: Stamina drain during sprint is calculated by **Health/Stamina & Damage Rules** (Player Controller only sends `is_sprinting` state). Ward Timer penalties from strikes are calculated by **Map & Spawn Director** (Player Controller receives penalty events).*

## Edge Cases

| Scenario | Expected Behavior | Rationale |
|----------|-------------------|-----------|
| Player tries to sprint with 0 stamina | Sprint input ignored; `is_sprinting = false` sent to stamina system; player stays at base speed | Prevents negative stamina; no frustrating "try to run but can't" visual |
| Player is in cover when strike hits | If fully inside valid cover (Mo Thuong), strike damage negated. If partial cover, full penalty applies | Matches Map & Spawn Director cover validity rules |
| Player presses E with no interactable nearby | Silently ignore (no error popup) | Prevents UI clutter; player learns by experimentation |
| Phase changes mid-sprint (Night ends) | Immediately disable WASD/sprint/interact; reset to DayUI state; send `OnSprintEnd` to stamina system | Clean state transition; no orphaned sprint drain |
| Player stuck in geometry at night | If position unchanged for `stuck_timeout_sec` (3 sec) with active input, teleport to nearest valid navmesh point; log warning | Prevents soft-lock without player noticing |
| Multiple strike warnings overlap | Display only nearest/highest-priority warning icon near reticle | Avoids UI clutter during panic moments |
| Bone Relic pickup during strike telegraph | Apply Time Drain immediately; do NOT cancel strike (per Map & Spawn Director rule) | Player chose the risk; consistent timing |
| Stamina depletes mid-sprint | Receive `OnStaminaDepleted` from Health/Stamina; immediately disable sprint, return to base speed | Graceful degradation; player feels consequence, not bug |
| FalseSafeMound collapse while player inside | Player gets knockback + Ward Timer penalty; treat as exposed strike hit | Fair: warning tell was given; player chose to stay |
| Cursor shows during Night phase | Force-hide cursor on `NightSurvival` entry | Maintains immersion; WASD+mouselook only |

## Dependencies

| System | Direction | Nature of Dependency |
|--------|-----------|---------------------|
| Game State / Phase State Machine | This depends on it | **Hard**: receives phase change events (`OnDayStart`, `OnNightStart`, `OnChoiceLock`, `OnResolve`, `OnReset`). Controller switches input mode and cursor visibility based on phase. |
| Map & Spawn Director | This depends on it | **Hard**: receives cover zone overlap events (`OnEnterCover`, `OnExitCover`, `OnStrikeWarning`). Sends player position + collider bounds for cover validation. |
| Health/Stamina & Damage Rules | Bidirectional | **Hard**: sends `is_sprinting` state for stamina drain. Receives `OnStaminaDepleted` (exit sprint) and Ward Timer events (`OnWardTimerCritical`, `OnWardTimerEmpty`) for death/failure. |
| Resource Effects | This feeds it | **Soft**: sends `OnResourcePickedUp(NgocCot)` event when player picks up Bone Relic from CursedMound. |
| Shrine Objective & Win/Lose Rules | Depends on this | **Hard**: receives player shrine arrival event (E key near EndShrine). Determines win condition. |
| Curse Effect Modules | Depends on this | **Hard**: reads player position, sprint state, and cover state to apply curse effects (hallucinations, speed modifiers). |
| Boss Chase AI | Depends on this | **Hard**: reads player position for searchlight targeting and chase behavior. |
| Night Survival Run | Depends on this | **Hard**: player movement and survival mechanics are central to night survival loop. |
| HUD & Diegetic Feedback | Depends on this | **Soft**: subscribes to player state changes (phase, sprint active, in cover, strike warning) for real-time HUD updates. |

## Tuning Knobs

| Parameter | Current Value | Safe Range | Effect of Increase | Effect of Decrease |
|-----------|---------------|------------|-------------------|-------------------|
| `base_move_speed` | 5.0 | 2.0-8.0 | Player outruns boss easily; loses tension | Frustrating; can't reach shrine in time |
| `sprint_multiplier` | 1.8 | 1.5-3.0 | Trivializes night survival; stamina burns faster | Sprint feels useless; stamina waste |
| `stuck_timeout_sec` | 3.0 | 2.0-5.0 | Player stays stuck too long; frustration | Premature teleport; feels glitchy |
| `cover_enter_threshold` | 1.0 | 0.8-1.0 | Easier to claim cover; less precise movement needed | Frustrating; even "almost covered" fails |

**Interacting Knobs:**
- `base_move_speed` + `sprint_multiplier` together determine max speed. Increasing both makes night too easy.
- `sprint_multiplier` interacts with Health/Stamina's `stamina_drain_per_sec` — higher multiplier = more stamina burned per second at max speed.

## Visual/Audio Requirements

| Event | Visual Feedback | Audio Feedback | Priority |
|-------|-----------------|----------------|----------|
| Enter DayService | Cursor appears; UI panels fade in; warm color grade | Calm ambient bed fades in | High |
| Enter NightSurvival | Cursor hides; cold color grade; vignette increase | Threat motif + louder wind/wave layer | Critical |
| Sprint Active | Slight FOV increase; motion blur; dust particles | Heavy breathing; footstep frequency increases | Medium |
| Enter Cover (Mo Thuong) | Player model crouches/anim; cover highlight glow | Soft "whisper" bed; muffled exterior sounds | High |
| Exit Cover | Player stands; cover glow fades | Exterior sounds return to full volume | High |
| Strike Warning | Ground warning shimmer near player; reticle warning icon | Rising crack sound; heartbeat intensifies | Critical |
| Strike Hit | Screen shake; red flash; knockback animation | Loud impact hit; distortion layer | Critical |
| Bone Relic Pickup | Vignette tighten + grain surge; green tint | Breath/crack/chant layer | High |
| Shrine Arrival (Success) | Shrine glow bloom; soft fade to white | Relief stinger; tension drop | Medium |
| Death/Failure | Rapid darken; camera fade to black | Distorted hit + low drone | Medium |

## UI Requirements

| Information | Display Location | Update Frequency | Condition |
|-------------|------------------|------------------|-----------|
| Current phase (Day/Night icon) | Top-center | On phase change | Always |
| Sprint active indicator | Near player (stamina bar) | Real-time | Night phase, sprint active |
| In cover indicator | Near player reticle | Real-time (enter/exit) | Night phase, near cover volume |
| Strike warning icon | Near reticle | On `OnStrikeWarning` event | Night phase, exposed in sweep |
| Stamina bar | Bottom-center HUD | Real-time | Night phase, sprint available |
| Bone Relic carried indicator | Top-right inventory slot | On pickup/drop | Night phase, carrying relic |
| Shrine direction cue | Edge of screen (arrow) | Real-time | Night phase, shrine not reached |

## Acceptance Criteria

- [ ] **Phase Gating**: During `DayService`, WASD/mouse movement is ignored; only UI clicks processed. Cursor is visible.
- [ ] **Phase Gating**: During `NightSurvival`, WASD/mouse-look works; UI clicks on NPCs ignored. Cursor is hidden.
- [ ] **Phase Gating**: During `ChoiceLock`, `Resolve`, `Reset`, all input ignored except skip/continue prompts.
- [ ] **Cover Validation**: Player collider fully inside Mo Thuong = "in cover" (sweep exposure check passes). Partial overlap = exposed.
- [ ] **Sprint Integration**: Holding Shift activates sprint, sends `is_sprinting = true` to Health/Stamina. Stamina drains per second.
- [ ] **Sprint Integration**: Releasing Shift or stamina = 0 sends `is_sprinting = false`. Speed returns to base.
- [ ] **E-Interact (Bone Relic)**: Near CursedMound, E key picks up Bone Relic, triggers `OnResourcePickedUp(NgocCot)` event to Resource Effects system.
- [ ] **E-Interact (Shrine)**: Near EndShrine, E key triggers shrine arrival event to Game State Machine.
- [ ] **Strike Warning**: On receiving `OnStrikeWarning` from Map Director, display warning icon near reticle. **Critical**: Warning icon must have z-order priority above Tier 4 panic effects (chromatic aberration) per Health/Stamina GDD. If 2D screen-space, use highest layer; if 3D world-space, ensure not obscured by post-processing.
- [ ] **Phase Transition Clean**: On `NightSurvival` exit, sprint disabled, E-interact disabled, cursor shown. No orphaned states.
- [ ] **Performance**: Input processing completes within 1ms average per frame on target PC.
- [ ] **Cross-System Events**: Verify events sent to Map Director (position, cover state), Health/Stamina (sprint state), Resource Effects (relic pickup), HUD (state changes) are delivered reliably.

## Open Questions

| Question | Owner | Deadline | Resolution |
|----------|-------|----------|------------|
| Should sprint have a cooldown after stamina depletion? | Systems Designer | Before prototype test #1 | Open |
| Should cover entry/exit have animation locks (can't sprint immediately after entering cover)? | Game Designer | Before vertical slice lock | Open |
| Should strike warning icon be 3D world-space or 2D screen-space? | UX-Designer | Before UI implementation | Open |
| Should Bone Relic pickup be automatic on entry or require E press? | Game Designer | Before vertical slice lock | Resolved: E press (player choice matters) |
