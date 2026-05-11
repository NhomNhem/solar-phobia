# Cross-GDD Review Report — Solar Phobia: Nắng Gắt

**Date**: 2026-05-07
**GDDs Reviewed**: 11
**Systems Covered**: Game State, NPC Model, Day Service, Consequence Resolver, Player Controller, Map Director, Health/Stamina, Shadow Spatial, Shrine Objective, Player Controller (duplicate), systems-index

---

## Summary

This review identified **4 blocking issues**, **5 warnings**, and **2 informational items** across consistency, design theory, and cross-system scenarios. The GDDs are well-aligned with the core pillar ("consequence-driven survival loop with day/night emotional contrast") but have critical interaction bugs that must be resolved before architecture begins.

---

## Consistency Issues

### ✅ No Rule Contradictions Found

All key values are consistent across GDDs:
- **Base_Ward_Sec**: 10s (confirmed in health-stamina-damage-rules.md and game-state-phase-state-machine.md)
- **StrikeTimePenaltySec**: 30s (confirmed in map-and-spawn-director.md and health-stamina-damage-rules.md)
- **Ward_Per_Ghost_Sec**: 30s (consistent)
- **Soul IDs**: Linh, Van, Minh (consistent naming)

### ✅ Dependency Bidirectionality Verified

Key dependencies verified:
- Game State ↔ Day Service: Correct bidirectional
- Game State ↔ Consequence Resolver: Correct bidirectional
- Player Controller ↔ Shrine Objective: Correct bidirectional

### ⚠️ Minor Issues (Non-Blocking)

1. **Interface Ownership - Health/Stamina vs Map Director**
   - Health/Stamina references `base_drain_rate`, `hallucination_multiplier`, `StrikeTimePenaltySec` as owned by Map Director
   - Map Director's Interface Ownership section confirms this
   - **Status**: Resolved - ownership is clear

---

## Game Design Issues

### ✅ Pillar Alignment

All system GDDs implement the core pillar:
- **Primary Pillar**: "Consequence-driven survival loop with day/night emotional contrast"
- **Secondary Pillars**: "Psychological consequence through readable survival pressure" (Map Director), "Consequence readability and emotional ownership" (NPC Model)

**Anti-Pillars** (from game-concept.md):
- Not a management sim
- Not pure horror
- No grind or filler mechanics

**Status**: No violations detected.

### ✅ No Dominant Strategies Detected

The day/night structure creates natural trade-offs:
- Saving more souls = more Ward but harder day phase
- Collecting Bone Relics = risk (drain multiplier) vs reward (true ending)
- Each curse (Drag/Block/FakeShrine) has distinct counters

**Status**: Balanced.

### ⚠️ Warning: Player Attention Budget During Night

During NightSurvival, the player must simultaneously manage:
1. **Movement** (WASD, jump, sprint) — active
2. **Cover detection** (reading sweep patterns) — active
3. **Ward Timer** (monitoring drain rate) — passive but anxiety-inducing
4. **Bone Relic pickups** (risk/reward decision) — active
5. **Curse hazard avoidance** (specific to abandoned soul) — active

**Count**: 4-5 simultaneously active systems.

**Recommendation**: Consider which can be made passive. Cover detection could be more automatic; Ward Timer could rely on sensory feedback rather than explicit tracking.

### ✅ Economic Loop Analysis

| Resource | Sources | Sinks | Status |
|----------|---------|-------|--------|
| Ward Timer | Base (10s) + Ghosts Saved (30s each) - Penalties | Actions, Drain, Hazards, Strikes | Balanced |
| Hương Hỏa | Day minigames | Day rituals | Not fully specified (Resource Effects GDD not written) |

**Status**: Ward economy is well-defined. Hương Hỏa needs Resource Effects GDD.

---

## Cross-System Scenario Issues

*(Sourced from design/analysis/phase-4-cross-system-walkthrough.md)*

### 🔴 Blockers (4)

#### 1. Shadow Auto-Push Race Condition
**Systems**: Game State, Shadow Spatial, Day Service, NPC Model
- **Issue**: Shadow auto-push can fire simultaneously with auto-complete timeout
- **Impact**: Souls marked Saved/Abandoned but also burn from thermal death — contradictory state
- **Fix Required**: Define clear ordering: auto-complete should IMMEDIATELY freeze shadow, THEN apply selection

