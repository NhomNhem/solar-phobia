using R3;
using System;

namespace SolarPhobia.Application.Services
{
    /// <summary>
    /// Interface for Ward Timer service.
    /// Manages the Ward timer value and applies costs for player actions.
    /// </summary>
    public interface IWardTimerService
    {
        /// <summary>Get current Ward timer value in seconds</summary>
        float CurrentWard { get; set; }

        /// <summary>Get current Ward timer value (method for consistency)</summary>
        float GetCurrentWard();

        /// <summary>Try to apply a cost to the Ward timer.</summary>
        /// <returns>True if cost was applied successfully</returns>
        bool TryApplyCost(float cost);

        /// <summary>Observable for Ward timer changes</summary>
        Observable<float> OnWardChanged { get; }
    }
}
