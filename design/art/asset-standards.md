# Section 8: Asset Standards — Solar Phobia

> **Status**: Production Ready
> **Created**: 2026-05-07
> **Technical Review**: Pending

---

## Overview

This section defines all technical standards for art asset production and import. All asset pipelines must respect the visual identity: **watercolour that survived fire** — warm, textured, and human in day; scorched and bare in night.

---

## 1. File Format Preferences

### 2D Assets (Sprites, Textures)

| Purpose | Format | Rationale |
|---------|--------|----------|
| **Source Images** | PSD (Photoshop) | Layer preservation, channel access for masks/normals |
| **Final Sprites** | PNG | Lossless for watercolor texture fidelity |
| **Tile Palettes** | PNG with transparency | Alpha required for organic edge bleeding |
| **Normal Maps** | PNG (RGB) | Unity URP expects tangent-space normals in RGB |
| **Emission Masks** | PNG (Grayscale) | Single channel, converted to emission in shader |
| **Brushstroke Textures** | PNG | Grain, pigment bleed patterns for shader overlay |

### 3D Assets (If Applicable)

| Purpose | Format | Rationale |
|---------|--------|----------|
| **Source Models** | FBX | Industry standard, Unity-friendly |
| **Reference Geometry** | OBJ | Only if FBX unavailable |
| **Material Definitions** |mat | Unity material files, not external |

### Audio Assets

| Purpose | Format | Notes |
|---------|--------|-------|
| **Sound Effects** | WAV (PCM) | Lossless for SFX editing |
| **Music/Stem** | OGG | Compressed, loop-friendly |
| **Ambient** | WAV → OGG | If ambient is looping |

> **Prohibition**: Never use JPG for any artwork. JPEG artifacts destroy watercolor texture integrity.

---

## 2. Naming Convention

Adheres to **technical-preferences.md** section "Naming Conventions (Unity/C#)" with asset-specific extensions:

### Pattern

```
[Category]_[Descriptor]_[State].ext
```

### Categories

| Category Prefix | Asset Type | Example |
|----------------|-----------|---------|
| `sp_` | Sprite | `sp_lantern_post_day.png` |
| `tx_` | Texture (material) | `tx_wood_plank_diffuse.png` |
| `nm_` | Normal map | `nm_wood_plank.png` |
| `em_` | Emission mask | `em_burn_edge.png` |
| `pr_` | Prop (sprite sheet) | `pr_net_rack.png` |
| `ch_` | Character | `ch_survivor_idle.png` |
| `fx_` | VFX sprite | `fxember_glow.png` |
| `ui_` | UI element | `ui_container_day.png` |
| `bg_` | Background tile | `bg_stone_wall_day.png` |

### State Suffixes (Required for Phase-Aware Assets)

| State Suffix | Phase | Usage |
|-------------|-------|-------|
| `_day` | DayService | Warm, lush watercolor |
| `_choice` | ChoiceLock | Transition, cracking |
| `_night` | NightSurvival | Charred, reduced |
| `_resolve` | Resolve | Half-standing |
| *(none)* | Universal | Works across all states |

### Sheet Naming

```
[Category]_[Descriptor]_sheet_NxM.ext
```
- `N` = columns, `M` = rows
- Example: `ch_survivor_walk_sheet_4x3.png` (4 columns, 3 rows)

### LOD Naming

```
[BaseName]_lod0.ext    // Highest detail (0-10m)
[BaseName]_lod1.ext    // Medium detail (10-30m)
[BaseName]_lod2.ext    // Low detail (30m+)
```

---

## 3. Texture Resolution Tiers

### Tier System

| Tier | Pixel Size | Distance | Usage |
|-----|----------|----------|-------|
| **Tier 1** | 1024×1024 | 0–10m | Hero assets, player character, key props, UI elements |
| **Tier 2** | 512×512 | 10–30m | Secondary props, environmental details |
| **Tier 3** | 256×256 | 30m+ | Background tiles, distant terrain, filler props |

### Resolution Rules

