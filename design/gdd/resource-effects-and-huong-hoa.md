# Resource Effects & Hương Hỏa

> **Status**: Approved
> **Author**: opencode
> **Last Updated**: 2026-05-11
> **Implements Pillar**: Consequence-driven survival loop with day/night emotional contrast

## Overview

Resource Effects & Hương Hỏa is the economic bridge between Solar Phobia's two day-phases: Tactile Rituals (generate Hương Hỏa via minigames) and Day Service & Selection (consume Hương Hỏa to assign buffs to saved souls). Hương Hỏa (Spirit Essence) is the single day-phase currency earned through successful ritual minigames (diêm, rót, vay) and spent to enhance saved souls with tea (Ward bonus), incense (safe zone), or offering (skill) effects. The amount of Hương Hỏa held determines effect magnitude, directly linking player skill in minigames to night-phase survival advantage. Without this system, Tactile Rituals have no mechanical payoff and ritual assignment has no stakes.

## Player Fantasy

The player should feel **earned preparation** — every successful match strike, tea pour, and fan rhythm directly translates to a tangible night-time advantage. Hương Hỏa is not abstract points; it's the palpable warmth of having done the rituals right. The fantasy is "crafting your own safety" — the more care you put into day rituals, the more tools you have for night survival. When a saved soul's tea extends your Ward timer by exactly the right amount, you should feel that your earlier effort paid off.

The emotional promise: **"Your hands prepared this salvation."** The tactile effort of the minigames becomes the mechanical power of the night buffs. Fail the rituals, and you face the night weak. Succeed, and your saved souls carry you forward.

## Detailed Design

### Core Rules

1. **Hương Hỏa as Currency**: Hương Hỏa (HH) is earned during Tactile Rituals (minigames) and spent during Day Service assignment. It is a day-only resource — unspent HH is lost when night starts.

2. **Earning Hương Hỏa** (owned by Tactile Rituals system):
   - Successful minigame: `+base_huong_hoa (+ difficulty_bonus)` HH
   - Failed minigame: `+0` HH, adds `penalty_per_fail` to `Day_Penalties_Sec`
   - Max HH per day: `max_huong_hoa_per_day` (prevents runaway scaling)

3. **Spending Hương Hỏa** (owned by Resource Effects system):
   - Ritual assignment (tea/incense/offering) to a soul costs `ritual_assignment_cost` HH
   - This cost is deducted when the ritual is placed on the soul card
   - If HH is insufficient, the assignment is still allowed but with **reduced effect** (graceful degradation)

4. **Effect Scaling**:
   - Each ritual effect has a base value and scales by the `remaining_HH / ritual_assignment_cost` ratio
   - Maximum effect achieved when HH >= `ritual_assignment_cost` at time of assignment
   - Minimum effect (floor) = 50% of base (graceful degradation floor)

5. **Preferred Ritual Bonus**:
   - Each soul has a preferred ritual that gives +50% effect multiplier
   - Linh → Tea (Diêm), Van → Incense (Rot), Minh → Offering (Vây)
   - Non-preferred rituals give only base effect (no bonus)

6. **Effect Types**:
   - **Tea (Light)**: Adds bonus seconds to initial Ward Timer at night start
   - **Incense (Safe Zone)**: Creates a stationary safe zone at the saved soul's position during night
   - **Offering (Skill)**: Reduces Ward cost or cooldown for a specific movement skill

### Resource Flow

```
Day Phase Timeline:

[Tactile Rituals Phase]         [Day Service & Selection Phase]
     ↓                                      ↓
  Player plays minigames → earns HH → assigns rituals to souls
     ↓                                      ↓
  HH accumulated                      HH spent on assignments
     ↓                                      ↓
  More skill = more HH              More HH = stronger buffs
     ↓                                      ↓
    [Night Phase]
     ↓                                      ↓
  Tea→+Ward bonus, Incense→safe zone, Offering→skill buff
```

### Effect Details