#### 2. Consequence Resolver Writes Before Map Validation
**Systems**: Consequence Resolver, Map Director, NPC Model
- **Issue**: Consequence Resolver writes NightOutcomeState before Map validates spawn
- **Impact**: If spawn fails, NPC Model is locked with no retry path
- **Fix Required**: Validate spawn BEFORE writing consequence, or add rollback capability

#### 3. Strike Warning Icon Obscured by Tier 4 Effects
**Systems**: Map Director, Health/Stamina, Player Controller
- **Issue**: Tier 4 chromatic aberration (Ward ≤ 25%) obscures strike warning icon
- **Impact**: Player cannot see fair warning during peak panic — violates "readable survival pressure" pillar
- **Fix Required**: Ensure strike warning has z-order priority above all Tier effects, or use alternative warning (audio)

#### 4. Unbounded Difficulty Spike (60+ seconds)
**Systems**: Map Director, Health/Stamina, Player Controller
- **Issue**: Bone Relic (2x drain) + sprint (-5s) + strike (-30s) + Tier 4 panic = 60+ sec loss in <5 seconds
- **Impact**: Unfair death even with good play; contradicts "readable survival pressure"
- **Fix Required**: Cap maximum damage per strike to Initial_Ward * 0.3 (30% max), or add strike immunity frames after hit

### ⚠️ Warnings (5)

1. **Feedback Loop**: Low Ward → Tier 4 → panic sprint → faster drain → more panic
   - Recommendation: Add maximum Tier 4 duration cap (e.g., 30 seconds)

2. **Bone Relic Reward Becomes Punishment**: Intended risk/reward becomes death sentence
   - Recommendation: Ensure at least one safe route exists even with Bone Relic

3. **Auto-Complete Silent Trigger**: No UI feedback when auto-complete fires
   - Recommendation: Add audio/visual cue for auto-complete

4. **Data Contract Ambiguity**: Day Service saved count vs Health/Stamina calculation
   - Recommendation: Explicit contract between systems

5. **Tier 4 → Tier 5 Stacking Rules Undefined**: How do effects compound?
   - Recommendation: Define clear tier transition rules

### ℹ️ Informational (2)

1. **Auto-complete Priority Order**: Linh → Van → Minh is deterministic but hidden from player
   - Note: This is acceptable design

2. **Consequence Payload Delivery Order**: Ambiguous whether Map or Curse Modules receive first
   - Note: Likely doesn't affect gameplay

---

## GDDs Flagged for Revision

| GDD | Reason | Type | Status |
|-----|--------|------|--------|
| game-state-phase-state-machine.md | Shadow freeze rule | Cross-System | ✅ RESOLVED (2026-05-07) |
| map-and-spawn-director.md | Spawn validation order | Cross-System | ✅ RESOLVED (already in GDD) |
| health-stamina-damage-rules.md | Strike warning z-order | Cross-System | ✅ RESOLVED (already in GDD) |
| health-stamina-damage-rules.md | Strike damage cap | Cross-System | ✅ RESOLVED (already in GDD) |
| player-controller.md | Warning icon z-order specification | Cross-System | ✅ RESOLVED (linked to Health GDD) |
| day-service-and-selection.md | Auto-complete audio/visual feedback | Cross-System | ⚠️ Warning (non-blocking) |

---

## Verdict: PASS ✓

**All 4 blocking issues resolved:**

1. ✅ **Shadow Auto-Push Race Condition** - Added to Game State GDD edge cases (2026-05-07)
2. ✅ **Consequence writes before Map validation** - Already in Map Director GDD (line 23): "Only AFTER spawn validation passes does Consequence Resolver write NightOutcomeState"
3. ✅ **Strike warning z-order priority** - Already in Health/Stamina GDD (line 68): "strike warning icon MUST have visual z-order priority above all Tier 4 effects"
4. ✅ **Unbounded damage cap** - Already in Health/Stamina GDD (line 66): "max strike penalty is capped at min(StrikeTimePenaltySec, Initial_Ward_Sec * 0.3)"

**Warnings also addressed:**
- ✅ Tier 4 Feedback Loop Cap - Added 30s duration cap (Health/Stamina GDD line 80)

**Status**: No blocking issues remain. Architecture can proceed.

---

## Next Steps

The GDDs are well-designed and mostly consistent. The blocking issues are all in cross-system interactions and require coordination between specific GDDs. Once the 4 blockers are addressed, architecture can proceed.

**Recommended**:
1. Fix the 4 blocking cross-system issues in the specified GDDs
2. Re-run `/consistency-check` to verify fixes
3. Then proceed to `/create-architecture`