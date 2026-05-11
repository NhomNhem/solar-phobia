# Asset Specs — System: Player Controller

> **Source**: design/gdd/player-controller.md
> **Art Bible**: design/art/art-bible.md
> **Generated**: 2026-05-07
> **Status**: 11 assets specced

---

## ASSET-001 — Player Sprite (Day)

| Field | Value |
|-------|-------|
| Category | Sprite / 2D Art |
| Dimensions | 64×128px (0.6m × 1.8m world, 4x scale) |
| Format | PNG with transparency |
| Naming | `player_day_sprite.png` |
| Animation Frames | 4-frame idle, 8-frame run, 4-frame sprint |

**Visual Description**:  
A dense, warm brushstroke forming an elongated humanoid shape. Visible brush direction in the stroke — horizontal on torso, vertical on limbs. No facial features — just a darker ochre "mask" where a face would be. Slight paper grain texture visible in the pigment. The silhouette is compact and legible at thumbnail scale.

**Art Bible Anchors**:
- §1 Tactile Deliberation: Warm, visible brushstrokes during day — hand-touched, not digitally clean
- §5 Character: Player is "the surviving mark" — dense, warm, persisting through both cycles

**Generation Prompt**:  
"Top-down view 2D game sprite, elongated humanoid figure, watercolour brushstroke aesthetic, warm ochre gold tones, visible brush direction texture, no facial features only darker mask area, paper grain texture showing through pigment, elongated proportions longer torso shorter limbs, transparent background PNG, game-ready asset"

---

## ASSET-002 — Player Sprite (Night)

| Field | Value |
|-------|-------|
| Category | Sprite / 2D Art |
| Dimensions | 64×128px (matches day) |
| Format | PNG with transparency |
| Naming | `player_night_sprite.png` |

**Visual Description**:  
The same brushstroke form but now edges fray and thin. Ember Orange bleeds from the outline — the character is literally burning at the edges. The warm ochre has desaturated toward Charcoal Black. At night, the player becomes what remains after fire — stripped to essentials, but still intact compared to the consumed environment.

**Art Bible Anchors**:
- §1 Heat and Void: Night palette — ink-wash, void, burned
- §3 Shape Language: Player is what's *left whole* in a field of debris — intact geometry in fragmented world

**Generation Prompt**:  
"Top-down view 2D game sprite, same humanoid form as day version, watercolour with fire damage aesthetic, edges fray and burn with ember orange bleeding, desaturated charcoal black tones, burnt edges, scorched effect, paper texture showing through damaged areas, transparent background, game-ready asset"

---

## ASSET-003 — Cover Highlight Glow (Safe)

| Field | Value |
|-------|-------|
| Category | VFX / Particles |
| Dimensions | 128×128px (expandable) |
| Format | PNG with transparency |
| Naming | `cover_safe_glow.png` |

**Visual Description**:  
A soft, warm glow effect — like wet paint pooling at the base of the cover mound. Sienna/burnt sienna tones. The glow pulses gently (breathing rhythm). Edges are soft, bleeding into the ground — not a hard outline.

**Art Bible Anchors**:
- §7 UI/HUD: Soft glow, wet-edge blur effect
- Color: Burnt Sienna for safe elements

**Generation Prompt**:  
"Soft radial watercolour wash effect, warm sienna tones, soft bleeding edges like wet paint pooling, gentle pulse animation capable, transparent background, game VFX sprite"

---

## ASSET-004 — Strike Warning Icon

| Field | Value |
|-------|-------|
| Category | UI / VFX |
| Dimensions | 64×64px |
| Format | PNG with transparency |
| Naming | `strike_warning_icon.png` |

**Visual Description**:  
A sharp, jagged shape — triangular with spiking edges — in Ember Orange. This shape communicates danger through silhouette alone (colorblind backup). The edges are burned/singed — not clean lines. Brief flicker animation capability.

**Art Bible Anchors**:
- §4 Colorblind Safety: Sharp triangular silhouette as backup for Ember (danger)
- §3 Shape Language: Night shapes are angular, sharp, fractured
- Critical: Must have z-order priority above Tier 4 panic effects

**Generation Prompt**:  
"Jagged triangular warning icon, sharp spiking edges, ember orange color, burned/singed edge effect not clean lines, flickering animation capable, transparent background, game UI element"

---

## ASSET-005 — Bone Relic Pickup VFX

| Field | Value |
|-------|-------|
| Category | VFX / Particles |
| Dimensions | 128×128px |
| Format | PNG with transparency |
| Naming | `bone_relic_pickup.png` |

