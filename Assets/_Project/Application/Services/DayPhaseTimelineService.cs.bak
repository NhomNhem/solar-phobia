using System;
using R3;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Services
{
    /// <summary>
    /// Implementation of Day phase timeline with 4 pressure phases.
    /// Timeline: Stability (0-90s) → Tension (90-180s) → Crisis (180-270s) → Collapse (270-300s)
    /// </summary>
    public class DayPhaseTimelineService : IDayPhaseTimelineService
    {
        private const float StabilityEnd = 90f;
        private const float TensionEnd = 180f;
        private const float CrisisEnd = 270f;
        private const float CollapseEnd = 300f;

        private const int StabilityCapacity = 4;
        private const int TensionCapacity = 3;
        private const int CrisisCapacity = 3;
        private const int CollapseCapacity = 1;

        private readonly ReactiveProperty<TimelinePhase> _currentPhase = new(TimelinePhase.Stability);
        private readonly Subject<TimelinePhaseChangedEvent> _phaseChangedSubject = new();

        private float _elapsedTime;
        private bool _isRunning;

        public TimelinePhase CurrentPhase => _currentPhase.Value;
        public ReadOnlyReactiveProperty<TimelinePhase> CurrentTimelinePhase => _currentPhase;
        public float ElapsedTime => _elapsedTime;
        public float PhaseDuration => CollapseEnd;
        public Observable<TimelinePhaseChangedEvent> OnTimelinePhaseChanged => _phaseChangedSubject;

        public void StartTimeline()
        {
            _elapsedTime = 0f;
            _currentPhase.Value = TimelinePhase.Stability;
            _isRunning = true;
        }

        public void Tick(float deltaTime)
        {
            if (!_isRunning) return;

            _elapsedTime += deltaTime;
            UpdatePhase();
        }

        public void Reset()
        {
            _isRunning = false;
            _elapsedTime = 0f;
            _currentPhase.Value = TimelinePhase.Stability;
        }

        public int GetCurrentCapacity() => _currentPhase.Value switch
        {
            TimelinePhase.Stability => StabilityCapacity,
            TimelinePhase.Tension => TensionCapacity,
            TimelinePhase.Crisis => CrisisCapacity,
            TimelinePhase.Collapse => CollapseCapacity,
            TimelinePhase.ChoiceLock => CollapseCapacity,
            _ => 0
        };

        private void UpdatePhase()
        {
            var previousPhase = _currentPhase.Value;
            var newPhase = DeterminePhase();

            if (newPhase != previousPhase)
            {
                _currentPhase.Value = newPhase;
                _phaseChangedSubject.OnNext(new TimelinePhaseChangedEvent(previousPhase, newPhase, _elapsedTime));
            }
        }

        private TimelinePhase DeterminePhase()
        {
            if (_elapsedTime >= CollapseEnd)
                return TimelinePhase.ChoiceLock;
            if (_elapsedTime >= CrisisEnd)
                return TimelinePhase.Collapse;
            if (_elapsedTime >= TensionEnd)
                return TimelinePhase.Crisis;
            if (_elapsedTime >= StabilityEnd)
                return TimelinePhase.Tension;
            return TimelinePhase.Stability;
        }
    }
}