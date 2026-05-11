# Consequence Resolver$

> **Status**: Approved$
> **Author**: User + opencode$
> **Last Updated**: 2026-05-06$
> **Implements Pillar**: Consequence-driven survival loop with day/night emotional contrast$

## Overview$

Consequence Resolver is the moral arithmetic of Solar Phobia's night phase—it reads the abandoned soul from Day Service's selection payload and translates that choice into a physical curse that hunts the player. The system owns the curse mapping logic, the NightOutcomeState write, and the deterministic assignment that ensures "the person you left behind becomes your nightmare." During `ChoiceLock`, it receives the `SelectionConfirmed(payload)` from Game State Machine, extracts the `abandoned_soul_id`, and writes one of {`Drag`, `Block`, `FakeShrine`} to `NightOutcomeState`. Without this system, the game's "consequence-driven" identity collapses—there's no link between who you abandoned and what hunts you at night.

## Player Fantasy$

The player should feel **"this is my fault"** — the curse that hunts them at night is a direct manifestation of the person they abandoned. There's no randomness here; the water trap, blood net, or collapsing platform exists *because* you left that specific soul behind. The fantasy is **ownership of horror** — "I made this nightmare, and now I must survive it."

Each curse should feel distinct and personal:
- **Linh (Water Trap)**: Should feel like drowning in guilt — constant, inescapable drain.
- **Van (Blood Net)**: Should feel like being hunted and restrained — sudden, sticky, suffocating.
- **Minh (Illusion)**: Should feel deceptive and terrifying — the ground itself betrays you.

The emotional promise: **"The person you left behind is now your nightmare."** The curse isn't a generic hazard; it's *that specific soul* punishing your choice.

## Detailed Design$

### Core Rules$

1. **Trigger**: Consequence Resolver is invoked by Game State Machine during `ChoiceLock` phase when `SelectionConfirmed(payload)` is received.
2. **Prerequisite**: Consequence Resolver only executes AFTER Map & Spawn Director validates spawn bundle. If Map validation fails, Consequence Resolver is NOT invoked (state remains in ChoiceLock, transitions to FatalError).
3. **Payload Read**: Extract `abandoned_soul_id` from the payload. This is the soul the player left behind.
4. **Curse Mapping** (deterministic):
   - `Linh` (abandoned) → `NightOutcomeState = Drag` (Water Trap — Nước Dâng).
   - `Van` (abandoned) → `NightOutcomeState = Block` (Blood Net — Lưới Máu).
   - `Minh` (abandoned) → `NightOutcomeState = FakeShrine` (Illusion — Ảo Ảnh).
5. **Model Write**: Write `NightOutcomeState` to NPC/Soul Data Model for the abandoned soul only.
6. **Curse Payload**: Send curse type, intensity, and location bias to:
   - Map & Spawn Director (spawn bias for night hazards).
   - Curse Effect Modules (visual/audio effects for night).
7. **One-Write Rule**: Consequence Resolver writes `NightOutcomeState` exactly once per run. Contradictory writes are rejected.

### States and Transitions$

| State | Entry Condition | Exit Condition | Behavior |
|-------|-----------------|----------------|----------|
| Idle | `ChoiceLock` phase + payload received | Abandoned soul mapped | Read payload; compute curse mapping using Master GDD contract |
| Writing | Curse mapped | `NightOutcomeState` written | Write outcome to NPC Model; prepare curse payload |
| Done | `NightOutcomeState` written | Payload sent to downstream systems | Send curse type to Map Director, Curse Modules |

### Interactions with Other Systems$

- **Game State / Phase State Machine -> Consequence Resolver**: Sends `SelectionConfirmed(payload)` with `abandoned_soul_id`. Triggers resolver during `ChoiceLock`.

- **NPC/Soul Data Model <-> Consequence Resolver**: 
  - Reads `SoulId`, `DaySelectionState` for the abandoned soul.
  - Writes `NightOutcomeState` = `Drag`/`Block`/`FakeShrine` for abandoned soul only.
  - Validates one-write rule (rejects duplicate writes).

