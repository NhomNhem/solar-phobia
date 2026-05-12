using R3;
using SolarPhobia.Application.Player.Events;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Phase.Day
{
    /// <summary>
    /// Contract for day-phase swap and shove mechanics.
    /// </summary>
    public interface IDayPhaseMechanicsService
    {
        bool TrySwap(string playerId, string soulId, PhaseState currentPhase);

        bool TryShove(string soulId, PhaseState currentPhase);

        Observable<SwapEvent> OnSwapInitiated { get; }

        Observable<ShoveEvent> OnShoveExecuted { get; }

        string GetSacrificedGhostId();

        void Reset();
    }
}
