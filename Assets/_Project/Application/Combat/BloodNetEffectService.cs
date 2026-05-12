using System;
using R3;
using SolarPhobia.Application.Consequences;
using SolarPhobia.Application.Ward;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Combat
{
    public class BloodNetEffectService : IBloodNetEffectService
    {
        public const float DefaultPenalty = 5.0f;
        public const float DefaultSlowDuration = 3.0f;
        public const float DefaultSlowMultiplier = 0.5f;

        private readonly ICurseEffectManager _curseEffectManager;
        private readonly IWardTimerPort _wardTimerService;
        private readonly IDisposable _hazardTriggeredSub;
        private float _slowTimeRemaining;
        private float _totalPenaltyApplied;

        public bool IsSlowed => _slowTimeRemaining > 0f;
        public float SlowTimeRemaining => _slowTimeRemaining;
        public float TotalPenaltyApplied => _totalPenaltyApplied;

        public BloodNetEffectService(ICurseEffectManager curseEffectManager, IWardTimerPort wardTimerService)
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
