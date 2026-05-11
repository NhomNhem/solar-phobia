# Systems Index: Solar Phobia: Nắng Gắt

> **Status**: Updated
> **Created**: 2026-05-06
> **Last Updated**: 2026-05-07
> **Source Concept**: design/gdd/game-concept.md

---

## Overview

Solar Phobia's mechanical scope centers on a consequence-driven day/night loop: choose who to save in a constrained daytime decision phase, then survive the night while facing direct gameplay consequences from the person left behind. The minimum system map prioritizes loop orchestration, consequence transformation, and readable survival pressure over content breadth. This index decomposes those needs into implementable systems, dependencies, and design order for the vertical slice.

---

## Systems Enumeration

| # | System Name | Category | Priority | Status | Design Doc | Depends On |
|---|-------------|----------|----------|--------|------------|------------|
| 1 | Game State / Phase State Machine | Core | MVP | Approved | design/gdd/game-state-phase-state-machine.md | — |
| 2 | NPC/Soul Data Model | Narrative | MVP | Approved | design/gdd/npc-soul-data-model.md | — |
| 3 | Map & Spawn Director | Core | MVP | Approved | design/gdd/map-and-spawn-director.md | — |
| 4 | Ward Timer / Nước Mắm Cốt | Core | MVP | Approved | design/gdd/health-stamina-damage-rules.md | — |
| 5 | Shadow Spatial Management | Gameplay | MVP | Approved | design/gdd/shadow-spatial-management.md | Game State / Phase State Machine |
| 6 | Day/Night Camera Transition | Core | MVP | Approved | design/gdd/day-night-camera-transition.md | Game State / Phase State Machine |
| 7 | Player Controller & Skills | Core | MVP | Approved | design/gdd/player-controller.md | Game State / Phase State Machine, Map & Spawn Director, Day/Night Camera Transition |
| 8 | Physical Crowding & Push | Gameplay | MVP | Approved | design/gdd/physical-crowding-and-push.md | NPC/Soul Data Model, Map & Spawn Director, Game State / Phase State Machine |
| 9 | Tactile Rituals | Gameplay | MVP | Approved | design/gdd/tactile-rituals.md | Game State / Phase State Machine, NPC/Soul Data Model, Ward Timer / Nước Mắm Cốt |
| 10 | Day Service & Selection | Gameplay | MVP | Approved | design/gdd/day-service-and-selection.md | NPC/Soul Data Model, Game State / Phase State Machine, Tactile Rituals |
| 11 | Shrine Objective & Win/Lose Rules | Core | MVP | Approved | design/gdd/shrine-objective-win-lose-rules.md | Game State / Phase State Machine, Player Controller & Skills, Ward Timer / Nước Mắm Cốt |
| 12 | Sensory Feedback System | UI | MVP | Approved | design/gdd/sensory-feedback-system.md | Ward Timer / Nước Mắm Cốt, Game State / Phase State Machine |
| 13 | Consequence Resolver | Gameplay | MVP | Approved | design/gdd/consequence-resolver.md | Day Service & Selection, NPC/Soul Data Model, Game State / Phase State Machine |
| 14 | Curse Effect Modules | Gameplay | MVP | Approved | design/gdd/curse-effect-modules.md | Consequence Resolver, Player Controller & Skills, Map & Spawn Director, Ward Timer / Nước Mắm Cốt |
| 15 | Boss Cá Ông Searchlight | Gameplay | MVP | Approved | design/gdd/boss-ca-ong-searchlight.md | Player Controller & Skills, Map & Spawn Director, Game State / Phase State Machine, Ward Timer / Nước Mắm Cốt |
| 16 | Night Survival Run | Gameplay | MVP | Approved | design/gdd/night-survival-run.md | Shrine Objective & Win/Lose Rules, Curse Effect Modules, Boss Cá Ông Searchlight, Ngọc Cốt / Relic System, Resource Effects & Hương Hỏa |
| 17 | Resource Effects & Hương Hỏa | Economy | Vertical Slice | Approved | design/gdd/resource-effects-and-huong-hoa.md | NPC/Soul Data Model, Game State / Phase State Machine, Day Service & Selection |
| 18 | Ngọc Cốt / Relic System | Gameplay | Vertical Slice | Not Started | — | Player Controller & Skills, Ward Timer / Nước Mắm Cốt, Game State / Phase State Machine |
| 19 | HUD-less Design & Sensory Feedback | UI | Vertical Slice | Not Started | — | Ward Timer / Nước Mắm Cốt, Sensory Feedback System, Night Survival Run |
| 20 | Audio State Director | Audio | Vertical Slice | Not Started | — | Game State / Phase State Machine, Boss Cá Ông Searchlight, Night Survival Run |
| 21 | Save Seed / Run Reset | Persistence | Alpha | Not Started | — | Game State / Phase State Machine, NPC/Soul Data Model, Map & Spawn Director, Day Service & Selection, Consequence Resolver, Night Survival Run |

