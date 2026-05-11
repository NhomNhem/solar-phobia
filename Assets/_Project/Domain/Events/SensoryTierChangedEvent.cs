// Assets/_Project/Domain/Events/SensoryTierChangedEvent.cs
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Domain.Events
{
    /// <summary>
    /// Event published when sensory tier changes.
    /// Fires only once per threshold crossing (AC-6 compliance).
    /// </summary>
    public readonly struct SensoryTierChangedEvent
    {
        /// <summary>New tier after change.</summary>
        public readonly SensoryTier NewTier;

        /// <summary>Previous tier before change.</summary>
        public readonly SensoryTier PreviousTier;

        /// <summary>Current Ward value when change occurred.</summary>
        public readonly float WardValue;

        /// <summary>Maximum Ward value for percentage calculation.</summary>
        public readonly float MaxWard;

        public SensoryTierChangedEvent(SensoryTier newTier, SensoryTier previousTier, float wardValue, float maxWard)
        {
            NewTier = newTier;
            PreviousTier = previousTier;
            WardValue = wardValue;
            MaxWard = maxWard;
        }
    }
}
