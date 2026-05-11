using System.Collections.Generic;
using SolarPhobia.Domain;

namespace SolarPhobia.Application.Messages
{
    public class GameSessionState
    {
        public GamePhase CurrentPhase { get; set; } = GamePhase.BOOT;
        public KindnessScore KindnessScore { get; set; } = new KindnessScore(0);
        public int SpiritEssence { get; set; }
        public string CurrentShrineId { get; set; }
        public string CurrentDialogueId { get; set; }
        public string CurrentOrderId { get; set; }
        public List<string> Flags { get; } = new List<string>();
    }
}

