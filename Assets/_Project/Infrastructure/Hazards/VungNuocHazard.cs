using NhemDangFugBixs.NhemLogging;
using UnityEngine;

namespace SolarPhobia.Infrastructure.Hazards
{
    /// <summary>
    /// Vũng Nước hazard - applies damage-over-time while player is standing in it.
    /// </summary>
    public class VungNuocHazard : MonoBehaviour
    {
        private static readonly INhemLogger Logger = new NhemUnityLogger();
        private float _damagePerSecond;
        private SphereCollider _trigger;

        public VungNuocHazard Initialize(float damagePerSecond)
        {
            _damagePerSecond = damagePerSecond;
            _trigger = gameObject.AddComponent<SphereCollider>();
            _trigger.isTrigger = true;
            _trigger.radius = 3f;
            return this;
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Logger.Log($"Player in Vung Nuoc zone - taking {_damagePerSecond} HP/s", this);
            }
        }
    }
}
