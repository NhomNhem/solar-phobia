## ADDED Requirements

### Requirement: Sensory Decay Post-Processing
The system SHALL provide a post-processing effect that distorts the camera view based on the player's mental or sensory decay level.

#### Scenario: Increase Sensory Decay
- **WHEN** the `_SP_SensoryDecay01` parameter increases above 0.5
- **THEN** the screen-space shader SHALL apply chromatic aberration and static noise proportional to the decay level.
