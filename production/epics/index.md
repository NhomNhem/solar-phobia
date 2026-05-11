# Epics Index

> **Last Updated**: 2026-05-07
> **Engine**: Unity 6000.3.11f1 (Unity 6)

---

## All Epics

| Epic | Layer | System | GDD | Stories | Status |
|------|-------|--------|-----|---------|--------|
| phase-state-machine | Foundation | Game State / Phase State Machine | game-state-phase-state-machine.md | Not yet created | Ready |
| soul-data-repository | Foundation | NPC/Soul Data Model | npc-soul-data-model.md | Not yet created | Ready |
| map-spawn-director | Foundation | Map & Spawn Director | map-and-spawn-director.md | Not yet created | Ready (ADR-0006 ✅) |
| ward-timer | Foundation | Ward Timer / Nước Mắm Cốt | health-stamina-damage-rules.md | Not yet created | Ready (ADR-0005 ✅) |
| shadow-spatial-management | Foundation | Shadow Spatial Management | shadow-spatial-management.md | Not yet created | Ready (ADR-0009 ✅) |
| day-night-camera-transition | Foundation | Day/Night Camera Transition | day-night-camera-transition.md | Not yet created | Ready (ADR-0010 ✅) |
| player-controller | Core | Player Controller & Skills | player-controller.md | Not yet created | Ready |
| physical-crowding-and-push | Core | Physical Crowding & Push | physical-crowding-and-push.md | Not yet created | Ready |
| tactile-rituals | Core | Tactile Rituals | tactile-rituals.md | Not yet created | Ready |
| day-service-and-selection | Core | Day Service & Selection | day-service-and-selection.md | Not yet created | Ready |
| shrine-objective-win-lose-rules | Core | Shrine Objective & Win/Lose Rules | shrine-objective-win-lose-rules.md | Not yet created | Ready |
| sensory-feedback-system | Core | Sensory Feedback System | sensory-feedback-system.md | Not yet created | Ready |
| consequence-resolver | Feature | Consequence Resolver | consequence-resolver.md | Not yet created | Ready |
| curse-effect-modules | Feature | Curse Effect Modules | curse-effect-modules.md | Not yet created | Ready |
| boss-ca-ong-searchlight | Feature | Boss Cá Ông Searchlight | boss-ca-ong-searchlight.md | Not yet created | Ready |
| night-survival-run | Feature | Night Survival Run | night-survival-run.md | Not yet created | Ready (ADR-0008 ✅) |
| hud-less-design | Presentation | HUD-less Design & Sensory Feedback | (referenced in systems-index) | Not yet created | Ready |
| audio-state-director | Presentation | Audio State Director | audio-core-loop.md | Not yet created | Ready |

---

## Summary by Layer

| Layer | Total | Ready | Blocked |
|-------|-------|-------|---------|
| Foundation | 6 | 6 | 0 |
| Core | 6 | 6 | 0 |
| Feature | 5 | 5 | 0 |
| Presentation | 2 | 2 | 0 |
| **Total** | **19** | **19** | **0** |

---

## All ADRs Present

All 10 ADRs exist in `docs/architecture/`:

| ADR | Title | Epic Coverage |
|-----|-------|----------------|
| ADR-0001 | Phase State Machine Architecture | phase-state-machine |
| ADR-0002 | Soul Data Repository Pattern | soul-data-repository |
| ADR-0003 | Player Controller Pattern | player-controller |
| ADR-0004 | Coding Standards & Scene Folder Structure | (project-wide) |
| ADR-0005 | Ward Timer Implementation | ward-timer |
| ADR-0006 | Map Generation Strategy | map-spawn-director |
| ADR-0007 | Consequence Resolver Pattern | consequence-resolver |
| ADR-0008 | Night Survival Run Architecture | night-survival-run |
| ADR-0009 | Shadow Spatial Management | shadow-spatial-management |
| ADR-0010 | Day/Night Camera Transition | day-night-camera-transition |

---

## Next Steps

1. Create stories for all 19 epics: `/create-stories [epic-slug]`
2. Run `/gate-check pre-production` when Foundation + Core epics are complete

---

## Usage

Run `/create-stories [epic-slug]` for each epic to break it into implementable stories.

Example:
```
/create-stories phase-state-machine
/create-stories soul-data-repository
/create-stories player-controller
```