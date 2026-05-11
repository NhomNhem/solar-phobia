# Sprint 1 -- 2026-05-07 to 2026-05-14

## Sprint Goal
Establish the foundation layer: implement Phase State Machine and Soul Data Repository that all other systems depend on.

## Capacity
- Total days: 7
- Buffer (20%): 1.4 days reserved for unplanned work
- Available: 5.6 days

## Tasks

### Must Have (Critical Path)

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|-------------|-------------------|
| SP1-001 | Create Phase State Machine with all 7 states (Bootstrapping, DayService, ChoiceLock, NightSurvival, Resolve, Reset, FatalError) | unity-specialist | 2.0 | — | Phase transitions work, events fire correctly |
| SP1-002 | Create Soul Data Repository (3 souls: Linh, Van, Minh) with phase-locked writes | unity-specialist | 1.5 | SP1-001 | GetSoul, GetSavedSouls, GetAbandonedSoul all work |
| SP1-003 | Create VContainer installers for core systems | unity-specialist | 0.5 | SP1-001, SP1-002 | All registered services resolve correctly |
| SP1-004 | Create R3 reactive bindings for phase state | unity-specialist | 1.0 | SP1-001 | CurrentPhase observable fires on state changes |

### Should Have

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|-------------|-------------------|
| SP1-005 | Write unit tests for Phase State Machine | unity-specialist | 0.5 | SP1-001 | 80% coverage, all transitions tested |
| SP1-006 | Write unit tests for Soul Repository | unity-specialist | 0.5 | SP1-002 | 80% coverage, phase locking tested |

### Nice to Have

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|-------------|-------------------|
| SP1-007 | Setup basic scene with VContainer bootstrap | unity-specialist | 0.5 | SP1-003 | Scene loads without errors |

## Carryover from Previous Sprint
N/A - First sprint

## Risks

| Risk | Probability | Impact | Mitigation |
|------|------------|--------|------------|
| R3 API unfamiliar | Medium | High | Reference ADR-0001 patterns, check current-best-practices.md |
| VContainer setup complexity | Low | Medium | Follow ADR patterns exactly |

## Dependencies on External Factors
- None - this is the foundation layer

## Definition of Done for this Sprint
- [ ] All Must Have tasks completed
- [ ] All tasks pass acceptance criteria
- [ ] No S1 or S2 bugs in delivered features
- [ ] Design documents updated for any deviations
- [ ] Code reviewed and merged

## Notes
- Per ADR-0001: Phase State Machine must use R3 ReactiveProperty
- Per ADR-0002: Soul Repository must use Dictionary for O(1) lookup
- Target: 5.6 days of work, 1.4 days buffer
- Stories will be created from this sprint plan