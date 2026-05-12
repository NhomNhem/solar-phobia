## 1. Global Shader Infrastructure

- [x] 1.1 Create `ShaderPropertyIds.cs` to cache property IDs for all `_SP_*` global parameters.
- [x] 1.2 Implement `SolarPhobiaVFXDirector.cs` as a persistent Singleton to drive global shader states.
- [x] 1.3 Add update logic to `SolarPhobiaVFXDirector` to set `_SP_Phase01`, `_SP_DayPressure01`, `_SP_NightDanger01`, and `_SP_SensoryDecay01` via `Shader.SetGlobalFloat`.
- [x] 1.4 Add logic to `SolarPhobiaVFXDirector` to track and set `_SP_PlayerWorldPos` via `Shader.SetGlobalVector`.

## 2. Camera System Implementation

- [x] 2.1 Set up two Cinemachine Virtual Cameras (DayFixed, NightFollow) in a test scene.
- [x] 2.2 Implement `PhaseCameraController.cs` to manage priority switching between these cameras.
- [x] 2.3 Add event listeners to `PhaseCameraController` to trigger camera switches when the game phase changes.
- [x] 2.4 Configure Cinemachine Brain blending settings for smooth phase transitions.

## 3. Core Shader HLSL Development

- [x] 3.1 Create `SolarPhobiaGlobals.hlsl` containing the `CBUFFER` for global shader parameters.
- [x] 3.2 Implement the World-Space Heat Wall shader using the globals for noise distortion.
- [x] 3.3 Create the Screen-Space "Edge Burn" post-process shader for the Heat Wall hazard.
- [x] 3.4 Implement the Sensory Decay screen-space shader (chromatic aberration + static noise).

## 4. Post-Processing Integration (Unity 6 / URP 2D)

- [x] 4.1 Create a new Fullscreen Shader Graph (as a wrapper) or Custom Render Pass to host the HLSL screen-space effects.
- [x] 4.2 Configure the URP 2D Renderer Data to include the new custom post-processing passes.
- [x] 4.3 Link the post-process material properties to the `SolarPhobiaVFXDirector` values.

## 5. Validation and Testing

- [x] 5.1 Verify smooth camera blending between Day and Night cameras in the Editor.
- [x] 5.2 Validate that all shaders react correctly to changes in `SolarPhobiaVFXDirector` parameters.
- [x] 5.3 Test the Heat Wall hazard visual progression from low to high pressure.
- [x] 5.4 Confirm that the "Edge Burn" and "Sensory Decay" effects are rendering correctly in the Game view.