- **Map & Spawn Director <- Consequence Resolver**: Receives curse type and intensity for night spawn bias:
  - `Drag` (Linh) → Mo Oan (CursedMound) placement, Water Trap zones.
  - `Block` (Van) → Lưới Máu (Blood Net) hazards, reduced visibility. 
  - `FakeShrine` (Minh) → FalseSafeMound (Ảo Ảnh) hazards, illusory platforms. 

- **Curse Effect Modules <- Consequence Resolver**: Receives curse type for night visual/audio effects:
  - `Drag` → Water sound layers, blue tint, drowning VFX. 
  - `Block` → Blood net VFX, red tint, restraint sounds. 
  - `FakeShrine` → Illusion VFX, ground collapse, deceptive platforms. 

## Formulas$

Consequence Resolver uses deterministic lookup (no mathematical formulas). The core mapping is:

| Abandoned Soul | NightOutcomeState | Curse Type | Intensity | Downstream Effects |
|-------------|-------------------|------------|-----------|-------------------|
| `Linh` | `Drag` | Water Trap (Nước Dâng) | 1.0 (constant) | Map: Mo Oan (CursedMound) placement, Water Trap zones; Curse: blue tint, drowning VFX |
| `Van` | `Block` | Blood Net (Lưới Máu) | 1.0 (constant) | Map: Lưới Máu (Blood Net) hazards, reduced visibility; Curse: red tint, restraint sounds |
| `Minh` | `FakeShrine` | Illusion (Ảo Ảnh) | 1.0 (constant) | Map: FalseSafeMound (Ảo Ảnh) hazards, illusory platforms; Curse: ground collapse, deceptive VFX |

### Payload Structure
Sent to downstream systems (`Map & Spawn Director`, `Curse Effect Modules`):
```json
{
  "curse_type": "Drag" | "Block" | "FakeShrine",
  "intensity": 1.0,
  "spawn_bias": { "abandoned_soul_id": "Linh" | "Van" | "Minh" }
```

**Expected output**: Curse type string + intensity float + spawn bias object.  
**Edge case**: If abandoned soul ID not in {Linh, Van, Minh}, emit `InvalidSoulId` error and default to `Drag`.

*Provisional: Downstream GDDs (Curse Effect Modules, Night Survival Run) are undesigned; payload structure may evolve.*

## Edge Cases$

| Scenario | Expected Behavior | Rationale |
|----------|-------------------|-----------|
| Invalid `abandoned_soul_id` (not in {Linh, Van, Minh}) | Reject write, emit `InvalidSoulId` error, set NightOutcomeState = `Drag` (default) | Prevents undefined curse mapping |
| Duplicate write attempt (same run) | Reject write, emit `DuplicateWrite` error, keep initial valid state | Enforces one-write rule |
| Day Service sends malformed payload (missing soul_id) | Reject, emit `InvalidPayload` error, enter `FatalError` in Game State | Preserves contract integrity |
| Downstream system (Map/Curse) not ready | Send payload anyway; downstream ignores if not in NightSurvival phase | Decouples timing, but may need retry later |
| Consequence Resolver fails to compute (bug) | Return default `Drag`, emit `ResolverFailed` warning | Graceful degradation, ensures night can start |
| All souls saved (0 abandoned) — impossible per Day Service rule | If occurs (bug), default to `Drag` | Fallback for invalid state |

## Dependencies$

### Upstream (This depends on):
| System | Direction | Nature of Dependency |
|--------|-----------|---------------------|
| Day Service & Selection | This depends on it | **Hard**: receives `SelectionConfirmed(payload)` with `abandoned_soul_id`. Cannot map without this. |
| NPC/Soul Data Model | This depends on it | **Hard**: reads `SoulId`, `DaySelectionState` for abandoned soul. Writes `NightOutcomeState`. |
| Game State / Phase State Machine | This depends on it | **Hard**: invoked on `ChoiceLock`; must return curse payload before Night. |

