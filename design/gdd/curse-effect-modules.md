# Curse Effect Modules

> **Status**: Approved
> **Author**: User + opencode
> **Last Updated**: 2026-05-07
> **Implements Pillar**: Consequence-driven survival loop with day/night emotional contrast

## Overview

Curse Effect Modules control the karma-based hazards that spawn during Night Phase based on which soul was abandoned during Day Phase. Each curse (Drag/Block/FakeShrine) manifests as specific environmental hazards with unique visual and audio signatures. Without this system, the abandoned soul has no mechanical consequence—choices feel arbitrary rather than meaningful.

## Player Fantasy

The player should feel **haunted specificity**—not generic danger, but *this particular* horror because of *this particular* choice. The water trap feels like Linh's drowning. The blood net feels like Van's capture. The illusion platform feels like Minh's betrayal. The horror is personal and deserved.

## Detailed Design

### Core Rules

1. **Curse Type Detection**: Receives curse_type (Drag/Block/FakeShrine) from Consequence Resolver at night start.
2. **Hazard Spawning**: Spawns corresponding hazard types based on curse:
   - **Drag (Linh abandoned)**: Water Trap zones (Vũng Nước Tử Thần) - DoT when standing in water
   - **Block (Van abandoned)**: Blood Net obstacles (Lưới Máu) - slow + penalty on contact
   - **FakeShrine (Minh abandoned)**: Illusion platforms (Bệ Đá Ảo Ảnh) - collapse after 0.2s
3. **Visual Mapping**: Each curse has distinct VFX (blue/red/shimmer) and audio (drowning/chain/rumble)
4. **Intensity**: Fixed at 1.0 for MVP (expandable to scale with game progress)

### States and Transitions

| State | Entry Condition | Exit Condition | Behavior |
|-------|-----------------|----------------|----------|
| Idle | Not NightSurvival | NightSurvival starts | No hazards spawned |
| CurseActive | NightSurvival + curse received | Night ends | Spawn and manage hazards |
| HazardTriggered | Player enters hazard zone | Hazard resolves | Apply effect (DoT/slow/collapse) |
| HazardCleared | Player exits hazard zone | Hazard inactive | Remove effect |

### Interactions with Other Systems

- **Consequence Resolver -> Curse Effect Modules**: Sends curse_type payload.
- **Night Survival Run <- Curse Effect Modules**: Hazards active during night.
- **Map & Spawn Director <- Curse Effect Modules**: Receives spawn bias for hazard placement.
- **Sensory Feedback System <- Curse Effect Modules**: Curse affects audio/visual degradation.

## Formulas

### Water Trap DoT (Drag - Linh)
```
water_damage_per_sec = -3.0 * deltaTime
```

### Blood Net Penalty (Block - Van)
```
blood_net_penalty = -5.0  // immediate
blood_net_slow = 0.5  // 50% for 3 seconds
```

### Illusion Platform Collapse (FakeShrine - Minh)
```
collapse_delay_sec = 0.2
```

## Edge Cases

| Scenario | Expected Behavior | Rationale |
|----------|-------------------|-----------|
| Multiple curses (impossible per design) | First curse only | One soul abandoned per run |
| Player already in hazard when curse triggers | Hazard activates immediately | No delay on entry |
| Hazard spawns on player | Hazard triggers immediately | Immediate feedback |

## Dependencies

| System | Direction | Nature of Dependency |
|--------|-----------|---------------------|
| Consequence Resolver | This depends on it | **Hard**: receives curse_type payload |
| Night Survival Run | This depends on it | **Hard**: hazards active during night |
| Map & Spawn Director | This depends on it | **Soft**: spawn bias coordination |
| Sensory Feedback System | Depends on this | **Soft**: curse affects sensory tier |

## Tuning Knobs

| Parameter | Current Value | Safe Range | Effect of Increase | Effect of Decrease |
|-----------|---------------|------------|-------------------|-------------------|
| `water_trap_dps` | 3.0 | 1.0-5.0 | More damage in water | Less damage |
| `blood_net_penalty` | 5.0 | 2.0-10.0 | More Ward loss | Less Ward loss |
| `blood_net_slow_pct` | 50 | 30-70 | More slow | Less slow |
| `illusion_delay` | 0.2 | 0.1-0.5 | Faster collapse | Slower collapse |

## Visual/Audio Requirements

| Curse | Visual | Audio |
|-------|--------|-------|
| Drag (Linh) | Blue water glow, ripple VFX | Bubbling, drowning sounds |
| Block (Van) | Red blood net overlay, chains | Chain rattling, fabric tear |
| FakeShrine (Minh) | Shimmer effect, ground cracks | Ground rumble, deceptive whisper |

## Acceptance Criteria

- [ ] Correct hazard spawns based on abandoned soul
- [ ] Water trap applies -3s/s DoT
- [ ] Blood net applies -5s + 50% slow for 3s
- [ ] Illusion platform collapses 0.2s after stepped on
- [ ] Distinct visual effects per curse
- [ ] Distinct audio per curse

## Open Questions

| Question | Owner | Deadline | Resolution |
|----------|-------|----------|------------|
| Should curse intensity scale with game progress? | Game Designer | Before expansion | Open