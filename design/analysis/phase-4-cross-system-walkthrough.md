# Phase 4: Cross-System Scenario Walkthrough

## Executive Summary

This phase analyzes 5 critical multi-system scenarios that occur during Solar Phobia's day/night gameplay loop. Each scenario involves simultaneous system activation and complex data flow. Analysis reveals **4 Blockers** (critical bugs that break gameplay), **3 Warnings** (compounding risks), and **2 Info** items (minor ordering ambiguities).

---

## Scenarios Identified

1. **Day Phase: Shadow shrink reaches critical size + player idles + auto-complete timeout**
2. **Choice Lock transition: Player confirms selection → Consequence Resolver computes curse**
3. **Night Phase: Boss searchlight sweep + player in open + Ward Timer critical + Bone Relic held**
4. **Ward Timer reaches critical threshold (25% health) while player is mid-sprint**
5. **Player collects Bone Relic during active boss sweep**

---

## Scenario 1: Day Phase Shadow Shrink + Idle + Auto-Complete Timeout

**Systems Involved**: Game State Machine, Shadow Spatial Management, Day Service & Selection, NPC/Soul Data Model

**Trigger**: 
- Day phase duration timer approaches end (~15-30 sec remaining)
- Player has NOT confirmed selection (still unselected or invalid pattern)
- Auto-commit timeout (15 sec) activates

**Sequence**:
1. Game State Machine sends OnDayStart → Shadow Spatial begins polygon contraction
2. Day Service displays soul UI, awaits player input
3. Auto-complete timeout fires (15 sec idle)
4. Day Service applies fallback priority: Linh → Van → Minh
5. Day Service sends SelectionConfirmed(payload) with auto-selected 2 Saved, 1 Abandoned
6. NPC Model records auto-selected state
7. Game State Machine validates and transitions to ChoiceLock
8. Shadow Spatial freezes polygon

**Data Flow Analysis**: 
- ✅ Day Service → Game State: payload well-defined
- ✅ Game State → Shadow Spatial: phase event triggers freeze
- ⚠️ **CRITICAL**: If crowd density > 0.5 souls/m² at timeout, souls auto-push INTO sunlight while auto-complete marks them Saved/Abandoned

**Failure Modes**:

| Issue | Severity | Description |
|-------|----------|-------------|
| **Race condition** | BLOCKER | Shadow auto-push fires while auto-complete fires. Souls burn before formal selection. NPC Model has contradictory LifeState (Lost but Saved/Abandoned). |
| **State transition** | BLOCKER | Shadow continues shrinking until selection confirmed, should freeze IMMEDIATELY on auto-complete timeout. |
| **Undefined behavior** | BLOCKER | If polygon area < 1 m² (auto thermal death), but auto-complete hasn't fired, all souls burn. Which souls appear in night data? |
| **No UI feedback** | WARNING | Auto-complete triggers silently. Player unaware. |

**Issues Found**:
- [BLOCKER] Race condition: shadow crowding + auto-complete can both fire simultaneously
- [BLOCKER] Shadow should freeze IMMEDIATELY on auto-complete, not after
- [BLOCKER] No spec for which souls survive if thermal death during auto-complete
- [WARNING] No UI feedback when auto-complete triggers
- [INFO] Auto-complete priority order (Linh → Van → Minh) deterministic but hidden from player

---

## Scenario 2: Choice Lock Transition - Selection → Consequence Resolver

**Systems Involved**: Game State Machine, Day Service, NPC Model, Consequence Resolver, Map Director, Health/Stamina

**Trigger**: 
- Valid selection confirmed (2 Saved, 1 Abandoned)
- Player clicks Confirm in Day Service UI

**Sequence**:
1. Day Service validates selection pattern
2. NPC Model receives lock: RecordLockState = Locked
3. Day Service sends SelectionConfirmed(payload) with abandoned_soul_id
4. Game State Machine validates payload
5. Game State Machine transitions to ChoiceLock
6. Consequence Resolver invoked
7. Consequence Resolver reads abandoned soul ID
8. Consequence Resolver maps curse: Linh→Drag, Van→Block, Minh→FakeShrine
9. Consequence Resolver writes NightOutcomeState to NPC Model
10. Map Director receives curse payload
11. Health/Stamina computes initial Ward Timer
12. Game State validates all payloads ready
13. Game State transitions to NightSurvival

