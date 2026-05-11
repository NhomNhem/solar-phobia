# Art Brief — Boss: Cá Ông (Whale)
## ASSET-020 (Day), ASSET-021 (Night), ASSET-027 (Eyes)

**Project**: Solar Phobia: Nắng Gắt  
**System**: Boss Cá Ông Searchlight  
**Asset ID**: ASSET-020, ASSET-021, ASSET-027  
**Style**: 2D Hand-painted Watercolour

---

## 1. Entity Overview

| Attribute | Description |
|-----------|-------------|
| **Name** | Cá Ông — "Grandfather Whale" |
| **Role** | Background boss entity — persistent threat during Night Phase |
| **Philosophy** | Not a character, a stain that keeps growing |
| **Lore** | The whale carcass Tú failed to bury — now consumes him in the Bardo |

### Cá Ông's Role
Cá Ông is the judgment — the consequence of Tú's cowardice. He's always present, always watching, always sweeping his searchlight. The player never fights him directly; they survive his pursuit. He's the shadow that hangs over the entire game.

---

## 2. Appearance — Day State (ASSET-020)

### Silhouette
| Element | Description |
|---------|-------------|
| **Form** | Massive whale skeleton — clearly readable but never fully visible |
| **Edges** | Soft, feathered — wet-on-wet watercolor wash effect |
| **Scale** | Background layer — hints at enormous size through fragments |
| **Behavior** | "A stain that keeps growing" — bleeds at edges, never contained |

### Color Palette
| Element | Color Name | Hex Code |
|---------|-----------|----------|
| **Primary** | Bruised Indigo | `#6B5B7A` |
| **Secondary** | Dusty Violet | `#8B7B9B` |
| **Shadow** | Deep Purple | `#4A3B5C` |
| **Edge Bleed** | Faded Lavender | `#B8A8C8` |

### Visual Technique
- Wet-on-wet wash — edges spread and feather like paint in water
- Partial visibility — you see parts, glimpses, not the whole
- Like looking through murky water — shapes shift and distort
- Never a complete form — always more suggested than shown

---

## 3. Appearance — Night State (ASSET-021)

### Silhouette
| Element | Description |
|---------|-------------|
| **Form** | Same whale skeleton, now evaporating |
| **Edges** | Lifting off, burning away — gaps and transparency |
| **Scale** | Feels larger, more oppressive in the dark |
| **Behavior** | The whale is consuming the world — edges consume instead of bleed |

### Changes from Day
| Change | Description |
|--------|-------------|
| **Edges** | No longer soft — now sharp, lifting, evaporating |
| **Gaps** | Bare paper shows through — fire burned through |
| **Color** | Darker, more oppressive — nearly consumes the sky |
| **Presence** | Feels closer, more threatening |

### Color Palette
| Element | Color Name | Hex Code |
|---------|-----------|----------|
| **Primary** | Deep Charcoal | `#1A1418` |
| **Secondary** | Burnt Umber | `#2D1F1F` |
| **Gaps** | Canvas Cream (bare paper) | `#F5F0E6` |
| **Edge Glow** | Ember Orange (faint) | `#D4652A` |

### Visual Technique
- Burned-through effect — paint lifting off to reveal bare canvas
- Oppressive presence — the whale looms larger in darkness
- Evaporating edges — shapes lose cohesion, dissolve into gaps
- Fire damage aesthetic — this is what "survived a fire" looks like

---

## 4. Appearance — Eyes (ASSET-027)

