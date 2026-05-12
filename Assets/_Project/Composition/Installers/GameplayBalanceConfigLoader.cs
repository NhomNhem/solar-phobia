using NhemDangFugBixs.NhemLogging;
using SolarPhobia.Shared.Configuration;
using UnityEngine;

namespace SolarPhobia.Composition.Installers
{
    internal static class GameplayBalanceConfigLoader
    {
        private const string ResourcePath = "Configs/gameplay-balance-config";
        private static readonly INhemLogger Logger = new NhemUnityLogger();

        public static GameplayBalanceConfig Load()
        {
            TextAsset configAsset = Resources.Load<TextAsset>(ResourcePath);
            if (configAsset == null)
            {
                Logger.LogWarning($"[GameplayBalanceConfigLoader] Missing Resources/{ResourcePath}. Using default balance config.");
                return GameplayBalanceConfig.CreateDefault();
            }

            GameplayBalanceConfig config = JsonUtility.FromJson<GameplayBalanceConfig>(configAsset.text);
            return config ?? GameplayBalanceConfig.CreateDefault();
        }
    }
}