**Data Flow Analysis**:
- ✅ Day Service → Game State: payload clear
- ✅ Game State → Consequence Resolver: deterministic
- ✅ Consequence Resolver → NPC Model: one-time write
- ⚠️ **CRITICAL**: If Map spawn validation FAILS, Consequence Resolver already wrote NightOutcomeState. NPC Model locked. No retry path.

**Failure Modes**:

| Issue | Severity | Description |
|-------|----------|-------------|
| **State order** | BLOCKER | Consequence Resolver writes BEFORE Map validates spawn. If spawn fails, NPC locked with duplicate-write error blocking retry. |
| **No rollback** | BLOCKER | No state rollback if spawn validation fails. Selection committed but night cannot start. |
| **Data mismatch** | WARNING | If Health/Stamina uses different "saved count" than payload, initial Ward incorrect. |

**Issues Found**:
- [BLOCKER] Consequence Resolver writes before Map validates. If Map fails, no retry path.
- [BLOCKER] NPC Model lock should release on FatalError for deterministic fallback
- [WARNING] No data contract between Day Service saved count and Health/Stamina calculation
- [INFO] Consequence payload delivery order to Map vs Curse Modules ambiguous

---

## Scenario 3: Night Phase - Sweep + Open + Ward Critical + Bone Relic

**Systems Involved**: Map Director, Player Controller, Health/Stamina, Game State, Shrine Objective

**Trigger**:
- Player in NightSurvival
- Position: outside cover, in sweep cone
- Ward Timer: ≤ 25% (Tier 4 - Panic)
- Inventory: carrying 1+ Bone Relics

**Sequence**:
1. Map Director patrol sweep executes
2. Map checks is_exposed = in_sweep AND (not in_valid_cover) = TRUE
3. StrikeTelegraph starts (1.2 sec warning)
4. Health/Stamina applies Tier 4 effects: chromatic aberration, whispering, +0.1s dash cooldown
5. Player Controller receives OnStrikeWarning, displays warning icon
6. Player tries sprint to escape → sends is_sprinting=true to Health/Stamina
7. Health/Stamina calculates effective drain: 1.0 * (1 + 1*2.0) = 3.0/sec during sprint
8. Player exits cover before telegraph ends
9. StrikeTelegraph completes
10. Map re-checks exposure: still TRUE
11. Strike penalty applied: ward_time = max(0, ward_time - 30)
12. If Ward ≤ 0: Shrine Objective detects OnWardTimerEmpty → lose condition

**Data Flow Analysis**:
- ✅ Map → Player Controller: strike warning clear
- ✅ Health/Stamina → Player Controller: Ward critical clear
- ✅ Map → Shrine Objective: strike penalty valid input
- ⚠️ **CRITICAL**: Tier 4 chromatic aberration OBSCURES strike warning icon. Player can't see fair warning during peak panic.
- ⚠️ **CRITICAL**: Compounding drains can delete 60+ seconds in <5 seconds.

**Failure Modes**:

| Issue | Severity | Description |
|-------|----------|-------------|
| **Visual obscuration** | BLOCKER | Strike warning icon lost in Tier 4 chromatic aberration. Violates "fair readability" principle. |
| **Unbounded difficulty** | BLOCKER | Bone Relic (2x) + sprint (-5s) + strike (-30s) + Tier 4 panic = 60+ sec loss possible. |
| **Feedback loop** | WARNING | Low Ward → Tier 4 effects → panic sprint → faster drain → more panic. No governor. |
| **Reward becomes punishment** | WARNING | Bone Relic intended as risk/reward becomes unbounded death sentence. |

