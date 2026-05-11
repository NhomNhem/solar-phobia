// Assets/_Project/Domain/Entities/Ghost.cs
using System.Collections.Generic;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Domain
{
    /// <summary>
    /// NPC ghost entity with soul data, day selection, and night outcome tracking.
    /// </summary>
    public class Ghost
    {
        public string Id { get; }
        public string DisplayName { get; }
        public int VisitCount { get; set; }
        public int Satisfaction { get; private set; }
        public NightOutcomeState NightOutcome { get; set; }

        public List<string> DialogueNodes { get; } = new List<string>();

        public Ghost(string id, string displayName)
        {
            Id = id;
            DisplayName = displayName;
            VisitCount = 0;
            Satisfaction = 100;
            NightOutcome = NightOutcomeState.None;
        }

        public void ApplyKindness(int delta)
        {
            Satisfaction = System.Math.Clamp(Satisfaction + delta * 10, 0, 100);
            VisitCount = System.Math.Max(VisitCount, 1);
        }
    }
}