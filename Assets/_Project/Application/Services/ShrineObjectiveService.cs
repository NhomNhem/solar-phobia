using System;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Services
{
    public class ShrineObjectiveService : IShrineObjectiveService
    {
        public const float DefaultTriggerRadius = 3.0f;
        public const float DefaultDebounceWindowSec = 0.5f;

        private readonly IPhaseStateMachine _phaseStateMachine;
        private readonly float _triggerRadius;
        private readonly float _debounceWindowSec;
        private DateTime _lastTriggerTime;

        public ShrineObjectiveService(
            IPhaseStateMachine phaseStateMachine,
            float triggerRadius = DefaultTriggerRadius,
            float debounceWindowSec = DefaultDebounceWindowSec)
        {
            _phaseStateMachine = phaseStateMachine;
            _triggerRadius = triggerRadius;
            _debounceWindowSec = debounceWindowSec;
        }

        public bool TryTriggerShrineArrival(float distanceToShrine)
        {
            if (_phaseStateMachine.CurrentState != PhaseState.NightSurvival)
            {
                return false;
            }

            if (distanceToShrine > _triggerRadius)
            {
                return false;
            }

            DateTime now = DateTime.UtcNow;
            if ((now - _lastTriggerTime).TotalSeconds < _debounceWindowSec)
            {
                return false;
            }

            _lastTriggerTime = now;

            return _phaseStateMachine.TryTransition(PhaseState.EndingEvaluation);
        }
    }
}
