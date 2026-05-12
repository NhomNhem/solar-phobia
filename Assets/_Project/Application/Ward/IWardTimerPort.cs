using R3;

namespace SolarPhobia.Application.Ward
{
    /// <summary>
    /// Application port for querying and mutating the ward timer.
    /// </summary>
    public interface IWardTimerPort
    {
        float CurrentWard { get; set; }

        float GetCurrentWard();

        bool TryApplyCost(float cost);

        Observable<float> OnWardChanged { get; }
    }
}
