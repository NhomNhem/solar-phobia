# Asset Specs — System: Map & Spawn Director

> **Source**: design/gdd/map-and-spawn-director.md
> **Art Bible**: design/art/art-bible.md
> **Generated**: 2026-05-07
> **Status**: 12 assets specced

---

## ASSET-030 — Safe Mound (Mo Thuong) — Day

| Field | Value |
|-------|-------|
| Category | Environment / Cover |
| Dimensions | 128×192px (1.5m × 2.5m world) |
| Format | PNG with transparency |
| Naming | `mound_safe_day.png` |

**Visual Description**:  
A rounded, organic mound form — curved hill shape, soft edges. Warm earth tones — Burnt Sienna transitioning to Ochre Gold. The mound feels protective, welcoming. Brushstroke texture visible, like a gentle wash.

**Art Bible Anchors**:
- §3 Shape: Day = organic/curved dominance
- §4 Color: Burnt Sienna = active memory, safe elements
- §6 Environment: "Safe zone" = sparse props, breath, illusion of safety

**Generation Prompt**:  
"Top-down game environment sprite, safe mound cover object, rounded organic curved shape, soft edges, warm earth tones burnt sienna to ochre gold, watercolour brushstroke texture, protective welcoming feel, game asset"

---

## ASSET-031 — Safe Mound (Mo Thuong) — Night

| Field | Value |
|-------|-------|
| Category | Environment / Cover |
| Dimensions | 128×192px |
| Format | PNG with transparency |
| Naming | `mound_safe_night.png` |

**Visual Description**:  
Same form but now darker — edges harden, the soft curves become more angular. Charcoal Black base with ember edge glow. The cover still works but feels more dangerous.

**Art Bible Anchors**:
- §3 Shape: Night = angular/geometric dominance
- §4 Color: Charcoal Black + Ember Orange edges

**Generation Prompt**:  
"Safe mound cover at night, same rounded form but darker, charcoal black with ember orange glowing edges, angular hardened edges, watercolour fire damage effect, game asset"

---

## ASSET-032 — Cursed Mound (Mo Oan) — Day

| Field | Value |
|-------|-------|
| Category | Environment / Cover + Item |
| Dimensions | 128×192px |
| Format | PNG with transparency |
| Naming | `mound_cursed_day.png` |

**Visual Description**:  
Same basic mound shape as Safe Mound (deceptive) but with subtle differentiator — a faint moss-green glow, and what looks like a small "clay head" turning. Slight unease in the otherwise safe form.

**Art Bible Anchors**:
- §4 Color: Not from primary palette — moss-green indicates cursed/danger + reward
- §6 Environment: "Hidden hazard in clutter" — deceptive

**Generation Prompt**:  
"Cursed mound with hidden bone relic, same safe-looking form but with faint moss-green glow, small clay head silhouette turning, subtle unease, watercolour, game asset"

---

## ASSET-033 — Cursed Mound (Mo Oan) — Night

| Field | Value |
|-------|-------|
| Category | Environment / Cover + Item |
| Dimensions | 128×192px |
| Format | PNG with transparency |
| Naming | `mound_cursed_night.png` |

**Visual Description**:  
At night, the moss-green glow intensifies. The clay head is now fully visible, watching. The mound pulses with dangerous potential.

**Art Bible Anchors**:
- §4 Color: Green = danger + reward (unique to Cursed Mound)
- §2 Mood: "CursedMound proximity: Faint moss-green glow + clay head turn"

**Generation Prompt**:  
"Cursed mound night version, intensified moss-green glow, clay head fully visible watching, pulsing danger, watercolour fire damage, game asset"

---

## ASSET-034 — False Safe Mound (Trap)

| Field | Value |
|-------|-------|
| Category | Environment / Cover + Trap |
| Dimensions | 128×192px |
| Format | PNG with transparency |
| Naming | `mound_false_day.png` |

**Visual Description**:  
Identical to Safe Mound in every way — the trap is that it looks exactly the same. No visual distinction until the warning tell triggers.

**Art Bible Anchors**:
- §6 Environment: "Hidden hazard in clutter" — deceptive
- Critical: Must be indistinguishable from Safe Mound until triggered

**Generation Prompt**:  
"False safe mound, looks identical to safe mound, deceptive trap, watercolour, game asset (WARNING: must look exactly like safe mound)"

---

## ASSET-035 — False Safe Mound Warning Tell

| Field | Value |
|-------|-------|
| Category | VFX / Warning |
| Dimensions | 128×192px |
| Format | PNG with transparency |
| Naming | `mound_false_warning.png` |

**Visual Description**:  
Fast fissure cracks appear — dark lines spreading across the mound surface. Dust shake effect. This is the 0.9s warning before collapse — the player sees this and must dodge.

**Art Bible Anchors**:
- §2 Mood: "Fairness" — warning tell always precedes danger
- §4 Color: Cracks = dark, dangerous

**Generation Prompt**:  
"Mound warning tell effect, fast spreading fissure cracks, dark lines across surface, dust shake effect, danger warning, game VFX"

---

## ASSET-036 — Mound Collapse Effect

| Field | Value |
|-------|-------|
| Category | VFX / Destruction |
| Dimensions | 192×192px |
| Format | PNG with transparency |
| Naming | `mound_collapse.png` |