### Eyes
| Element | Description |
|---------|-------------|
| **Form** | Two simple dark spots — Charcoal Black (#1A1418) |
| **Detail** | No whites, no pupils — just absence |
| **Expression** | Watching, but not "seeing" as humans do |
| **Scale** | Tiny relative to skeleton, but always visible |

### Philosophy
The whale doesn't look at you — it *watches*. There's no emotion in those eyes, only presence. They mark where it is, what it's tracking, but there's no recognition, no malice, only the blank void of judgment.

---

## 5. Searchlight Cone (Visual)

### The Sweep
| Element | Description |
|---------|-------------|
| **Color** | Green searchlight — per GDD (NOT blue) |
| **Shape** | Cone sweeping across the screen |
| **Effect** | Like a spotlight through murky water |
| **VFX** | Particle mist/fog in the beam |

### Searchlight States
| State | Visual |
|-------|--------|
| **Idle/Sweep** | Slow green cone sweep — oppressive, methodical |
| **Telegraph** | Cone brightens, color shifts — warning |
| **Strike** | Bone/rock projectile arcs toward player |
| **Cooldown** | Broken/fragmented whale icon — bruised indigo |

---

## 6. Technical Specifications

### File Requirements

| Parameter | Value |
|-----------|-------|
| **Canvas Size** | 2048 × 1024 px (whale skeleton), 64 × 64 px (eyes) |
| **Resolution** | 300 DPI |
| **Format** | PNG with transparency (PSD source preferred) |
| **Sprite Type** | Background layer — parallax-ready |

### Layer Structure (Whale Skeleton)

```
WHALE_SKELETON.psd
├── [Folder] SKELETON_STRUCTURE
│   ├── Rib_cage (massive curved form)
│   ├── Spine (elongated, partial)
│   ├── Skull (massive, whale shape)
│   └── Tail_section (fading out)
├── [Folder] TEXTURE_EFFECT
│   ├── Wet_wash_edges (Day)
│   ├── Evaporating_edges (Night)
│   └── Paper_bleed (both phases)
├── [Folder] STATE_VARIANT
│   ├── Day_version (soft, contained)
│   ├── Night_version (evaporating, gaps)
│   └── Searchlight_cone_element
└── [Folder] EYES
    ├── Eye_left (simple dark spot)
    └── Eye_right (simple dark spot)
```

### Animation (If Needed)
| State | Frames | Notes |
|-------|--------|-------|
| Idle (Day) | 2 | Subtle breathing — slight swell |
| Idle (Night) | 2 | More aggressive — pulsing edges |
| Sweep | — | Cone rotation (programmatic) |
| Strike | 8 | Brief impact frame |

---

## 7. Art Bible Anchors

| Rule | Application |
|------|-------------|
| **§5 Character: Whale** | "Not a character, a stain that keeps growing" |
| **§5 Whale: Day** | "Bruised Indigo with soft feathered edges" |
| **§5 Whale: Night** | "Edges evaporate, gaps and transparency" |
| **§5 Whale: Eyes** | "Two simple dark spots, no whites, no pupils" |
| **§4 Color: Indigo = Warning** | Searchlight telegraph |
| **§1 Visual Identity** | "Every frame looks like watercolour that survived a fire" |

---

## 8. Do & Don't

| ✅ Do | ❌ Don't |
|-------|----------|
| Partial, glimpsed form — never complete | Full, visible whale shape |
| Soft wet-on-wet edges (Day) | Hard, defined edges (Day) |
| Evaporating, gaps (Night) | Intact form (Night) |
| Two simple dark spots for eyes | Detailed eyes with pupils/whites |
| Bruised Indigo primary (Day) | Bright blues or greens |
| Oppressive, consuming presence | "Friendly" or approachable |
| Green searchlight cone | Any other searchlight color |

---

## 9. Reference

- **Art Style**: Wet-on-wet watercolor, ink wash, large-scale background painting
- **Reference**: Hokusai's Great Wave (partial forms), Turner's seascapes (atmospheric)
- **Similar Games**: Darkest Dungeon (oppressive backgrounds), Blasphemous (religious horror art)

---

## 10. Delivery Checklist

- [ ] ASSET-020 — Whale Skeleton Silhouette Day (2048×1024 PNG)
- [ ] ASSET-021 — Whale Skeleton Silhouette Night (evaporating edges)
- [ ] ASSET-027 — Whale Eyes (two simple dark spots)
- [ ] Optional: Searchlight cone VFX sprite
- [ ] Optional: Strike projectile sprite

---

*End of Art Brief — Cá Ông (Whale)*