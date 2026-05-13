using R3;
using System;

namespace SolarPhobia.Application.Features.Shrines
{
    public interface IShrineObjectiveService
    {
        bool TryTriggerShrineArrival(float distanceToShrine);
        Observable<Unit> OnShrineReached { get; }
    }
}


