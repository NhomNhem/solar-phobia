using System;

namespace SolarPhobia.Application.Phase.Reset
{
    public interface IWardDeathTriggerService : IDisposable
    {
        bool HasTriggeredDeath { get; }

        void Reset();
    }
}
