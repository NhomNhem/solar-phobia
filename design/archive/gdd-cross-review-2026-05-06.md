## Cross-GDD Review Report — Solar Phobia: Nắng Gắt

**Date**: 2026-05-06  
**Reviewed By**: Copilot Cross-GDD Review Skill (Phases 1–4)  
**GDDs Reviewed**: 9 system GDDs + 1 concept  
**Systems Covered**: 
- Core: Game State/Phase State Machine, Player Controller, Shrine Objective & Win/Lose
- Narrative: NPC/Soul Data Model, Consequence Resolver
- Gameplay: Day Service & Selection, Shadow Spatial Management, Health/Stamina & Damage Rules, Map & Spawn Director
- Concept: Game Concept Document

**Review Scope**: Vertical Slice MVP (Act 1 One Day/Night Loop)

---

## Executive Summary

✅ **DESIGN COHERENCE**: All 9 GDDs are philosophically aligned with the core pillar ("consequence-driven survival loop with day/night emotional contrast"). No anti-pillar violations detected.

⚠️ **CONSISTENCY CONCERNS**: 5 blocking issues identified (unowned tuning knobs, hard dependencies on undesigned systems, stale status fields, race conditions). These must be resolved before architecture begins.

🔴 **SCENARIO RISKS**: 4 critical interaction failures detected through step-by-step walkthroughs. Most dangerous: simultaneous strike hit + shrine arrival event with conflicting outcome priorities, and spawn validation failure with no fallback.

**VERDICT**: **CONCERNS** — Architecture can proceed only after resolving the 5 blocking consistency issues and stabilizing the 4 critical scenario failures.

---

### Consistency Issues

#### Blocking (must resolve before architecture)

🔴 **Unowned Tuning Knobs — Cross-GDD**
Three core tuning knobs have no defined owner, causing traceability gaps:
- `base_drain_rate`: Referenced in game-state-phase-state-machine.md, health-stamina-damage-rules.md, map-and-spawn-director.md — no owner declared
- `hallucination_multiplier`: Referenced in health-stamina-damage-rules.md, map-and-spawn-director.md — no owner declared
- `StrikeTimePenaltySec`: Referenced in game-state-phase-state-machine.md, shrine-objective-win-lose-rules.md, health-stamina-damage-rules.md, map-and-spawn-director.md (all agree on 30s) — no owner declared
→ Each tuning knob must name its owning GDD in the "Interface ownership" section.

🔴 **Hard Dependency on Non-Existent GDD**
day-service-and-selection.md (Approved) lists "Resource Effects" as **Hard** dependency (`depends on it`), but Resource Effects GDD is "Not Started" (does not exist).
→ Either downgrade to **Soft** dependency with forward-looking note, or create the Resource Effects GDD before keeping this as Hard.

🔴 **Stale Status Fields in Approved GDDs**
- day-service-and-selection.md: Content shows `> **Status**: In Design` but systems-index.md lists it as `Approved`
- consequence-resolver.md: Content shows `> **Status**: In Design` but systems-index.md lists it as `Approved`
→ These GDDs should update their internal status header to match systems-index, or systems-index should be corrected.

🔴 **Scenario 1 Race Condition — Spawn Bundle Validation Missing Fallback**
game-state-phase-state-machine.md line 24: "Transition to `NightSurvival` only after curse payload and night spawn data are valid." BUT line 106 only covers "Consequence payload generation fails" — no fallback for spawn bundle validation failure.
→ Add spawn bundle failure handling: retry x3 → `FatalError`, or add default spawn fallback.

#### Warnings (should resolve)

⚠️ **Forward References to Unplanned GDDs**
Multiple GDDs reference "Curse Effect Modules" (Not Started) and "Resource Effects" (Not Started):
- day-service-and-selection.md, consequence-resolver.md, shrine-objective-win-lose-rules.md, player-controller.md, map-and-spawn-director.md
→ Add timeline/planning context for these references, or remove Hard dependency claims.

