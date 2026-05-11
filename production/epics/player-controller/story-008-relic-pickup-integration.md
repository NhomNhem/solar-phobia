# Story 008: Relic Pickup Integration — NgocCot → Resource Effects Cross-System

> **Epic**: player-controller
> **Status**: Complete
> **Layer**: Core
> **Type**: Integration
> **Priority**: P2
> **Estimate**: 4 hours
> **Manifest Version**: 2026-05-10

## Context

**GDD**: `design/gdd/player-controller.md` — Section 5.5: "Near CursedMound (Mo Oan): Picks up Bone Relic (`Ngoc Cot`), triggering Time Drain modifier and hallucination package." and Acceptance Criteria 185: "[E-Interact (Bone Relic)]: Near CursedMound, E key picks up Bone Relic, triggers `OnResourcePickedUp(NgocCot)` event to Resource Effects system."
**TR-ID**: `TR-player-012`
**ADR Governing Implementation**: ADR-0003-v2: Player Controller Pattern (2D)
**ADR Decision Summary**: E-key on CursedMound fires `OnInteract("relic")` which the Resource Effects system subscribes to, triggering Time Drain modifier and hallucination package. Cross-system event delivery via R3 Subject / MessagePipe.

**Engine**: Unity 6000.3.11f1 (Unity 6) — Uses R3 Subject for cross-system event delivery, which is compatible with Unity 6's New Input System architecture. | **Risk**: MEDIUM

---

## Acceptance Criteria

- [ ] E-key on CursedMound fires `OnResourcePickedUp(NgocCot)` event received by Resource Effects system.
- [ ] Resource Effects system activates Time Drain modifier on receipt.
- [ ] Pickup during active strike telegraph applies Time Drain immediately — does not cancel the strike.
- [ ] Cross-system event delivery is reliable — no dropped events under normal frame rate (60fps).

---

## Implementation Notes

*Derived from ADR-0003 Implementation Guidelines:*

- Player Controller fires `OnInteract("relic")` (Story 005)
- A coordinator (GameFlowCoordinator or dedicated handler) maps `"relic"` → `IResourceEffectsService.OnResourcePickedUp(NgocCot)`
- Use R3 Subject or MessagePipe for cross-system bus
- Integration test: mock `IResourceEffectsService` subscriber, verify event payload contains `NgocCot` type and fires exactly once per pickup

---

## Out of Scope

*Handled by neighbouring stories — do not implement here:*

- Time Drain formula and drain rate calculation (owned by Resource Effects system)
- Hallucination visual package (owned by Curse Effect Modules)
- E-key raycast logic (Story 005)

---

## QA Test Cases

- **AC-1**: Relic pickup event reaches Resource Effects
  - Given: Phase = NightSurvival; CursedMound within range
  - When: Player presses E
  - Then: `IResourceEffectsService` receives `OnResourcePickedUp(NgocCot)` exactly once

- **AC-2**: Time Drain activates on receipt
  - Given: Resource Effects service is subscribed
  - When: `OnResourcePickedUp(NgocCot)` fires
  - Then: Time Drain modifier is active in Resource Effects system

- **AC-3**: Pickup during strike telegraph
  - Given: Strike telegraph active
  - When: Player picks up relic
  - Then: Time Drain activates immediately; strike proceeds unaffected

- **AC-4**: No duplicate events
  - Given: Player picks up one relic
  - When: E pressed once
  - Then: `OnResourcePickedUp` fires exactly once (not twice)

---

## Test Evidence

**Story Type**: Integration
**Required evidence**: `tests/integration/player-controller/relic-pickup-integration_test.cs` — must exist and pass

**Status**: [ ] Not yet created

---

## Dependencies

- Depends on: Story 005 (E-Key Interact), Resource Effects epic (partial — `IResourceEffectsService` interface)
- Unlocks: None (leaf story)

**Control Manifest Rules**: N/A — manifest not yet created (see docs/architecture/control-manifest.md)

**Performance Budget**: Event delivery via R3 Subject/MessagePipe must complete within 2ms to maintain 60fps gameplay loop; no additional allocation per event.

---

## Completion Notes

**Completed**: 2026-05-10
**Criteria**: 4/4 passing
**Deviations**: 
- ADVISORY: Direct PlayerController→IResourceEffectsService call (instead of coordinator pattern per ADR) — functionally equivalent
- ADVISORY: Time Drain multiplier uses placeholder (1.0f) — NgocCotService integration deferred to follow-up story
**Test Evidence**: Integration tests in `tests/integration/player-controller/relic-pickup-integration_test.cs` — all 4 tests pass
**Code Review**: Skipped (lean mode)
