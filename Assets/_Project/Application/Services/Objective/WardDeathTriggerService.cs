using R3;
using SolarPhobia.Application.Services.Interfaces;
using SolarPhobia.Domain;
using SolarPhobia.Domain.ValueObjects;
using System;

namespace SolarPhobia.Application.Services.Objective
{
    public class WardDeathTriggerService : IWardDeathTriggerService
    {
        private readonly SolarPhobia.Domain.IWardTimerService _wardTimer;
        private readonly IPhaseStateMachine _phaseStateMachine;
        private readonly IDisposable _depletedSubscription;
        private bool _hasTriggeredDeath;

        public bool HasTriggeredDeath => _hasTriggeredDeath;

        public WardDeathTriggerService(SolarPhobia.Domain.IWardTimerService wardTimer, IPhaseStateMachine phaseStateMachine)
        {
            _wardTimer = wardTimer;
            _phaseStateMachine = phaseStateMachine;

            _depletedSubscription = _wardTimer.OnDepleted
            .Subscribe(_ => OnDepleted());
        }

        public void Dispose()
        {
            _depletedSubscription?.Dispose();
        }

        private void OnDepleted()
        {
            if (_hasTriggeredDeath)
            {
                return;
            }

            if (_phaseStateMachine.CurrentState == PhaseState.EndingEvaluation)
            {
                return;
            }

            if (_phaseStateMachine.TryTransition(PhaseState.EndingEvaluation))
            {
                _hasTriggeredDeath = true;
            }
        }
    }
}
