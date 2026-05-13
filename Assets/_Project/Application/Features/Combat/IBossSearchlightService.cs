using R3;
using UnityEngine;

namespace SolarPhobia.Application.Features.Combat
{
    public interface IBossSearchlightService
    {
        void ActivateSearchlight();
        void DeactivateSearchlight();
        bool IsPlayerExposed(Vector3 playerPosition, bool isInCover);
        void ExecuteStrike();
        Observable<bool> OnTelegraphActive { get; }
        Observable<float> OnWardPenalty { get; }
    }
}
