# NPC/Soul Data Model

> **Status**: Approved
> **Author**: User + Copilot
> **Last Updated**: 2026-05-06
> **Implements Pillar**: Consequence readability and emotional ownership

## Overview

NPC/Soul Data Model is the canonical source of truth for every soul used in Solar Phobia's day/night loop. It defines identity, daily selection state, abandonment outcome, and persistent run-scoped metadata so Day Service, Consequence Resolver, and downstream survival systems consume consistent data contracts. The player does not interact with this system directly as a minigame; they interact with decisions that read/write this model. Without this system, consequences become ambiguous because different systems could disagree about who was saved, who was abandoned, and which curse should trigger.

## Player Fantasy

This system should make the player feel that every person they choose has identity and consequence, not just a slot in a mechanic. The emotional target is clarity with guilt: "I know exactly who I left behind, and the night reflects that choice." When this model is working, players trust that outcomes are consistent and fair, which reinforces ownership rather than randomness.

## Detailed Design

### Core Rules

1. The vertical slice defines exactly three canonical souls: `Linh`, `Van`, `Minh`; each has a unique immutable `SoulId`.
2. `SoulId` is the only authority key used across systems; display names/localized strings are metadata, never lookup keys.
3. At run start, each soul is initialized with `DaySelectionState = Unselected`, `NightOutcomeState = None`, `LifeState = Alive`.
4. During `DayService`, only Day Service & Selection may write `DaySelectionState`.
5. Valid daily selection is exactly 2 souls marked `Saved` and 1 soul marked `Abandoned`.
6. On transition to `ChoiceLock`, model writes are frozen except by state-machine-authorized flows.
7. Consequence Resolver reads the frozen model once and writes `NightOutcomeState` for the abandoned soul (`Drag`, `Block`, or `FakeShrine`).
8. A soul cannot be both `Saved` and `Abandoned` in the same run; contradictory writes are rejected.
9. Reset clears run-scoped fields (`DaySelectionState`, `NightOutcomeState`, transient tags) and restores default values for a new loop.
10. Canonical fallback order for deterministic auto-complete is `Linh -> Van -> Minh` (from Game State GDD contract).

### States and Transitions

| State Field | Allowed Values | Entry Condition | Exit Condition | Owner |
|-------------|----------------|-----------------|----------------|-------|
| `DaySelectionState` | `Unselected`, `Saved`, `Abandoned` | Run reset or boot | ChoiceLock begins | Day Service & Selection |
| `NightOutcomeState` | `None`, `Drag`, `Block`, `FakeShrine` | Run reset or boot (`None`) | Consequence resolved in ChoiceLock | Consequence Resolver |
| `LifeState` | `Alive`, `Lost` | Run reset (`Alive`) | Run failure/content event | Objective/Story-authorized systems |
| `RecordLockState` | `Writable`, `Locked` | `Writable` at DayService | `Locked` at ChoiceLock, unlock at Reset | Game State / Phase State Machine |

Transition summary:
1. `Unselected -> Saved` or `Unselected -> Abandoned` only in DayService.
2. `Saved/Abandoned` cannot transition again after `RecordLockState = Locked`.
3. `NightOutcomeState: None -> OutcomeType` only once per run.
4. Any transition violating phase lock is rejected with `InvalidPhaseWrite`.

### Interactions with Other Systems

- **Game State / Phase State Machine -> NPC Model**: controls lock/unlock lifecycle and phase write permissions.
- **Day Service & Selection <-> NPC Model**: writes per-soul day selection, reads current counts and validity.
- **Consequence Resolver <-> NPC Model**: reads abandoned soul identity; writes resolved curse mapping to `NightOutcomeState`.
- **Resource Effects <- NPC Model**: reads saved soul set to compute available night resources.
- **Save Seed / Run Reset <-> NPC Model**: snapshots model for analytics/reset; restores defaults for next loop.

Contract events:
- `OnSelectionChanged(SoulId, NewDaySelectionState)`
- `OnSelectionValidated(SavedCount, AbandonedCount, IsValid)`
- `OnNightOutcomeAssigned(SoulId, NightOutcomeState)`
- `OnModelReset()`

## Formulas

### Selection Validity

```
is_selection_valid = (saved_count == required_saved_count) AND (abandoned_count == total_soul_count - required_saved_count)
```

| Variable | Type | Range | Source | Description |
|----------|------|-------|--------|-------------|
| saved_count | int | 0-3 | runtime | Number of souls marked `Saved` |
| abandoned_count | int | 0-3 | runtime | Number of souls marked `Abandoned` |
| required_saved_count | int | 1-2 | config | Required saved soul count (vertical slice: 2) |
| total_soul_count | int | 2-3 | config | Total active souls in current loop (vertical slice: 3) |

**Expected output range**: boolean  
**Edge case**: if `total_soul_count < required_saved_count + 1`, force invalid and emit `ConfigInvalid`.

### Soul Priority Score (deterministic tie-break support)

```
priority_score = base_priority + chapter_modifier
```

| Variable | Type | Range | Source | Description |
|----------|------|-------|--------|-------------|
| base_priority | int | 1-3 | data | Default order (`Linh=1, Van=2, Minh=3`) |
| chapter_modifier | int | -2 to 2 | data (optional) | Future chapter-level override |

**Expected output range**: -1 to 5  
**Edge case**: equal scores resolve by static `SoulId` lexical order.

### Data Health Ratio (debug metric)

```
data_health_ratio = valid_record_count / total_record_count
```

