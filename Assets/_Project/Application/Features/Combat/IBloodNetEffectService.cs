using System;

namespace SolarPhobia.Application.Features.Combat
{
    public interface IBloodNetEffectService : IDisposable
    {
        bool IsSlowed { get; }
        float SlowTimeRemaining { get; }
        float TotalPenaltyApplied { get; }
        void Tick(float deltaTime);
    }
}