**Tea (Light) — Ward Bonus**:
- On night start, add `tea_ward_bonus_sec` to `Initial_Ward_Sec`
- Scaled: `actual_bonus = floor(base_bonus * scaling_ratio * (preferred ? 1.5 : 1.0))`
- Applied before NightSurvival phase initialization
- Stacking: one tea per saved soul, effects sum

**Incense (Safe Zone)**:
- Creates an `incense_safe_zone_radius` safe zone centered on the saved soul's assigned position
- Within safe zone: Ward drain pauses (0 drain/sec)
- Scaled: `actual_radius = base_radius * scaling_ratio * (preferred ? 1.5 : 1.0)`
- Duration: `incense_duration_sec` (fixed, not scaled)
- One safe zone per saved soul with incense assigned

**Offering (Skill)**:
- Reduces Ward cost of a specific movement skill by `offering_skill_discount_pct` %
- Scaled: `actual_discount = base_discount * scaling_ratio * (preferred ? 1.5 : 1.0)`
- Skill affected determined by which saved soul received the offering
- Effect persists for entire night phase

### Graceful Degradation

If `IResourceEffectApplier` (or equivalent DI service) is not registered:
1. Log warning: `"[ResourceEffects] IResourceEffectApplier not registered — ritual effects disabled"`
2. No HH is consumed during assignment
3. Ritual assignment proceeds visually (tea icon on card, etc.)
4. No gameplay effect is applied at night start
5. Base night outcome is preserved (as if no rituals were assigned)

### Interface Contract

```csharp
public interface IResourceEffectApplier
{
    /// <summary>
    /// Apply tea effect: adds Ward bonus. Scaled by current Hương Hỏa ratio.
    /// </summary>
    void ApplyTeaEffect(string soulId, float scalingRatio, bool isPreferred);

    /// <summary>
    /// Apply incense effect: creates safe zone. Scaled by current Hương Hỏa ratio.
    /// </summary>
    void ApplyIncenseEffect(string soulId, float scalingRatio, bool isPreferred);

    /// <summary>
    /// Apply offering effect: reduces skill cost. Scaled by current Hương Hỏa ratio.
    /// </summary>
    void ApplyOfferingEffect(string soulId, float scalingRatio, bool isPreferred);

    /// <summary>
    /// Get current Hương Hỏa amount. Must be thread-safe (called from UI).
    /// </summary>
    int GetCurrentHuongHoa();

    /// <summary>
    /// Spend Hương Hỏa. Returns false if insufficient.
    /// </summary>
    bool TrySpendHuongHoa(int amount);
}
```

## Formulas

### Hương Hỏa Earned (per minigame)
```
hh_earned = base_huong_hoa + difficulty_bonus
           (0 if minigame failed)
```

| Variable | Type | Range | Source | Description |
|----------|------|-------|--------|-------------|
| base_huong_hoa | int | 10 | config | Base HH per successful ritual |
| difficulty_bonus | int | 0-10 | config | Extra HH from time-of-day scaling |

**Expected output range**: 0-20 per ritual attempt

### Effect Scaling Ratio
```
scaling_ratio = clamp(current_HH / ritual_assignment_cost, 0.5, 1.0)
```

| Variable | Type | Range | Source | Description |
|----------|------|-------|--------|-------------|
| current_HH | int | 0-max | runtime | Hương Hỏa at moment of assignment |
| ritual_assignment_cost | int | 10-20 | config | HH cost per ritual assignment |

**Expected output range**: 0.5-1.0 (clamped to 0.5 floor)

### Tea Ward Bonus
```
tea_ward_bonus_sec = floor(base_tea_ward_sec * scaling_ratio * preferred_multiplier)
```

| Variable | Type | Range | Source | Description |
|----------|------|-------|--------|-------------|
| base_tea_ward_sec | float | 5.0-15.0 | config | Base Ward seconds from tea |
| scaling_ratio | float | 0.5-1.0 | computed | From HH at assignment time |
| preferred_multiplier | float | 1.0 or 1.5 | runtime | 1.5 if preferred ritual, else 1.0 |

