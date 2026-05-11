using System;

namespace SolarPhobia.Application.Services.Interfaces
{
    public interface IBloodNetEffectService : IDisposable
    {
        bool IsSlowed { get; }
        float SlowTimeRemaining { get; }
        float TotalPenaltyApplied { get; }
        void Tick(float deltaTime);
    }
}
