## 1. Stable Include Path

- [x] 1.1 Move `SolarPhobiaGlobals.hlsl` from `Assets/_Project/_Art/Shaders/Environments/` to `Assets/_Project/_Art/Shaders/` root
- [x] 1.2 Update `HeatWall_World.shader` include to `Assets/_Project/_Art/Shaders/SolarPhobiaGlobals.hlsl`
- [x] 1.3 Update `HeatWall_EdgeBurn.shader` include to `Assets/_Project/_Art/Shaders/SolarPhobiaGlobals.hlsl`
- [x] 1.4 Update `SensoryDecay.shader` include to `Assets/_Project/_Art/Shaders/SolarPhobiaGlobals.hlsl`
- [x] 1.5 Delete old `Assets/_Project/_Art/Shaders/Environments/SolarPhobiaGlobals.hlsl`

## 2. CBUFFER and Texture Declaration

- [x] 2.1 Convert `sampler2D _MainTex` to `TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);`
- [x] 2.2 Wrap all material properties in `CBUFFER_START(UnityPerMaterial)` / `CBUFFER_END`
- [x] 2.3 Move `tex2D` calls to `SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv)`

## 3. Fragment-Space Distortion

- [x] 3.1 Move noise/hash function from vertex to fragment shader
- [x] 3.2 Add vertex-to-fragment `float3 positionWS` interpolator
- [x] 3.3 Apply `worldPos.xy` for 2D noise input (not `.xz`)
- [x] 3.4 Compute per-pixel UV offset from noise in fragment shader

## 4. New Material Properties

- [x] 4.1 Add `_DistortionStrength` Range property to `Properties` block
- [x] 4.2 Add `_EdgeIntensity` Range property to `Properties` block
- [x] 4.3 Declare both in `UnityPerMaterial` CBUFFER
- [x] 4.4 Wire `_DistortionStrength` into fragment noise intensity
- [x] 4.5 Wire `_EdgeIntensity` into fragment colour mix

## 5. Defensive Clamping

- [x] 5.1 Wrap `_SP_DayPressure01` with `saturate()` in distortion calculation
- [x] 5.2 Wrap `_SP_DayPressure01` with `saturate()` in alpha fade

## 6. Verify and Clean Up

- [x] 6.1 Open `HeatWall_World.shader` in Unity, verify no compile errors
- [x] 6.2 Open `HeatWall_EdgeBurn.shader` in Unity, verify no compile errors
- [x] 6.3 Open `SensoryDecay.shader` in Unity, verify no compile errors
- [ ] 6.4 Verify heat wall displays correctly in URP 2D Game View (manual - test in Game View)
- [ ] 6.5 Verify edge burn remains readable against grayscale background (manual - test in Game View)
- [ ] 6.6 Verify heat shimmer is not glued to camera (follows world-space) (manual - test in Game View)
- [x] 6.7 Verify SRP Batcher reports the material as compatible - SRP Batcher: True, CBUFFER pattern correct
