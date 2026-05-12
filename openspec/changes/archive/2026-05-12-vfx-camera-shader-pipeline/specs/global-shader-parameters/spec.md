## ADDED Requirements

### Requirement: Centralized Shader Parameter Updates
The `SolarPhobiaVFXDirector` SHALL update global shader parameters every frame based on the current game state.

#### Scenario: Update Global Phase Parameter
- **WHEN** the current game phase value changes
- **THEN** the `SolarPhobiaVFXDirector` SHALL call `Shader.SetGlobalFloat("_SP_Phase01", value)`.

#### Scenario: Update Player World Position
- **WHEN** the player character moves in the world
- **THEN** the `SolarPhobiaVFXDirector` SHALL call `Shader.SetGlobalVector("_SP_PlayerWorldPos", playerPos)`.

### Requirement: Type-Safe Shader Property Access
The system SHALL use a static `ShaderPropertyIds` class to cache property IDs for all global parameters.

#### Scenario: Initialization of Property IDs
- **WHEN** the game starts
- **THEN** the `ShaderPropertyIds` class SHALL initialize integer IDs using `Shader.PropertyToID` for all required parameters.
