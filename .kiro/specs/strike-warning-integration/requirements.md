# Requirements Document

## Introduction

The Strike Warning Integration feature connects the Map Director's strike telegraph system to the player's reticle UI in Solar Phobia. When the Map Director signals an incoming strike, a warning icon appears near the player's reticle on a Screen Space — Overlay canvas at the highest sort order, ensuring it renders above all Tier 4 panic effects (chromatic aberration, screen shake). The feature also maintains a continuous position feed from the Player Controller to the Map Director each frame during NightSurvival, enabling the director to perform cover validation and sweep targeting.

This feature is governed by ADR-0003 (Player Controller Pattern) and implements TR-player-009.

---

## Glossary

- **Player_Controller**: The MonoBehaviour responsible for player input, movement, and UI state during NightSurvival. Subscribes to Map Director events and drives the reticle HUD.
- **Map_Director**: The service implementing `IMapSpawnDirector`. Emits `OnStrikeWarning` and `OnStrikeResolved` observables and consumes player position data.
- **Strike_Warning_Controller**: The component responsible for managing the active set of strike warnings and selecting the highest-priority icon to display.
- **Reticle_HUD**: The Screen Space — Overlay canvas element that hosts the player's reticle and the strike warning icon.
- **Warning_Icon**: The UI element displayed near the reticle when a strike is incoming.
- **OnStrikeWarning**: The `Observable<bool>` on `IMapSpawnDirector` that emits `true` when a strike telegraph begins.
- **OnStrikeResolved**: The `Observable<bool>` on `IMapSpawnDirector` that emits `false` when a strike telegraph ends or is cancelled.
- **Tier_4_Effects**: Post-processing effects active during the DeathSpiral sensory tier, including chromatic aberration and screen shake, rendered below the highest canvas sort order.
- **NightSurvival**: The game phase during which the player can move, sprint, and interact. The only phase in which strike warnings are active and position updates are sent.
- **UpdatePlayerPosition**: The method `IMapSpawnDirector.UpdatePlayerPosition(Vector2 position, Bounds bounds)` called each frame to report the player's current world position and collider bounds.
- **Priority_Selection**: The rule that when multiple simultaneous strike warnings are active, only the warning icon for the nearest or highest-priority strike is displayed.

---

## Requirements

### Requirement 1: Subscribe to Strike Warning Events

**User Story:** As a player, I want to see a warning icon near my reticle when a strike is incoming, so that I have time to react and seek cover.

#### Acceptance Criteria

1. WHEN the `NightSurvival` phase becomes active, THE `Player_Controller` SHALL subscribe to `Map_Director.OnStrikeWarning`.
2. WHEN the `NightSurvival` phase exits, THE `Player_Controller` SHALL unsubscribe from `Map_Director.OnStrikeWarning` and dispose the subscription.
3. WHEN `Map_Director.OnStrikeWarning` emits `true`, THE `Strike_Warning_Controller` SHALL register the incoming strike and trigger display of the `Warning_Icon` near the `Reticle_HUD`.
4. WHEN `Map_Director.OnStrikeWarning` emits `false` (strike resolved or cancelled), THE `Strike_Warning_Controller` SHALL deregister the resolved strike and update the `Warning_Icon` display accordingly.
5. IF `Player_Controller` receives `OnStrikeWarning` while the current phase is not `NightSurvival`, THEN THE `Strike_Warning_Controller` SHALL ignore the event and perform no UI update.

---

### Requirement 2: Warning Icon Z-Order Above Tier 4 Effects

**User Story:** As a player, I want the strike warning icon to always be visible above panic effects like chromatic aberration, so that the warning is never obscured when I need it most.

#### Acceptance Criteria

1. THE `Reticle_HUD` canvas SHALL be configured as Screen Space — Overlay with a sort order higher than all `Tier_4_Effects` post-processing layers.
2. WHILE `Tier_4_Effects` (chromatic aberration, screen shake) are active, THE `Warning_Icon` SHALL remain fully visible and unobscured.
3. THE `Warning_Icon` sort order SHALL be set at canvas initialisation and SHALL NOT change at runtime.

---

### Requirement 3: Priority Selection for Multiple Simultaneous Warnings

**User Story:** As a player, I want to see only one warning icon when multiple strikes overlap, so that the UI stays readable during the most intense moments.

#### Acceptance Criteria

1. THE `Strike_Warning_Controller` SHALL maintain an ordered list of all currently active strike warnings.
2. WHEN two or more strike warnings are simultaneously active, THE `Strike_Warning_Controller` SHALL display the `Warning_Icon` for the nearest strike to the player's current position only.
3. WHEN the nearest active strike resolves, THE `Strike_Warning_Controller` SHALL immediately re-evaluate the remaining active warnings and display the `Warning_Icon` for the new nearest strike.
4. WHEN all active strike warnings resolve, THE `Strike_Warning_Controller` SHALL hide the `Warning_Icon`.
5. THE `Reticle_HUD` SHALL display at most one `Warning_Icon` at any time regardless of the number of simultaneous active warnings.

---

### Requirement 4: Warning Icon Clears on Strike Resolution

**User Story:** As a player, I want the warning icon to disappear as soon as the strike resolves, so that I am not misled by a lingering icon after the danger has passed.

#### Acceptance Criteria

1. WHEN `Map_Director.OnStrikeWarning` emits `false` for the currently displayed strike, THE `Strike_Warning_Controller` SHALL hide the `Warning_Icon` within the same frame.
2. IF the resolved strike was not the currently displayed warning (a lower-priority warning resolved), THEN THE `Strike_Warning_Controller` SHALL remove it from the active list without changing the displayed `Warning_Icon`.
3. WHEN `NightSurvival` phase exits with one or more active warnings still registered, THE `Strike_Warning_Controller` SHALL clear all active warnings and hide the `Warning_Icon`.
4. THE `Warning_Icon` SHALL NOT remain visible after all active strike warnings have resolved.

---

### Requirement 5: Player Position Feed to Map Director

**User Story:** As the Map Director, I need the player's world position and collider bounds each frame during NightSurvival, so that I can perform accurate cover validation and sweep targeting.

#### Acceptance Criteria

1. WHILE the `NightSurvival` phase is active, THE `Player_Controller` SHALL call `Map_Director.UpdatePlayerPosition(Vector2 position, Bounds bounds)` once per `Update()` tick.
2. THE `Player_Controller` SHALL pass the player's current world-space `Vector2` position and the axis-aligned `Bounds` of the player's active collider to `UpdatePlayerPosition`.
3. WHEN the `NightSurvival` phase exits, THE `Player_Controller` SHALL cease calling `UpdatePlayerPosition`.
4. IF `Map_Director` is null or not yet initialised when `UpdatePlayerPosition` would be called, THEN THE `Player_Controller` SHALL skip the call and log a warning via `Debug.LogWarning`.
