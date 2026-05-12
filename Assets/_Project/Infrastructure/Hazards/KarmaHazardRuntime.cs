using SolarPhobia.Application.Services;
using UnityEngine;

namespace SolarPhobia.Infrastructure.Hazards
{
    public sealed class KarmaHazardRuntime : IKarmaHazardRuntime
    {
        private readonly System.Collections.Generic.List<GameObject> _activeHazards = new();

        public bool TrySpawn(string hazardType, Vector3 position, float effectValue)
        {
            var hazard = new GameObject($"KarmaHazard_{hazardType}");
            hazard.transform.position = position;

            switch (hazardType)
            {
                case "LuoiMau":
                    hazard.AddComponent<LuoiMauHazard>().Initialize(effectValue);
                    break;
                case "VungNuoc":
                    hazard.AddComponent<VungNuocHazard>().Initialize(effectValue);
                    break;
                case "BeDaDaoAnh":
                    hazard.AddComponent<BeDaDaoAnhHazard>().Initialize(effectValue);
                    break;
                default:
                    Object.Destroy(hazard);
                    return false;
            }

            _activeHazards.Add(hazard);
            return true;
        }

        public void ClearAll()
        {
            foreach (var hazard in _activeHazards)
            {
                if (hazard == null)
                {
                    continue;
                }

#if UNITY_EDITOR
                if (!UnityEngine.Application.isPlaying)
                {
                    Object.DestroyImmediate(hazard);
                    continue;
                }
#endif
                Object.Destroy(hazard);
            }

            _activeHazards.Clear();
        }
    }
}
