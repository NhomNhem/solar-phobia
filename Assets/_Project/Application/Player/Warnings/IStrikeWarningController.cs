using System.Collections.Generic;
using R3;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Player.Warnings
{
    /// <summary>
    /// Manages the ordered set of active strike warnings and drives the Warning_Icon display.
    /// Implements TR-player-009: Strike Warning Integration (multi-warning priority selection).
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
        void OnStrikeWarningReceived(bool warningActive, PlayerInputMode mode, Float2 playerPosition);

        /// <summary>
        /// Reports the player's current position and bounds to the Map Director.
        /// Call once per frame during NightSurvival.
        /// </summary>
        void ReportPlayerPosition(Float2 position, Bounds2D bounds, PlayerInputMode mode);

        /// <summary>
        /// Clears all active warnings and hides the icon.
        /// Called on NightSurvival phase exit.
        /// </summary>
        void ClearAll();
    }
}