⚠️ **Numeric Type Mismatches**
- shrine-objective-win-lose-rules.md: `Ghosts_Saved * 30s` (integer)
- health-stamina-damage-rules.md: `Ward_Per_Ghost_Sec = 30.0s` (float)
- Same for `StrikeTimePenaltySec`: `30` vs `30.0` across GDDs
→ Values agree (30s) but type inconsistency may cause config/serialization issues.

⚠️ **In Design GDD Cited as Authority**
game-state-phase-state-machine.md and health-stamina-damage-rules.md cite map-and-spawn-director.md (In Design) as source for configurable ranges (`0.1–5.0`). In Design documents are not finalized.
→ Either finalize map-and-spawn-director.md first, or move range ownership to a Completed GDD.

⚠️ **Missing Reciprocal Dependencies**
game-state-phase-state-machine.md lists many dependent GDDs, but bidirectionality is not verified — some listed dependents may not list it back.
→ Verify each dependent GDD lists Game State in their Dependencies section.

---

### Game Design Issues

#### Warnings

⚠️ **Night Phase Cognitive Overload**
5 simultaneous active systems during core Night loop:
1. Player Controller (WASD, sprint, cover, E-interact) — active
2. Ward Timer drain + Strike penalties (Health/Stamina) — active
3. Map & Spawn Director (cover validity, strike warnings, boss sweeps) — active
4. Curse Effects (Drag/Block/FakeShrine) — active
5. Shrine Objective (direction, proximity) — active
→ Research suggests 3-4 is comfortable limit. Consider: simplify curse effects for MVP, or make some systems passive.

⚠️ **Bone Relic Reward Conflict**
Bone Relic (Ngoc Cot) pickup applies Time Drain multiplier (`effective_ward_drain = base_drain_rate * (1 + bones_carried * hallucination_multiplier)`), making the "reward" actively harm survival. Player has no incentive to risk Mo Oan pickup.
→ Add offsetting benefit (e.g., reveals shrine direction, reduces strike telegraph duration) or make Time Drain temporary (60s).

⚠️ **Strike Penalty Overpowered**
Strike penalty = -30s Ward Timer (map-and-spawn-director.md, health-stamina-damage-rules.md) is 30× base drain rate (1.0/sec). Two strikes = -60s, which can instantly kill a player with low initial Ward.
→ Consider reducing to -10s to -15s, or scale with distance-to-shrine remaining.

⚠️ **Shrine Arrival vs Ward=0 Unfair Loss**
shrine-objective-win-lose-rules.md line 97: "Ward Timer = 0 exactly | Immediate lose" and game-state-phase-state-machine.md priority: "Death > ShrineReached". Player can reach shrine, press E, but die because `OnWardTimerEmpty` processes before `OnShrineReached`.
→ Add 100ms timing window: if E was pressed before Ward=0, prioritize win.

⚠️ **Ward Timer Has No Positive Sink**
Health/Stamina GDD defines Ward Timer as pure drain resource — no way to spend it for advantage, no positive sink. Player can't mitigate consequences via resource management.
→ Consider adding limited positive uses (e.g., spend Ward to reveal shrine direction, or brief speed boost).

---

### Cross-System Scenario Issues

Scenarios walked: 4

#### Blockers
🔴 **Scenario 1: Day Choice → Night Curse Chain** — Day Service, Game State, Consequence Resolver, Map Director, NPC Model
Step 7: Spawn bundle validation failure has no documented fallback in Game State GDD (only consequence payload failure is covered).
→ Add spawn validation failure handling: retry x3 → `FatalError`.

🔴 **Scenario 1: Day Choice → Night Curse Chain** — Consequence Resolver, Map Director
Steps 4-7: Race condition if Map Director validates spawn bundle before receiving curse payload, or NPC Model write is delayed.
→ Enforce sequential execution: Consequence Resolver writes NightOutcomeState (step 4-5) → sends payload to Map Director (step 6) → Map validates (step 7) → Game State checks night_start_ready (step 8).

#### Warnings
⚠️ **Scenario 2: Night Survival → Strike Hit** — Map Director, Health/Stamina
Strike penalty (-30s) is 30× base drain rate (1.0/sec), disproportionately punishing single mistake.
→ Reduce to -10s to -15s, or scale with difficulty.

