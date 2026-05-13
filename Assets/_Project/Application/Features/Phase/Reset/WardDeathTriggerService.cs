using System;
using R3;
using SolarPhobia.Application.Features.Phase.Flow;
using SolarPhobia.Application.Features.Phase.Reset;
using SolarPhobia.Application.Features.Ward;
using SolarPhobia.Domain.ValueObjects;
using VContainer;
using VContainer.Unity;

namespace SolarPhobia.Application.Features.Phase.Reset
{
    /// <summary>
    /// Transitions the run to ending evaluation when Ward depletes during NightSurvival.
    /// Implements Ward Timer story 001: death trigger.
    /// </summary>
    public class WardDeathTriggerService : IWardDeathTriggerService, IInitializable
    {
        private readonly IWardTimerPort _wardTimer;
        private readonly IPhaseStateMachine _phaseStateMachine;
        private IDisposable _depletedSubscription;
        private bool _hasTriggeredDeath;

        public bool HasTriggeredDeath => _hasTriggeredDeath;

        public WardDeathTriggerService(
            IWardTimerPort wardTimer,
            IPhaseStateMachine phaseStateMachine)
        {
            _wardTimer = wardTimer;
            _phaseStateMachine = phaseStateMachine;
        }

        public void Initialize()
        {
            _depletedSubscription = _wardTimer.OnDepleted
                .Subscribe(_ => OnDepleted());
        }

        public void Dispose()
        {
            _depletedSubscription?.Dispose();
        }

        public void Reset()
        {
            _hasTriggeredDeath = false;
        }

        private void OnDepleted()
        {
            if (_hasTriggeredDeath)
            {
                return;
            }

            if (_phaseStateMachine.CurrentState != PhaseState.NightSurvival)
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


