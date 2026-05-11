# Story 001: Selection Logic & Validation

> **Epic**: day-service-and-selection
> **Status**: Complete
> **Layer**: Core
> **Type**: Logic
> **Manifest Version**: N/A (control-manifest not yet created)
> **Estimate**: 0.5 days (S)

## Context

**GDD**: `design/gdd/day-service-and-selection.md`
**Requirement**: TR-day-service-??? (not yet registered in tr-registry.yaml)

**ADR Governing Implementation**: None — this system has no ADR yet. Follow project conventions: clean architecture layering, VContainer DI, R3 reactive state.

**Engine**: Unity 6000.3.11f1 (Unity 6) | **Risk**: LOW
**Engine Notes**: Pure logic class — no Unity API dependency except for test assertions. VContainer register as singleton.

---

## Acceptance Criteria

*From GDD `design/gdd/day-service-and-selection.md`, scoped to this story:*

- [ ] **AC-1 Selection Validation**: Valid pattern = exactly 2 `Saved`, 1 `Abandoned`. Any other pattern = invalid.
- [ ] **AC-5 Auto-Complete**: On idle timeout (15s configurable), auto-select using priority order Linh→Van→Minh; mark 2 Saved, 1 Abandoned.
- [ ] **AC-6 Invalid Pattern Block**: 3 Saved/0 Abandoned, 1 Saved/2 Abandoned, 3 Abandoned/0 Saved = block confirm.
- [ ] **AC-8 Performance**: Per-soul validation completes within 0.05ms average on target PC.
- [ ] **State Tracking**: Selection state per soul (Saved/Abandoned/Unselected) tracked and queryable.

---

## Implementation Notes

### Core Validation

```csharp
public class DaySelectionValidator
{
    private const int RequiredSaved = 2;
    private const int TotalSouls = 3;

    public SelectionValidationResult Validate(IReadOnlyList<SoulSelectionState> selections)
    {
        int saved = selections.Count(s => s.State == SelectionState.Saved);
        int abandoned = selections.Count(s => s.State == SelectionState.Abandoned);

        bool isValid = saved == RequiredSaved && abandoned == TotalSouls - RequiredSaved;

        return new SelectionValidationResult
        {
            IsValid = isValid,
            SavedCount = saved,
            AbandonedCount = abandoned,
            TotalSouls = TotalSouls,
            ErrorMessage = isValid ? null : $"Need exactly {RequiredSaved} Saved, {TotalSouls - RequiredSaved} Abandoned"
        };
    }
}
```

### Auto-Complete Logic

Priority order: Linh (Saved) → Van (Saved) → Minh (Abandoned). Uses configurable timeout (`auto_complete_timeout_sec`, default 15.0).

### Data Models

```
SoulSelectionState { SoulId, SelectionState (Saved/Abandoned/Unselected) }
SelectionValidationResult { IsValid, SavedCount, AbandonedCount, TotalSouls, ErrorMessage }
```

### Phase Gating

This story handles validation and auto-complete logic only. UI integration and phase gating belong to Story 002.

### Performance

Pure logic: dictionary lookups and integer counts. O(n) where n ≤ 3. Well within 0.05ms.

---

## Out of Scope

- **Story 002 (Selection UI)**: Soul card rendering, selection state visualization, confirm button, phase gating
- **Story 003 (Ritual Assignment)**: Tea/incense/offering drag mechanics, Resource Effects

---

## QA Test Cases

- **AC-1: Selection Validation**
  - Given: 2 Saved, 1 Abandoned selection
  - When: Validate() is called
  - Then: IsValid == true
  - Edge cases: 3 Saved/0 Abandoned, 1 Saved/2 Abandoned, 0 Saved/3 Abandoned, all Unselected

- **AC-5: Auto-Complete**
  - Given: idle timeout expires with no valid selection
  - When: AutoComplete() is called
  - Then: Linh and Van marked Saved, Minh marked Abandoned
  - Edge case: timeout fires after partial selection (1 Saved, 0 Abandoned)

- **AC-6: Invalid Pattern Block**
  - Given: 3 Saved, 0 Abandoned
  - When: Validate() is called
  - Then: IsValid == false, ErrorMessage set
  - Edge cases: all abandoned, mixed invalid

---

## Test Evidence

**Story Type**: Logic
**Required evidence**: `Editor/Tests/DayServiceSelectionLogicTests.cs` — must exist and pass

**Status**: [x] Created and passing — 21/21 tests pass

---

## Completion Notes
**Completed**: 2026-05-11
**Criteria**: 5/5 passing (21 automated tests, all passing)
**Deviations**: None
**Test Evidence**: Logic — `Assets/_Project/Application/Editor/Tests/DayServiceSelectionLogicTests.cs` (21/21 passing)
**Code Review**: Skipped (Lean mode)

---

## Dependencies

- Depends on: None (pure logic, no DI required)
- Unlocks: Story 002 (Selection UI)
