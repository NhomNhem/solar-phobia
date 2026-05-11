using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Messages.Events
{
    public readonly struct SelectionChangedEvent
    {
        public string SoulId { get; }
        public DaySelectionState OldState { get; }
        public DaySelectionState NewState { get; }

        public SelectionChangedEvent(string soulId, DaySelectionState oldState, DaySelectionState newState)
        {
            SoulId = soulId;
            OldState = oldState;
            NewState = newState;
        }
    }
}
