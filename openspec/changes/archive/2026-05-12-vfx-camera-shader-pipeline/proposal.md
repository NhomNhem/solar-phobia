## Why

The transition between the stationary ritual phase (Day) and the dynamic parkour phase (Night) in Solar Phobia requires a unified but flexible camera and shader pipeline. Currently, we lack a structured framework to handle phase-dependent camera behaviors, global shader parameters for environmental hazards (like the Heat Wall), and post-processing effects that represent the player's sensory and mental state. This change establishes the foundational VFX and camera architecture to support these core gameplay loops.

## What Changes

- **New Phase-Based Camera System**: Implementation of `PhaseCameraController` to manage the transition between fixed Cinemachine framing (Day) and smooth-follow tracking (Night).
- **Global VFX Management**: Introduction of `SolarPhobiaVFXDirector` to drive global shader parameters and post-process volumes.
- **Universal Shader Parameter API**: Formalized `ShaderPropertyIds` to provide type-safe access to global shader variables.
- **Unified Hazard Visuals**: Hybrid Heat Wall effect combining screen-space edge burning and world-space noise distortion.
- **Sensory Post-Processing**: Shader-driven screen-space effects for mental/sensory decay.

## Capabilities

### New Capabilities
- `phase-camera-control`: Managing transition between fixed and dynamic Cinemachine cameras.
- `global-shader-parameters`: Centralized control of phase, pressure, and position data for all shaders.
- `environment-hazard-vfx`: World-space and screen-space hybrid effects for the Heat Wall and ocean.
- `sensory-decay-fx`: Screen-space post-processing for mental/camera effects.

### Modified Capabilities
- (None - First implementation of these systems)

## Impact

- **Systems**: Camera (Cinemachine), Rendering (URP 2D / Unity 6 Post-Processing).
- **Global State**: All world and screen-space shaders will now rely on `SolarPhobiaVFXDirector` for core environmental data.
- **Workflow**: Shader development must now strictly adhere to the world-space vs. screen-space architectural split.
