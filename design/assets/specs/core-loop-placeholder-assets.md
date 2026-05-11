# Placeholder Assets — Core Loop (Development)

> **Purpose**: Temporary colored blocks for physics/logic prototyping
> **Generated**: 2026-05-07
> **Status**: Placeholder — replace with final art later

---

## Design Notes (from GDD)

- Player must fully fit inside Mound cover volumes (cover detection requires full containment)
- Night phase is side-scrolling with horizontal lane
- Movement speed: 2.0-8.0 units/sec (default 5.0)

---

## ASSET-P001 — Player Character

| Field | Value |
|-------|-------|
| Category | Placeholder (Character) |
| Dimensions (Bounding Box) | **Width: 0.6m, Height: 1.8m, Depth: 0.4m** |
| Pivot Position | **Center-bottom** (feet at Y=0) |
| Resolution (Temp) | Solid color — no texture needed |
| Color (Unity) | `Cyan` (#00FFFF) — high visibility |

**Hitbox Notes**:
- Capsule collider or box collider recommended
- Center at (0, 0.9, 0) — pivot is feet
- Y-range: 0 to 1.8 meters

**For Cover Check**:
- Player must be **fully inside** Mound collider
- Mound interior must be at least 0.8m × 2.0m to fit player

---

## ASSET-P002 — Safe Mound (Mo Thuong)

| Field | Value |
|-------|-------|
| Category | Placeholder (Environment/Cover) |
| Dimensions (Bounding Box) | **Width: 1.5m, Height: 2.5m, Depth: 1.0m** |
| Pivot Position | **Center-bottom** (base at Y=0) |
| Resolution (Temp) | Solid color — no texture needed |
| Color (Unity) | `Green` (#00FF00) — safe cover |

**Cover Validation**:
- Player collider (0.6 × 1.8) must be fully inside this volume
- Interior clear zone: minimum 1.0m × 2.0m

---

## ASSET-P003 — Cursed Mound (Mo Oan)

| Field | Value |
|-------|-------|
| Category | Placeholder (Environment/Cover + Item) |
| Dimensions (Bounding Box) | **Width: 1.5m, Height: 2.5m, Depth: 1.0m** |
| Pivot Position | **Center-bottom** (base at Y=0) |
| Resolution (Temp) | Solid color — no texture needed |
| Color (Unity) | `Magenta` (#FF00FF) — danger + reward |

**Behavior**:
- Same size as Safe Mound (for cover)
- Player proximity triggers Bone Relic pickup zone (radius 1.0m)

---

## ASSET-P004 — False Safe Mound (Trap)

| Field | Value |
|-------|-------|
| Category | Placeholder (Environment/Cover + Trap) |
| Dimensions (Bounding Box) | **Width: 1.5m, Height: 2.5m, Depth: 1.0m** |
| Pivot Position | **Center-bottom** (base at Y=0) |
| Resolution (Temp) | Solid color — no texture needed |
| Color (Unity) | `Yellow` (#FFFF00) — warning/trap |

**Behavior**:
- Same size as Safe Mound (deceptive)
- Triggers warning tell (0.9s) before collapse
- Player must dodge during tell window

---

## ASSET-P005 — Searchlight Sweep Zone

| Field | Value |
|-------|-------|
| Category | Placeholder (Hazard/Zone) |
| Dimensions (Bounding Box) | **Width: 5.0m (sweep width), Height: 10.0m, Depth: 0.5m** |
| Pivot Position | **Center** |
| Resolution (Temp) | Solid color with transparency |
| Color (Unity) | `Red` with 50% alpha (#FF000080) |

**Behavior**:
- Sweep interval: 6.0 seconds
- Sweep width: 5.0 units (from GDD)
- Telegraph duration: 1.5 seconds before strike

---

## ASSET-P006 — Shrine (Start/End)

| Field | Value |
|-------|-------|
| Category | Placeholder (Objective) |
| Dimensions (Bounding Box) | **Width: 2.0m, Height: 3.0m, Depth: 2.0m** |
| Pivot Position | **Center-bottom** |
| Resolution (Temp) | Solid color — no texture needed |
| Color (Unity) | `Gold` (#FFD700) — objective |

**Behavior**:
- Start Shrine: Player spawns here at night start
- End Shrine: Triggers win condition on E-key interaction

---

## ASSET-P007 — Lane/Ground

| Field | Value |
|-------|-------|
| Category | Placeholder (Environment) |
| Dimensions | **Width: 100.0m (example), Height: 0.5m, Depth: 4.0m** |
| Pivot Position | **Center-top** (surface at Y=0) |
| Resolution (Temp) | Solid color — no texture needed |
| Color (Unity) | `Gray` (#808080) — ground |

**Notes**:
- Y-level should be at 0 (ground surface)
- Collider type: Box collider for ground plane
- Player walks on top surface

---

## ASSET-P008 — Boss Searchlight Source

| Field | Value |
|-------|-------|
| Category | Placeholder (Boss/Threat) |
| Dimensions (Visual Only) | **Cone angle: 30°, Range: 50m** |
| Position | **Off-screen (offshore background)** |
| Resolution (Temp) | Directional light + colored cone mesh |
| Color (Unity) | `Green` (#00FF00) — whale searchlight |

**Behavior**:
- Not physically in play area — background element
- Projects sweep cone across lane
- Player detection via sweep cone intersection

---

## Unity Setup Quick Reference

### Player Setup
```
GameObject: "Player"
├── Transform: Position (0, 0.9, 0)
├── CharacterController: 
│   ├── Center: (0, 0.9, 0)
│   ├── Radius: 0.3
│   └── Height: 1.8
└── SpriteRenderer (placeholder): Cyan box
```

### Mound Setup
```
GameObject: "Mound_Safe"
├── Transform: Position (X, 1.25, Z)
├── BoxCollider:
│   ├── Size: (1.5, 2.5, 1.0)
│   └── IsTrigger: false (for cover)
└── SpriteRenderer: Green box
```

### Ground Setup
```
GameObject: "Ground"
├── Transform: Position (50, -0.25, 0)
├── BoxCollider:
│   ├── Size: (100, 0.5, 4)
└── SpriteRenderer: Gray box
```

---

## Next Steps

| Action | Command |
|--------|---------|
| Create actual sprites | `/asset-spec system:player-controller` |
| Create VFX | `/asset-spec system:boss-ca-ong-searchlight` |
| Audit placeholders | `/asset-audit` |

---

*End of Placeholder Assets Spec*