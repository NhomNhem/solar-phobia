using SolarPhobia.Domain.ValueObjects;
using System;

namespace SolarPhobia.Application.Messages.Events
{
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
