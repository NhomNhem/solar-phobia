# Story 003: Ritual Assignment

> **Epic**: day-service-and-selection
> **Status**: Complete
> **Layer**: Core
> **Type**: Integration
> **Manifest Version**: N/A (control-manifest not yet created)
> **Estimate**: 0.5 days (S)

## Context

**GDD**: `design/gdd/day-service-and-selection.md`
**Requirement**: TR-day-service-??? (not yet registered in tr-registry.yaml)

**ADR Governing Implementation**: None — this system has no ADR yet. Follow project conventions: clean architecture layering, VContainer DI, R3 reactive state.

**Engine**: Unity 6000.3.11f1 (Unity 6) | **Risk**: MEDIUM
**Engine Notes**: UI Toolkit for ritual interaction (drag from source to soul card). VContainer for Resource Effects binding. Note: Resource Effects has NO GDD yet — tea/incense/offering effects assumed but not formalized.

---

## ⚠️ Deferral Warning — Resolved

This story previously depended on **Resource Effects GDD** which was Not Started. That GDD has now been authored (`design/gdd/resource-effects-and-huong-hoa.md`, Approved). The `IResourceEffectApplier` interface defined in the GDD is referenced by `RitualAssignmentService` with graceful degradation fallback.

---

## Acceptance Criteria

*From GDD `design/gdd/day-service-and-selection.md`, scoped to this story:*

- [x] **AC-2 Ritual Assignment**: `RitualAssignmentService.TryAssignRitual(soulId, ritual)` assigns tea/incense/offering to any valid soul. Controller gates by phase and abandoned state. Each soul receives one ritual (reassignment changes it). Tests: `AssignRitual_*` (7 service + 7 integration).
- [x] **Preferred Ritual Bonus**: `IsPreferredRitual()` maps Linh→Tea, Van→Incense, Minh→Offering. Preferred flag passed to `IResourceEffectApplier` for 1.5x multiplier. Tests: `IsPreferredRitual_*` (4 service + 2 integration).
- [x] **Visual Feedback**: Soul cards have `ritual-icon` label element. Ritual source container added to UXML with Tea/Incense/Offering source elements. USS styles for `.ritual-source`, `.ritual-icon`.
- [x] **Ritual Removal**: `TryRemoveRitual()` removes assignment before confirm. Controller gates removal after confirm. Tests: `RemoveRitual_*` (3 service + 2 integration).
- [x] **Graceful Degradation**: `RitualAssignmentService()` (no-arg constructor) runs without `IResourceEffectApplier` — logs warning, assigns visually, no crash. Tests: `GracefulDegradation_*` (2 tests).

---

## Implementation Notes

### Ritual Types

```
RitualType.Tea (Diêm)     → light bonus → +Ward Timer seconds
RitualType.Incense (Rot)  → safe zone   → creates safe zone at saved soul's position
RitualType.Offering (Vây) → skill       → grants skill bonus for night phase
```

### Preferred Ritual Mapping

| Soul | Preferred Ritual | Bonus |
|------|-----------------|-------|
| Linh | Tea (Diêm) | +50% Ward Timer bonus |
| Van | Incense (Rot) | +50% safe zone radius |
| Minh | Offering (Vây) | +50% skill duration |

### UI Interaction

Three ritual source elements at bottom of screen (tea pot, incense bundle, offering bowl). Player drags onto soul card. Drop target highlights on valid hover. Visual feedback on successful assignment.

### Resource Effects Integration

```csharp
public interface IResourceEffectApplier
{
    void ApplyTeaEffect(string soulId, float multiplier);
    void ApplyIncenseEffect(string soulId, float multiplier);
    void ApplyOfferingEffect(string soulId, float multiplier);
}
```

If `IResourceEffectApplier` is not registered in DI container (graceful degradation), log warning and no-op.

---

## Out of Scope

- **Story 001 (Selection Logic)**: Validation and auto-complete logic
- **Story 002 (Selection UI)**: Soul card rendering, confirm flow, phase gating
- Resource Effects GDD authoring

---

## QA Test Cases

- **AC-2: Ritual Assignment**
  - Given: valid soul card displayed
  - When: tea dragged onto Linh's card
  - Then: tea icon shown on card, Linh's ritual = Tea, bonus multiplier applied
  - Edge cases: drag to invalid target, drag back to source bin

- **Preferred Ritual Bonus**
  - Given: Linh assigned Tea (preferred)
  - When: bonus calculated
  - Then: bonus multiplier > 1.0
  - Edge case: Minh assigned Tea (non-preferred) → standard multiplier

- **Graceful Degradation**
  - Given: IResourceEffectApplier not registered
  - When: ritual assigned
  - Then: warning logged, no crash, base outcome preserved

---

## Test Evidence

**Story Type**: Integration
**Required evidence**: `Editor/Tests/RitualAssignmentTests.cs` OR playtest session log

**Status**: [x] Created — `RitualAssignmentTests.cs` (17 tests) + `DayServiceRitualIntegrationTests.cs` (18 tests + stub), all passing. Also updated/verified existing `DayServiceSelectionLogicTests.cs` (21) and `DayServiceSelectionUITests.cs` (22).

---

## Dependencies

- Depends on: Story 002 (Selection UI) — must be DONE
- Depends on: Resource Effects GDD and system (Not Started) — soft dependency
- Unlocks: Night survival buffs based on ritual choices
