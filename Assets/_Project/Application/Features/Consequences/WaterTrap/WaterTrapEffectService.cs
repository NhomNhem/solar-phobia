using System;
using System.Collections.Generic;
using R3;
using SolarPhobia.Application.Features.Consequences;
using SolarPhobia.Application.Features.Ward;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Features.Consequences.WaterTrap
{
    /// <summary>
    /// Water Trap curse effect (Drag - Linh abandoned).
    /// Applies continuous DoT when the player stands in water hazard zones.
    /// </summary>
    public class WaterTrapEffectService : IWaterTrapEffectService
    {
        public const float DefaultDamagePerSecond = 3.0f;

        private readonly ICurseEffectManager _curseEffectManager;
        private readonly IWardTimerPort _wardTimerService;
        private readonly IDisposable _hazardTriggeredSub;
        private readonly IDisposable _hazardClearedSub;
        private readonly HashSet<string> _activeWaterHazards = new();
        private bool _isActive;
        private float _totalDamageApplied;

        public bool IsActive => _isActive;
        public float TotalDamageApplied => _totalDamageApplied;

        public WaterTrapEffectService(
            ICurseEffectManager curseEffectManager,
            IWardTimerPort wardTimerService)
        {
            _curseEffectManager = curseEffectManager;
            _wardTimerService = wardTimerService;

            _hazardTriggeredSub = _curseEffectManager.OnHazardTriggered.Subscribe(OnHazardTriggered);
            _hazardClearedSub = _curseEffectManager.OnHazardCleared.Subscribe(OnHazardCleared);
        }

        public void Tick(float deltaTime)
        {
            if (!_isActive)
            {
                return;
            }

            var damage = DefaultDamagePerSecond * deltaTime;
            if (_wardTimerService.TryApplyCost(damage))
            {
                _totalDamageApplied += damage;
            }
        }

        public void Dispose()
        {
            _hazardTriggeredSub?.Dispose();
            _hazardClearedSub?.Dispose();
        }

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


