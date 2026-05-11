using System;
using System.Collections.Generic;
using R3;
using UnityEngine;
using VContainer;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Services
{
    /// <summary>
    /// Implementation of karma hazard spawning service for Night phase.
    /// Maps sacrificed ghost types to specific hazards:
    /// - Van → Lưới Máu (slow penalty, 0.5× movement)
    /// - Linh → Vũng Nước (DoT, 5 HP/s)
    /// - Minh → Bệ Đá Ảo Ảnh (0.2s collapse on trigger)
    /// </summary>
    public class KarmaHazardService : IKarmaHazardService, IDisposable
    {
        // ── Dependencies ──────────────────────────────
        private PhaseState _currentPhaseValue;
        private IDisposable _phaseSubscription;

        // ── State ──────────────────────────────
        private readonly ReactiveProperty<KarmaHazardData> _hazardSpawned = new();
        private readonly List<GameObject> _activeHazards = new();

        /// <summary>
        /// Observable that triggers when a hazard is spawned.
        /// </summary>
        public Observable<KarmaHazardData> OnHazardSpawned => _hazardSpawned;

        /// <summary>
        /// Initializes a new instance of the KarmaHazardService class.
        /// </summary>
        [Inject]
        public KarmaHazardService(IPhaseStateMachine phaseStateMachine)
        {
            _phaseSubscription = phaseStateMachine.CurrentPhase
                .Subscribe(newPhase => _currentPhaseValue = newPhase);
        }

        /// <inheritdoc/>
        public void SpawnHazardForGhost(string ghostType, Vector3 position)
        {
            if (!IsNightSurvivalPhase())
                return;

            if (string.IsNullOrEmpty(ghostType))
                return;

            string hazardType = MapGhostToHazard(ghostType);
            float effectValue = GetEffectValue(hazardType);

            var hazardData = new KarmaHazardData
            {
                GhostType = ghostType,
                HazardType = hazardType,
                Position = position,
                EffectValue = effectValue
            };

            GameObject hazard = SpawnHazardPrefab(hazardType, position, effectValue);
            if (hazard != null)
            {
                _activeHazards.Add(hazard);
                _hazardSpawned.Value = hazardData;
            }
        }

        /// <inheritdoc/>
        public void ClearHazards()
        {
            foreach (var hazard in _activeHazards)
            {
                if (hazard != null)
                {
#if UNITY_EDITOR
                    if (!UnityEngine.Application.isPlaying)
                    {
                        GameObject.DestroyImmediate(hazard);
                        continue;
                    }
#endif
                    GameObject.Destroy(hazard);
                }
            }
            _activeHazards.Clear();
        }

        /// <summary>
        /// Map ghost type to hazard type.
        /// </summary>
        public static string MapGhostToHazard(string ghostType)
        {
            return ghostType switch
            {
                "Van" => "LuoiMau",
                "Linh" => "VungNuoc",
                "Minh" => "BeDaDaoAnh",
                _ => "Unknown"
            };
        }

        /// <summary>
        /// Get effect value for hazard type.
        /// </summary>
        public static float GetEffectValue(string hazardType)
        {
            return hazardType switch
            {
                "LuoiMau" => 0.5f,
                "VungNuoc" => 5f,
                "BeDaDaoAnh" => 0.2f,
                _ => 0f
            };
        }

        /// <summary>
        /// Check if ghost type has a valid hazard mapping.
        /// </summary>
        public static bool HasHazardMapping(string ghostType)
        {
            return ghostType switch
            {
                "Van" or "Linh" or "Minh" => true,
                _ => false
            };
        }

        // ── Private Methods ──────────────────────────────
        private GameObject SpawnHazardPrefab(string hazardType, Vector3 position, float effectValue)
        {
            GameObject hazard = new GameObject($"KarmaHazard_{hazardType}");
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
            }

            return hazard;
        }

        private bool IsNightSurvivalPhase()
        {
            return _currentPhaseValue == PhaseState.NightSurvival;
        }

        /// <summary>
        /// Dispose all subscriptions and clear hazards.
        /// </summary>
        public void Dispose()
        {
            _phaseSubscription?.Dispose();
            _hazardSpawned?.Dispose();
            ClearHazards();
        }
    }
}
