using System;

namespace SolarPhobia.Application.Features.Phase.Reset
{
    public interface IWardDeathTriggerService : IDisposable
    {
        bool HasTriggeredDeath { get; }

        void Reset();
    }
}

