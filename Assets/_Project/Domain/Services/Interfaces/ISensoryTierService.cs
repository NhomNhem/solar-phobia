// Assets/_Project/Domain/Services/ISensoryTierService.cs
using System;
using R3;
using SolarPhobia.Domain.Events;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Domain
{
    /// <summary>
    /// Interface for sensory tier feedback service.
    /// Implements TR-state-005: Sensory tiers trigger at 75%, 50%, 25%, ≤10s thresholds.
    /// Callers must dispose the service when the night phase ends to release the Ward subscription.
    /// </summary>
    public interface ISensoryTierService : IDisposable
    {
        /// <summary>
        /// Observable that emits when sensory tier changes (one-shot per downward threshold crossing).
        /// </summary>
        Observable<SensoryTierChangedEvent> OnTierChanged { get; }

        /// <summary>
        /// Observable that emits exactly once when Ward reaches 0 (player death).
        /// </summary>
        Observable<NightFailedEvent> OnNightFailed { get; }

        /// <summary>
        /// Current sensory tier (reactive property for continuous monitoring).
        /// </summary>
        ReadOnlyReactiveProperty<SensoryTier> CurrentTier { get; }

        /// <summary>
        /// Binds the service to a Ward timer observable and sets the max Ward for percentage calculations.
        /// Safe to call again on re-initialization — disposes the previous subscription first.
        /// </summary>
        void Initialize(ReadOnlyReactiveProperty<float> wardObservable, float maxWard);
    }
}
