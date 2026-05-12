## 1. HLSL Path & Macro Standardization

- [x] 1.1 Update `HeatWall_World.shader` to use absolute include path for `SolarPhobiaGlobals.hlsl`.
- [x] 1.2 Update `HeatWall_EdgeBurn.shader` to use absolute include path for `SolarPhobiaGlobals.hlsl`.
- [x] 1.3 Update `SensoryDecay.shader` to use absolute include path for `SolarPhobiaGlobals.hlsl`.
- [x] 1.4 Refactor all shaders to use `TEXTURE2D` and `SAMPLER` macros for texture declarations.

## 2. SRP Batcher Optimization

- [x] 2.1 Wrap all material properties in `HeatWall_World.shader` with `CBUFFER_START(UnityPerMaterial)`.
- [x] 2.2 Wrap all material properties in `HeatWall_EdgeBurn.shader` with `CBUFFER_START(UnityPerMaterial)`.
- [x] 2.3 Wrap all material properties in `SensoryDecay.shader` with `CBUFFER_START(UnityPerMaterial)`.

## 3. Fragment-Space Distortion Refactor

- [x] 3.1 Move distortion logic in `HeatWall_World.shader` from `vert` to `frag`.
- [x] 3.2 Implement noise-based UV perturbation in the fragment stage of `HeatWall_World`.
- [x] 3.3 Ensure the world-space position is correctly interpolated for fragment-space noise calculation.

## 4. Validation

- [x] 4.1 Verify SRP Batcher compatibility for each shader in the Unity Frame Debugger.
- [x] 4.2 Validate smooth distortion visuals in the Game view.
- [x] 4.3 Confirm no include errors across different test scenes.
