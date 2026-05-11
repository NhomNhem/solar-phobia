# Art Brief — Environment: Mounds, Shrines, Props
## ASSET-030 to ASSET-041 (and Village Props)

**Project**: Solar Phobia: Nắng Gắt  
**System**: Map & Spawn Director, Environment  
**Asset IDs**: ASSET-030 to ASSET-041 + village props  
**Style**: 2D Hand-painted Watercolour

---

## 1. Environment Philosophy

> **Core Principle**: The environment is not scenery — it is the archaeological layer of a choice. Every structure, every surface, every absence tells the story of what was saved and what was burned.

The Vietnamese fishing village aesthetic grounds the cosmic horror in human hands. These are not abstract horrors — they are the remnants of someone's home, someone's livelihood, someone's prayers.

---

## 2. Mounds (Mo)

### Overview

| Mound Type | Asset IDs | Day Visual | Night Visual | Function |
|------------|-----------|------------|--------------|----------|
| **Safe Mound** (Mo Thuong) | ASSET-030, ASSET-031 | Rounded, organic, warm Burnt Sienna | Darker, angular edges, Charcoal Black + ember glow | Valid cover |
| **Cursed Mound** (Mo Oan) | ASSET-032, ASSET-033 | Looks safe + moss-green glow + clay head | Intensified green, clay head watching | Contains relic + danger |
| **False Mound** (Bệ Đá Ảo Ảnh) | ASSET-034 | Identical to Safe Mound | Same | Trap — collapses 0.2s after stepped on |

### Safe Mound (Mo Thuong) — ASSET-030 (Day) / ASSET-031 (Night)