**Issues Found**:
- [BLOCKER] Strike warning icon visually obscured by Tier 4 chromatic aberration
- [BLOCKER] Compounding drain mechanics create unfair difficulty spike (60+ sec loss)
- [WARNING] Feedback loop: low Ward → Tier 4 → sprint → drain. Add maximum Tier 4 duration (30 sec)
- [WARNING] Route viability check may NOT guarantee fairness under "hit twice with Bone Relic" scenario

---

## Scenario 4: Ward Timer at 25% (Critical) While Mid-Sprint

**Systems Involved**: Health/Stamina, Player Controller, Game State, HUD/Diegetic Feedback

**Trigger**:
- Player sprinting (Shift held)
- Ward Timer drops to ≤ 25% threshold

**Sequence**:
1. Health/Stamina tracks Ward Timer drain (1.0/sec base, 5.0/sec with sprint)
2. Ward Timer drops to 25% of Initial_Ward_Sec
3. Health/Stamina emits OnWardTimerCritical event (Tier 4)
4. Player Controller receives event, applies Tier 4 effects to HUD
5. HUD displays critical color/pulsing
6. Player still sprinting (initiated before threshold)
7. Player Controller checks dash cooldown: +0.1s per Tier 4
8. Player releases Shift or Ward = 0

**Data Flow Analysis**:
- ✅ Health/Stamina → Player Controller: event clear
- ✅ Player Controller → HUD: feedback update clear
- ⚠️ "Increased dash cooldown +0.1s" ambiguous: during or after sprint?
- ⚠️ Tier 4 → Tier 5 effect stacking rules undefined

**Failure Modes**:

| Issue | Severity | Description |
|-------|----------|-------------|
| **Ambiguous cooldown** | INFO | Does +0.1s cooldown apply during sprint or only after? Unclear. |
| **Tier stacking undefined** | INFO | If Tier 4 AND Tier 5 active simultaneously, do effects stack or replace? |
| **Feedback loop** | WARNING | Low Ward → Tier 4 → harder navigation → time runs out (same as Scenario 3). |

**Issues Found**:
- [INFO] Tier stacking rules (Tier 4 → Tier 5) undefined. Clarify stacking behavior.
- [INFO] "Increased dash cooldown +0.1s" ambiguous. Clarify application window.

---

## Scenario 5: Collect Bone Relic During Active Boss Sweep

**Systems Involved**: Map Director, Player Controller, Resource Effects (not designed!), Health/Stamina, Shrine Objective

**Trigger**:
- Player presses E near CursedMound (Mo Oan) with Bone Relic
- Searchlight sweep active
- Player currently in cover

**Sequence**:
1. Map Director executes sweep cycle
2. Map checks player exposure: is_exposed = false (in cover)
3. Player Controller registers E key near CursedMound
4. Player Controller sends OnResourcePickedUp(NgocCot) to Resource Effects
5. Resource Effects applies Time Drain modifier
6. Bone Relic added to inventory
7. Health/Stamina receives Time Drain activation
8. Health/Stamina updates effective drain to 3.0/sec
9. Player still in cover during active sweep
10. Sweep telegraph completes, still in cover
11. Exposure still FALSE → NO STRIKE
12. Player exits cover with Bone Relic → exposed with 3x drain

**Data Flow Analysis**:
- ✅ Player Controller → Resource Effects: pickup event clear
- ✅ Resource Effects → Health/Stamina: Time Drain applied
- ⚠️ **CRITICAL**: Resource Effects GDD DOES NOT EXIST. Time Drain spec is provisional.
- ⚠️ Hallucination timing undefined: immediate or delayed?

**Failure Modes**:

| Issue | Severity | Description |
|-------|----------|-------------|
| **Missing design** | WARNING | Resource Effects GDD not designed. Time Drain activation, hallucination details, inventory cap undefined. |
| **Ambiguous timing** | INFO | Hallucination VFX appears when? During pickup or when exiting cover? |
| **Compounding difficulty** | WARNING | Relic pickup + Tier 4 panic + sweep = unfair combined difficulty with Scenario 3. |

**Issues Found**:
- [WARNING] Resource Effects GDD not designed. Time Drain timing and hallucination package undefined.
- [WARNING] Combining Scenario 5 (relic pickup) + Scenario 3 (sweep + Tier 4) creates unfair difficulty stack.
- [INFO] Hallucination activation timing ambiguous.

