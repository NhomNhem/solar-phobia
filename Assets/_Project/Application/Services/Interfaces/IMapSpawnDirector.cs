// Assets/_Project/Application/Services/Interfaces/IMapSpawnDirector.cs
using R3;
using SolarPhobia.Application.Services.Map;
using UnityEngine;

namespace SolarPhobia.Application.Services
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

        // ── TR-map-001: Chunk Generation ──────────────────────────

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

        // ── TR-map-007: Player Controller Signals ─────────────────

        /// <summary>
        /// Fires when a strike telegraph begins (true) or is cancelled/resolved (false).
        /// Player Controller subscribes to show/hide the warning icon near reticle.
        /// </summary>
        Observable<bool> OnStrikeWarning { get; }

        /// <summary>
        /// Fires when the player enters a cover trigger zone.
        /// Payload: cover tag (e.g. "MoThuong", "FalseSafeMound").
        /// </summary>
        Observable<string> OnEnterCover { get; }

        /// <summary>
        /// Fires when the player exits a cover trigger zone.
        /// Payload: cover tag.
        /// </summary>
        Observable<string> OnExitCover { get; }

        /// <summary>
        /// Called by Player Controller each frame during NightSurvival.
        /// Map Director uses position + bounds for cover validation and sweep targeting.
        /// </summary>
        void UpdatePlayerPosition(Vector2 position, Bounds bounds);
    }
}
