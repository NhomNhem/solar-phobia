# Unity 6 — Breaking Changes

> Last verified: 2026-05-07

This document lists breaking changes from Unity 2022 LTS through Unity 6000.3.x that may affect this project.

---

## HIGH Risk Changes (must address)

### Object.FindObjectsOfType / FindObjectOfType — OBSOLETE

**Status**: Obsolete in Unity 6.0+

**Old API**:
```csharp
var objects = FindObjectsOfType<GameObject>();
var obj = FindObjectOfType<PlayerController>();
```

**Replacement**: Use `Object.FindObjectsByType` with `FindObjectsSortMode.None`:
```csharp
var objects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
var obj = Object.FindObjectOfType<PlayerController>();
```

**Risk**: Legacy code using old APIs will break compilation.

---

### Enlighten Baked Global Illumination — REMOVED

**Status**: Removed in Unity 6.0

**Impact**: If project uses Enlighten for GI, must switch to another solution (e.g., URP/HDRP Progressive GPU).

**Recommendation**: For this project, use URP with realtime GI or lightprobes.

---

### Auto-Generated Lighting — REMOVED

**Status**: Removed in Unity 6.0

**Impact**: Projects relying on automatic lighting generation need explicit LightingSettings.

---

### Universal Compatibility Mode — REMOVED

**Status**: Removed in Unity 6.3 LTS

**Impact**: Projects using Universal Compatibility Mode must migrate to standard rendering.

---

## MEDIUM Risk Changes (be aware)

### UI Toolkit Event Handling — REORGANIZED

**Status**: Changed in Unity 6.0+

**Impact**: Custom UXML controls may need event handler updates.

**Reference**: `docs/engine-reference/unity/modules/ui-toolkit.md`

---

### [SerializeField] — Stricter Validation

**Status**: Changed in Unity 6.3+

**Impact**: `[SerializeField]` can now only be applied to fields. Applying to properties/methods is compile error.

---

### Android UnityPlayer Class — Renamed

**Status**: Changed in Unity 6.0+

**Old**: `UnityPlayer` extends `FrameLayout`  
**New**: Use `UnityPlayerForActivityOrService` or `UnityPlayerForApplication`

---

### LightingSettings Gaussian Filter — Floating Point

**Status**: Changed in Unity 6.0+

**Old**: Integer radius values  
**New**: Floating point radius values

---

## LOW Risk Changes (cosmetic/niche)

- Graphics formats: `DepthAuto`, `ShadowAuto`, `VideoAuto` obsolete
- Mipmap Limits no longer affect runtime Textures by default
- Package Manager: `UPM_CACHE_PATH` env vars no longer supported
- Light probes: ambient probe and skybox reflection probe no longer baked by default

---

## Cross-Reference

- See `deprecated-apis.md` for APIs to avoid
- See `current-best-practices.md` for recommended alternatives
- See `modules/ui-toolkit.md` for UI Toolkit migration details