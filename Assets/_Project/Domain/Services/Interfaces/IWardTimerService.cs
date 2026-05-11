// Assets/_Project/Domain/Services/IWardTimerService.cs
using R3;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Domain
{
    /// <summary>
    /// Interface for ward timer service.
    /// Implements TR-state-005: Ward Timer Initialization — Base + (Saved × 30) Formula
    /// </summary>
    public interface IWardTimerService
    {
        /// <summary>
        /// Initialize Ward timer for night phase.
        /// Formula: InitialWard = 10 + (GhostsSaved × 30) - DayPenalties
        /// </summary>
        void Initialize(int ghostsSaved, int failedLightInterrupts, int soulPanicEvents);

        /// <summary>
        /// Apply a penalty to the ward timer.
        /// </summary>
        void ApplyPenalty(float amount);

        /// <summary>
        /// Set the passive drain rate.
        /// Formula: baseDrain + (boneCount × hallucinationMultiplier)
        /// </summary>
        void SetDrainRate(float baseDrain, int boneCount, float hallucinationMultiplier);

        /// <summary>
        /// Current Ward timer value (0 = depleted/death).
        /// </summary>
        float CurrentWard { get; }

        /// <summary>
        /// Observable for ward timer value changes.
        /// </summary>
        ReadOnlyReactiveProperty<float> CurrentWardObservable { get; }

        /// <summary>
        /// Observable for current sensory tier changes.
        /// </summary>
        ReadOnlyReactiveProperty<SensoryTier> CurrentTier { get; }

        /// <summary>
        /// Observable that emits when Ward reaches 0 (player death).
        /// </summary>
        Observable<Unit> OnDepleted { get; }

        /// <summary>
        /// Maximum Ward value set during initialization.
        /// </summary>
        float MaxWard { get; }
    }
}