**Form**: Rounded, organic hill shape — curved edges, soft silhouette.
**Day**: Warm earth tones — Burnt Sienna (#8B4513) → Ochre Gold (#D4A017). Protective, welcoming, brushstroke texture visible.
**Night**: Darker — edges harden, become angular. Charcoal Black (#1A1418) base with Ember Orange (#D4652A) edge glow.

| Element | Day | Night |
|---------|-----|-------|
| Silhouette | Rounded, soft | Angular, hard |
| Color | Burnt Sienna → Ochre | Charcoal + Ember |
| Edge | Soft, bleeding | Sharp, lifting |
| Feeling | Safe, protective | Dangerous, hard-won |

### Cursed Mound (Mo Oan) — ASSET-032 (Day) / ASSET-033 (Night)

**Deceptive**: Looks safe like Safe Mound, but has subtle differentiators.
**Day**: Faint moss-green glow (#4A7B4A), small "clay head" silhouette turning — subtle unease.
**Night**: Green intensifies, clay head fully visible and watching — pulses with danger.

| Element | Day | Night |
|---------|-----|-------|
| Distinction | Faint green + clay head | Intense green + watching |
| Color | Moss-green glow | Bright moss-green |
| Feeling | Subtle wrongness | Obvious danger |
| Reward | Hidden relic inside | Relic + hazard |

### False Mound (Trap) — ASSET-034

**Critical**: Must be **indistinguishable** from Safe Mound until triggered.
- No visual difference in Day state
- Player only knows it's false when warning tell triggers (0.9s before collapse)
- Visual deception is the gameplay mechanic

---

## 3. Mound VFX

### Warning Tell — ASSET-035

**Purpose**: 0.9s warning before False Mound collapses.
**Visual**: Fast-spreading fissure cracks — dark lines across mound surface. Dust shake effect.
**Color**: Dark cracks (Charcoal), dust particles (Canvas Cream).

### Collapse Effect — ASSET-036

**Purpose**: Visual feedback when False/Cursed Mound collapses.
**Visual**: Fragments flying, dust cloud, geometric pieces (night aesthetic). Ember particles rising.
**Color**: Ember Orange on destruction, Charcoal Black fragments.

---

## 4. Shrines (Bệ Đá / Am Tho)

### Start Shrine — ASSET-037

**Form**: Simple altar/niche structure — Vietnamese ancestor worship reference.
**Day**: Warm Ochre Gold (#FFD700) glow. Safe, welcoming. Player begins here.
**Visual**: Eye-level altar niche, visible incense holder area, warm glow effect.

### End Shrine — ASSET-038

**Form**: Same shrine structure as Start.
**Night**: Intense golden glow — triumphant, salvation. Soft bloom effect.
**Win Trigger**: Reaching End Shrine + pressing E triggers win state.

| Element | Start | End |
|---------|-------|-----|
| Form | Same structure | Same structure |
| Glow | Warm Ochre | Intense Golden |
| Feeling | Beginning, safety | Victory, salvation |
| Effect | Soft glow | Bloom/pulse |

---

## 5. Ground Tiles

### Ground Tile Day — ASSET-039

| Parameter | Value |
|-----------|-------|
| Size | 64×64px tileable |
| Visual | Beach/sand — warm earth tones |
| Technique | Watercolour wash, not harsh |
| Variation | Subtle variation between tiles |

### Ground Tile Night — ASSET-040

| Parameter | Value |
|-----------|-------|
| Size | 64×64px tileable |
| Visual | Dark Charcoal Black base |
| Technique | Ember edge hints — ground feels like walking on embers |
| Feeling | Scorched, dangerous |

---

## 6. UI Element

### Shrine Direction Marker — ASSET-041

**Purpose**: Arrow/marker at screen edge pointing toward shrine.
**Day**: Ochre Gold.
**Night**: Ember Orange.
**Animation**: Slight pulse — draws attention without urgency.
**Style**: Edge elements feel like torn paper scraps (per §7 UI).

---

## 7. Village Props (Cultural Elements)

### From Art Bible §6

These environment elements create the fishing village atmosphere:

| Prop | Description | Day Visual | Night Visual |
|------|-------------|------------|--------------|
| **Rafter Frames** | Exposed wooden beams, triangular roof silhouettes | Warm wood grain texture | Angular black silhouettes |
| **Lantern Posts** | Bamboo poles with paper lanterns | Sienna glow (#8B4513) | Single ember point in void |
| **Nón Lá Canopy** | Conical woven leaf stalls | Curved silhouette, woven strands | Sharp geometric shadow |
| **Boat Hulls** | Partial fishing boat hulls (1/3 visible) | Warm ochre wood | Charred black with ribs |
| **Cổng Đá** | Low stone archways (forces crouch) | Moss-covered warm grey | Ink-black void passage |
| **Altar Niches** | Recessed wall cavities | Embers smoldering | Cold, empty, dark |
| **Net Drying Racks** | A-frame bamboo + ghost nets | 3 densities = safe/transitional/danger | Vertical line chaos |

### Prop Density Rules (§6)

| Area Type | Density | Rationale |
|-----------|---------|-----------|
| DayService | Sparse (3-5/screen) | Space, breath, illusion of safety |
| ChoiceLock | Medium (8-12/screen) | Tightening begins |
| NightSurvival | Dense (15-25/screen) | Clutter = obstacle = horror |
| Resolve (Win) | Sparse + Bare | Half-standing, half-gone |
| Resolve (Lose) | Empty | Bare canvas — absence itself |

---

## 8. Technical Specifications

### File Requirements

| Parameter | Value |
|-----------|-------|
| **Mound Sprites** | 128×192px, PNG with transparency |
| **Shrine Sprites** | 128×192px, PNG with transparency |
| **Ground Tiles** | 64×64px, PNG tileable |
| **UI Marker** | 48×48px, PNG with transparency |
| **Resolution** | 300 DPI |

### Layer Structure Example (Safe Mound)

```
MOUND_SAFE.psd
├── [Folder] BASE_FORM
│   ├── Silhouette (rounded/organic)
│   ├── Body (fill)
│   └── Edge_detail
├── [Folder] TEXTURE_EFFECT
│   ├── Brushstroke_texture
│   ├── Paper_grain
│   └── Edge_bleed (Day) / Burn_edge (Night)
├── [Folder] STATE_VARIANT
│   ├── Day_version (warm, soft)
│   └── Night_version (dark, angular)
└── [Folder] SHADOW
    └── Ground_shadow
```

---

## 9. Art Bible Anchors

| Element | Anchor |
|---------|--------|
| Day shapes | §3: Organic/curved dominance |
| Night shapes | §3: Angular/geometric dominance |
| Safe cover | §4: Burnt Sienna = active memory |
| Danger | §4: Ember Orange = active threat |
| Consumed | §4: Charcoal Black = post-decision void |
| Cursed unique | §4: Moss-green = danger + reward |
| Shrines | §6: Altar Niches — cultural anchor |
| Mounds | §3: Cover objects with psychological state |

---

## 10. Do & Don't

| ✅ Do | ❌ Don't |
|-------|----------|
| Rounded organic forms (Day) | Hard geometric shapes (Day) |
| Soft bleeding edges (Day) | Sharp defined edges (Day) |
| Angular hard edges (Night) | Soft rounded edges (Night) |
| False Mound = identical to Safe | False Mound = visually different |
| Cursed = subtle green + clay head | Cursed = obvious red danger |
| Shrines = cultural Vietnamese reference | Shrines = generic temple |
| Dense clutter (Night) = hazard seed | Dense clutter = random decoration |
| Every prop answers "who lived here?" | Props with no story purpose |

---

## 11. Reference

- **Art Style**: Watercolour environment painting, visible brushstroke
- **Reference**: Vietnamese fishing village photography (interpret, don't replicate)
- **Similar Games**: Hollow Knight (atmospheric backgrounds), Gris (watercolour environment)

---

## 12. Delivery Checklist

### Mounds
- [ ] ASSET-030 — Safe Mound Day (128×192 PNG)
- [ ] ASSET-031 — Safe Mound Night
- [ ] ASSET-032 — Cursed Mound Day
- [ ] ASSET-033 — Cursed Mound Night
- [ ] ASSET-034 — False Mound Day (identical to Safe)
- [ ] ASSET-035 — False Mound Warning Tell (VFX)
- [ ] ASSET-036 — Mound Collapse Effect (VFX)

### Shrines
- [ ] ASSET-037 — Start Shrine (128×192 PNG)
- [ ] ASSET-038 — End Shrine

### Ground
- [ ] ASSET-039 — Ground Tile Day (64×64 PNG, tileable)
- [ ] ASSET-040 — Ground Tile Night

### UI
- [ ] ASSET-041 — Shrine Direction Marker (48×48 PNG)

### Optional Village Props
- [ ] Rafter Frames (set)
- [ ] Lantern Posts (Day/Night)
- [ ] Nón Lá Canopy
- [ ] Boat Hulls (partial)
- [ ] Cổng Đá (Stone Gate)
- [ ] Altar Niches
- [ ] Net Drying Racks (3 densities)

---

*End of Art Brief — Environment*