---

## Categories

| Category | Description | Typical Systems |
|----------|-------------|-----------------|
| **Core** | Foundation systems everything depends on | Loop state machine, player controller, objective/win-lose logic, ward timer, camera transition |
| **Gameplay** | The systems that make the game fun | Day selection, consequences, curses, hazards, chase pressure, shadow management, tactile rituals, physical crowding |
| **Economy** | Resource creation and consumption | Tea/incense/offering effects and conversion, Hương Hỏa points |
| **Persistence** | Save state and continuity | Run reset and deterministic restart |
| **UI** | Player-facing information displays | Choice feedback, danger readability, objective cues, sensory feedback |
| **Audio** | Sound and music systems | Day/night mix state, threat escalation cues |
| **Narrative** | Story and identity state delivery | NPC/soul identity and abandonment state |

---

## Priority Tiers

| Tier | Definition | Target Milestone | Design Urgency |
|------|------------|------------------|----------------|
| **MVP** | Required for the core loop to function. Without these, you can't test "is this fun?" | First playable prototype | Design FIRST |
| **Vertical Slice** | Required for one complete, polished area. Demonstrates the full experience. | Vertical slice / demo | Design SECOND |
| **Alpha** | All features present in rough form. Complete mechanical scope, placeholder content OK. | Alpha milestone | Design THIRD |
| **Full Vision** | Polish, edge cases, nice-to-haves, and content-complete features. | Beta / Release | Design as needed |

---

## Dependency Map

### Foundation Layer (no dependencies)
1. Game State / Phase State Machine — orchestrates day -> choice -> night transitions for all systems.
2. NPC/Soul Data Model — provides stable identity/state for selection and consequence transformation.

### Foundation Layer (depends on Foundation)
3. Map & Spawn Director — depends on: Game State / Phase State Machine (day/night map selection).
4. Ward Timer / Nước Mắm Cốt — depends on: Game State / Phase State Machine (countdown only at night), NPC/Soul Data Model (saved count → initial ward).
5. Shadow Spatial Management — depends on: Game State / Phase State Machine (only active during day).
6. Day/Night Camera Transition — depends on: Game State / Phase State Machine (trigger on phase change).

### Core Layer (depends on Foundation)
7. Player Controller & Skills — depends on: Game State / Phase State Machine, Map & Spawn Director, Day/Night Camera Transition.
8. Physical Crowding & Push — depends on: NPC/Soul Data Model, Map & Spawn Director, Game State / Phase State Machine.
9. Tactile Rituals — depends on: Game State / Phase State Machine, NPC/Soul Data Model, Ward Timer / Nước Mắm Cốt.
10. Day Service & Selection — depends on: NPC/Soul Data Model, Game State / Phase State Machine, Tactile Rituals.
11. Shrine Objective & Win/Lose Rules — depends on: Game State / Phase State Machine, Player Controller & Skills, Ward Timer / Nước Mắm Cốt.
12. Sensory Feedback System — depends on: Ward Timer / Nước Mắm Cốt, Game State / Phase State Machine.
13. Resource Effects & Hương Hỏa — depends on: NPC/Soul Data Model, Game State / Phase State Machine, Day Service & Selection.

### Feature Layer (depends on Core)
14. Consequence Resolver — depends on: Day Service & Selection, NPC/Soul Data Model, Game State / Phase State Machine.
15. Curse Effect Modules — depends on: Consequence Resolver, Player Controller & Skills, Map & Spawn Director, Ward Timer / Nước Mắm Cốt.
16. Boss Cá Ông Searchlight — depends on: Player Controller & Skills, Map & Spawn Director, Game State / Phase State Machine, Ward Timer / Nước Mắm Cốt.
17. Ngọc Cốt / Relic System — depends on: Player Controller & Skills, Ward Timer / Nước Mắm Cốt, Game State / Phase State Machine.
18. Night Survival Run — depends on: Shrine Objective & Win/Lose Rules, Curse Effect Modules, Boss Cá Ông Searchlight, Ngọc Cốt / Relic System, Resource Effects & Hương Hỏa.

