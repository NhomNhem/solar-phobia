// Assets/_Project/Domain/Events/NightFailedEvent.cs
namespace SolarPhobia.Domain.Events
{
    /// <summary>
    /// Event published when Ward reaches 0 and night phase fails.
    /// Triggers transition to NightOutcomeState with Death outcome.
    /// </summary>
    public readonly struct NightFailedEvent
    {
        /// <summary>Final Ward value (always 0).</summary>
        public readonly float FinalWard;

        /// <summary>Time survived in night phase (seconds).</summary>
        public readonly float SurvivalTime;

        public NightFailedEvent(float finalWard, float survivalTime)
        {
            FinalWard = finalWard;
            SurvivalTime = survivalTime;
        }
    }
}
