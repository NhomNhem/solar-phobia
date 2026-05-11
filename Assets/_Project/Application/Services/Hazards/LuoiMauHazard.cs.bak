using System;
using UnityEngine;

namespace SolarPhobia.Application.Services.Hazards
{
    public class LuoiMauHazard : MonoBehaviour
    {
        private float _slowMultiplier;
        private SphereCollider _trigger;

        public LuoiMauHazard Initialize(float slowMultiplier)
        {
            _slowMultiplier = slowMultiplier;
            _trigger = gameObject.AddComponent<SphereCollider>();
            _trigger.isTrigger = true;
            _trigger.radius = 5f;
            return this;
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log($"Player in Luoi Mau zone - speed reduced to {_slowMultiplier}×");
            }
        }
    }
}
