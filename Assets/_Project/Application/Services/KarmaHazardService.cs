using System;
using R3;
using SolarPhobia.Application.Phase.Flow;
using SolarPhobia.Domain.ValueObjects;
using SolarPhobia.Shared.Configuration;
using UnityEngine;
using VContainer;

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
        private readonly GameplayBalanceConfig _balanceConfig;
        private readonly IKarmaHazardRuntime _hazardRuntime;
        private PhaseState _currentPhaseValue;
        private IDisposable _phaseSubscription;

        // ── State ──────────────────────────────
        private readonly ReactiveProperty<KarmaHazardData> _hazardSpawned = new();

        /// <summary>
        /// Observable that triggers when a hazard is spawned.
        /// </summary>
        public Observable<KarmaHazardData> OnHazardSpawned => _hazardSpawned;

        /// <summary>
        /// Initializes a new instance of the KarmaHazardService class.
        /// </summary>
        public KarmaHazardService(IPhaseStateMachine phaseStateMachine)
            : this(phaseStateMachine, GameplayBalanceConfig.CreateDefault(), new NullKarmaHazardRuntime())
        {
        }

        [Inject]
        public KarmaHazardService(
            IPhaseStateMachine phaseStateMachine,
            GameplayBalanceConfig balanceConfig,
            IKarmaHazardRuntime hazardRuntime)
        {
            _balanceConfig = balanceConfig ?? GameplayBalanceConfig.CreateDefault();
            _hazardRuntime = hazardRuntime ?? new NullKarmaHazardRuntime();
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
            float effectValue = GetConfiguredEffectValue(hazardType);

            var hazardData = new KarmaHazardData
            {
                GhostType = ghostType,
                HazardType = hazardType,
                Position = position,
                EffectValue = effectValue
            };

            if (_hazardRuntime.TrySpawn(hazardType, position, effectValue))
            {
                _hazardSpawned.Value = hazardData;
            }
        }

        /// <inheritdoc/>
        public void ClearHazards()
        {
            _hazardRuntime.ClearAll();
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
        private bool IsNightSurvivalPhase()
        {
            return _currentPhaseValue == PhaseState.NightSurvival;
        }

        private float GetConfiguredEffectValue(string hazardType)
        {
            return hazardType switch
            {
                "LuoiMau" => _balanceConfig.KarmaHazard.LuoiMauEffectValue,
                "VungNuoc" => _balanceConfig.KarmaHazard.VungNuocEffectValue,
                "BeDaDaoAnh" => _balanceConfig.KarmaHazard.BeDaDaoAnhEffectValue,
                _ => 0f
            };
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

        private sealed class NullKarmaHazardRuntime : IKarmaHazardRuntime
        {
            public bool TrySpawn(string hazardType, Vector3 position, float effectValue)
            {
                return true;
            }

            public void ClearAll()
            {
            }
        }
    }
}
