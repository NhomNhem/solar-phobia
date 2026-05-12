using R3;
using SolarPhobia.Application.Services.Interfaces;
using SolarPhobia.Domain;
using SolarPhobia.Domain.ValueObjects;
using System;

namespace SolarPhobia.Application.Services.Combat
{
    public class BloodNetEffectService : IBloodNetEffectService
    {
        public const float DefaultPenalty = 5.0f;
        public const float DefaultSlowDuration = 3.0f;
        public const float DefaultSlowMultiplier = 0.5f;

        private readonly ICurseEffectManager _curseEffectManager;
        private readonly IWardTimerService _wardTimerService;
        private readonly IDisposable _hazardTriggeredSub;
        private float _slowTimeRemaining;
        private float _totalPenaltyApplied;

        public bool IsSlowed => _slowTimeRemaining > 0f;
        public float SlowTimeRemaining => _slowTimeRemaining;
        public float TotalPenaltyApplied => _totalPenaltyApplied;

        public BloodNetEffectService(ICurseEffectManager curseEffectManager, IWardTimerService wardTimerService)
        {
            _curseEffectManager = curseEffectManager;
            _wardTimerService = wardTimerService;

            _hazardTriggeredSub = _curseEffectManager.OnHazardTriggered
            .Subscribe(OnHazardTriggered);
        }

        public void Tick(float deltaTime)
        {
            if (_slowTimeRemaining > 0f)
            {
                _slowTimeRemaining = Math.Max(0f, _slowTimeRemaining - deltaTime);
            }
        }

        public void Dispose()
        {
            _hazardTriggeredSub?.Dispose();
        }

        private void OnHazardTriggered(HazardEvent evt)
        {
            if (evt.CurseType != NightOutcomeState.Block)
            {
                return;
            }

            if (_wardTimerService.TryApplyCost(DefaultPenalty))
            {
                _totalPenaltyApplied += DefaultPenalty;
            }

            _slowTimeRemaining = DefaultSlowDuration;
        }
    }
}
