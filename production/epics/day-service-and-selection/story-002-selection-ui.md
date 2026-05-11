# Story 002: Selection UI & Confirm Flow

> **Epic**: day-service-and-selection
> **Status**: Complete
> **Layer**: Core
> **Type**: Integration
> **Manifest Version**: N/A (control-manifest not yet created)
> **Estimate**: 1.0 days (M)

## Context

**GDD**: `design/gdd/day-service-and-selection.md`
**Requirement**: TR-day-service-??? (not yet registered in tr-registry.yaml)

**ADR Governing Implementation**: None — this system has no ADR yet. Follow project conventions: clean architecture layering, VContainer DI, R3 reactive state.

**Engine**: Unity 6000.3.11f1 (Unity 6) | **Risk**: MEDIUM
**Engine Notes**: UI Toolkit (UXML/USS) for soul card rendering. VContainer injection for Game State Machine binding. New Input System for cursor control.

**Control Manifest Rules (this layer)**:
- VContainer `[Inject]` fields must be `internal` (not private) for source generator compatibility
- Use `ReactiveProperty<T>` / `ReadOnlyReactiveProperty<T>` for observable state

---

## Acceptance Criteria

*From GDD `design/gdd/day-service-and-selection.md`, scoped to this story:*

- [x] **AC-3 Confirm Flow**: `TryConfirmSelection()` creates `SelectionConfirmedPayload` with `SavedSoulIds`, `AbandonedSoulId`, `RitualAssignments` (empty). Calls `_phaseStateMachine.TryTransition(PhaseState.ChoiceLock)`. Tests: `ConfirmFlow_*` (4 tests, all pass).
- [x] **AC-4 Phase Gating**: `OnPhaseChanged` shows UI on `DayService`, hides on `ChoiceLock` and all other phases. Resets local state on re-entry. Tests: `PhaseGating_*` (4 tests, all pass).
- [x] **AC-7 Consequence Payload**: Payload embeds `AbandonedSoulId` and `SavedSoulIds`. Tests: `ConsequencePayload_*` (2 tests, all pass).
- [x] **AC-9 Cross-System Events**: `TryConfirmSelection()` sends to `IPhaseStateMachine`. `ToggleSoulSelection()` updates `SoulRepository` via `TrySetSelection()`. Tests: `CrossSystem_*` (2 tests, all pass).
- [x] **AC-10 UI Feedback**: `IsConfirmEnabled` reactive property tracks validity via `DaySelectionValidator`. `LastValidation.ErrorMessage` available. `IResourceEffectApplier` graceful degradation logged. Tests: `UIFeedback_*` (4 tests, all pass).
- [x] **ChoiceLock Transition**: `_confirmed` flag blocks re-confirm and further toggles. UI hidden on ChoiceLock phase. Tests: `ChoiceLock_*` (2 tests), `Reset_ClearsConfirmedPayload` — all pass.

---

## Implementation Notes

### UI Structure

Use UI Toolkit (UXML + USS) for the day decision panel:
- Three soul cards (Linh, Van, Minh) with portrait, name, click-to-toggle Saved/Abandoned
- Saved/Abandoned counter display
- Validation status message
- Confirm button (enabled only when valid)
- Lock overlay (active during ChoiceLock)

### Selection Flow

1. `DayService` phase active → UI shown, Player Controller disabled (cursor visible)
2. Player clicks soul cards to toggle Saved/Abandoned state
3. `DaySelectionValidator` (from Story 001) called on each change
4. Confirm button enabled when `IsValid == true`
5. Player clicks Confirm → `SelectionConfirmed()` sent to `IPhaseStateMachine`
6. Game State Machine transitions to `ChoiceLock`
7. UI locked, overlay shown, no further interaction

### Payload Structure

```csharp
public class SelectionConfirmedPayload
{
    public List<string> SavedSoulIds { get; set; }
    public string AbandonedSoulId { get; set; }
    public Dictionary<string, string> RitualAssignments { get; set; } // SoulId -> RitualType
}
```

### Phase Gating

Register a subscription to `IPhaseStateMachine.CurrentPhase`:
- When phase == `DayService` → enable UI, disable Player Controller, show cursor
- When phase == `ChoiceLock` → disable UI, freeze payload
- All other phases → hide UI, enable Player Controller normally

### Integration Points

- `IPhaseStateMachine` (Phase State Machine) — for phase detection and `SelectionConfirmed()` call
- `ISoulRepository` (NPC/Soul Data Model) — read soul data (name, portrait) for cards
- `IGhostRepository` — write `DaySelectionState` per soul

---

## Out of Scope

- **Story 001 (Selection Logic)**: Validation and auto-complete logic
- **Story 003 (Ritual Assignment)**: Tea/incense/offering drag mechanics
- Audio: handled by Audio State Director stories (separate epic)
- Visual effects: handled by Sensory Feedback system (separate epic)

---

## QA Test Cases

- **AC-3: Confirm Flow**
  - Given: valid selection (2 Saved, 1 Abandoned)
  - When: confirm button clicked
  - Then: `SelectionConfirmed(payload)` sent with correct saved/abandoned IDs
  - Edge cases: double-click confirm, rapid confirm after selection change

- **AC-4: Phase Gating**
  - Given: phase is NOT DayService (e.g., Dialogue, Order)
  - When: UI state checked
  - Then: Day Service UI hidden, Player Controller enabled, cursor hidden
  - Edge cases: phase transition during animation

- **AC-7: Consequence Payload**
  - Given: confirm triggered with 2 Saved, 1 Abandoned
  - When: payload inspected
  - Then: AbandonedSoulId matches the abandoned soul
  - Edge cases: payload structure matches contract

- **AC-10: UI Feedback**
  - Given: soul card toggles through states
  - When: state changes
  - Then: visual feedback matches state (Saved/Abandoned/Unselected)
  - Edge cases: all unselected, rapid toggling

---

## Test Evidence

**Story Type**: Integration
**Required evidence**: `Editor/Tests/DayServiceSelectionUITests.cs` OR playtest session log

**Status**: [x] Created — `DayServiceSelectionUITests.cs` (22 tests, all passing)

---

## Dependencies

- Depends on: Story 001 (Selection Logic & Validation) — must be DONE
- Unlocks: Story 003 (Ritual Assignment), Consequence Resolver (already done)
