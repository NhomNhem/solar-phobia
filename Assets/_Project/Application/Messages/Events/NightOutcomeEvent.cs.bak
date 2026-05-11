using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Messages.Events
{
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
