## Context

The initial VFX camera and shader pipeline implementation established the core C# infrastructure and basic HLSL shaders. However, the shaders were implemented with vertex-based distortion and lacked standard URP/Unity 6 optimizations for batching and include path stability. This design addresses those technical debts.

## Goals / Non-Goals

**Goals:**
- Convert all vertex-space distortion logic to fragment-space logic for smoother visuals.
- Standardize all HLSL scripts to be SRP Batcher friendly.
- Replace relative include paths with absolute project-relative paths.
- Update texture sampling to use modern URP macros.

**Non-Goals:**
- Changing the C# `SolarPhobiaVFXDirector` logic.
- Adding new VFX effects outside of the current scope (Heat Wall, Sensory Decay).

## Decisions

### 1. Fragment-Space Distortion via Screen UV Perturbation
- **Decision**: Calculate world-space noise in the fragment shader and use it to perturb the UVs (or screen UVs for post-processing).
- **Rationale**: Vertex-based distortion is limited by the geometry resolution. Fragment-space distortion provides per-pixel precision, which is essential for "heat haze" and "sensory decay" effects to look smooth on simple quads or fullscreen quads.
- **Alternatives**: Increasing mesh resolution (too much performance overhead).

### 2. SRP Batcher Standardization
- **Decision**: Wrap all non-global properties in `CBUFFER_START(UnityPerMaterial)`.
- **Rationale**: This is a mandatory optimization for URP. It allows Unity to batch objects sharing the same shader but having different material properties, reducing CPU overhead for draw calls.
- **Alternatives**: Ignoring batching (poor performance).

### 3. Absolute Include Paths
- **Decision**: Use `Assets/_Project/_Art/Shaders/Environments/SolarPhobiaGlobals.hlsl` format for all includes.
- **Rationale**: Relative paths (`../Globals.hlsl`) break easily if shaders are moved or if they are included from different directory depths (e.g., from a test scene folder). Absolute paths are robust.

## Risks / Trade-offs

- **[Risk] Fragment shader performance** → [Mitigation] Keep the noise hash functions simple (avoid multi-octave Perlin if possible) and use half-precision where appropriate.
- **[Risk] Path fragility** → [Mitigation] Ensure the directory structure is stable before committing to absolute paths.
