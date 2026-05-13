using R3;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Domain.Features.Ward
{
    public interface IWardTimerService
    {
        float CurrentWard { get; set; }

        float GetCurrentWard();

        ReadOnlyReactiveProperty<float> CurrentWardObservable { get; }

        ReadOnlyReactiveProperty<SensoryTier> CurrentTier { get; }

        Observable<Unit> OnDepleted { get; }

        float MaxWard { get; }

        void Initialize(int ghostsSaved, int failedLightInterrupts, int soulPanicEvents);

        void ApplyPenalty(float amount);

        void SetDrainRate(float baseDrain, int boneCount, float hallucinationMultiplier);

        bool TryApplyCost(float amount);
    }
}
