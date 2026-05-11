# Game State / Phase State Machine

> **Status**: Approved
> **Author**: User + Copilot + expanded design
> **Last Updated**: 2026-05-07
> **Implements Pillar**: Consequence-driven survival loop with day/night emotional contrast

## Overview

Game State / Phase State Machine is the authoritative runtime controller for Solar Phobia's core loop. It transitions the run through Day Service, Choice Lock, Night Survival, and Resolve/Reset phases, and enforces which systems are active in each phase so rules cannot overlap incorrectly.

**Theme**: "Opposites Attract" — The static, tactile deliberation of Day (hy vọng giả tạo) versus the frantic, physical panic of Night (tốc độ cuồng nộ). The game tests "Hopeless Hope" — the unbearable tension between wanting to save everyone and the mathematical impossibility of doing so.

**Lore Foundation**: The player is Tú — a coward who discovered the sacred Cá Ông (Whale) carcass during a storm but fled instead of fulfilling the "Con trưởng" (Funeral Master) duty. The Mặt Trời Rỗng (Hollow Sun) — a karma radiation entity with a hollow eye socket — is the judge of this sin. The Cõi Trung Ấm (Bardo/Intermediate realm) is a twisted recreation of Lý Sơn island where Tú must relive this guilt forever.

This system exists to guarantee that player decisions in daytime are committed before nighttime begins, making consequence timing immediate and reliable. Without it, the game's consequence-driven identity breaks.

## Player Fantasy

**Day Phase Fantasy**: Calm deliberation with tactile discomfort. Like *Papers, Please*, the player is an observer making moral choices via UI — selecting souls, assigning rituals — with no physical movement. The fantasy is "burden of leadership" — every click carries weight, but there's no physical danger yet.

**Night Phase Fantasy**: Panic and tension. Drawing from *Amnesia: The Dark Descent* and *Outlast*, the player feels physically vulnerable and hunted. WASD movement, sprint, and cover mechanics reinforce "I am being chased and cannot fight back" — pure survival flight.

**Emotional Promise**: "Your hands are stained with the choice." Daytime calm makes nighttime panic feel earned, not random. When the player survives or fails, they connect it to their earlier choices, not unfair mechanics.

## Detailed Design

### Core Rules

**PHASE 1: DAY SERVICE (5-minute structured timeline)**

1. A run starts in `DayService`.
2. During `DayService`, only daytime actions are legal: inspect souls, assign tea/incense/offering, confirm save choices.
3. The Day Phase follows a **5-minute structured timeline** with escalating pressure:
   - **0:00–1:30 (Stability)**: Space for 4 people (Tú + 3 souls). Tactile minigames (diêm, rot, vay) to earn Hương Hỏa (Spirit Essence).
   - **1:30–3:00 (Tension)**: Shadows shrink 30%. Light Interrupts appear randomly. Souls enter Panic AI state, blocking paths.
   - **3:00–4:30 (Crisis)**: Only space for 3 people. Player must frantically Swap positions.
   - **4:30–5:00 (Collapse)**: One soul MUST be abandoned. Shove mechanic forces the decision.
4. **Swap Mechanic**: Player cannot jump over souls. To move a soul from edge to safety, player must approach and physically swap positions (0.5s animation).
5. **Shove Mechanic**: At phase end, player MUST push one soul out of the shadow. Screen shake, audio impact, soul burns with scream. This writes `sacrificed_ghost_id`.
6. Transition to `ChoiceLock` occurs only when exactly 2 souls are marked saved (or configured minimum) and the player confirms, OR when timer expires with auto-commit policy.
7. `ChoiceLock` is atomic: unsaved soul is finalized; Consequence Resolver computes curse payload for night.

**PHASE 2: NIGHT SURVIVAL**

8. Transition to `NightSurvival` only after curse payload and night spawn data are valid.
9. During `NightSurvival`, daytime selection UI/actions are disabled; survival systems are enabled:
   - WASD movement + CharacterController
   - Sprint (Shift), Spirit Dash (-5s Ward), Swing (-2s), Glide (-1s/s)
   - Cover detection (fully inside Mound collider)
   - Boss Cá Ông Searchlight sweep pattern
   - Karma-based hazards based on `sacrificed_ghost_id`:
     - **Ông Văn abandoned** → Lưới Máu (Blood Net): -5s + 50% slow for 3s
     - **Em Linh abandoned** → Vũng Nước Tử Thần: -3s/s DoT
     - **Anh Minh abandoned** → Bệ Đá Ảo Ảnh: collapses 0.2s after stepped on
   - Ngọc Cốt (Bone Relic) pickups increase Ward drain multiplicatively