**Expected output range**: 2.5-22.5 sec (floor)
**Edge case**: 0 if no tea assigned

### Incense Safe Zone Radius
```
incense_radius = base_incense_radius * scaling_ratio * preferred_multiplier
```

| Variable | Type | Range | Source | Description |
|----------|------|-------|--------|-------------|
| base_incense_radius | float | 3.0-6.0 | config | Base radius in Unity units |
| scaling_ratio | float | 0.5-1.0 | computed | From HH at assignment time |
| preferred_multiplier | float | 1.0 or 1.5 | runtime | 1.5 if preferred ritual, else 1.0 |

**Expected output range**: 1.5-9.0 units
**Edge case**: 0 if no incense assigned

### Offering Skill Discount
```
offering_discount_pct = base_offering_discount_pct * scaling_ratio * preferred_multiplier
```

| Variable | Type | Range | Source | Description |
|----------|------|-------|--------|-------------|
| base_offering_discount_pct | float | 0.10-0.30 | config | Base skill Ward cost reduction |
| scaling_ratio | float | 0.5-1.0 | computed | From HH at assignment time |
| preferred_multiplier | float | 1.0 or 1.5 | runtime | 1.5 if preferred ritual, else 1.0 |

**Expected output range**: 5%-45% reduction
**Edge case**: 0% if no offering assigned

## Edge Cases

| Scenario | Expected Behavior | Rationale |
|----------|-------------------|-----------|
| 0 Hương Hỏa at assignment time | Assignment proceeds with 0.5x minimum scaling_ratio; log info | Graceful degradation always works |
| Max HH exceeds per-day cap | Additional rituals succeed but award 0 HH; log warning | Prevents infinite HH exploit |
| All 3 souls assigned same ritual (e.g., all tea) | Valid; each effect applies independently | Flexible strategy |
| Ritual assigned, HH spent, then reassignment | Refund previous HH cost, deduct new cost | Non-punishing UI |
| Player earns HH but never assigns rituals | HH lost at night start; base Ward only | Meaningful choice to skip |
| Preferred ritual assigned to wrong soul | Standard effect (no 1.5x), no error | Non-preferred is valid but suboptimal |
| Incense safe zone overlaps with boss sweep area | Safe zone takes priority; Ward pauses | Safe zone = sanctuary |
| Multiple saved souls with incense | Each creates independent safe zone | Saved souls each contribute |
| Offering assigned to soul that dies/cursed soul | Offering effect canceled; log warning | Abandoned souls don't get buffs |
| Very high HH (e.g., 60 from perfect play) | Caps at scaling_ratio = 1.0; no further benefit | Prevents over-farming, keeps night challenging |

## Dependencies

### Upstream (This depends on):

| System | Direction | Nature of Dependency |
|--------|-----------|---------------------|
| NPC/Soul Data Model | This depends on it | **Hard**: reads soul identity for preferred ritual mapping and effect targeting |
| Day Service & Selection | This depends on it | **Soft**: receives assignment events with soulId, ritualType, HH state |
| Tactile Rituals | This depends on it | **Hard**: reads HH earned from minigame results |

### Downstream (Depends on this):

| System | Direction | Nature of Dependency |
|--------|-----------|---------------------|
| Night Survival Run | Depends on this | **Hard**: consumes TeaWardBonus for Initial_Ward_Sec, IncenseSafeZone positions, OfferingSkillDiscount |
| Health/Stamina & Damage Rules | Depends on this | **Hard**: reads TeaWardBonus in Ward initialization formula |
| Day Service & Selection (Story 003) | Depends on this | **Hard**: HH cost display, effect preview on assignment |
| Map & Spawn Director | Depends on this | **Soft**: receives safe zone positions for hazard exclusion |

## Tuning Knobs

