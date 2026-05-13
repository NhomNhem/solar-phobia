## Why

The HeatWall_World shader and its global include pipeline were implemented as a functional prototype but lack the production hardening needed for Unity 6 URP 2D. Include paths are relative and fragile, material properties bypass SRP Batcher, distortion is computed per-vertex (too coarse for heat haze), and noise uses worldPos.xz (incorrect for a 2D XY-plane game). Without these fixes, the shader will not survive a production build, will not batch efficiently, and will visually break under the Limbo-style 2D camera framing.

## What Changes

- **Include Path Stabilization**: Move `SolarPhobiaGlobals.hlsl` to a stable shared location (`Assets/_Project/_Art/Shaders/`) and use project-relative include paths (`Assets/_Project/_Art/Shaders/SolarPhobiaGlobals.hlsl`).
- **SRP Batcher CBUFFER**: Wrap all material properties (`_MainTex`, `_Color`, `_NoiseScale`, `_NoiseSpeed`, `_DistortionStrength`, `_EdgeIntensity`) in `CBUFFER_START(UnityPerMaterial)` / `CBUFFER_END`.
- **2D World-Space Noise**: Change noise sampling from `worldPos.xz` to `worldPos.xy` for correct 2D XY-plane behaviour.
- **Fragment-Space Distortion**: Move heat shimmer and noise calculation from vertex to fragment shader for per-pixel heat haze.
- **New Material Properties**: Add `_DistortionStrength` and `_EdgeIntensity` parameters with sensible defaults.
- **Global Pressure Clamping**: Clamp `_SP_DayPressure01` usage to safe `[0,1]` range in the shader.
- **Preserve Hybrid Design**: Keep the dual-layer architecture — screen-space edge burn (HeatWall_EdgeBurn) + world-space noise (HeatWall_World).

## Capabilities

### New Capabilities
- `heatwall-production-hardening`: Production-ready HeatWall_World shader with SRP Batcher compatibility, 2D-correct world-space noise, per-pixel heat distortion, and stable include paths for Unity 6 URP 2D.

### Modified Capabilities
- (None — this change only refines implementation details; functional requirements from `environment-hazard-vfx` spec remain unchanged.)

## Impact

- **Shaders**: `HeatWall_World.shader` — major rewrite of distortion approach and CBUFFER. `SolarPhobiaGlobals.hlsl` — moved to stable path.
- **Includes**: All shaders referencing `SolarPhobiaGlobals.hlsl` via relative paths must be updated to the new project-absolute path.
- **Performance**: Negligible. Fragment noise is cheap (simple hash), and CBUFFER enables SRP Batcher which reduces draw calls.
- **Visuals**: Heat shimmer will no longer appear glued to the camera. Noise follows world XY-plane. Edge burn remains readable.
