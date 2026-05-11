# Asset Specs — System: Boss Cá Ông Searchlight

> **Source**: design/gdd/boss-ca-ong-searchlight.md
> **Art Bible**: design/art/art-bible.md
> **Generated**: 2026-05-07
> **Status**: 10 assets specced

---

## ASSET-020 — Whale Skeleton Silhouette (Day)

| Field | Value |
|-------|-------|
| Category | Environment / Boss |
| Dimensions | 512×256px (scaled) |
| Format | PNG with transparency |
| Naming | `whale_skeleton_day.png` |

**Visual Description**:  
A massive whale skeleton silhouette in Bruised Indigo — the background presence. Soft feathered edges like a wet-on-wet watercolour wash. The shape bleeds at the edges, never fully contained. You see parts of it — glimpses through water — not the full form.

**Art Bible Anchors**:
- §5 Character: Whale is "a stain that keeps growing" — not a shape, a presence
- §4 Color: Bruised Indigo = tipping point, between states

**Generation Prompt**:  
"Massive whale skeleton silhouette, watercolour wet-on-wet wash effect, bruised indigo blue-violet color, soft bleeding edges, partial form visible not fully contained, background layer, game asset"

---

## ASSET-021 — Whale Skeleton Silhouette (Night)

| Field | Value |
|-------|-------|
| Category | Environment / Boss |
| Dimensions | 512×256px |
| Format | PNG with transparency |
| Naming | `whale_skeleton_night.png` |

**Visual Description**:  
The same silhouette but now edges evaporate/lift off — the whale is consuming the world. Gaps and transparency where fire has burned through. The form becomes more oppressive as the night progresses.

**Art Bible Anchors**:
- §2 Mood: "Primal terror" — whale as consuming force
- §3 Shape: Night = fractured, exposed skeleton

**Generation Prompt**:  
"Whale skeleton silhouette at night, evaporating edges, burned through gaps showing bare paper, more oppressive form, watercolour damage effect, game background"

---

## ASSET-022 — Searchlight Cone (Sweep)

| Field | Value |
|-------|-------|
| Category | VFX / Hazard |
| Dimensions | 256×512px (cone shape) |
| Format | PNG with transparency |
| Naming | `searchlight_cone.png` |

**Visual Description**:  
A cone shape in bright green/cyan (unique — not from palette, represents artificial light). The edges are harsh — a searchlight is mechanical, not organic. Slight glow/bloom effect. The light source is offshore, casting across the play area.

**Art Bible Anchors**:
- §2 Mood: "The only light comes from the burning horizon" — but this is cold artificial light, not fire
- Contrast: Searchlight = cold, fire = warm

**Generation Prompt**:  
"Searchlight cone effect, bright cyan-green color, harsh mechanical edges not organic, glow/bloom effect, transparent background, game hazard VFX"

---

## ASSET-023 — Sweep Telegraph Warning

| Field | Value |
|-------|-------|
| Category | VFX / Warning |
| Dimensions | 128×128px (expandable) |
| Format | PNG with transparency |
| Naming | `sweep_warning.png` |

**Visual Description**:  
Ground shimmer effect — like heat distortion but cold. Diamond-shaped warning markers (per colorblind rules) that pulse. The warning appears 1.5s before strike.

**Art Bible Anchors**:
- §4 Colorblind: Pulsing diamond shape for warning/indigo
- §7 UI: Warning elements must be legible

**Generation Prompt**:  
"Ground warning shimmer effect, pulsing diamond markers, cold green color, heat distortion style but cold, warning indicator, game VFX"

---

## ASSET-024 — Strike Impact Effect

| Field | Value |
|-------|-------|
| Category | VFX / Impact |
| Dimensions | 256×256px |
| Format | PNG with transparency |
| Naming | `strike_impact.png` |

**Visual Description**:  
Screen shake visual — red flash with screen shake. Ember Orange burst at impact point. The strike hits hard — this is critical. Slight chromatic aberration burst.

**Art Bible Anchors**:
- §4 Color: Ember Orange = active danger, immediate threat
- §2 Mood: "Primal terror" — strike is visceral

**Generation Prompt**:  
"Strike impact effect, red flash with ember orange burst, screen shake visual, hard impact, critical danger, game VFX"

---

## ASSET-025 — Debris/Cover Rock (New Cover)

| Field | Value |
|-------|-------|
| Category | Environment / Cover |
| Dimensions | 64×64px |
| Format | PNG with transparency |
| Naming | `debris_rock.png` |

