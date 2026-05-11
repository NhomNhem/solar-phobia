# Cross-GDD Review Report
Date: 2026-05-07
GDDs Reviewed: 10 system GDDs
Systems Covered: Game State, Player Controller, Day Service, Shrine Objective, Consequence Resolver, Shadow Spatial, Health/Stamina, Map/Spawn, NPC Soul Data Model

---

## Summary

Found **1 blocking issue** requiring resolution before architecture can begin.

---

## Consistency Issues

### Blocking (must resolve before architecture)

🔴 **Formula Contradiction: Base_Ward_Sec Value Mismatch**

| GDD | Base_Ward_Sec Value | Source Line |
|-----|---------------------|-------------|
| `game-state-phase-state-machine.md` | **10 sec** | Line 129 |
| `health-stamina-damage-rules.md` | **60 sec** | Line 25 |
| V5.0 Master GDD (reference) | **10 sec** | Matches Phase State Machine |

**Problem**: Two GDDs define the same formula with contradictory values.

- Phase State Machine: "Base_Ward_Sec = 10 (intentionally short to prevent speedrunning)"
- Health/Stamina: "Base_Ward_Sec – Minimum survival time (e.g., 60.0 s)"

**Impact**: The Ward Timer initialization will produce wildly different results depending on which GDD is implemented. At 0 souls saved:
- Phase State Machine implementation: 10s base → ~10-70s total
- Health/Stamina implementation: 60s base → ~60-120s total

This is a **6x difference** in expected survival time!

**Resolution**: The Phase State Machine GDD (updated 2026-05-07 with V5.0 lore) and the V5.0 Master GDD both agree on **10 seconds**. The Health/Stamina GDD value should be updated from 60s to 10s.

**Action Required**: Update `health-stamina-damage-rules.md` line 25 to:
```
- `Base_Ward_Sec` – Minimum survival time (e.g., **10 s**).
```

---

### Warnings (should resolve, but won't block)

⚠️ **Stale Reference in Shrine Objective GDD**

`shrine-objective-win-lose-rules.md` line 88 says:
> "Ward Timer initialization (`Initial_Ward_Sec = Base_Ward_Sec + (Ghosts_Saved * 30s) - Day_Penalties_Sec`) is defined in **Health/Stamina & Damage Rules**."

However, this formula is now more accurately defined in `game-state-phase-state-machine.md` (the authoritative source after V5.0 update). The reference to Health/Stamina is now partially stale.

**Recommendation**: Update shrine-objective-win-lose-rules.md to reference Game State / Phase State Machine as the authoritative source, or simply note both GDDs define it (with Phase State Machine being the master reference).

---

## Game Design Issues

### No Blocking Issues

No dominant strategies, economic imbalances, or pillar drift detected. The GDDs appear coherent when viewed together.

### Warnings

⚠️ **Cognitive Load During Night Phase**

During NightSurvival, players must simultaneously manage:
1. WASD movement + sprint/dash/glide (active)
2. Cover detection against searchlight (active)
3. Karma hazard avoidance based on abandoned soul (active)
4. Ngọc Cốt bone pickup decisions (active)
5. Ward timer awareness via sensory feedback (passive)

This is 4 active systems - within the 3-4 limit, but tight. The HUD-less design helps by making Ward timer monitoring passive rather than active.

---

## Cross-System Scenario Issues

### Scenarios Walked: 3

1. **Day Phase Selection** (DayService → ChoiceLock)
2. **Night Phase Transition** (ChoiceLock → NightSurvival)
3. **Shrine Reach (Win Condition)**

#### No Blockers Found

All three scenarios have clear activation order, valid data flows, and no undefined behavior:

- **Day Selection**: Clean 2-of-3 selection → consequence payload → night start
- **Night Transition**: Valid payload + spawn data → enable night systems
- **Shrine Win**: Clear proximity + E-press → Resolve state

---

## Dependency Bidirectionality Check

All checked dependencies are properly reciprocal:

| GDD Pair | Direction | Status |
|----------|-----------|--------|
| Phase State Machine ↔ Health/Stamina | Bidirectional | ✅ |
| Phase State Machine ↔ Player Controller | Bidirectional | ✅ |
| Phase State Machine ↔ Shrine Objective | Bidirectional | ✅ |
| Phase State Machine ↔ Day Service | Bidirectional | ✅ |
| Phase State Machine ↔ Consequence Resolver | Bidirectional | ✅ |
| Health/Stamina ↔ Map/Spawn Director | Bidirectional | ✅ |

---

## GDDs Flagged for Revision

| GDD | Reason | Type | Priority |
|-----|--------|------|----------|
| health-stamina-damage-rules.md | Base_Ward_Sec = 60s contradicts Phase State Machine's 10s | Consistency | **Blocking** |
| shrine-objective-win-lose-rules.md | References Health/Stamina as Ward Timer authority (should reference Phase State Machine) | Stale Reference | Warning |

---

## Verdict: CONCERNS

**Cannot proceed to architecture until blocking issue is resolved.**

The Base_Ward_Sec formula contradiction must be fixed. Once resolved, re-run this review to verify PASS.

### Required Actions Before Re-Running:

1. **Fix Base_Ward_Sec**: Update `health-stamina-damage-rules.md` line 25 from "60.0 s" to "10 s"
2. **Re-verify**: After fix, re-run `/review-all-gdds` for final PASS verdict
3. **Then proceed**: Run `/create-architecture` and write ADRs

---

## Recommendation

Would you like me to fix the blocking issue now? (Update health-stamina-damage-rules.md with correct Base_Ward_Sec = 10s)