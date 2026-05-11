using R3;
using UnityEngine;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Services
{
    /// <summary>
    /// Interface for boss searchlight sweep and strike logic.
    /// </summary>
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