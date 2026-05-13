using R3;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Features.Ward
{
    /// <summary>
    /// Application port for querying and mutating the ward timer.
    /// </summary>
    public interface IWardTimerPort
    {
        float CurrentWard { get; set; }

        float GetCurrentWard();

        bool TryApplyCost(float cost);

        ReadOnlyReactiveProperty<float> CurrentWardObservable { get; }

        ReadOnlyReactiveProperty<SensoryTier> CurrentTier { get; }

        float MaxWard { get; }

        Observable<Unit> OnDepleted { get; }

        Observable<float> OnWardChanged { get; }
    }
}

