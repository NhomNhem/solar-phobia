# Playtest Report

> **Status**: Ready for session
> **Purpose**: Validate core loop and identify critical issues before Polish phase

---

## Session Info

- **Date**: _________________
- **Build**: _________________ (commit/date)
- **Duration**: _________________
- **Tester**: _________________
- **Platform**: [ ] PC  [ ] Mac  [ ] Other: _______
- **Input Method**: [ ] KB+M  [ ] Gamepad
- **Session Type**: [ ] First time  [ ] Returning  [ ] Targeted test

---

## Test Focus Areas

Based on MVP GDDs, prioritize testing these core systems:

| Priority | System | GDD Reference |
|----------|--------|----------------|
| 1 | Phase State Machine (Day → Choice → Night) | `game-state-phase-state-machine.md` |
| 2 | Day Service & Selection (who to save) | `day-service-and-selection.md` |
| 3 | Night Survival Run (consequences) | `night-survival-run.md` |
| 4 | Player Controller & Skills | `player-controller.md` |
| 5 | Ward Timer / Nước Mắm Cốt | `health-stamina-damage-rules.md` |
| 6 | Sensory Feedback (HUD-less) | `sensory-feedback-system.md` |

---

## First Impressions (First 5 minutes)

- **Understood the goal?** [ ] Yes [ ] Partially [ ] No
- **Understood the controls?** [ ] Yes [ ] Partially [ ] No  
- **Emotional response**: [ ] Engaged [ ] Confused [ ] Bored [ ] Frustrated [ ] Excited
- **Notes**: _________________________________________________

---

## Core Loop Test

### Day Phase
- [ ] Spawned into day phase correctly
- [ ] Understood objective (move souls to shadow, avoid sun)
- [ ] Controls felt responsive
- [ ] Sensory feedback was clear (danger cues)

### Choice Phase
- [ ] Understood a choice was required
- [ ] Could identify which soul to save
- [ ] Selection UI was clear

### Night Phase
- [ ] Transition to night was clear
- [ ] Understood new objective (reach shrine)
- [ ] Consequences from choice were apparent
- [ ] Hazards were fair but challenging

### Win/Lose
- [ ] Win condition (reach shrine) worked
- [ ] Lose condition (timer/catch) worked
- [ ] End state was satisfying

---

## Gameplay Flow

### What worked well
- _________________________________________________
- _________________________________________________

### Pain points
- _________________________________________________ [ ] High [ ] Medium [ ] Low
- _________________________________________________ [ ] High [ ] Medium [ ] Low

### Confusion points
- _________________________________________________
- _________________________________________________

### Moments of delight
- _________________________________________________
- _________________________________________________

---

## Feature-Specific Feedback

### Phase State Machine
- **Transitions clear?** [ ] Yes [ ] No
- **Issues**: _________________________________________________

### Day Selection
- **Decision understandable?** [ ] Yes [ ] No
- **Stakes felt real?** [ ] Yes [ ] No

### Night Survival
- **Difficulty**: [ ] Too Easy [ ] Just Right [ ] Too Hard
- **Pacing**: [ ] Too Slow [ ] Good [ ] Too Fast

### Sensory Feedback (HUD-less)
- **Timer urgency clear?** [ ] Yes [ ] No [ ] N/A
- **Danger readable?** [ ] Yes [ ] No
- **Needed UI elements**: _________________________________________________

---

## Bugs Encountered

| # | Description | Severity | Reproducible |
|---|-------------|----------|---------------|
| 1 | _________________________________ | [H][M][L] | [ ] Yes |
| 2 | _________________________________ | [H][M][L] | [ ] Yes |
| 3 | _________________________________ | [H][M][L] | [ ] Yes |

---

## Quantitative Data

- **Deaths**: ____ 
- **Time in day phase**: ____ min
- **Time in night phase**: ____ min
- **Souls saved**: ____ / ____
- **Features discovered**: _________________________________________________
- **Features missed**: _________________________________________________

---

## Overall Assessment

- **Would play again?** [ ] Yes [ ] No [ ] Maybe
- **Difficulty**: [ ] Too Easy [ ] Just Right [ ] Too Hard
- **Pacing**: [ ] Too Slow [ ] Good [ ] Too Fast
- **Session length preference**: [ ] Shorter [ ] Good [ ] Longer

---

## Top 3 Priorities from this session

1. _________________________________________________
2. _________________________________________________
3. _________________________________________________

---

## GDD Verification

Check if gameplay matches design intent from GDDs:

| GDD | Matches? | Notes |
|-----|----------|-------|
| `game-state-phase-state-machine.md` | [ ] Yes [ ] No | |
| `day-service-and-selection.md` | [ ] Yes [ ] No | |
| `night-survival-run.md` | [ ] Yes [ ] No | |
| `player-controller.md` | [ ] Yes [ ] No | |
| `health-stamina-damage-rules.md` | [ ] Yes [ ] No | |
| `sensory-feedback-system.md` | [ ] Yes [ ] No | |

---

## Readiness for Polish

**Blockers found?** [ ] Yes (list below) [ ] No

- _________________________________________________

**Verdict**: [ ] Ready for Polish  [ ] Needs fixes  [ ] Needs more testing