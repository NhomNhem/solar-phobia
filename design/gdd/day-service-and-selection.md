# Day Service & Selection

> **Status**: Approved
> **Author**: User + opencode
> **Last Updated**: 2026-05-06
> **Implements Pillar**: Consequence-driven survival loop with day/night emotional contrast

## Overview$

Day Service & Selection is the moral core of Solar Phobia's day phase—the moment where the player must choose which 2 of 3 souls to save and which one to abandon to the sun. The system owns the selection UI, validation logic, and confirmation flow that locks the player's choice before night begins. During `DayService`, the player interacts with three soul cards (Linh, Van, Minh) via tactile rituals (diem thap nhang, rot chao thuc, vay nuoc mua) to assign tea (light), incense (safe zones), or offerings (skills). The system enforces the "2 saved, 1 abandoned" rule and sends the validated selection payload to Game State Machine on confirmation. Without this system, the game's consequence-driven identity breaks—there's no moral weight if the player doesn't actively choose who to save and who to leave behind.

## Player Fantasy$

The player should feel **moral weight and visceral discomfort** as they serve and ultimately abandon one soul. The day phase isn't about "completing tasks"—it's about **choosing who lives and who burns**. Serving tea (diem thap nhang) should feel tactile and intimate (ASMR-like), creating a false sense of care before the inevitable push. The moment of abandonment—physically shoving a soul out of the shade—should feel **genuinely uncomfortable**, like you're becoming the monster.

The fantasy is **"I am the judge, and I am cruel."** The 2.5D top-down view creates emotional distance, but the tactile rituals (diem, rot, vay) bridge that gap—you're physically present in their final moments. When you finally push the third soul into the sunlight, the game should make you feel the weight of that act. The player isn't "playing a game"—they're making a sacrifice and knowing it's irreversible.

The emotional promise: **Your hands are stained with the choice.** The tactile intimacy of service makes the abandonment feel personal, not mechanical.

## Detailed Design$

### Core Rules$

1. **Day Service Phase**: Three soul cards displayed (Linh, Van, Minh) in 2.5D top-down view. Each card shows portrait, name, and ritual needs.
2. **Selection Requirement**: Player must select exactly 2 souls as `Saved`, 1 soul as `Abandoned`. 
3. **Tactile Rituals**:
   - **Diêm (Tea)**: Assign to soul for light bonus (tea = light). Click tea pot, drag to soul card.
   - **Rot (Incense)**: Assign to soul for safe zone (incense = safe zone). Click incense, drag to soul card.
   - **Vây (Offering)**: Assign to soul for skill bonus (offering = skill). Click offeing bowl, drag to soul card.
4. **Ritual Assignment**: Each soul has preferred ritual (Linh = tea, Van = incense, Minh = offeing). Preferred ritual gives bonus effect.
5. **Selection Validation**: Pattern must be exactly 2 `Saved`, 1 `Abandoned`. Auto-complete uses priority order `Linh -> Van -> Minh` if player fails to select.
6. **Confirm Button**: Sends `SelectionConfirmed(payload)` to Game State Machine with SoulIds and ritual assignments.
7. **ChoiceLock Transition**: Triggers when valid selection + confirm. Day Service UI disabled, payload frozen.
8. **Phase Gating**: Player Controller disabled (UI only), cursor visible. WASD/mouse-look ignored. 

### States and Transitions$

| State | Entry Condition | Exit Condition | Behavior |
|-------|-----------------|----------------|----------|
| DayService | `DayService` phase active | Valid selection (2 Saved, 1 Abandoned) + confirm | Display soul cards, process ritual assignments, enable confirm button when valid |
| Validating | Selection changes | Pattern valid (2 Saved, 1 Abandoned) | Show validation state, update confirm button (enabled/disabled) |
| ChoiceLock | Valid + confirmed | Consequence payload ready | Send `SelectionConfirmed(payload)` to Game State Machine, disable all UI interaction |

### Interactions with Other Systems$

- **Game State / Phase State Machine <- Day Service**: Sends `SelectionConfirmed(payload)` with `saved_soul_ids[]`, `abandoned_soul_id`, ritual assignments. Game State Machine transitions to `ChoiceLock`. 

- **NPC/Soul Data Model <-> Day Service**: 
  - Reads soul data (SoulId, name, portrait, preferred ritual). 
  - Writes `DaySelectionState` (Saved/Abandoned) for each soul. 
  - Validates selection pattern (2 Saved, 1 Abandoned). 

- **Resource Effects <- Day Service**: Tea/incense/offering effects applied based on ritual assignments:
  - Tea (light) -> Increases Ward Timer for saved souls. 
  - Incense (safe zone) -> Creates safe zone for saved souls. 
  - Offering (skill) -> Grants skill bonus for saved souls. 

