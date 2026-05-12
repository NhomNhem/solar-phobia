## ADDED Requirements

### Requirement: SRP Batcher Compatibility
All VFX shaders SHALL be compatible with the Unity SRP Batcher.

#### Scenario: Successful Batching
- **WHEN** multiple objects using the same shader (but different material properties) are rendered
- **THEN** the SRP Batcher SHALL handle them in a single batch, provided all material properties are enclosed in a `UnityPerMaterial` CBUFFER.

### Requirement: Fragment-Based Noise Distortion
The Heat Wall and Sensory Decay shaders SHALL perform noise calculations in the fragment (pixel) stage.

#### Scenario: Smooth Heat Haze
- **WHEN** the Heat Wall shader is applied to a quad
- **THEN** the noise distortion SHALL appear continuous and smooth across the surface, regardless of the mesh's vertex density.

### Requirement: Standardized Absolute Includes
All HLSL include directives SHALL use the full project-relative path.

#### Scenario: Robust Compilation
- **WHEN** a shader is compiled from any directory in the project
- **THEN** the `#include "Assets/_Project/_Art/Shaders/Environments/SolarPhobiaGlobals.hlsl"` directive SHALL correctly resolve the file.
