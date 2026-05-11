using System;
using System.Collections;
using UnityEngine;

namespace SolarPhobia.Application.Services
{
    /// <summary>
    /// Bệ Đá Ảo Ảnh hazard - collapses for 0.2s when player enters trigger.
    /// </summary>
    public class BeDaDaoAnhHazard : MonoBehaviour
    {
        private float _collapseDuration;
        private BoxCollider _trigger;
        private bool _isCollapsed;

        public BeDaDaoAnhHazard Initialize(float collapseDuration)
        {
            _collapseDuration = collapseDuration;
            _trigger = gameObject.AddComponent<BoxCollider>();
            _trigger.isTrigger = true;
            _trigger.size = new Vector3(2f, 2f, 2f);
            return this;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && !_isCollapsed)
            {
                StartCoroutine(CollapseRoutine());
            }
        }

        private IEnumerator CollapseRoutine()
        {
            _isCollapsed = true;
            Debug.Log($"Be Da Dao Anh collapsed for {_collapseDuration}s");
            yield return new WaitForSeconds(_collapseDuration);
            _isCollapsed = false;
        }
    }
}