- **Player Controller <- Day Service**: Disabled during DayService (UI only). Cursor visible, WASD/mouse-look ignored. 

- **Map & Spawn Director <- Day Service**: Receives saved/abandoned assignment for night spawn bias (abandoned soul affects map generation). 

## Formulas$

### Selection Validity$

```
is_valid = (saved_count == required_saved_count) AND (abandoned_count == total_soul_count - required_saved_count)
```

| Variable | Type | Range | Source | Description |
|----------|------|-------|--------|-------------|
| saved_count | int | 0-3 | runtime | Number of souls marked `Saved` |
| abandoned_count | int | 0-3 | runtime | Number of souls marked `Abandoned` |
| required_saved_count | int | 1-2 | config | Required saved soul count (vertical slice: 2) |
| total_soul_count | int | 2-3 | config | Total active souls in current loop (vertical slice: 3) |

**Expected output range**: boolean  
**Edge case**: if `total_soul_count < required_saved_count + 1`, force invalid and emit `ConfigInvalid`. 

---

### Auto-Complete Priority Score (Fallback)$

```
priority_score = base_priority + chapter_modifier
```

| Variable | Type | Range | Source | Description |
|----------|------|-------|--------|-------------|
| base_priority | int | 1-3 | NPC Model | Default order (`Linh=1, Van=2, Minh=3`) |
| chapter_modifier | int | -2 to 2 | config (optional) | Future chapter-level override |

**Expected output range**: -1 to 5  
**Edge case**: equal scores resolve by static `SoulId` lexical order. 

---

*Note: Tea/incense/offering effect formulas (light bonus, safe zone creation, skill grant) are defined in **Resource Effects GDD** (Not Started — no GDD yet). Day Service assumes: tea = +Ward Sec, incense = safe zone, offeing = skill unlock.*

## Edge Cases$

| Scenario | Expected Behavior | Rationale |
|----------|-------------------|-----------|
| Player selects 3 Saved, 0 Abandoned | Invalid; block confirm; show "Need exactly 2 Saved, 1 Abandoned" | Enforces core rule |
| Player selects 1 Saved, 2 Abandoned | Invalid; block confirm; show "Need exactly 2 Saved, 1 Abandoned" | Enforces core rule |
| Confirm clicked with invalid selection | Block + show `InvalidSelectionCount`; don't send payload | Prevents bad transitions |
| Two clicks on same soul (Saved -> Abandoned -> Saved) | Valid toggle; if pattern met (2 Saved, 1 Abandoned) enable confirm | Flexible UI |
| Resource Effects not loaded (tea/incense/offering fail) | Log warning; continue without ritual effects; player gets base outcome | Graceful degradation |
| Player tries ritual during wrong phase | Ignored by Player Controller gating; ritual not processed | Consistent with phase rules |
| Auto-complete triggers (player idle timeout) | Apply priority order `Linh -> Van -> Minh`; mark 2 Saved, 1 Abandoned | Deterministic fallback |
| Duplicate ritual (same type to 2 souls) | Valid if souls are different; each soul can have own ritual | Flexible assignment |
| Payload sent but Game State not in ChoiceLock | Log error; don't send `SelectionConfirmed` | Defensive programming |
| NPC Model missing soul record | Reject selection; emit `MissingSoulRecord` error | Prevents undefined routing |

## Dependencies$

### Upstream (This depends on):

| System | Direction | Nature of Dependency |
|--------|-----------|---------------------|
| Game State / Phase State Machine | This depends on it | **Hard**: receives `SelectionConfirmed(payload)` with saved/abandoned SoulIds and ritual assignments. Transitions to `ChoiceLock` on valid confirmation. |
| NPC/Soul Data Model | This depends on it | **Hard**: reads soul data (SoulId, name, portrait, preferred ritual). Writes `DaySelectionState` (Saved/Abandoned) for each soul. |
| Resource Effects | This depends on it | **Soft**: applies tea (light), incense (safe zone), offeing (skill) effects based on ritual assignments. **Note: Resource Effects has NO GDD yet** — assuming tea = +Ward Sec, incense = safe zone, offeing = skill. |
| Map & Spawn Director | This depends on it | **Soft**: sends saved/abandoned assignment for night spawn bias (abandoned soul affects map generation). |

### Downstream (Depends on this):

| System | Direction | Nature of Dependency |
|--------|-----------|---------------------|
| Consequence Resolver | Depends on this | **Hard**: reads abandoned soul from payload to assign curse outcome (Drag/Block/FakeShrine). |
| Curse Effect Modules | Depends on this | **Soft**: curse effects depend on which soul was abandoned (and thus which hazards appear at night). |
| Shrine Objective & Win/Lose Rules | Depends on this | **Soft**: indirectly — selection affects Ward Timer initial value (saved souls = more Ward Sec). |