10. **Boss Searchlight**: Cá Ông skeleton in background sweeps a cone. If player exposed (not in valid cover) during sweep, strike telegraph triggers then applies -30s penalty.
11. Transition to `Resolve` occurs on shrine reached (success) or Ward timer = 0 (failure).
12. `Resolve` records outcome metrics, then transitions to `Reset`.
13. `Reset` clears run-scoped state and returns to `DayService` for next loop.

**PHASE STATE ENFORCEMENT**

14. Any subsystem update request checks current phase contract; out-of-phase requests are rejected with reason codes.
15. `AutoCommitPolicy = CurrentValidElseDefault` is deterministic: (a) keep current valid selection; else (b) fill missing slots from fixed priority list `Linh -> Van -> Minh`; if still invalid, transition to `FatalError`.
16. **Shadow Freeze on Auto-Complete**: When auto-commit timeout fires during DayService, the shadow polygon FREEZES IMMEDIATELY before applying the auto-selected soul assignment. This prevents race condition where auto-push (thermal death) and auto-complete fire simultaneously causing contradictory soul states (Saved/Abandoned + Burned).
17. In release builds, `FatalError` attempts safe fallback: emit failure snapshot, transition to `Resolve`, then `Reset`.

### States and Transitions

| State | Entry Condition | Exit Condition | Behavior |
|-------|-----------------|----------------|----------|
| Bootstrapping | Scene loaded | Core references valid | Initializes state machine, then enters DayService |
| DayService | Run start or Reset complete | Player confirms valid 2-of-3 selection | Enables service/selection flow only |
| ChoiceLock | DayService exit | Consequence payload + night setup ready | Commits abandoned soul, computes curse |
| NightSurvival | ChoiceLock exit | Shrine reached OR player dead | Enables hazards/chase/objective loop |
| Resolve | NightSurvival exit | Outcome persisted | Generates result snapshot |
| Reset | Resolve exit | Runtime reset finished | Clears transient state, loops to DayService |
| FatalError | Invalid critical contract | Dev: manual recovery; Release: fallback to Resolve -> Reset | Freezes progression in dev, preserves loop recoverability in release |

### Interactions with Other Systems

- **Day Service & Selection -> State Machine**: submits selection payload; receives allowed-action mask.
- **Consequence Resolver <- State Machine**: triggered on `ChoiceLock`; returns curse payload contract.
- **Night Survival Run <- State Machine**: receives start signal + payload bundle.
- **Boss Chase AI / Solar Hazard / Resource Effects <- State Machine**: phase gating + lifecycle events (`OnNightStart`, `OnNightEnd`).
- **HUD/Audio <- State Machine**: consume phase change events for UI mode and mix snapshots.
- **Save/Reset <- State Machine**: receives deterministic outcome snapshot and reset command.

## Formulas

### Phase Duration Budget

```
phase_duration = base_phase_duration * pacing_multiplier
```

| Variable | Type | Range | Source | Description |
|----------|------|-------|--------|-------------|
| base_phase_duration | float sec | 60-240 | config | Default duration per phase |
| pacing_multiplier | float | 0.5-1.5 | config/run mode | Global pacing scale |

**Expected output range**: 30-360 sec
**Edge case**: clamp output to [30, 360] sec.

### Night Start Eligibility

```
night_start_ready = has_valid_selection AND consequence_payload_ready AND spawn_bundle_ready AND initial_ward_sec > 0
```

| Variable | Type | Range | Source | Description |
|----------|------|-------|--------|-------------|
| has_valid_selection | bool | {0,1} | Day Service | Exactly required saves selected |
| consequence_payload_ready | bool | {0,1} | Consequence Resolver | Curse data compiled |
| spawn_bundle_ready | bool | {0,1} | Map/Spawn Director | Night spawn points validated |
| initial_ward_sec | float sec | 0-600 | Health/Stamina & Damage Rules | Initial Ward Timer value from Day→Night translation |

**Expected output range**: boolean
**Edge case**: if false after timeout, enter FatalError.

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

### Run Outcome Score (telemetry only)

```
outcome_score = shrine_reached*100 - death_penalty - night_time_seconds*0.2
```

| Variable | Type | Range | Source | Description |
|----------|------|-------|--------|-------------|
| shrine_reached | int | 0-1 | Objective system | Success marker |
| death_penalty | float | 0-100 | config | Penalty on failure |
| night_time_seconds | float sec | 0-600 | runtime | Survival duration |

**Expected output range**: -20 to 100
**Edge case**: clamp to [-50, 100] for analytics stability.

### Sensory Readability Tiers (HUD-less Design)

The game uses a HUD-less design philosophy. Instead of watching numbers, players **feel** death approaching through systematic sensory degradation:

