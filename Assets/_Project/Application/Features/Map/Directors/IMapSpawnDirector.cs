using R3;
using SolarPhobia.Application.Features.Map.Generation;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Features.Map.Directors
{
    /// <summary>
    /// Core interface for the Map &amp; Spawn Director.
    /// Implements TR-map-001: Deterministic seed-based chunk generation.
    /// Implements TR-map-007: Player Controller signals (cover/strike overlap events).
    /// </summary>
    public interface IMapSpawnDirector
    {
        /// <summary>Current run seed. Set by Initialize().</summary>
        int Seed { get; }

        /// <summary>
        /// Initializes the director with a deterministic seed for this run.
        /// Must be called before GenerateChunk().
        /// </summary>
        void Initialize(int seed);

        /// <summary>
        /// Generates chunk data for the given chunk index.
        /// Deterministic: same seed + index always produces identical output.
        /// </summary>
        ChunkData GenerateChunk(int index);

        /// <summary>
        /// Fires when a strike telegraph begins (true) or is cancelled/resolved (false).
        /// </summary>
        Observable<bool> OnStrikeWarning { get; }

        /// <summary>
        /// Fires when the player enters a cover trigger zone.
        /// </summary>
        Observable<string> OnEnterCover { get; }

        /// <summary>
        /// Fires when the player exits a cover trigger zone.
        /// </summary>
        Observable<string> OnExitCover { get; }

        /// <summary>
        /// Called by Player Controller each frame during NightSurvival.
        /// Map Director uses position + bounds for cover validation and sweep targeting.
        /// </summary>
        void UpdatePlayerPosition(Float2 position, Bounds2D bounds);
    }
}

