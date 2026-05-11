# Manual Walkthrough: Cursor Visibility — Phase-Driven Show/Hide

> **Story**: production/epics/player-controller/story-006-cursor-visibility.md
> **TR-ID**: TR-player-007
> **Type**: UI — Manual Walkthrough
> **Status**: [ ] Not yet verified

---

## Prerequisites

- Unity Editor open with a test scene containing:
  - A `PlayerController` MonoBehaviour wired to `CursorController`
  - `PhaseStateMachine` running and accessible
- Game is in Play mode

---

## Test Steps

### AC-1: DayService shows cursor

1. Start Play mode — game initialises in `DayService` phase
2. **Observe**: OS cursor is **visible** on screen
3. **Observe**: Mouse cursor can hover over and click UI elements (NPC cards, buttons)
4. **Verify**: `Cursor.lockState == CursorLockMode.None` (cursor moves freely)

**Expected**: ✅ Cursor visible, free movement, UI clickable
**Result**: [ ] Pass  [ ] Fail
**Notes**:

---

### AC-2: NightSurvival hides cursor

1. Advance game to `NightSurvival` phase (via normal game flow or debug shortcut)
2. **Observe**: OS cursor **disappears** from screen
3. **Observe**: Moving the mouse rotates the camera (mouselook active)
4. **Verify**: `Cursor.lockState == CursorLockMode.Locked`

**Expected**: ✅ Cursor hidden, mouse controls camera look
**Result**: [ ] Pass  [ ] Fail
**Notes**:

---

### AC-3: ChoiceLock / EndingEvaluation restores cursor

1. From `NightSurvival`, advance to `EndingEvaluation` phase
2. **Observe**: OS cursor **reappears** on screen
3. **Observe**: Cursor can interact with outcome prompts / UI
4. **Verify**: `Cursor.lockState == CursorLockMode.None`

**Expected**: ✅ Cursor visible, UI accessible
**Result**: [ ] Pass  [ ] Fail
**Notes**:

---

### AC-4: Synchronous — no frame delay

1. Trigger a phase transition (e.g. DayService → NightSurvival)
2. **Observe**: Cursor state changes **in the same frame** as the phase change
3. There must be no visible "flash" of cursor before it hides

**Expected**: ✅ Instant cursor hide on NightSurvival entry
**Result**: [ ] Pass  [ ] Fail
**Notes**:

---

### Edge Case: Alt+Tab during NightSurvival

1. While in `NightSurvival`, press Alt+Tab to switch away from the game window
2. Return focus to the game window
3. **Observe**: Cursor re-hides and mouselook resumes

**Expected**: ✅ Cursor re-locked on focus return
**Result**: [ ] Pass  [ ] Fail
**Notes**:

---

## Sign-off

| Field | Value |
|-------|-------|
| Tester | |
| Date | |
| Build | |
| Verdict | [ ] PASS  [ ] FAIL  [ ] PASS WITH NOTES |
| Notes | |
