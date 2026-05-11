// Assets/_Project/Domain/Services/INgocCotService.cs
namespace SolarPhobia.Domain.Services
{
    /// <summary>
    /// Interface for Ngọc Cốt relic pickup tracking.
    /// Tracks bone count from relic pickups and provides the ward drain multiplier.
    /// </summary>
    public interface INgocCotService
    {
        /// <summary>
        /// Gets the current bone count from Ngọc Cốt pickups.
        /// </summary>
        int BoneCount { get; }

        /// <summary>
        /// Gets the bone multiplier for ward drain calculation.
        /// Formula: 1 + (boneCount × 0.25)
        /// </summary>
        float BoneMultiplier { get; }

        /// <summary>
        /// Attempts to collect a Ngọc Cốt relic.
        /// </summary>
        /// <returns>True if pickup was successful, false if max (3) already reached.</returns>
        bool TryCollectRelic();

        /// <summary>
        /// Resets bone count at the start of each night phase.
        /// </summary>
        void ResetForNight();
    }
}
