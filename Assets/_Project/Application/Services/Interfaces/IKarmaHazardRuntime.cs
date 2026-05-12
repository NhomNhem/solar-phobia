using UnityEngine;

namespace SolarPhobia.Application.Services
{
    public interface IKarmaHazardRuntime
    {
        bool TrySpawn(string hazardType, Vector3 position, float effectValue);
        void ClearAll();
    }
}
