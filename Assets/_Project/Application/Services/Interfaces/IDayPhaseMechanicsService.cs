using System;
using R3;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Services
{
    /// <summary>
    /// Service handling Day Phase mechanics: Swap and Shove soul interactions.
    /// Implements TR-state-001 requirements for player-soul positioning.
    /// Uses R3 for reactive events and follows ADR-0001 architecture.
    /// </summary>
    public interface IDayPhaseMechanicsService
    {
        /// <summary>Attempt to swap player position with a soul at shadow edge</summary>
        /// <returns>True if swap initiated successfully</returns>
        bool TrySwap(string playerId, string soulId, PhaseState currentPhase);

        /// <summary>Force shove one soul into sunlight (abandonment)</summary>
        /// <returns>True if shove executed successfully</returns>
        bool TryShove(string soulId, PhaseState currentPhase);

        /// <summary>Observable event when a swap is initiated</summary>
        Observable<SwapEvent> OnSwapInitiated { get; }

        /// <summary>Observable event when a soul is shoved</summary>
        Observable<ShoveEvent> OnShoveExecuted { get; }

        /// <summary>Get the soul ID marked for sacrifice (if any)</summary>
        string GetSacrificedGhostId();

        /// <summary>Reset service state (called on PhaseState.Reset)</summary>
        void Reset();
    }
}