### Downstream (Depends on this):
| System | Direction | Nature of Dependency |
|--------|-----------|---------------------|
| Curse Effect Modules | Depends on this | **Soft**: receives curse type from payload for visual/audio effects. |
| Map & Spawn Director | Depends on this | **Soft**: receives curse type + bias for night hazard placement. |
| Boss Cá Ông Searchlight | Depends on this | **Soft**: reads curse type for AI behavior (e.g., Lưới Máu affects searchlight). |
| Night Survival Run | Depends on this | **Hard**: curse type defines core hazards the player must survive. |
| Ngọc Cốt / Relic System | Depends on this | **Soft**: curse type may affect relic spawn bias. |

### Interface ownership:
- **Consequence Resolver owns** curse mapping logic, payload structure, one-write rule.
- **Day Service owns** `abandoned_soul_id` assignment.
- **NPC Model owns** `NightOutcomeState` write.

## Tuning Knobs$

No tunable parameters for vertical slice — curse mapping is fully deterministic. Future expansion may add:

| Parameter | Current Value | Safe Range | Effect of Increase | Effect of Decrease |
|-----------|---------------|------------|-------------------|-------------------|
| `curse_intensity_multiplier` | 1.0 | 0.5-3.0 | Stronger curse effects (harder night) | Weaker curses (easier night) |
| `default_cursetype` | `Drag` | {Drag, Block, FakeShrine} | Changes fallback for invalid soul ID | N/A |

**Interacting Knobs:**
- `curse_intensity_multiplier` multiplies base intensity sent to Curse Effect Modules/Map Director.
- These knobs are provisional — downstream GDDs (Curse Effect Modules) are not designed yet.

## Visual/Audio Requirements$

| Event | Visual Feedback | Audio Feedback | Priority |
|-------|-----------------|----------------|----------|
| Curse Assigned (Linh → Drag) | Blue tint, water ripple VFX, drowning overlay | Water sound layers, bubbling, distant drowning | High |
| Curse Assigned (Van → Block) | Red tint, blood net overlay, restraint VFX | Blood net tension sounds, rattling chains | High |
| Curse Assigned (Minh → FakeShrine) | Illusion shimmer, ground crack VFX, deceptive platforms | Ground collapse rumble, deceptive whisper | High |
| Payload Sent to Map/Curse | Confirmation ping, curse icon appears | Short tension stinger | Medium |

## UI Requirements$

| Information | Display Location | Update Frequency | Condition |
|-------------|------------------|------------------|-----------|
| Curse type preview (optional) | Decision panel footer | On lock commit | ChoiceLock |
| Curse icon (Linh/Van/Minh) | HUD top-right | On curse assignment | NightSurvival |
| No direct UI — system runs in background | N/A | N/A | Always |

## Acceptance Criteria$

- [ ] **Curse Mapping**: Deterministic lookup Linh→Drag, Van→Block, Minh→FakeShrine. No randomness.
- [ ] **One-Write Rule**: Writes `NightOutcomeState` exactly once per run. Contradictory writes rejected.
- [ ] **Invalid Soul ID**: Rejects writes for soul IDs not in {Linh, Van, Minh}, emits `InvalidSoulId` error.
- [ ] **Duplicate Write**: Rejects second write attempt, emits `DuplicateWrite` error.
- [ ] **Payload Delivery**: Sends curse type, intensity=1.0, spawn bias to Map Director + Curse Effect Modules.
- [ ] **Performance**: curse mapping completes within 0.1ms average on target PC.
- [ ] **Cross-System Events**: Verified that Map Director and Curse Effect Modules receive payload reliably.

## Open Questions$

| Question | Owner | Deadline | Resolution |
|----------|-------|----------|------------|
| Should curse intensity scale with game progress (chapter/act)? | Game Designer | Before post-vertical slice | Open |
| Should default curse for invalid soul ID be configurable (currently Drag)? | Systems Designer | Before MVP lock | Open |
| How should payload structure evolve when Curse Effect Modules GDD is written? | Systems Designer | Before Feature layer design | Open |
| Should curse assignment be visible to player before night starts? | UX-Designer | Before first playtest | Open |