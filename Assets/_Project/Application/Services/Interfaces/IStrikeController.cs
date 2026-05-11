// Assets/_Project/Application/Services/Interfaces/IStrikeController.cs
using R3;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Services
{
    /// <summary>
    /// Manages the boss searchlight strike telegraph and penalty.
    /// Implements TR-map-004: Strike Telegraph + Penalty.
    ///
    /// Flow:
    ///   1. Player becomes exposed (in sweep cone, not in cover)
    ///   2. Telegraph timer starts → OnStrikeWarning fires (Player Controller shows icon)
    ///   3. If still exposed when telegraph expires → OnWardCostIncurred fires (-30s)
    ///   4. If player takes cover before telegraph expires → strike cancelled
    ///   5. Strike never fires inside shrine safe zone
    /// </summary>
    public interface IStrikeController
    {
        /// <summary>Whether a strike telegraph is currently active.</summary>
        bool IsTelegraphActive { get; }

        /// <summary>Remaining telegraph time in seconds (0 when not active).</summary>
        float TelegraphRemaining { get; }

        /// <summary>
        /// Fires when a strike telegraph begins.
        /// Player Controller subscribes to show the warning icon near reticle.
        /// </summary>
        Observable<bool> OnStrikeWarning { get; }

        /// <summary>
        /// Fires when a strike resolves (player was still exposed at telegraph end).
        /// Payload: Ward cost in seconds (StrikeTimePenaltySec).
        /// Ward Timer subscribes and deducts the cost.
        /// </summary>
        Observable<float> OnWardCostIncurred { get; }

        /// <summary>Strike Ward penalty in seconds. Default: 30s, range: 5-60s.</summary>
        float StrikeTimePenaltySec { get; set; }

        /// <summary>Telegraph duration in seconds. Default: 1.5s, range: 0.8-2.5s.</summary>
        float StrikeTelegraphSec { get; set; }

        /// <summary>
        /// Updates strike state each frame.
        /// </summary>
        /// <param name="isExposed">Whether the player is currently exposed (from SweepExposureCalculator).</param>
        /// <param name="inShrineZone">Whether the player is in the shrine safe zone.</param>
        /// <param name="mode">Current player input mode — strike only active in NightMovement.</param>
        /// <param name="deltaTime">Frame delta time in seconds.</param>
        void Tick(bool isExposed, bool inShrineZone, PlayerInputMode mode, float deltaTime);
    }
}