- **Power of 2 only**: 256, 512, 1024, 2048. No 768, 1440, etc.
- **Square default**: Landscape aspect only when 16:9 specifically required
- **Maximum**: 2048×2048 for hero elements; 1024×1024 for everything else

### Mipmap Requirements

| Asset Type | Mipmap | Rationale |
|-----------|-------|----------|
| Background tiles | Enabled | Performance at distance |
| UI elements | Disabled | Sharp scaling required |
| Normal maps | Enabled | Prevents banding |
| Emission masks | Disabled | Edge precision required |

---

## 4. Polycount Budgets (If 3D Elements)

> **Note**: Solar Phobia is primarily 2D with 3D elements for depth. These budgets apply to any 3D geometry.

### Character Budgets

| Category | Triangles | Animated | Notes |
|----------|----------|-----------|-------|
| **Player** | 3,000–5,000 | Yes | Hero character — receives closest camera |
| **NPC/Survivor** | 1,500–3,000 | Yes | Secondary, visible in Day |
| **Lost One** | 500–1,000 | Partial | Background, silhouette focus |
| **Guardian** | 2,000–4,000 | Yes | Boss/guardian type |

### Environment Budgets

| Category | Triangles | Animated | Notes |
|----------|----------|-----------|-------|
| **Hero Prop** | 500–1,500 | No | Lantern, altar — foreground |
| **Standard Prop** | 100–500 | No | Crates, posts |
| **Background** | 50–200 | No | Distance clutter |

### General Rule

> **Budget = Performance × Visual Priority**. If it reads clearly at required distance, use fewer triangles.

---

## 5. LOD Level Expectations

### LOD Structure

| LOD Level | Distance | Detail Reduction | Purpose |
|----------|----------|-----------------|---------|
| **LOD0** | 0–10m | 100% (full) | Primary view |
| **LOD1** | 10–30m | 50% triangles | Standard cull |
| **LOD2** | 30m+ | 25% triangles | Background |

### LOD Implementation

| Approach | When to Use |
|----------|-------------|
| **Sprite swap** | 2D assets only — use `_lod0/1/2` suffixes |
| **Mesh LOD** | Any 3D geometry with custom level meshes |
| **Hybrid** | Sprite for LOD0, mesh for LOD1+ (performance optimization) |

### LOD Transition Rules

- **Smooth transition**: Use Unity's LOD Group with **cross-fade** (not pop)
- **Fade range**: 10% distance (e.g., 9m–11m for 10m threshold)
- **Maximum LODs per asset**: 3 (LOD0, LOD1, LOD2)

---

## 6. Export Settings

### PNG Export

| Setting | Value | Notes |
|--------|-------|-------|
| **Bit Depth** | 8-bit (32-bit if HDR) | Standard for color + alpha |
| **Color Mode** | RGB (RGBA for transparency) | No indexed color |
| **Compression** | None (for import) | Let Unity compress |
| **Interlace** | None | Progressive not needed |

### Sprite Sheet Export

| Setting | Value | Notes |
|--------|-------|-------|
| **Padding** | 2px minimum | Prevents bleed in atlas |
| **Powers of 2** | Yes | Atlas optimization |
| **Trimmed** | Yes | Remove empty space |
| **Extrude** | 1px | UV edge repeat protection |

### Material Export (URP)

| Property | Setting | Notes |
|----------|---------|-------|
| **Render Pipeline** | URP | Not Built-in |
| **Shader** | Custom (Painted) | Matches Art Bible shader spec |
| **Color Space** | Linear | Required for HDR painting |
| **Post-Processing** | Enabled | Global volume |

### Normal Map Export

| Setting | Value | Notes |
|--------|-------|-------|
| **Format** | RGB 24-bit | Tangent-space encoded |
| **Color Mode** | RGB | No monochrome |
| **Compression** | None | Raw import |
| **sRGB** | No (Linear) | Data texture |

### Emission Export

| Setting | Value | Notes |
|--------|-------|-------|
| **Format** | Grayscale 8-bit | Intensity only |
| **Range** | 0–1 | HDR intensities via shader |
| **White = Emission** | Yes | 1 = max glow |

