using System;
using UnityEngine;

namespace SolarPhobia.Application.Services
{
    /// <summary>
    /// Vũng Nước hazard - applies damage-over-time while player is standing in it.
    /// </summary>
    public class VungNuocHazard : MonoBehaviour
    {
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
                Debug.Log($"Player in Vung Nuoc zone - taking {_damagePerSecond} HP/s");
            }
        }
    }
}
