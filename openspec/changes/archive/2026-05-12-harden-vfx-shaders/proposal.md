## Why

The initial implementation of the VFX camera and shader pipeline revealed several areas for technical improvement: unstable include paths, lack of SRP Batcher support, and coarse vertex-based distortion for the Heat Wall. To ensure production readiness and visual fidelity in Unity 6, we need to harden these shaders by standardizing their architecture and moving complex calculations to the fragment stage for smoother visual results.

## What Changes

- **Include Path Standardization**: Update all HLSL `#include` directives to use absolute project paths (e.g., `Assets/_Project/...`) to prevent compilation errors across different folder structures.
- **SRP Batcher Optimization**: Wrap all material-specific properties in `CBUFFER_START(UnityPerMaterial)` and standardise texture sampling with `TEXTURE2D`/`SAMPLER` macros.
- **Fragment-Space Distortion**: Refactor the Heat Wall world-space shader to calculate noise distortion per fragment (pixel) instead of per vertex, providing a smoother "heat haze" effect.
- **Standardized Post-Process Blocks**: Align all post-process shaders with Unity 6 URP 2D Render Graph best practices.

## Capabilities

### New Capabilities
- `vfx-shader-hardening`: Standards for SRP Batcher friendliness, absolute path includes, and fragment-space distortion logic.

### Modified Capabilities
- (None - Refinement of implementation details rather than functional requirements)

## Impact

- **Performance**: Improved SRP Batcher compatibility will reduce draw call overhead.
- **Stability**: Standardized paths will reduce build-time errors.
- **Visuals**: Significantly smoother Heat Wall distortion.
