using System;
using R3;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Services
{
    /// <summary>
    /// Service managing the Day phase timeline with 4 pressure phases.
    /// Tracks elapsed time and automatically transitions between phases.
    /// Uses R3 for reactive state and Zlinq for zero-allocation LINQ operations.
    /// </summary>
    public interface IDayPhaseTimelineService
    {
        /// <summary>Current timeline phase (Stability, Tension, Crisis, Collapse, ChoiceLock)</summary>
        TimelinePhase CurrentPhase { get; }

        /// <summary>Observable timeline phase for reactive subscriptions</summary>
        ReadOnlyReactiveProperty<TimelinePhase> CurrentTimelinePhase { get; }

        /// <summary>Elapsed time in seconds since timeline started</summary>
        float ElapsedTime { get; }

        /// <summary>Total timeline duration (300 seconds)</summary>
        float PhaseDuration { get; }

        /// <summary>Event emitted when timeline phase changes</summary>
        Observable<TimelinePhaseChangedEvent> OnTimelinePhaseChanged { get; }

        /// <summary>Starts the timeline from Stability phase</summary>
        void StartTimeline();

        /// <summary>Advances timeline by deltaTime seconds</summary>
        void Tick(float deltaTime);

        /// <summary>Resets timeline to initial state</summary>
        void Reset();

        /// <summary>Gets maximum soul capacity for current phase</summary>
        int GetCurrentCapacity();
    }
}
