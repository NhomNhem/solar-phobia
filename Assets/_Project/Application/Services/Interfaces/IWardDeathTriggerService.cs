using System;

namespace SolarPhobia.Application.Services.Interfaces
{
    public interface IWardDeathTriggerService : IDisposable
    {
        bool HasTriggeredDeath { get; }
    }
}