**Visual Description**:  
A green-tinged vignette effect with grain surge — the world tightens and becomes grainy. Subtle green tint overlay (Linh's color = danger/reward). The effect feels like ink spreading across wet paper. Duration: 0.5s hold.

**Art Bible Anchors**:
- §2 Mood: NightSurvival has "grain surge" visual element
- §6 Environment: Prop density increases danger

**Generation Prompt**:  
"Vignette tightening effect, green tinted watercolour spread, grain texture overlay, ink spreading across wet paper effect, subtle glow, transparent background, game VFX"

---

## ASSET-006 — Phase Icon (Day)

| Field | Value |
|-------|-------|
| Category | UI |
| Dimensions | 32×32px |
| Format | PNG with transparency |
| Naming | `phase_icon_day.png` |

**Visual Description**:  
A simple icon representing the sun/warmth — a soft circular brushstroke in Ochre Gold. Slightly warm, not hot. The shape is organic — not a perfect circle. Visible brush texture.

**Art Bible Anchors**:
- §4 Color: Ochre Gold = cost, but in UI represents "time remaining" — weighted warmth

**Generation Prompt**:  
"Icon for day phase indicator, soft circular watercolour brushstroke, ochre gold warm tones, organic not perfectly round, visible brush texture, small game UI icon"

---

## ASSET-007 — Phase Icon (Night)

| Field | Value |
|-------|-------|
| Category | UI |
| Dimensions | 32×32px |
| Format | PNG with transparency |
| Naming | `phase_icon_night.png` |

**Visual Description**:  
A similar circle form but now it's darker — Charcoal Black with ember edge glow. The shape is more angular/jagged, reflecting the night state.

**Art Bible Anchors**:
- §4 Color: Charcoal Black = consumed, Ember Orange = danger

**Generation Prompt**:  
"Icon for night phase indicator, angular jagged circular shape, charcoal black with ember orange edge glow, burned effect, small game UI icon"

---

## ASSET-008 — Sprint Active Indicator

| Field | Value |
|-------|-------|
| Category | UI |
| Dimensions | 64×32px (expandable bar) |
| Format | PNG with transparency |
| Naming | `sprint_indicator.png` |

**Visual Description**:  
A horizontal bar that fills as sprint is held. Day: Ochre Gold fill with soft edges. Night: Ember Orange with pulsing glow. The fill behavior is like paint flowing into a channel.

**Art Bible Anchors**:
- §7 UI Animation: "Seeping glow" — color bleeds outward on hover/active

**Generation Prompt**:  
"Horizontal progress bar sprite, watercolour fill effect, soft edges like paint flowing, ochre gold for day state, ember orange for night, game UI element"

---

## ASSET-009 — In Cover Indicator

| Field | Value |
|-------|-------|
| Category | UI |
| Dimensions | 32×32px |
| Format | PNG with transparency |
| Naming | `cover_indicator.png` |

**Visual Description**:  
A small shield-like shape — organic, rounded, soft edges. Paper White with subtle warm undertone. Appears near reticle when in valid cover.

**Art Bible Anchors**:
- §4 Color: White/Paper = safety, unmarked self

**Generation Prompt**:  
"Small shield icon, organic rounded shape, soft edges, paper white with warm undertone, watercolour style, game UI element near reticle"

---

## ASSET-010 — Shrine Direction Arrow

| Field | Value |
|-------|-------|
| Category | UI |
| Dimensions | 48×48px |
| Format | PNG with transparency |
| Naming | `shrine_arrow.png` |

**Visual Description**:  
A directional arrow with hand-painted aesthetic — slightly uneven, brushstroke quality. Ochre Gold in day, fading to Ember in night. Located at screen edge, pointing toward shrine.

**Art Bible Anchors**:
- §7 UI: Edge elements feel like torn paper scraps

**Generation Prompt**:  
"Directional arrow, hand-painted watercolour aesthetic, slightly uneven brushstroke quality, ochre gold, game UI edge element"

---

## ASSET-011 — Stamina Bar

| Field | Value |
|-------|-------|
| Category | UI |
| Dimensions | 200×16px |
| Format | PNG with transparency |
| Naming | `stamina_bar.png` |

**Visual Description**:  
A bar container with fill. The container has soft rounded edges (day) or cracked edges (night). Fill uses gradient from Paper White to Ochre Gold (healthy) to Ember Orange (depleted). The bar feels like looking at your own vital organ — diegetic.

**Art Bible Anchors**:
- §7 UI: "Diegetic framing — player body as anchor"
- §4 Color: Gradient health to danger

**Generation Prompt**:  
"Health/stamina bar sprite, watercolour style, soft rounded container, gradient fill from cream to gold to ember orange, game UI element bottom center"

---

## Audio Assets (Descriptions Only — No Generation)

| Asset | Description | Reference |
|-------|-------------|-----------|
| Footstep (Day) | Soft, measured — gentle brush on paper | Ambient, minimal |
| Footstep (Night) | Quicker, harder — paint splattering | Urgent |
| Sprint Breathing | Heavy breath, increasingly urgent | Film-style |
| Cover Enter | Soft whisper bed fades in | Environmental |
| Cover Exit | Muffled sounds return to full | Environmental |
| Strike Warning | Rising crack sound, heartbeat intensifies | Critical |
| Strike Hit | Loud impact, distortion layer | Critical |

---

## Summary

| Asset ID | Name | Category | Status |
|----------|------|----------|--------|
| ASSET-001 | Player Sprite (Day) | Sprite | Needed |
| ASSET-002 | Player Sprite (Night) | Sprite | Needed |
| ASSET-003 | Cover Highlight Glow | VFX | Needed |
| ASSET-004 | Strike Warning Icon | UI | Needed |
| ASSET-005 | Bone Relic Pickup VFX | VFX | Needed |
| ASSET-006 | Phase Icon (Day) | UI | Needed |
| ASSET-007 | Phase Icon (Night) | UI | Needed |
| ASSET-008 | Sprint Active Indicator | UI | Needed |
| ASSET-009 | In Cover Indicator | UI | Needed |
| ASSET-010 | Shrine Direction Arrow | UI | Needed |
| ASSET-011 | Stamina Bar | UI | Needed |

---

*End of Player Controller Asset Specs*