| Parameter | Current Value | Safe Range | Effect of Increase | Effect of Decrease |
|-----------|---------------|------------|-------------------|-------------------|
| `base_huong_hoa` | 10 | 5-20 | More HH per ritual, easier to max effects | Harder to afford assignments |
| `ritual_assignment_cost` | 15 | 10-20 | More HH needed per assignment, more minigame pressure | Trivial to max effects |
| `max_huong_hoa_per_day` | 60 | 30-90 | Higher ceiling, rewards perfect play | Caps strategy, rewards precision |
| `base_tea_ward_sec` | 10.0 | 5.0-15.0 | Higher Ward bonus per tea | Lower impact of tea choice |
| `base_incense_radius` | 4.0 | 3.0-6.0 | Bigger safe zones | Tighter safe zones, more risk |
| `base_offering_discount_pct` | 0.20 | 0.10-0.30 | Bigger skill cost reduction | Minimal skill benefit |
| `graceful_min_scaling` | 0.5 | 0.25-1.0 | Floor higher, less penalty for low HH | Floor lower, harsher penalty |
| `preferred_multiplier` | 1.5 | 1.25-2.0 | Stronger incentive for preferred rituals | Preferred bonus less meaningful |
| `incense_duration_sec` | 30.0 | 15.0-60.0 | Safe zone lasts longer | Brief sanctuary |

**Interacting Knobs:**
- `base_huong_hoa` + `ritual_assignment_cost` + `max_huong_hoa_per_day` control the economy curve: how many assignments can be maxed per day.
- `base_tea_ward_sec` interacts directly with Ward Timer initialization (`Base_Ward_Sec` + `Ward_Per_Ghost_Sec` from Health GDD).
- `base_incense_radius` + `incense_duration_sec` control safe zone viability as a night strategy.

## Acceptance Criteria

- [ ] **HH Earning**: Successful tactile minigame awards correct HH amount (base + bonus). Failed minigame awards 0 HH.
- [ ] **HH Spending**: Ritual assignment deducts `ritual_assignment_cost` HH. Insufficient HH uses `graceful_min_scaling`.
- [ ] **HH Cap**: HH capped at `max_huong_hoa_per_day`. Excess awarded but not stored.
- [ ] **Tea Effect**: `ApplyTeaEffect()` adds correct Ward bonus to Initial_Ward_Sec. Preferred ritual applies 1.5x multiplier.
- [ ] **Incense Effect**: `ApplyIncenseEffect()` creates safe zone at soul position with correct radius. Preferred ritual applies 1.5x.
- [ ] **Offering Effect**: `ApplyOfferingEffect()` reduces skill Ward cost by correct percentage. Preferred ritual applies 1.5x.
- [ ] **Scaling Ratio**: Ratio clamped to 0.5-1.0 based on HH at assignment time.
- [ ] **Graceful Degradation**: If `IResourceEffectApplier` not registered, warn log, allow visual assignment, no gameplay effect.
- [ ] **Reassignment Refund**: Changing a ritual assignment refunds previous HH cost and deducts new cost.
- [ ] **HH Reset**: Unspent HH is lost on night start (reset to 0).
- [ ] **No Cross-Night Carryover**: HH does not persist between day/night cycles.
- [ ] **Performance**: Effect calculation completes within 0.1ms average.

## Open Questions

| Question | Owner | Deadline | Resolution |
|----------|-------|----------|------------|
| Should HH be visible in the Day Service UI during assignment? | UX Designer | Before story-003 implementation | Open |
| Should preferred ritual show a preview of the bonus effect before assignment? | UX Designer | Before story-003 implementation | Open |
| Should incense safe zone be visible on the night map during Day Service? | Systems Designer | Before implementation | Open |
| Can a saved soul receive multiple rituals (e.g., tea + incense)? | Game Designer | Before vertical slice | Likely: one ritual per soul |
| Should reassignment cost a penalty (partial refund) or full refund? | Systems Designer | Before implementation | Full refund (non-punishing) |