**Visual Description**:  
The mound crumbles — fragments flying, dust cloud. The form breaks into geometric pieces (night aesthetic). Ember particles rising from destruction.

**Art Bible Anchors**:
- §3 Shape: Destruction = fragments, geometry exposed
- §4 Color: Ember = active destruction

**Generation Prompt**:  
"Mound collapse destruction effect, fragments flying, geometric pieces, dust cloud, ember particles rising, watercolour fire destruction, game VFX"

---

## ASSET-037 — Start Shrine

| Field | Value |
|-------|-------|
| Category | Environment / Objective |
| Dimensions | 128×192px |
| Format | PNG with transparency |
| Naming | `shrine_start.png` |

**Visual Description**:  
A simple altar/niche structure — cultural reference to Vietnamese ancestor worship. Warm Ochre Gold glow. The player begins here at night start. Safe, welcoming.

**Art Bible Anchors**:
- §6 Environment: "Altar Niches" — cultural element
- §4 Color: Ochre Gold = warm, safe, the beginning

**Generation Prompt**:  
"Start shrine altar, Vietnamese cultural reference, simple niche structure, warm ochre gold glow, welcoming safe, watercolour, game environment"

---

## ASSET-038 — End Shrine

| Field | Value |
|-------|-------|
| Category | Environment / Objective |
| Dimensions | 128×192px |
| Format | PNG with transparency |
| Naming | `shrine_end.png` |

**Visual Description**:  
Same shrine structure as Start — but now it glows intensely, triumphant. Soft bloom effect. Reaching here triggers win state.

**Art Bible Anchors**:
- §2 Mood: "Exhausted salvation" — shrine as salvation
- §4 Color: Golden glow = success

**Generation Prompt**:  
"End shrine objective, same structure as start but intense golden glow, soft bloom effect, success victory, watercolour, game environment"

---

## ASSET-039 — Ground Tile (Day)

| Field | Value |
|-------|-------|
| Category | Environment / Tileset |
| Dimensions | 64×64px (tileable) |
| Format | PNG (repeatable) |
| Naming | `ground_day.png` |

**Visual Description**:  
Beach/sand ground texture — warm earth tones. Watercolour wash effect, not harsh. Subtle variation between tiles to avoid repetition. Day phase ground.

**Art Bible Anchors**:
- §6 Environment: Beach/village ground
- §2 Mood: Day = warm, safe, measured

**Generation Prompt**:  
"Ground tile, beach sand texture, warm earth tones, watercolour wash effect, subtle variation, tileable, game environment"

---

## ASSET-040 — Ground Tile (Night)

| Field | Value |
|-------|-------|
| Category | Environment / Tileset |
| Dimensions | 64×64px (tileable) |
| Format | PNG (repeatable) |
| Naming | `ground_night.png` |

**Visual Description**:  
Same ground but now dark — Charcoal Black base with ember edge hints. The ground itself feels dangerous, like walking on embers.

**Art Bible Anchors**:
- §4 Color: Charcoal Black = consumed ground
- §2 Mood: Night = scorched, dangerous

**Generation Prompt**:  
"Ground tile night version, dark charcoal black, ember edge hints, dangerous feel, watercolour fire effect, tileable, game environment"

---

## ASSET-041 — Shrine Direction Marker

| Field | Value |
|-------|-------|
| Category | UI |
| Dimensions | 48×48px |
| Format | PNG with transparency |
| Naming | `shrine_marker.png` |

**Visual Description**:  
Arrow or marker at screen edge pointing toward shrine direction. Ochre Gold (day) / Ember (night). Slight pulse to draw attention without urgency.

**Art Bible Anchors**:
- §7 UI: Edge elements feel like torn paper scraps
- §4 Color: Direction = warm = goal

**Generation Prompt**:  
"Shrine direction marker, screen edge arrow, ochre gold to ember gradient, slight pulse, watercolour style, game UI element"

---

## Audio Assets (Descriptions Only)

| Asset | Description | Trigger |
|-------|-------------|---------|
| Mound proximity whisper | Soft whisper bed increases near CursedMound | Near CursedMound |
| FalseMound warning tell | Audio crack + bone crack crescendo | Warning tell active |
| Mound collapse | Crash, rubble settling | Collapse triggers |
| Shrine ambient | Soft, safe, protective tone | At shrine |

---

## Summary

| Asset ID | Name | Category | Status |
|----------|------|----------|--------|
| ASSET-030 | Safe Mound (Day) | Environment | Needed |
| ASSET-031 | Safe Mound (Night) | Environment | Needed |
| ASSET-032 | Cursed Mound (Day) | Environment | Needed |
| ASSET-033 | Cursed Mound (Night) | Environment | Needed |
| ASSET-034 | False Mound (Day) | Environment | Needed |
| ASSET-035 | False Mound Warning | VFX | Needed |
| ASSET-036 | Mound Collapse | VFX | Needed |
| ASSET-037 | Start Shrine | Environment | Needed |
| ASSET-038 | End Shrine | Environment | Needed |
| ASSET-039 | Ground Tile (Day) | Environment | Needed |
| ASSET-040 | Ground Tile (Night) | Environment | Needed |
| ASSET-041 | Shrine Direction Marker | UI | Needed |

---

*End of Map & Spawn Director Asset Specs*