| Tier | Ward % | Name | Visual Feedback | Audio Feedback | Mechanical Penalty |
|------|--------|------|----------------|----------------|-------------------|
| 1 | >75% | **Stable** | Clear, sharp image | BGM + wave sounds strong | None |
| 2 | ≤75% | **Creeping Dread** | Vignette (α 0.3) | Lowpass filter on ambient | None |
| 3 | ≤50% | **Heavy Burden** | Heavy breathing + vignette | BPM increases | Dash cooldown +0.1s |
| 4 | ≤25% | **Panic** | Chromatic aberration + controller rumble | Whispering audio layers | None (psychological) |
| 5 | ≤10s | **Death Spiral** | Tunnel vision (80% obscured) + tinnitus | Tinnitus + distorted hit sounds | None (inevitable) |

This creates "Hopeless Hope" — the player knows death is coming but fights anyway.

## Edge Cases

| Scenario | Expected Behavior | Rationale |
|----------|-------------------|-----------|
| Player confirms with < required saved souls | Stay in DayService; show blocking reason `InvalidSelectionCount` | Prevents ambiguous consequence mapping |
| Consequence payload generation fails in ChoiceLock | Retry once; if still invalid -> FatalError | Night cannot start without deterministic curse |
| Spawn bundle validation fails in ChoiceLock | Retry x3; if still invalid -> FatalError | Night cannot start without valid spawn data |
| Two transition triggers fire same frame (e.g., shrine + death) | Resolve by priority: Death > ShrineReached; single transition only | Avoids dual outcome corruption |
| Timer expires during DayService without confirmation | Auto-commit current valid selection; if invalid, choose deterministic default policy | Keeps loop cadence and avoids deadlock |
| Night systems report active during DayService | Force-disable and emit contract violation event | Preserves phase integrity |
| Reset interrupted by missing references | In dev: enter FatalError; in release: force failure Resolve snapshot then Reset | Avoids partial-reset ghost state while preserving recoverability |
| Shadow auto-push fires simultaneously with auto-complete timeout | **Shadow polygon FREEZES IMMEDIATELY** on auto-complete timeout, THEN apply selection | Prevents race condition where auto-push (thermal death) and auto-complete fire simultaneously causing contradictory soul states (Saved/Abandoned + Burned) |

## Dependencies

| System | Direction | Nature of Dependency |
|--------|-----------|---------------------|
| Day Service & Selection | Depends on this | **Hard**: receives phase allowlist; emits `SelectionConfirmed(payload)` |
| Consequence Resolver | Depends on this | **Hard**: invoked on `ChoiceLock`; must return `CursePayload` before Night |
| Map & Spawn Director | This depends on it | **Hard**: provides validated spawn bundle for night start |
| Health/Stamina & Damage Rules | Depends on this | **Hard**: consumes Ward values for phase gating and night penalties |
| NPC/Soul Data Model | Depends on this | **Hard**: provides soul data needed for night spawn logic |
| Player Controller | Depends on this | **Hard**: movement/input mode changes by phase |
| Shrine Objective & Win/Lose Rules | Depends on this | **Hard**: transition triggers `Resolve` conditions |
| Solar Residue Hazard | Depends on this | **Hard**: enabled only in NightSurvival |
| Boss Chase AI | Depends on this | **Hard**: chase lifecycle controlled by phase events |
| Resource Effects | Depends on this | **Soft**: can precompute in day; consumed in night under phase gates |
| HUD & Diegetic Feedback | Depends on this | **Soft**: subscribes to phase-change events for presentation mode |
| Audio State Director | Depends on this | **Soft**: maps phase state to mix snapshot transitions |
| Save Seed / Run Reset | Depends on this | **Hard**: needs deterministic run result snapshot + reset command |

## Tuning Knobs

| Parameter | Current Value | Safe Range | Effect of Increase | Effect of Decrease |
|-----------|---------------|------------|-------------------|-------------------|
| DayDurationSec | 180 | 60-300 | More planning time, lower panic cadence | Faster loop, higher pressure |
| ChoiceLockTimeoutSec | 5 | 1-15 | More forgiveness before commit | Snappier but harsher commits |
| NightDurationCapSec | 360 | 120-480 | Longer survival windows | Tighter runs |
| PacingMultiplier | 1.0 | 0.5-1.5 | Slower overall tempo | Faster tempo |
| AutoCommitPolicy | `CurrentValidElseDefault` | enum | More deterministic fallback behavior | More player-dependent outcomes |
| TransitionDebounceMs | 100 | 0-300 | Fewer duplicate transitions | Risk of double-trigger |
| FatalErrorRetryCount | 1 | 0-3 | More resilience to transient failures | Faster fail-fast |
| PhaseEventDispatchMode | `OrderedReliable` | enum | Safer cross-system sync | Potentially lower throughput |

## Visual/Audio Requirements