⚠️ **Scenario 3: Bone Relic Pickup During Strike Telegraph** — Player Controller, Map Director, Health/Stamina
Bone Relic pickup applies permanent Time Drain multiplier PLUS uncancelled strike penalty (-30s) can instantly deplete Ward Timer.
→ Cap total ward drain penalty per night, or make Bone Relic drain temporary (lasts 60s).

⚠️ **Scenario 4: Shrine Arrival vs Ward Timer = 0** — Shrine Objective, Game State, Health/Stamina
Player reaches shrine, presses E, but dies because `OnWardTimerEmpty` processes before `OnShrineReached` (Death > ShrineReached priority).
→ Add 100ms timing window for shrine arrival to override death if E was pressed first.

#### Info
ℹ️ **Scenario 2: Night Survival → Strike Hit** — Map Director
Strike telegraph duration = 1.2s (Map Director). Very short for controller players to reach cover.
→ Scale telegraph duration with difficulty: Easy 1.5s, Hard 0.8s.

ℹ️ **Scenario 4: Shrine Arrival vs Ward Timer = 0** — Shrine Objective
Simultaneous event handling needs explicit timestamp ordering beyond Game State priority rule.
→ Add Shrine Objective local event queue with timestamp ordering to handle near-simultaneous events fairly.

---

### GDDs Flagged for Revision

| GDD | Reason | Type | Priority |
|-----|--------|------|----------|
| day-service-and-selection.md | Hard dependency on non-existent Resource Effects GDD; Status header says "In Design" but Approved in index | Consistency + Status | Blocking |
| consequence-resolver.md | Status header says "In Design" but Approved in index | Status | Blocking |
| game-state-phase-state-machine.md | Spawn bundle validation failure fallback missing; Unowned tuning knobs referenced | Consistency + Scenario | Blocking |
| health-stamina-damage-rules.md | Unowned tuning knobs (base_drain_rate, hallucination_multiplier); cites In Design GDD as authority | Consistency | Warning |
| map-and-spawn-director.md | Unowned tuning knobs; Strike penalty may be overpowered | Consistency + Design | Warning |
| player-controller.md | Night phase cognitive load (5 simultaneous systems) | Design | Warning |
| shrine-objective-win-lose-rules.md | Shrine vs Death simultaneous event handling; numeric type mismatch (30 vs 30.0) | Design + Consistency | Warning |

---

### Verdict: CONCERNS

**Blocking issues: 4** (Unowned knobs ownership, Hard dep on non-existent GDD, stale status fields, spawn validation missing fallback)
**Warning issues: 7** (forward refs, cognitive load, bone relic conflict, strike penalty, shrine timing, ward sink, type mismatches)

Architecture can begin ONLY after resolving the 4 blocking issues listed above.

### Required actions before re-running:
1. **Assign tuning knob ownership** in each GDD's "Interface ownership" section:
   - `base_drain_rate`, `hallucination_multiplier` → Health/Stamina GDD owns (or Map & Spawn if it configures them)
   - `StrikeTimePenaltySec` → Map & Spawn Director owns (it defines the strike mechanic)
2. **Fix day-service-and-selection.md**: Downgrade Resource Effects to Soft dependency OR create Resource Effects GDD
3. **Fix stale status fields**: Update day-service-and-selection.md and consequence-resolver.md headers to `> **Status**: Approved`
4. **Add spawn bundle validation fallback** to game-state-phase-state-machine.md edge cases

---

### Session State Update
- Verdict: CONCERNS
- GDDs reviewed: 10
- Flagged for revision: day-service-and-selection.md, consequence-resolver.md, game-state-phase-state-machine.md, health-stamina-damage-rules.md, map-and-spawn-director.md, player-controller.md, shrine-objective-win-lose-rules.md
- Blocking issues: 4 (Unowned knobs, Hard dep on non-existent GDD, stale status, spawn validation missing)
- Recommended next: Fix 4 blocking issues → re-run /review-all-gdds → /gate-check pre-production
- Report: design/gdd/gdd-cross-review-2026-05-06.md
