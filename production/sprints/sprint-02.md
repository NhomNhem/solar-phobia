# Sprint 2 -- 2026-05-12 to 2026-05-18

## Sprint Goal
Resolve gate blockers (Camera Transition, Consequence Resolver) and deliver a complete Day→Night playable loop.

## Capacity
- Total days: 7
- Buffer (20%): 1.4 days reserved for unplanned work
- Available: 5.6 days

## Tasks

### 🚩 Blocker Resolution (Highest Priority)
*Gate check findings: Camera Transition + Consequence Resolver are structural gaps blocking the full day/night loop.*

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|-------------|-------------------|
| SP2-001 | **Day/Night Camera Transition** — R3-driven controller subscribing to `OnPhaseChanged` for abrupt zoom, color swap, vignette. Side-scroll follow at night via DOTween. | unity-specialist | 1.5 | Phase State Machine, ADR-0010, story-001 | 7 acceptance criteria from GDD (Day fixed, Night follow, 0.5s abrupt transition, mouse-look, Resolve cinematics, ChoiceLock locked, phase-triggered) |
| SP2-002 | **Consequence Resolver** — Deterministic Dictionary mapping abandoned SoulId → `NightOutcomeState` (Drag/Block/FakeShrine). One-write guard. Payload to CurseEffectManager + MapSpawnDirector. | unity-specialist | 0.5 | Phase State Machine, Ghost entity, ADR-0007, story-001 | All 6 acceptance criteria from GDD (mapping, one-write, invalid ID, duplicate reject, payload delivery, Ghost integration) |

### Must Have (Critical Path)

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|-------------|-------------------|
| SP2-003 | Day Service & Selection UI (soul cards, 2-of-3 validation, confirm, ChoiceLock trigger) | unity-specialist | 1.5 | Phase State Machine, Soul Repository | 2-of-3 selection works, confirm sends payload, ChoiceLock fires |
| SP2-004 | Curse Effect: Drag / Water Trap (Linh's consequence) | unity-specialist | 1.0 | SP2-002, Map & Spawn Director | Water trap spawns during NightSurvival, player drag mechanic works |

### Should Have

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|-------------|-------------------|
| SP2-005 | Ward Timer (countdown with visual feedback, death at 0) | unity-specialist | 1.5 | Phase State Machine, ADR-0005 | Timer counts down, <25% triggers visual distortion, 0 = death |
| SP2-006 | Shrine arrival + win condition (EndShrine E-key triggers Resolve) | unity-specialist | 0.5 | Player Controller, Phase State Machine, SP2-001 | E-key at EndShrine triggers ShrineArrival phase, camera pans |

### Nice to Have

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|-------------|-------------------|
| SP2-007 | Night→Day reset flow (Reset phase returns to DayService) | unity-specialist | 0.5 | SP2-001, SP2-003 | After death/win, reset to DayService with clean state |

## Carryover from Previous Sprint
None — Sprint 1 completed all tasks.

## Risks

| Risk | Probability | Impact | Mitigation |
|------|------------|--------|------------|
| Camera transition performance (0.5s abrupt zoom) | Low | High | Profile camera tweens early, use DOTween over manual Lerp |
| Day Service UI scope creep (full tactile rituals vs basic selection) | Medium | Medium | GDD defines "basic" as selection-only for milestone; defer rituals |
| Curse system integration with Map Spawn Director | Medium | Medium | Map Director stories already Complete — verify interface compatibility |
| Ward Timer death triggers overlap with Shrine win detection | Low | Medium | Define clear priority: shrine arrival wins even if timer is at 0? |
| Consequence Resolver payload not reaching CurseEffectManager | Low | High | Wire via direct method call first, refactor to R3 Subject if needed |

## Dependencies on External Factors
- None — ADR-0005 (Ward Timer), ADR-0007 (Consequence Resolver), ADR-0010 (Camera Transition) all exist
- Story files created: `day-night-camera-transition/story-001`, `consequence-resolver/story-001`

## Definition of Done for this Sprint
- [ ] All Blocker Resolution tasks completed (SP2-001, SP2-002)
- [ ] All Must Have tasks completed
- [ ] All tasks pass acceptance criteria
- [ ] No S1 or S2 bugs in delivered features
- [ ] Design documents updated for any deviations
- [ ] Code reviewed and merged