| Variable | Type | Range | Source | Description |
|----------|------|-------|--------|-------------|
| valid_record_count | int | 0-3 | validation pass | Records passing schema checks |
| total_record_count | int | 1-3 | runtime | Active records in loop |

**Expected output range**: 0.0-1.0  
**Edge case**: if `total_record_count == 0`, force `data_health_ratio = 0` and emit fatal validation error.

## Edge Cases

| Scenario | Expected Behavior | Rationale |
|----------|-------------------|-----------|
| Duplicate `SoulId` detected at load | Reject model initialization and enter fatal config error | Prevents undefined identity routing |
| Missing soul record during selection write | Reject write, emit `MissingSoulRecord`, keep prior valid state | Avoids corrupt partial updates |
| Two systems try to write `DaySelectionState` in same frame | Respect owner priority (Day Service only), reject other writes | Preserves ownership boundaries |
| Selection reaches `3 Saved, 0 Abandoned` | Mark invalid; block transition to ChoiceLock | Maintains consequence requirement |
| Reset requested during ChoiceLock | Queue reset until Resolve completes, then apply | Prevents half-committed consequence state |
| Consequence writes `NightOutcomeState` for a saved soul | Reject write and emit `OutcomeTargetMismatch` | Enforces abandoned-soul-only consequence |
| `total_soul_count` configured to 2 for cut scope | Auto-adjust valid pattern to `1 Saved, 1 Abandoned` | Supports scoped-down fallback without code rewrite |

## Dependencies

| System | Direction | Nature of Dependency |
|--------|-----------|---------------------|
| Game State / Phase State Machine | Depends on this and this depends on it | **Hard**: phase lock contract controls write windows; model provides deterministic selection payload |
| Day Service & Selection | Depends on this | **Hard**: reads/writes day selection states and validity |
| Consequence Resolver | Depends on this | **Hard**: reads abandoned identity and writes night outcome mapping |
| Resource Effects | Depends on this | **Soft**: uses saved set for resource grant logic |
| Save Seed / Run Reset | Depends on this | **Hard**: needs full model snapshot and reset operation |
| Map & Spawn Director | This depends on it | **Hard**: provides validated spawn bundle for night start |

Interface ownership:
- **Model owns** schema, validation, lock enforcement.
- **Day Service owns** day selection decisions.
- **Consequence Resolver owns** night outcome assignment for abandoned soul.

## Tuning Knobs

| Parameter | Current Value | Safe Range | Effect of Increase | Effect of Decrease |
|-----------|---------------|------------|-------------------|-------------------|
| TotalSoulCount | 3 | 2-3 | More choice surface and narrative complexity | Tighter, simpler choice space |
| RequiredSavedCount | 2 | 1-2 | More protection, less sacrifice pressure | Harsher sacrifice pressure |
| DuplicateIdPolicy | `Fatal` | enum | Stricter data integrity | Higher tolerance, more runtime recovery complexity |
| MissingRecordRetryCount | 0 | 0-2 | More resilience to transient load issues | Faster fail-fast |
| PriorityOrderMode | `StaticList` | enum (`StaticList`,`ScoreBased`) | More flexible chapter-specific ordering | Fully deterministic baseline |
| SelectionWriteDebounceMs | 50 | 0-200 | Fewer accidental multi-write races | Faster responsiveness, more race risk |
| ValidationTickRateHz | 10 | 1-30 | Faster corruption detection | Lower overhead, slower detection |

## Visual/Audio Requirements

| Event | Visual Feedback | Audio Feedback | Priority |
|-------|-----------------|----------------|----------|
| Soul selected as Saved | Portrait highlight + warm rim | Soft confirm click | High |
| Soul set to Abandoned | Portrait desaturate + warning glyph | Low, uneasy tone | High |
| Invalid selection pattern | UI pulse red on confirm | Error ping | Critical |
| Consequence assignment committed | Night preview icon appears | Single tension stinger | High |

## UI Requirements

| Information | Display Location | Update Frequency | Condition |
|-------------|------------------|------------------|-----------|
| Soul identity (name + icon) | Day decision panel | Static per loop | Always in DayService |
| Current selection state per soul | Soul card badges | Real-time | DayService |
| Saved/Abandoned count and validity | Top of decision panel | Real-time | DayService and ChoiceLock |
| Locked state indicator | Panel header lock icon | On phase change | ChoiceLock onward |
| Consequence preview tag (optional) | Soul card footer | On lock commit | ChoiceLock |

## Acceptance Criteria

- [ ] Model initializes exactly 3 unique records (`Linh`, `Van`, `Minh`) for default vertical slice.
- [ ] Selection validity only passes when pattern is `2 Saved, 1 Abandoned` (or configured equivalent).
- [ ] Out-of-phase writes are rejected with explicit reason code.
- [ ] Consequence Resolver can write `NightOutcomeState` only for the abandoned soul.
- [ ] Reset always returns model fields to run-start defaults with no stale flags.
- [ ] Duplicate/missing record cases produce deterministic, logged outcomes per policy.
- [ ] `SelectionConfirmed(payload)` contains deterministic order and stable `SoulId` references.
- [ ] Data access performance: per-record lookup completes within 0.05 ms average on target PC.

## Open Questions

| Question | Owner | Deadline | Resolution |
|----------|-------|----------|------------|
| Should chapter content switch to `ScoreBased` priority mode, or keep static order for full game? | Game Designer | Before Act 2 planning | Open |
| Should consequence preview be hidden until ChoiceLock for stronger surprise? | Creative Director | Before first external playtest | Open |
| Should `LifeState` be expanded beyond `Alive/Lost` for long-form narrative branches? | Narrative Director | Before post-jam scope expansion | Open |
