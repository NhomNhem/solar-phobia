namespace SolarPhobia.Domain.ValueObjects
{
    public readonly struct SoulSelectionState
    {
        public string SoulId { get; }
        public DaySelectionState State { get; }

        public SoulSelectionState(string soulId, DaySelectionState state)
        {
            SoulId = soulId;
            State = state;
        }
    }
}
