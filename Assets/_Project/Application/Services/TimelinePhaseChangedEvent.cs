using System;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Services
{
    /// <summary>
    /// Event emitted when timeline transitions between phases.
    /// </summary>
    public readonly struct TimelinePhaseChangedEvent
    {
        public readonly TimelinePhase PreviousPhase;
        public readonly TimelinePhase NewPhase;
        public readonly float ElapsedTime;

        public TimelinePhaseChangedEvent(TimelinePhase previousPhase, TimelinePhase newPhase, float elapsedTime)
        {
            PreviousPhase = previousPhase;
            NewPhase = newPhase;
            ElapsedTime = elapsedTime;
        }
    }
}
