using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Messages
{
    /// <summary>
    /// Event published when the game phase changes.
    /// </summary>
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