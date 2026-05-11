// Assets/_Project/Application/Services/Interfaces/IStrikeWarningController.cs
using System.Collections.Generic;
using R3;
using SolarPhobia.Domain.ValueObjects;
using UnityEngine;

namespace SolarPhobia.Application.Services
{
    /// <summary>
    /// Manages the ordered set of active strike warnings and drives the Warning_Icon display.
    /// Implements TR-player-009: Strike Warning Integration (multi-warning priority selection).
    ///
    /// Rules:
    ///   - Maintains ordered List&lt;StrikeWarning&gt; of all active warnings
    ///   - Displays only the nearest strike's icon (by distance to player position)
    ///   - Re-evaluates on every register/deregister
    ///   - Phase-gated: only active in NightMovement mode
    ///   - ClearAll() called on phase exit
    /// </summary>
    public interface IStrikeWarningController
    {
        /// <summary>
        /// Whether the warning icon should currently be displayed.
        /// True when at least one active warning exists and mode is NightMovement.
        /// </summary>
        ReadOnlyReactiveProperty<bool> IsWarningActive { get; }

        /// <summary>Read-only view of the active warning list (for testing).</summary>
        IReadOnlyList<StrikeWarning> ActiveWarnings { get; }

        /// <summary>
        /// Processes a strike warning event from the Map Director.
        /// true = register new warning; false = deregister most-recent warning.
        /// </summary>
        /// <param name="warningActive">true = warning started, false = warning cleared.</param>
        /// <param name="mode">Current player input mode — warning only shown in NightMovement.</param>
        /// <param name="playerPosition">Player's current world-space position at the time of the event.</param>
        void OnStrikeWarningReceived(bool warningActive, PlayerInputMode mode, Vector2 playerPosition);

        /// <summary>
        /// Reports the player's current position and bounds to the Map Director.
        /// Also updates the cached player position used for nearest-strike selection.
        /// Call once per frame during NightSurvival.
        /// </summary>
        /// <param name="position">Player's current world-space position.</param>
        /// <param name="bounds">Axis-aligned bounds of the player's active collider.</param>
        /// <param name="mode">Current player input mode.</param>
        void ReportPlayerPosition(Vector2 position, Bounds bounds, PlayerInputMode mode);

        /// <summary>
        /// Clears all active warnings and hides the icon.
        /// Called on NightSurvival phase exit.
        /// </summary>
        void ClearAll();
    }
}