---

## 7. Engine-Specific Constraints (Unity 6000.3.11f1)

### Rendering Pipeline

| Setting | Value | Rationale |
|--------|-------|----------|
| **Pipeline** | URP (Universal) | 2D primary, watercolor shader support |
| **Renderer** | 2D | Sprite-focused, no 3D PBR |
| **HDR** | On (Linear) | Warm palette needs HDR range |

### Sprite Settings (Unity Import)

| Property | Setting | Rationale |
|----------|---------|----------|
| **Sprite Mode** | Multiple (sheets) / Single | Per asset |
| **Packing Tag** | [Category]_[Element] | Atlas grouping |
| **Filter Mode** | Bilinear | Soft edge for watercolor |
| **Compression** | Normal Quality | Preserve texture |
| **Max Size** | 2048 | Tier 1 max |

### Texture Import Settings

| Property | Setting | Rationale |
|----------|---------|----------|
| **sRGB (Color Texture)** | Yes | Diffuse/albedo |
| **sRGB (Emission)** | No | Data texture |
| **sRGB (Mask)** | No | Data texture |
| **Wrap Mode** | Repeat | Tiling support |
| **mipmap** | Per-tier rule | See Section 3 |

### UI-Specific Constraints

| Setting | Value | Rationale |
|----------|---------|----------|
| **Canvas Scaler** | Scale With Screen Size | Resolution independence |
| **Reference Resolution** | 1920×1080 | 16:9 baseline |
| **Dynamic Pixels Per Unit** | 1 | Sprite crispness |
| **Raycast Target** | On (interactive elements only UI |

### Memory Constraints

| Budget | Value | Notes |
|--------|-------|-------|
| **Texture Memory** | 256MB target | Across all assets |
| **Runtime Allocation** | Pool + reuse | No per-frame allocations |
| **Atlas Size** | 4096×4096 max | URP sprite atlas |

### Platform Constraints

| Platform | Compression | Notes |
|----------|-------------|-------|
| **Windows** | DXT5 (BC3) | Default |
| **WebGL** | ASTC or DXT5 | ASTC preferred for mobile web |
| **Mobile (future)** | ASTC 6×6 | Alpha-ready |

---

## 8. Quality Gates

### Import Validation Checklist

- [ ] All textures power-of-2 (256/512/1024/2048)
- [ ] No JPEG artifacts visible
- [ ] Alpha通道 clean (no premultiplied issues)
- [ ] Normal maps import as Linear, not sRGB
- [ ] Sprite pivot points centered (or per-spec)
- [ ] Material uses correct shader (Painted)
- [ ] Emission masked correctly (white = emission)

### Performance Gate

- [ ] No texture exceeds Tier 1 resolution without approval
- [ ] Sprite atlas packing efficiency > 85%
- [ ] No uncompressed textures in build
- [ ] Mipmaps enabled per-tier rules

### Visual Gate

- [ ] Day/Night sprite pairs align (no offset drift)
- [ ] Color matches palette (Art Bible Section 2)
- [ ] Edge quality preserved (no aliasing)
- [ ] Paper grain visible at 100% zoom

---

## Appendix: Asset Delivery Template

### File Organization

```
Assets/
├── _Project/
│   ├── Art/
│   │   ├── Source/           # PSD, FBX source files
│   │   ├── Sprites/         # Final PNG exports
│   │   ├── Materials/       # .mat files
│   │   ├── Shaders/        # .shadergraph, .hlsl
│   │   └── atlases/         # Built atlases
│   └── Textures/            # Standalone textures
```

### Manifest Entry Example

```yaml
sp_lantern_post_day:
  category: sp_
  descriptor: lantern_post
  state: day
  tier: 1
  resolution: 1024x1024
  atlas: environment
  shader_input: [main, emission]
  notes: "Hero prop, emission on paper texture"
```

---

*This section is complete when approved by Technical Director and Art Director jointly.*