using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Messages
{
    /// <summary>
    /// Event published when a soul's night outcome is determined.
    /// </summary>
    public readonly struct NightOutcomeEvent
    {
        public string SoulId { get; }
        public NightOutcomeState Outcome { get; }

        public NightOutcomeEvent(string soulId, NightOutcomeState outcome)
        {
            SoulId = soulId;
            Outcome = outcome;
        }
    }
}