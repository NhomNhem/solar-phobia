using System.Collections.Generic;
using System.Linq;
using R3;
using SolarPhobia.Domain;
using SolarPhobia.Domain.Events;
using SolarPhobia.Domain.Repositories;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Infrastructure.Features.Soul
{
    /// <summary>
    /// Repository for managing soul entities.
    /// </summary>
    public class SoulRepository : ISoulRepository, System.IDisposable
    {
        private readonly Dictionary<string, SolarPhobia.Domain.Soul> _souls;
        private readonly Subject<SelectionChangedEvent> _selectionSubject = new();
        private string _sacrificedGhostId;

        public IReadOnlyList<SolarPhobia.Domain.Soul> Souls => _souls.Values.ToList();
        public Observable<SelectionChangedEvent> OnSelectionChanged => _selectionSubject;

        public SoulRepository()
        {
            _souls = new Dictionary<string, SolarPhobia.Domain.Soul>
            {
                ["linh"] = new SolarPhobia.Domain.Soul("linh", "Em Linh"),
                ["van"] = new SolarPhobia.Domain.Soul("van", "Ông Văn"),
                ["minh"] = new SolarPhobia.Domain.Soul("minh", "Anh Minh")
            };
        }

        public SolarPhobia.Domain.Soul GetSoul(string id)
        {
            return _souls.TryGetValue(id, out var soul) ? soul : null;
        }

        public bool TrySetSelection(string soulId, DaySelectionState state, PhaseState currentPhase)
        {
            if (currentPhase != PhaseState.DayService)
            {
                return false;
            }

            var soul = GetSoul(soulId);
            if (soul == null)
            {
                return false;
            }

            if (state == DaySelectionState.Saved && soul.DaySelection == DaySelectionState.Abandoned)
            {
                return false;
            }

            var oldState = soul.DaySelection;
            soul.SetDaySelection(state);

            _selectionSubject.OnNext(new SelectionChangedEvent(soulId, oldState, state));

            return true;
        }

        public bool TrySetNightOutcome(string soulId, NightOutcomeState outcome, PhaseState currentPhase)
        {
            if (currentPhase != PhaseState.ChoiceLock && currentPhase != PhaseState.NightSurvival)
            {
                return false;
            }

            var soul = GetSoul(soulId);
            if (soul == null || soul.DaySelection != DaySelectionState.Abandoned)
            {
                return false;
            }

            soul.SetNightOutcome(outcome);
            return true;
        }

        public IReadOnlyList<SolarPhobia.Domain.Soul> GetSavedSouls()
        {
            return _souls.Values.Where(s => s.DaySelection == DaySelectionState.Saved).ToList();
        }

        public IReadOnlyList<SolarPhobia.Domain.Soul> GetAbandonedSoul()
        {
            return _souls.Values.Where(s => s.DaySelection == DaySelectionState.Abandoned).ToList();
        }

        public bool IsSelectionValid(int requiredSaved)
        {
            int savedCount = GetSavedSouls().Count;
            int abandonedCount = GetAbandonedSoul().Count;

            return savedCount == requiredSaved &&
                   abandonedCount == _souls.Count - requiredSaved;
        }

        public void Reset()
        {
            foreach (var soul in _souls.Values)
            {
                soul.SetDaySelection(DaySelectionState.Unselected);
                soul.SetNightOutcome(NightOutcomeState.None);
                soul.SetLife(LifeState.Alive);
            }
        }

        public bool IsAtShadowEdge(string soulId)
        {
            return _souls.ContainsKey(soulId);
        }

        public void SwapPositions(string playerId, string soulId)
        {
        }

        public string GetFirstSoulAtShadowEdge()
        {
            return _souls.Keys.FirstOrDefault();
        }

        public void MarkAbandoned(string soulId)
        {
            var soul = GetSoul(soulId);
            if (soul != null)
            {
                soul.SetDaySelection(DaySelectionState.Abandoned);
            }
        }

        public void SetSacrificedGhostId(string soulId)
        {
            _sacrificedGhostId = soulId;
        }

        public void Dispose()
        {
            _selectionSubject?.Dispose();
        }
    }
}