| Event | Visual Feedback | Audio Feedback | Priority |
|-------|-----------------|----------------|----------|
| Enter DayService | Warm LUT + cursor visible | Calm ambient bed | High |
| Enter ChoiceLock | Brief desaturation pulse + lock icon | Low drum hit + tension rise | High |
| Day → Night Transition | Camera zoom-out abrupt, cold grade swap, vignette increase | Threat motif + wind/wave layer swells | Critical |
| Enter NightSurvival | Cursor hides, cold grade + motion blur | Boss theme + louder threat audio | Critical |
| Sprint Active | FOV increase + motion blur + dust particles | Heavy breathing + footstep frequency | Medium |
| Enter Cover | Player crouches, cover highlight glow | Muffled exterior sounds | High |
| Exit Cover | Player stands, glow fades | Exterior sounds return | High |
| Strike Warning | Ground shimmer + warning icon near reticle | Rising crack sound | Critical |
| Strike Hit | Screen shake + red flash + knockback | Loud impact + distortion | Critical |
| Bone Relic Pickup | Vignette tighten + chromatic aberration | Breath/crack/chant layer | High |
| Ward Tier Changes | Progressive sensory degradation | Layered audio changes per tier | Critical |
| Enter Resolve (Success) | Shrine glow bloom + soft fade to white | Relief stinger | Medium |
| Enter Resolve (Failure) | Rapid darken + camera shake settle | Distorted hit + low drone | Medium |
| Enter FatalError | Red debug overlay (dev build) | Error beep only (dev) | Low |

## UI Requirements

| Information | Display Location | Update Frequency | Condition |
|-------------|------------------|------------------|-----------|
| Current phase name/icon | Top-center | On phase change | Always |
| Allowed action hints | Context panel | On mode change | DayService/ChoiceLock |
| Choice validity state | Decision panel | Real-time | DayService |
| Night objective status (Reach Shrine) | Top-right | Real-time | NightSurvival |
| Failure reason code (if blocked) | Toast/log panel | On rejection event | Any |

## Acceptance Criteria

### Phase Transitions
- [ ] Run always enters `DayService` after boot and after each reset.
- [ ] Day → ChoiceLock transition is blocked unless required selection count is valid.
- [ ] ChoiceLock commits exactly one abandoned soul and triggers Consequence Resolver once.
- [ ] NightSurvival cannot start unless `night_start_ready == true`.
- [ ] Out-of-phase actions are rejected with explicit reason code (no silent acceptance).
- [ ] Night exit resolves to exactly one outcome (success or failure), never both.
- [ ] Resolve produces deterministic snapshot consumed by Reset.
- [ ] Reset clears run-scoped transient state and returns to DayService without stale flags.

### Day Phase Mechanics
- [ ] 5-minute timeline with escalating pressure phases (Stability → Tension → Crisis → Collapse).
- [ ] Swap mechanic allows repositioning souls (0.5s animation, no jump-over).
- [ ] Shove mechanic forces one soul abandonment at phase end.
- [ ] `sacrificed_ghost_id` is written and persisted to night phase.

### Night Phase Mechanics
- [ ] WASD movement with CharacterController works in NightSurvival.
- [ ] Sprint (Shift), Spirit Dash (-5s), Swing (-2s), Glide (-1s/s) functional.
- [ ] Cover detection requires full collider inside Mound collider.
- [ ] Boss Searchlight sweep pattern scans lane; exposed players trigger telegraph.
- [ ] Karma hazards spawn based on `sacrificed_ghost_id`:
  - Van → Lưới Máu (slow penalty)
  - Linh → Vũng Nước (DoT)
  - Minh → Bệ Đá Ảo Ảnh (0.2s collapse)
- [ ] Ngọc Cốt pickups increase Ward drain multiplicatively.

### Survival System
- [ ] Ward Timer initializes: Base (10s) + (GhostsSaved × 30s) - DayPenalties.
- [ ] Passive drain: 1.0/s base + (bones × hallucination_multiplier).
- [ ] Sensory tiers trigger at 75%, 50%, 25%, ≤10s thresholds.
- [ ] Ward = 0 triggers death/Resolve failure.

### Performance
- [ ] Phase change events delivered in order to subscribed systems.
- [ ] Phase transition processing completes within 0.5 ms average on target PC.
- [ ] Pacing validation: median Day phase 120-220 sec, median Night phase 180-360 sec in test runs.

## Open Questions

| Question | Owner | Deadline | Resolution |
|----------|-------|----------|------------|
| Should DayService use hard timer in first playable or manual confirm only? | Game Designer | Before prototype test #1 | Open |
| Should Death-priority outcome policy be configurable by mode (default remains Death > ShrineReached)? | Systems Designer | Before content-complete milestone | Open |
| Should the fallback soul priority order (`Linh -> Van -> Minh`) be data-driven per chapter/act? | Game Designer | Before content expansion beyond vertical slice | Open |
