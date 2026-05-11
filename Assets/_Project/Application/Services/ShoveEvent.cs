namespace SolarPhobia.Application.Services
{
    /// <summary>Event emitted when a soul is shoved into sunlight.</summary>
    public readonly struct ShoveEvent
    {
        public readonly string SoulId;
        public readonly string SacrificedGhostId; // Written to SoulRepository
        public readonly float AnimationDuration; // Screen shake + burn

        public ShoveEvent(string soulId, string sacrificedGhostId)
        {
            SoulId = soulId;
            SacrificedGhostId = sacrificedGhostId;
            AnimationDuration = 1.0f; // Screen shake + soul burn
        }
    }
}
