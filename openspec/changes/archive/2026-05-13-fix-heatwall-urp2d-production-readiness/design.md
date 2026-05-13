## Context

The HeatWall uses a dual-layer hybrid design: a world-space mesh shader (`HeatWall_World.shader`) that applies noise-based distortion, and a screen-space fullscreen pass (`HeatWall_EdgeBurn.shader`) that burns the frame edges. Both rely on shared global parameters from `SolarPhobiaGlobals.hlsl` (`_SP_DayPressure01`), which is currently co-located with `HeatWall_World.shader` in `Environments/` and included via fragile relative paths from the `PostProcess/` folder.

The world shader uses vertex-based distortion with `worldPos.xz` (3D XZ-plane), but Solar Phobia is a 2D game running on the XY-plane. Material properties are bare globals — no `UnityPerMaterial` CBUFFER, so SRP Batcher cannot batch them.

Three shaders reference `SolarPhobiaGlobals.hlsl`: `HeatWall_World.shader` (relative), `HeatWall_EdgeBurn.shader` (relative), and `SensoryDecay.shader` (relative).

## Goals / Non-Goals

**Goals:**
- Move `SolarPhobiaGlobals.hlsl` to a stable shared location and update all three includes.
- Wrap material properties in `UnityPerMaterial` CBUFFER for SRP Batcher compatibility.
- Change noise from `worldPos.xz` to `worldPos.xy` (correct 2D plane).
- Move distortion from vertex to fragment shader for per-pixel heat haze.
- Add `_DistortionStrength` and `_EdgeIntensity` material properties.
- Clamp `_SP_DayPressure01` with `saturate()` to guard against out-of-range values.

**Non-Goals:**
- No functional requirement changes to the heat wall behaviour (spec unchanged).
- No changes to `SensoryDecay.shader` beyond the include path update.
- No changes to `HeatWall_EdgeBurn.shader` beyond the include path update and CBUFFER.
- No Shader Graph conversion — stays HLSL/ShaderLab.

## Decisions

1. **Stable include folder**: Place `SolarPhobiaGlobals.hlsl` at `Assets/_Project/_Art/Shaders/` root (shared between `Environments/` and `PostProcess/`). Use project-relative path `Assets/_Project/_Art/Shaders/SolarPhobiaGlobals.hlsl` in `#include`. Unity 6 supports this syntax reliably. Alternative (putting it in a `Shared/` subfolder) adds unnecessary depth.

2. **CBUFFER scoping**: Wrap only per-material properties (`_MainTex_ST`, `_Color`, `_NoiseScale`, `_NoiseSpeed`, `_DistortionStrength`, `_EdgeIntensity`) in `UnityPerMaterial`. Global parameters (`_SP_DayPressure01`, etc.) stay as standalone `float` uniforms outside CBUFFER — they are set via `Shader.SetGlobal*` and are not per-material.

3. **Fragment-space distortion**: Use a simple 2D noise function (`hash(float2)`) in the fragment shader to sample world-space position with `_NoiseScale` and `_Time` tiling. Multiply by `_DistortionStrength * _SP_DayPressure01` for intensity. Apply as UV offset to `_MainTex` sampling, and as a subtle vertex position offset passed via interpolator. Alternative (full screen-space grab pass) would break the world-space nature of the effect.

4. **Texture declaration**: Switch from `sampler2D` to `TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);` for SRP Batcher compatibility.

5. **Clamping strategy**: Apply `saturate(_SP_DayPressure01)` at point of use rather than at the property definition, since the global is set externally and we want defensive safety without modifying the global parameter system.

6. **Noise domain**: Use `worldPos.xy` (not `.xz`) since the game operates on the XY world plane. This ensures heat shimmer follows world-space positions correctly under the fixed Day camera framing.

## Risks / Trade-offs

- [Risk] Fragment noise on many visible heat wall quads could be expensive. → Mitigation: Noise is a single `hash()` call — negligible cost. SRP Batcher savings offset any fragment cost.
- [Risk] CBUFFER may cause unexpected property warnings if names mismatch the C# side. → Mitigation: Use exact property names as defined in `Properties` block. The C# `MaterialPropertyBlock` / `Shader.PropertyToID` will match automatically.
- [Risk] 2D (`worldPos.xy`) noise may reveal tiling patterns at extreme scales. → Mitigation: `_NoiseScale` tuning; if needed later, add a secondary noise layer (out of scope here).
- [Risk]_SP_DayPressure01 could be NaN or negative from gameplay code. → Mitigation: `saturate()` catches all out-of-range cases.

## Open Questions

- Should `_EdgeIntensity` also be added to `HeatWall_EdgeBurn.shader`? The user specification lists it for the world shader only; the edge burn already uses `_SP_DayPressure01` as its intensity driver. The edge burn likely already reads fine, but `_EdgeIntensity` could be added as a multiplier in a future pass.