### Presentation Layer (depends on Features)
19. HUD-less Design & Sensory Feedback — depends on: Ward Timer / Nước Mắm Cốt, Sensory Feedback System, Night Survival Run.
20. Audio State Director — depends on: Game State / Phase State Machine, Boss Cá Ông Searchlight, Night Survival Run.

### Polish Layer (depends on everything)
21. Save Seed / Run Reset — depends on: Game State / Phase State Machine, NPC/Soul Data Model, Map & Spawn Director, Day Service & Selection, Consequence Resolver, Night Survival Run.

---

## Recommended Design Order

| Order | System | Priority | Layer | Agent(s) | Est. Effort |
|-------|---------|----------|-------|----------|-------------|
| 1 | Game State / Phase State Machine | MVP | Foundation | game-designer | M |
| 2 | NPC/Soul Data Model | MVP | Foundation | game-designer | S |
| 3 | Map & Spawn Director | MVP | Foundation | game-designer | S |
| 4 | Ward Timer / Nước Mắm Cốt | MVP | Foundation | systems-designer | S |
| 5 | Shadow Spatial Management | MVP | Foundation | systems-designer | M |
| 6 | Day/Night Camera Transition | MVP | Foundation | gameplay-programmer | S |
| 7 | Player Controller & Skills | MVP | Core | gameplay-programmer, game-designer | M |
| 8 | Physical Crowding & Push | MVP | Core | gameplay-programmer | M |
| 9 | Tactile Rituals | MVP | Core | game-designer, ui-programmer | M |
| 10 | Day Service & Selection | MVP | Core | game-designer, ui-programmer | M |
| 11 | Shrine Objective & Win/Lose Rules | MVP | Core | game-designer | S |
| 12 | Sensory Feedback System | MVP | Core | systems-designer | S |
| 13 | Consequence Resolver | MVP | Feature | systems-designer, game-designer | M |
| 14 | Curse Effect Modules | MVP | Feature | systems-designer, gameplay-programmer | M |
| 15 | Boss Cá Ông Searchlight | MVP | Feature | ai-programmer, technical-artist | M |
| 16 | Night Survival Run | MVP | Feature | game-designer, gameplay-programmer | L |
| 17 | Resource Effects & Hương Hỏa | Vertical Slice | Core | systems-designer | S |
| 18 | Ngọc Cốt / Relic System | Vertical Slice | Feature | systems-designer | S |
| 19 | HUD-less Design & Sensory Feedback | Vertical Slice | Presentation | ui-programmer, ux-designer | M |
| 20 | Audio State Director | Vertical Slice | Presentation | sound-designer, audio-director | S |
| 21 | Save Seed / Run Reset | Alpha | Polish | gameplay-programmer | S |

---

## Circular Dependencies

- None found.

---

## High-Risk Systems

| System | Risk Type | Risk Description | Mitigation |
|--------|-----------|------------------|------------|
| Consequence Resolver | Design | If curse mapping is unclear, choices feel arbitrary instead of meaningful. | Prototype 3 fixed curse mappings early and run fast playtests for readability. |
| Night Survival Run | Scope | Combines hazards, chase, curses, and objective timing; can over-expand quickly. | Hard-lock one map and one completion condition for slice; defer variants. |
| Boss Cá Ông Searchlight | Technical | Poor pacing/tuning can make night either trivial or unfair. | Start with simple pursuit states and tune via fixed benchmark runs. |
| Ward Timer / Nước Mắm Cốt | Design | HUD-less design requires clear sensory feedback; unclear feedback = unfair deaths. | Test sensory feedback thresholds early with playtests. |
| Curse Effect Modules | Design | Multiple curses interacting can create impossible combinations. | Limit to 1-2 curses active simultaneously for MVP. |

---

## Progress Tracker

| Metric | Count |
|--------|-------|
| Total systems identified | 21 |
| Design docs started | 10 |
| Design docs reviewed | 5 |
| Design docs approved | 15 |
| MVP systems designed | 6/16 |
| Vertical Slice systems designed | 0/4 |

---

## Next Steps

- [ ] Design MVP-tier systems first (use `/design-system [system-name]`)
- [ ] Run `/design-review` on each completed GDD
- [ ] Run `/gate-check pre-production` when MVP systems are designed
- [ ] Prototype the highest-risk system early (`/prototype [system]`)
