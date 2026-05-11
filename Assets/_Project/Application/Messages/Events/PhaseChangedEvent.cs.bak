using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Messages.Events
{
    public readonly struct PhaseChangedEvent
    {
        public PhaseState PreviousPhase { get; }
        public PhaseState NewPhase { get; }

        public PhaseChangedEvent(PhaseState previousPhase, PhaseState newPhase)
        {
            PreviousPhase = previousPhase;
            NewPhase = newPhase;
        }
    }
}
