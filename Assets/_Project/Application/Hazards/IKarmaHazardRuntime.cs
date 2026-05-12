using UnityEngine;

namespace SolarPhobia.Application.Hazards
{
    public interface IKarmaHazardRuntime
    {
        bool TrySpawn(string hazardType, Vector3 position, float effectValue);
        void ClearAll();
    }
}
