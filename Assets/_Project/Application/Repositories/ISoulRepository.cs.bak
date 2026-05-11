using System.Collections.Generic;
using R3;
using SolarPhobia.Application.Messages;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Repositories
{
    /// <summary>
    /// Repository interface for soul entities.
    /// </summary>
    public interface ISoulRepository
    {
        IReadOnlyList<Soul> Souls { get; }
        Soul GetSoul(string id);
        bool TrySetSelection(string soulId, DaySelectionState state, PhaseState currentPhase);
        bool TrySetNightOutcome(string soulId, NightOutcomeState outcome, PhaseState currentPhase);
        IReadOnlyList<Soul> GetSavedSouls();
        IReadOnlyList<Soul> GetAbandonedSoul();
        bool IsSelectionValid(int requiredSaved);
        void Reset();
        Observable<SelectionChangedEvent> OnSelectionChanged { get; }
        bool IsAtShadowEdge(string soulId);
        void SwapPositions(string playerId, string soulId);
        string GetFirstSoulAtShadowEdge();
        void MarkAbandoned(string soulId);
        void SetSacrificedGhostId(string soulId);
    }
}