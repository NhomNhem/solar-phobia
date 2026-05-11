using System;

namespace SolarPhobia.Application.Services
{
    public interface IShrineObjectiveService
    {
        bool TryTriggerShrineArrival(float distanceToShrine);
    }
}
