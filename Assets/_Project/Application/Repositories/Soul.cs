using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Repositories
{
    /// <summary>
    /// Represents a soul entity managed by the repository.
    /// </summary>
    public class Soul
    {
        public string Id { get; }
        public string LocalizedName { get; }
        public DaySelectionState DaySelection { get; private set; }
        public NightOutcomeState NightOutcome { get; private set; }
        public LifeState Life { get; private set; }

        public void SetDaySelection(DaySelectionState state) => DaySelection = state;
        public void SetNightOutcome(NightOutcomeState state) => NightOutcome = state;
        public void SetLife(LifeState state) => Life = state;

        public Soul(string id, string localizedName)
        {
            Id = id;
            LocalizedName = localizedName;
            DaySelection = DaySelectionState.Unselected;
            NightOutcome = NightOutcomeState.None;
            Life = LifeState.Alive;
        }
    }
}