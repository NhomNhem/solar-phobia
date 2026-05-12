## Context

Solar Phobia utilizes Unity 6 (6000.3.11f1) and URP 2D. The game's visual identity relies on a strict Limbo-esque grayscale palette with symbolic colored highlights (warning colors). The gameplay is split into a static "Day" phase (ritual management) and a dynamic "Night" phase (parkour and survival). This dual-phase nature necessitates a camera system that can transition seamlessly and a shader pipeline that reacts to global environmental states (phase, heat pressure, player position).

## Goals / Non-Goals

**Goals:**
- Implement a `PhaseCameraController` that manages two distinct Cinemachine virtual cameras.
- Create a `SolarPhobiaVFXDirector` that acts as the single source of truth for global shader parameters.
- Define a clear separation between world-space and screen-space shader implementations.
- Establish a type-safe `ShaderPropertyIds` class to avoid string-based property lookups in update loops.
- Implement the Heat Wall as a hybrid shader effect.

**Non-Goals:**
- Designing individual boss AI or specific ritual mechanics.
- Full UI/UX implementation beyond post-processing overlays.
- Optimization for mobile platforms (target is PC/Console).

## Decisions

### 1. Scripted HLSL over Shader Graph for Core VFX
- **Decision**: Use handwritten HLSL (.shader + .hlsl) for the Heat Wall, Global Parameters, and Screen-Space effects.
- **Rationale**: Unity 6 URP 2D offers more control over custom render passes and global buffers via code. HLSL allows for cleaner world-space coordinate manipulation and efficient global parameter access that can be shared across multiple materials without per-material setup.
- **Alternatives**: Shader Graph (reserved for low-priority environment props only).

### 2. Global Shader Parameters via `Shader.SetGlobal*`
- **Decision**: Use `Shader.SetGlobalFloat`, `Shader.SetGlobalVector`, etc., driven by `SolarPhobiaVFXDirector`.
- **Rationale**: Centralizing environmental state (Phase, Heat Wall progress) globally ensures all shaders in the scene react synchronously without needing to reference specific material instances.
- **Alternatives**: Material Property Blocks (too granular for global environmental state).

### 3. Cinemachine State-Driven Transitions
- **Decision**: Use Cinemachine's `Brain` and `Virtual Cameras` with priority switching managed by `PhaseCameraController`.
- **Rationale**: Cinemachine handles blending and framing natively. By switching priorities, we get smooth transitions between the fixed "Day" frame and "Night" follow-camera for free.
- **Alternatives**: Manual Camera.main manipulation (too error-prone).

### 4. Hybrid Heat Wall Implementation
- **Decision**: Split the Heat Wall into a world-space noise mesh/plane and a screen-space post-process "edge burn".
- **Rationale**: The world-space component provides physical presence and parallax as the player moves. The screen-space component ensures the "danger" feeling is maintained even when the world-space mesh is off-screen or the player is looking away.

## Risks / Trade-offs

- **[Risk] Unity 6 URP 2D Compatibility** → [Mitigation] Strictly follow the new URP 2D Render Graph APIs for any custom post-processing.
- **[Risk] World-Space Coordinate Precision** → [Mitigation] Use `_SP_PlayerWorldPos` as an anchor to prevent floating point jitter in high-offset world-space shaders.
- **[Risk] Global Parameter Overhead** → [Mitigation] Only update global parameters in `SolarPhobiaVFXDirector` when values actually change (event-driven or dirty flag).