**Visual Description**:  
Fallen rock that becomes new cover after strike. Angular, geometric — fire-created geometry. Charcoal Black with ember edges still glowing. Becomes a safe spot in subsequent sweeps.

**Art Bible Anchors**:
- §3 Shape: Night = angular/geometric dominance
- §4 Color: Charcoal = consumed, Ember edges = still dangerous

**Generation Prompt**:  
"Fallen rock debris, angular geometric shape, charcoal black with ember orange glowing edges, new cover object, game environment"

---

## ASSET-026 — Strike Telegraph Ground shimmer

| Field | Value |
|-------|-------|
| Category | VFX / Ground Effect |
| Dimensions | 128×128px |
| Format | PNG with transparency |
| Naming | `telegraph_shimmer.png` |

**Visual Description**:  
Ground shimmer beneath player when targeted — the light has found them. Bright cyan/green glow from below. Shaking effect.

**Art Bible Anchors**:
- §7 UI: "Diegetic" — threat from the world, not UI
- §2 Mood: "Being seen" — exposed, vulnerable

**Generation Prompt**:  
"Ground shimmer effect from below, bright cyan-green glow, targeted player indicator, shaking effect, game VFX"

---

## ASSET-027 — Whale Eyes (Simple Dark Spots)

| Field | Value |
|-------|-------|
| Category | Environment / Detail |
| Dimensions | 32×32px |
| Format | PNG with transparency |
| Naming | `whale_eyes.png` |

**Visual Description**:  
Two simple dark spots — Charcoal Black, no whites, no pupils. Just absence. The whale watches but doesn't see as humans do.

**Art Bible Anchors**:
- §5 Character: "Two simple dark spots (Charcoal Black) — no whites, no pupils, just absence"

**Generation Prompt**:  
"Whale eye spots, simple charcoal black circles, no pupil no white, just absence, minimal detail, game environment"

---

## ASSET-028 — Boss Entry Ripple Effect

| Field | Value |
|-------|-------|
| Category | VFX / Ambient |
| Dimensions | 256×256px |
| Format | PNG with transparency |
| Naming | `boss_entry_ripple.png` |

**Visual Description**:  
Water ripple effect when whale breaches/attacks — like paint dropping into water. Concentric rings expanding outward. Ember Orange to transparent gradient.

**Art Bible Anchors**:
- §1 Tactile Deliberation: Visible brushstroke/ink dropping — hand-touch aesthetic
- §2 Mood: Whale presence in water

**Generation Prompt**:  
"Water ripple effect, concentric rings, paint dropping into water aesthetic, ember orange gradient, game ambient VFX"

---

## ASSET-029 — Cooldown State Indicator

| Field | Value |
|-------|-------|
| Category | UI |
| Dimensions | 64×32px |
| Format | PNG with transparency |
| Naming | `boss_cooldown_icon.png` |

**Visual Description**:  
Simple icon showing boss is in cooldown — a broken/fragmented whale shape. Bruised Indigo — transitional state, not active threat.

**Art Bible Anchors**:
- §4 Color: Bruised Indigo = between states, transitional

**Generation Prompt**:  
"Cooldown indicator icon, broken fragmented whale shape, bruised indigo color, transitional state, game UI element"

---

## Audio Assets (Descriptions Only)

| Asset | Description | Trigger |
|-------|-------------|---------|
| Sweep drone | Low rumbling bass pulse — whale breathing | Sweep active |
| Target lock | Tone rises when player detected | Player enters cone |
| Telegraph crack | Sharp crack sound, rising intensity | Telegraph starts |
| Strike hit | Loud impact, distortion | Strike resolves |
| Debris crash | Rock falling, rubble settling | Debris created |
| Cooldown silence | Threat motif drops out | Cooldown state |

---

## Summary

| Asset ID | Name | Category | Status |
|----------|------|----------|--------|
| ASSET-020 | Whale Skeleton (Day) | Environment | Needed |
| ASSET-021 | Whale Skeleton (Night) | Environment | Needed |
| ASSET-022 | Searchlight Cone | VFX | Needed |
| ASSET-023 | Sweep Warning | VFX | Needed |
| ASSET-024 | Strike Impact | VFX | Needed |
| ASSET-025 | Debris Rock (New Cover) | Environment | Needed |
| ASSET-026 | Telegraph Shimmer | VFX | Needed |
| ASSET-027 | Whale Eyes | Environment | Needed |
| ASSET-028 | Boss Entry Ripple | VFX | Needed |
| ASSET-029 | Cooldown Indicator | UI | Needed |

---

*End of Boss Cá Ông Searchlight Asset Specs*