---

## Summary by Severity

### BLOCKERS: 4

1. **Scenario 1** — Shadow crowding auto-push + auto-complete fire simultaneously, burning souls before selection
2. **Scenario 1** — Shadow should freeze IMMEDIATELY on auto-complete, not continue shrinking
3. **Scenario 2** — Consequence Resolver writes before Map validates spawn. If spawn fails, NPC locked with no retry path.
4. **Scenario 3** — Strike warning obscured by Tier 4 chromatic aberration. Player can't see fair warning.

### WARNINGS: 3

1. **Scenario 3** — Bone Relic (2x) + sprint (-5s) + strike (-30s) + Tier 4 panic = unbounded difficulty spike (60+ sec loss)
2. **Scenario 3** — Feedback loop: Low Ward → Tier 4 effects → sprint → faster drain → more panic. No brake.
3. **Scenario 5** — Resource Effects GDD not designed. Time Drain timing and hallucination details undefined.

### INFO: 2

1. **Scenario 1** — Auto-complete triggers silently. No UI feedback.
2. **Scenario 4** — Tier 4 → Tier 5 effect stacking rules undefined. Effect combinations unclear.

---

## Critical Findings & Recommendations

### CRITICAL: Scenario 3 Unfair Difficulty Must Be Fixed Before Release

**The problem**: Bone Relic (2x drain) + sprint (-5s) + strike (-30s) + Tier 4 panic effects combine into **unbounded difficulty spike**. One sweep while sprinting with Bone Relic can delete 60+ seconds. Unfair and contradicts "readable survival pressure" design pillar.

**Required fixes**:
1. Draw strike warning icon in 3D world space (above chromatic aberration)
2. Cap strike penalty at 15s if player carries Bone Relic (vs. normal 30s)
3. Stress-test route viability: "player hit twice carrying Bone Relic, can reach shrine?"
4. Add Tier 4 effect governor: auto-downgrade to Tier 3 after 30 sec continuous

### CRITICAL: Scenario 2 System Ordering Must Be Fixed

**The problem**: Consequence Resolver writes NightOutcomeState BEFORE Map validates spawn. If spawn validation fails, NPC Model locked and cannot retry. Breaks recovery path.

**Required fix**:
1. Reorder: Map validation FIRST, then Consequence Resolver invoked
2. Add state rollback: release NPC lock if spawn validation fails

### CRITICAL: Scenario 1 Atomicity Must Be Fixed

**The problem**: Shadow shrinking continues while auto-complete fires. Souls can be thermally burned before being formally selected, creating corrupted run data.

**Required fixes**:
1. Shadow freezes IMMEDIATELY on auto-complete timeout
2. Abort auto-complete if soul already burned; force manual selection
3. Add thermal death check before finalizing selection pattern

---

## Next Steps

1. **Update Game State Machine GDD** — Add explicit note: "Map validation before Consequence Resolver"
2. **Update Shadow Spatial Management GDD** — Specify: "Shadow freezes IMMEDIATELY on auto-complete, not after"
3. **Design Resource Effects GDD** — Define Time Drain timing, hallucination package, inventory limits
4. **Add stress-test scenarios** to acceptance criteria for Scenario 3
5. **Implement 3D world-space strike warning** instead of 2D screen-space
6. **Add Tier 4 duration governor** (30 sec max before downgrade to Tier 3)
7. **Cap strike penalty** at 15s when carrying Bone Relic
8. **Clarify Tier effect stacking** in Health/Stamina GDD

---

## Conclusion

Phase 4 analysis has identified **4 game-breaking blockers** that must be fixed before vertical slice testing:

1. **Shadow/auto-complete race condition** corrupting run data
2. **Consequence Resolver ordering** preventing fallback recovery
3. **Strike warning visibility** violating fairness principle
4. **Unbounded difficulty spike** from compounding drain mechanics

All blockers have clear reproduction steps and recommended fixes in this document.
