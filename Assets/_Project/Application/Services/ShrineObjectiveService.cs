using R3;
using SolarPhobia.Domain.ValueObjects;
using System;

namespace SolarPhobia.Application.Services
{
    public class ShrineObjectiveService : IShrineObjectiveService, IDisposable
    {
        public const float DefaultTriggerRadius = 3.0f;
        public const float DefaultDebounceWindowSec = 0.5f;

        private readonly IPhaseStateMachine _phaseStateMachine;
        private readonly float _triggerRadius;
        private readonly float _debounceWindowSec;
        private readonly Subject<Unit> _onShrineReachedSubject = new();
        private DateTime _lastTriggerTime;
        private bool _hasTriggered;
        private IDisposable _phaseSubscription;
        private bool _disposed;

        public Observable<Unit> OnShrineReached => _onShrineReachedSubject;

        public ShrineObjectiveService(
            IPhaseStateMachine phaseStateMachine,
            float triggerRadius = DefaultTriggerRadius,
            float debounceWindowSec = DefaultDebounceWindowSec)
        {
            _phaseStateMachine = phaseStateMachine;
            _triggerRadius = triggerRadius;
            _debounceWindowSec = debounceWindowSec;

            _phaseSubscription = _phaseStateMachine.OnPhaseChanged
                .Subscribe(evt =>
                {
                    if (_hasTriggered && evt.NewPhase != PhaseState.ShrineArrival)
                    {
                        _hasTriggered = false;
                    }
                });
        }

        public bool TryTriggerShrineArrival(float distanceToShrine)
        {
            if (_hasTriggered)
            {
                return false;
            }

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
            _hasTriggered = true;

            _onShrineReachedSubject.OnNext(Unit.Default);

            return _phaseStateMachine.TryTransition(PhaseState.ShrineArrival);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _phaseSubscription?.Dispose();
                _onShrineReachedSubject?.Dispose();
                _disposed = true;
            }
        }
    }
}
