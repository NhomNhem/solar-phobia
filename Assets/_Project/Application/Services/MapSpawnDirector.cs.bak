// Assets/_Project/Application/Services/MapSpawnDirector.cs
using System;
using R3;
using SolarPhobia.Application.Services.Map;
using UnityEngine;
using VContainer;

namespace SolarPhobia.Application.Services
{
    /// <summary>
    /// Deterministic seed-based map chunk generator.
    /// Implements TR-map-001 and TR-map-007.
    /// </summary>
    public class MapSpawnDirector : IMapSpawnDirector
    {
        // ── Constants ─────────────────────────────────────────────
        public const int    DefaultSafeMoundsPerChunk  = 2;
        public const double CursedMoundProbability     = 0.4;
        public const double FalseSafeMoundProbability  = 0.15;

        // ── R3 Reactive State ──────────────────────────────────────
        private readonly Subject<bool>   _onStrikeWarning = new();
        private readonly Subject<string> _onEnterCover    = new();
        private readonly Subject<string> _onExitCover     = new();

        // ── State ─────────────────────────────────────────────────
        private int  _seed;
        private bool _initialized;
        private Vector2 _playerPosition;
        private Bounds  _playerBounds;

        // ── Public Interface ───────────────────────────────────────
        public int Seed => _seed;

        // TR-map-007 signals
        public Observable<bool>   OnStrikeWarning => _onStrikeWarning;
        public Observable<string> OnEnterCover    => _onEnterCover;
        public Observable<string> OnExitCover     => _onExitCover;

        // ── Constructor ────────────────────────────────────────────
        [Inject]
        public MapSpawnDirector() { }

        // ── IMapSpawnDirector ──────────────────────────────────────
        public void Initialize(int seed)
        {
            _seed        = seed;
            _initialized = true;
        }

        public ChunkData GenerateChunk(int index)
        {
            if (!_initialized)
                throw new InvalidOperationException(
                    "MapSpawnDirector.Initialize(seed) must be called before GenerateChunk().");

            int chunkSeed = _seed + index;
            var rng = new System.Random(chunkSeed);

            int safeMoundCount = DefaultSafeMoundsPerChunk;
            var safeMoundPositions = new float[safeMoundCount];
            for (int i = 0; i < safeMoundCount; i++)
                safeMoundPositions[i] = (float)rng.NextDouble();

            bool hasCursedMound = rng.NextDouble() < CursedMoundProbability;
            int  cursedCount    = hasCursedMound ? 1 : 0;
            var  cursedPositions = new float[cursedCount];
            if (hasCursedMound)
                cursedPositions[0] = (float)rng.NextDouble();

            bool hasFalseSafeMound = rng.NextDouble() < FalseSafeMoundProbability;

            return new ChunkData(index, chunkSeed, safeMoundCount, cursedCount,
                                 hasFalseSafeMound, safeMoundPositions, cursedPositions);
        }

        public void UpdatePlayerPosition(Vector2 position, Bounds bounds)
        {
            _playerPosition = position;
            _playerBounds   = bounds;
        }

        // ── Internal helpers (called by MonoBehaviour layer) ───────

        /// <summary>Called by MonoBehaviour from OnTriggerEnter2D on cover volumes.</summary>
        public void NotifyCoverEnter(string coverTag) => _onEnterCover.OnNext(coverTag);

        /// <summary>Called by MonoBehaviour from OnTriggerExit2D on cover volumes.</summary>
        public void NotifyCoverExit(string coverTag) => _onExitCover.OnNext(coverTag);

        /// <summary>Called by StrikeController when telegraph state changes.</summary>
        public void NotifyStrikeWarning(bool active) => _onStrikeWarning.OnNext(active);
    }
}
