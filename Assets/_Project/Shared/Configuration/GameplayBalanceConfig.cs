using System;

namespace SolarPhobia.Shared.Configuration
{
    /// <summary>
    /// Data-driven gameplay balance configuration loaded from project assets.
    /// </summary>
    [Serializable]
    public sealed class GameplayBalanceConfig
    {
        public WardTimerBalanceConfig WardTimer = new();
        public MapSpawnBalanceConfig MapSpawn = new();
        public StrikeBalanceConfig Strike = new();
        public KarmaHazardBalanceConfig KarmaHazard = new();
        public ResourceEffectsBalanceConfig ResourceEffects = new();

        public static GameplayBalanceConfig CreateDefault()
        {
            return new GameplayBalanceConfig();
        }
    }

    [Serializable]
    public sealed class WardTimerBalanceConfig
    {
        public float BaseWardSec = 10.0f;
        public float WardPerGhostSec = 30.0f;
        public float FailedLightInterruptPenalty = 10.0f;
        public float SoulPanicPenalty = 5.0f;
        public float MaxDayPenalties = 30.0f;
        public float DefaultBaseDrain = 1.0f;
    }

    [Serializable]
    public sealed class MapSpawnBalanceConfig
    {
        public int DefaultSafeMoundsPerChunk = 2;
        public float CursedMoundProbability = 0.4f;
        public float FalseSafeMoundProbability = 0.15f;
    }

    [Serializable]
    public sealed class StrikeBalanceConfig
    {
        public float DefaultStrikeTimePenaltySec = 30.0f;
        public float DefaultStrikeTelegraphSec = 1.5f;
        public float MinTelegraphSec = 0.8f;
        public float MaxTelegraphSec = 2.5f;
    }

    [Serializable]
    public sealed class KarmaHazardBalanceConfig
    {
        public float LuoiMauEffectValue = 0.5f;
        public float VungNuocEffectValue = 5.0f;
        public float BeDaDaoAnhEffectValue = 0.2f;
    }

    [Serializable]
    public sealed class ResourceEffectsBalanceConfig
    {
        public float DefaultNgocCotDrainMultiplier = 1.0f;
    }
}
