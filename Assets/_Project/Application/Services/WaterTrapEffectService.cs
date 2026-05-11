using System;
using System.Collections.Generic;
using R3;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Services
{
    /// <summary>
    /// Water Trap curse effect (Drag - Linh abandoned).
    /// Applies continuous DoT (-3.0/s) when player stands in water hazard zones.
    /// </summary>
    public class WaterTrapEffectService : IWaterTrapEffectService
    {
        // ── Constants ──────────────────────────────────────────────
        public const float DefaultDamagePerSecond = 3.0f;

        // ── Dependencies ────────────────────────────────────────────
        private readonly ICurseEffectManager _curseEffectManager;
        private readonly IWardTimerService _wardTimerService;
        private readonly IDisposable _hazardTriggeredSub;
        private readonly IDisposable _hazardClearedSub;

        // ── State ───────────────────────────────────────────────────
        private bool _isActive;
        private float _totalDamageApplied;
        private readonly HashSet<string> _activeWaterHazards = new();

        /// <inheritdoc/>
        public bool IsActive => _isActive;

        /// <inheritdoc/>
        public float TotalDamageApplied => _totalDamageApplied;

        /// <summary>
        /// Initializes a new instance of the WaterTrapEffectService class.
        /// </summary>
        public WaterTrapEffectService(
            ICurseEffectManager curseEffectManager,
            IWardTimerService wardTimerService)
        {
            _curseEffectManager = curseEffectManager;
            _wardTimerService = wardTimerService;

            _hazardTriggeredSub = _curseEffectManager.OnHazardTriggered
                .Subscribe(OnHazardTriggered);

            _hazardClearedSub = _curseEffectManager.OnHazardCleared
                .Subscribe(OnHazardCleared);
        }

        /// <inheritdoc/>
        public void Tick(float deltaTime)
        {
            if (!_isActive)
            {
                return;
            }

            float damage = DefaultDamagePerSecond * deltaTime;
            if (_wardTimerService.TryApplyCost(damage))
            {
                _totalDamageApplied += damage;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _hazardTriggeredSub?.Dispose();
            _hazardClearedSub?.Dispose();
        }

        // ── Private Methods ─────────────────────────────────────────

        private void OnHazardTriggered(HazardEvent evt)
        {
            if (evt.CurseType != NightOutcomeState.Drag)
            {
                return;
            }

            _activeWaterHazards.Add(evt.HazardId);
            _isActive = true;
        }

        private void OnHazardCleared(HazardEvent evt)
        {
            if (evt.CurseType != NightOutcomeState.Drag)
            {
                return;
            }

            _activeWaterHazards.Remove(evt.HazardId);

            if (_activeWaterHazards.Count == 0)
            {
                _isActive = false;
                _totalDamageApplied = 0f;
            }
        }
    }
}
