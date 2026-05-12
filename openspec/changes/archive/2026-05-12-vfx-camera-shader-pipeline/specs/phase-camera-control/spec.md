## ADDED Requirements

### Requirement: Phase Transition Camera Management
The system SHALL manage the transition between the Day phase (fixed framing) and Night phase (player following) using Cinemachine Virtual Cameras.

#### Scenario: Switch from Day to Night Phase
- **WHEN** the game phase changes from Day to Night
- **THEN** the `PhaseCameraController` SHALL set the Day Virtual Camera priority to 0 and the Night Virtual Camera priority to 10.

#### Scenario: Switch from Night to Day Phase
- **WHEN** the game phase changes from Night to Day
- **THEN** the `PhaseCameraController` SHALL set the Day Virtual Camera priority to 10 and the Night Virtual Camera priority to 0.
