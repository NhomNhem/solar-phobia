## ADDED Requirements

### Requirement: Hybrid Heat Wall Rendering
The Heat Wall SHALL be rendered using a combination of a world-space noise mesh and a screen-space post-process effect.

#### Scenario: World-Space Heat Noise
- **WHEN** the player is within range of the Heat Wall
- **THEN** the world-space mesh shader SHALL apply a scrolling noise distortion using `_SP_DayPressure01`.

#### Scenario: Screen-Space Edge Burn
- **WHEN** the Heat Wall pressure increases
- **THEN** the screen-space post-process effect SHALL apply a color-tinted "burn" effect to the edges of the camera frame.
