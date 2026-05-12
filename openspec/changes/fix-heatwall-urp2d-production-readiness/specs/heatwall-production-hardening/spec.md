## ADDED Requirements

### Requirement: SRP Batcher Compatible Material Properties
The HeatWall_World shader SHALL declare all per-material properties inside a `CBUFFER_START(UnityPerMaterial)` / `CBUFFER_END` block.

#### Scenario: Material Properties in UnityPerMaterial
- **WHEN** the shader compiles
- **THEN** the `UnityPerMaterial` CBUFFER SHALL contain `_MainTex_ST`, `_Color`, `_NoiseScale`, `_NoiseSpeed`, `_DistortionStrength`, and `_EdgeIntensity`

#### Scenario: Global Parameters Outside CBUFFER
- **WHEN** the shader compiles
- **THEN** the `_SP_DayPressure01` SHALL remain outside `UnityPerMaterial` as a standalone uniform

### Requirement: Project-Relative Include Paths
All `#include` directives referencing `SolarPhobiaGlobals.hlsl` SHALL use project-absolute paths (`Assets/_Project/_Art/Shaders/SolarPhobiaGlobals.hlsl`).

#### Scenario: HeatWall_World Include
- **WHEN** `HeatWall_World.shader` compiles
- **THEN** its `#include` for `SolarPhobiaGlobals.hlsl` SHALL resolve via a project-absolute path

#### Scenario: HeatWall_EdgeBurn Include
- **WHEN** `HeatWall_EdgeBurn.shader` compiles
- **THEN** its `#include` for `SolarPhobiaGlobals.hlsl` SHALL resolve via a project-absolute path

#### Scenario: SensoryDecay Include
- **WHEN** `SensoryDecay.shader` compiles
- **THEN** its `#include` for `SolarPhobiaGlobals.hlsl` SHALL resolve via a project-absolute path

### Requirement: 2D World-Space Noise
The HeatWall_World shader SHALL sample noise in world-space XY coordinates (not XZ) for correct 2D plane behaviour.

#### Scenario: XY-Plane Noise
- **WHEN** the fragment shader samples noise
- **THEN** the noise input coordinate SHALL use `worldPos.xy` multiplied by `_NoiseScale`

### Requirement: Fragment-Space Heat Distortion
The heat shimmer/noise calculation SHALL be evaluated per-pixel in the fragment shader, not per-vertex.

#### Scenario: Per-Pixel Noise
- **WHEN** the fragment shader runs
- **THEN** the heat distortion offset SHALL be computed from the current pixel's world position, not interpolated from vertex values

### Requirement: Defensive Global Pressure Clamping
The shader SHALL clamp `_SP_DayPressure01` to the `[0, 1]` range at every point of use.

#### Scenario: Clamp in Distortion Calculation
- **WHEN** `_SP_DayPressure01` is used for noise intensity
- **THEN** it SHALL be wrapped in `saturate()` before any multiplication

#### Scenario: Clamp in Alpha Fade
- **WHEN** `_SP_DayPressure01` is used for alpha fade
- **THEN** it SHALL be wrapped in `saturate()` before any multiplication

### Requirement: New Material Parameters
The HeatWall_World shader SHALL expose `_DistortionStrength` (Range) and `_EdgeIntensity` (Range) material properties.

#### Scenario: Distortion Strength Drives Haze
- **WHEN** `_DistortionStrength` is increased
- **THEN** the heat haze offset magnitude SHALL increase proportionally

#### Scenario: Edge Intensity Drives Burn Mix
- **WHEN** `_EdgeIntensity` is increased
- **THEN** the edge burn colour mix SHALL become more prominent