## Tuning Knobs$

| Parameter | Current Value | Safe Range | Effect of Increase | Effect of Decrease |
|-----------|---------------|------------|-------------------|-------------------|
| `required_saved_count` | 2 | 1-2 | More salvation, less sacrifice pressure | Harsher moral burden |
| `auto_complete_timeout_sec` | 15.0 | 10.0-30.0 | Player gets less agency; auto-choice triggers | More waiting; player controls timing |
| `ritual_effect_multiplier` | 1.0 | 1.0-3.0 | Stronger tea/incense/offering effects | Weaker bonuses, harder night |
| `validation_debounce_ms` | 20 | 10-50 | Fewer accidental multi-clicks | More race conditions in rapid UI |

**Interacting Knobs:**
- `required_saved_count` + `auto_complete_timeout_sec` control player agency vs. automation. 
- `ritual_effect_multiplier` interacts with Resource Effects (tea = +Ward Sec, incense = safe zone, offeing = skill).
- These knobs assume Resource Effects GDD is written (currently Not Started). 

## Visual/Audio Requirements$

| Event | Visual Feedback | Audio Feedback | Priority |
|-------|-----------------|----------------|----------|
| Soul selected as Saved | Portrait highlight + warm rim | Soft confirm click | High |
| Soul set to Abandoned | Portrait desaturate + warning glyph | Low, uneasy tone | High |
| Invalid selection pattern | UI pulse red on confirm | Error ping | Critical |
| Consequence assignment committed | Night preview icon appears | Single tension stinger | High |
| Ritual drag (tea/incense/offering) | Trail effect follows cursor | ASMR foley (pour, scrape, clink) | Medium |
| Confirm button (valid) | Pulse glow + expand | Relief chime | Critical |

## UI Requirements$

| Information | Display Location | Update Frequency | Condition |
|-------------|------------------|------------------|-----------|
| Soul identity (name + icon) | Day decision panel | Static per loop | Always in DayService |
| Current selection state per soul | Soul card badges | Real-time | DayService |
| Saved/Abandoned count and validity | Top of decision panel | Real-time | DayService and ChoiceLock |
| Locked state indicator | Panel header lock icon | On phase change | ChoiceLock onward |
| Consequence preview tag (optional) | Soul card footer | On lock commit | ChoiceLock |

## Acceptance Criteria$

- [ ] **Selection Validation**: Valid pattern = exactly 2 `Saved`, 1 `Abandoned`. Any other pattern = invalid, confirm blocked. 
- [ ] **Ritual Assignment**: Tea (diem) -> light bonus, Incense (rot) -> safe zone, Offering (vay) -> skill. Each soul can receive one ritual. 
- [ ] **Confirm Flow**: Valid + confirmed sends `SelectionConfirmed(payload)` to Game State Machine with SoulIds and ritual assignments. 
- [ ] **Phase Gating**: DayService phase only. WASD/mouse-look ignored (Player Controller). Cursor visible, UI active. 
- [ ] **Auto-Complete**: On idle timeout (15s), auto-select using priority order `Linh -> Van -> Minh`; mark 2 Saved, 1 Abandoned. **Must include audio/visual feedback**: play distinct "auto-select" sound and flash UI border when auto-complete triggers, so player knows why selection changed. 
- [ ] **Invalid Pattern Block**: 3 Saved/0 Abandoned, 0 Saved/3 Abandoned = block confirm + show `InvalidSelectionCount`. 
- [ ] **Consequence Payload**: Abandoned soul ID sent to Game State Machine for Consequence Resolver. Saved souls sent for Resource Effects. 
- [ ] **Performance**: per-soul validation completes within 0.05ms average on target PC. 
- [ ] **Cross-System Events**: Verify `SelectionConfirmed` delivered to Game State Machine, NPC Model updated (DaySelectionState), Resource Effects receive ritual assignments. 
- [ ] **UI Feedback**: Soul cards show selection state (Saved/Abandoned/unselected), validation status, ritual assignment visual. 

## Open Questions$

| Question | Owner | Deadline | Resolution |
|----------|-------|----------|------------|
| Should ritual assignment be mandatory (must assign tea/incense/offering to all saved souls)? | Game Designer | Before vertical slice lock | Open |
| Should auto-complete be visible to player (showing which souls will be selected)? | UX-Designer | Before first playtest | Open |
| What happens if NPC Model has missing soul record during selection write? | Systems Designer | Before DayService implementation | Open |
| Should ritual types be expanded per soul (Linh = tea ONLY, or can she receive incense too)? | Game Designer | Before content expansion | Resolved: One ritual per soul, preferred type gives bonus |
