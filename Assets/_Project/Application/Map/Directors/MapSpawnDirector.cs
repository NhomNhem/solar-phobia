using System;
using R3;
using SolarPhobia.Application.Map.Generation;
using SolarPhobia.Domain.ValueObjects;
using SolarPhobia.Shared.Configuration;
using VContainer;

namespace SolarPhobia.Application.Map.Directors
{
    /// <summary>
    /// Deterministic seed-based map chunk generator.
    /// Implements TR-map-001 and TR-map-007.
    /// </summary>
    public class MapSpawnDirector : IMapSpawnDirector
    {
        public static int DefaultSafeMoundsPerChunk => GameplayBalanceConfig.CreateDefault().MapSpawn.DefaultSafeMoundsPerChunk;

        private readonly Subject<bool> _onStrikeWarning = new();
        private readonly Subject<string> _onEnterCover = new();
        private readonly Subject<string> _onExitCover = new();

        private int _seed;
        private bool _initialized;
        private Float2 _playerPosition;
        private Bounds2D _playerBounds;
        private readonly GameplayBalanceConfig _balanceConfig;

        public int Seed => _seed;
        public Observable<bool> OnStrikeWarning => _onStrikeWarning;
        public Observable<string> OnEnterCover => _onEnterCover;
        public Observable<string> OnExitCover => _onExitCover;

        public MapSpawnDirector()
            : this(GameplayBalanceConfig.CreateDefault())
        {
        }

        [Inject]
        public MapSpawnDirector(GameplayBalanceConfig balanceConfig)
        {
            _balanceConfig = balanceConfig ?? GameplayBalanceConfig.CreateDefault();
        }

        public void Initialize(int seed)
        {
            _seed = seed;
            _initialized = true;
        }

        public ChunkData GenerateChunk(int index)
        {
            if (!_initialized)
            {
                throw new InvalidOperationException(
                    "MapSpawnDirector.Initialize(seed) must be called before GenerateChunk().");
            }

            int chunkSeed = _seed + index;
            var rng = new System.Random(chunkSeed);
            MapSpawnBalanceConfig config = _balanceConfig.MapSpawn;

            int safeMoundCount = config.DefaultSafeMoundsPerChunk;
            var safeMoundPositions = new float[safeMoundCount];
            for (int i = 0; i < safeMoundCount; i++)
            {
                safeMoundPositions[i] = (float)rng.NextDouble();
            }

            bool hasCursedMound = rng.NextDouble() < config.CursedMoundProbability;
            int cursedCount = hasCursedMound ? 1 : 0;
            var cursedPositions = new float[cursedCount];
            if (hasCursedMound)
            {
                cursedPositions[0] = (float)rng.NextDouble();
            }

            bool hasFalseSafeMound = rng.NextDouble() < config.FalseSafeMoundProbability;

            return new ChunkData(index, chunkSeed, safeMoundCount, cursedCount,
                hasFalseSafeMound, safeMoundPositions, cursedPositions);
        }

        public void UpdatePlayerPosition(Float2 position, Bounds2D bounds)
        {
            _playerPosition = position;
            _playerBounds = bounds;
        }

        public void NotifyCoverEnter(string coverTag) => _onEnterCover.OnNext(coverTag);
        public void NotifyCoverExit(string coverTag) => _onExitCover.OnNext(coverTag);
        public void NotifyStrikeWarning(bool active) => _onStrikeWarning.OnNext(active);
    }
}
