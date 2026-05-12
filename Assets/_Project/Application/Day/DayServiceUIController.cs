using System;
using System.Collections.Generic;
using System.Linq;
using R3;
using SolarPhobia.Application.Messages;
using SolarPhobia.Application.Phase.Flow;
using SolarPhobia.Application.Repositories;
using SolarPhobia.Domain.ValueObjects;
using SolarPhobia.Application.Rituals;

namespace SolarPhobia.Application.Day
{
    public class DayServiceUIController : IDayServiceUIController, IDisposable
    {
        // ── Dependencies ────────────────────────────────────────────
        private readonly IPhaseStateMachine _phaseStateMachine;
        private readonly ISoulRepository _soulRepository;
        private readonly IDaySelectionValidator _validator;
        private readonly IRitualAssignmentService _ritualService;

        // ── Reactive State ──────────────────────────────────────────
        private readonly ReactiveProperty<bool> _isUIVisible = new(false);
        private readonly ReactiveProperty<SelectionConfirmedPayload> _lastPayload = new();
        private readonly ReactiveProperty<bool> _isConfirmEnabled = new(false);

        public ReadOnlyReactiveProperty<bool> IsUIVisible => _isUIVisible;
        public ReadOnlyReactiveProperty<SelectionConfirmedPayload> LastConfirmedPayload => _lastPayload;
        public ReadOnlyReactiveProperty<bool> IsConfirmEnabled => _isConfirmEnabled;
        public SelectionValidationResult LastValidation { get; private set; }
        public IReadOnlyDictionary<string, RitualType> RitualAssignments => _ritualService.Assignments;
        public bool IsUIVisibleValue => _isUIVisible.Value;
        public SelectionConfirmedPayload LastConfirmedPayloadValue => _lastPayload.Value;
        public bool IsConfirmEnabledValue => _isConfirmEnabled.Value;

        // ── Internal State ──────────────────────────────────────────
        private readonly Dictionary<string, DaySelectionState> _localSelections = new();
        private bool _confirmed;
        private readonly IDisposable _phaseSubscription;

        private static readonly IReadOnlyList<string> SoulIds = new[] { "linh", "van", "minh" };

        public DayServiceUIController(
            IPhaseStateMachine phaseStateMachine,
            ISoulRepository soulRepository,
            IDaySelectionValidator validator,
            IRitualAssignmentService ritualService)
        {
            _phaseStateMachine = phaseStateMachine;
            _soulRepository = soulRepository;
            _validator = validator;
            _ritualService = ritualService;

            ResetLocalState();

            _phaseSubscription = _phaseStateMachine.CurrentPhase.Subscribe(OnPhaseChanged);
        }

        private void OnPhaseChanged(PhaseState phase)
        {
            if (phase == PhaseState.DayService)
            {
                ResetLocalState();
                _isUIVisible.Value = true;
            }
            else if (phase == PhaseState.ChoiceLock)
            {
                _isUIVisible.Value = false;
            }
            else
            {
                _isUIVisible.Value = false;
            }
        }

        /// <summary>
        /// Assign a ritual to a soul. Returns false if soul is abandoned or already confirmed.
        /// </summary>
        public bool AssignRitual(string soulId, RitualType ritual)
        {
            if (_confirmed)
                return false;

            if (!_localSelections.ContainsKey(soulId))
                return false;

            if (_localSelections[soulId] == DaySelectionState.Abandoned)
                return false;

            return _ritualService.TryAssignRitual(soulId, ritual);
        }

        /// <summary>
        /// Remove a ritual assignment from a soul. Returns false if no ritual assigned.
        /// </summary>
        public bool RemoveRitual(string soulId)
        {
            if (_confirmed)
                return false;

            return _ritualService.TryRemoveRitual(soulId);
        }

        /// <summary>
        /// Check if a ritual type is the preferred ritual for a given soul.
        /// </summary>
        public bool IsPreferredRitual(string soulId, RitualType ritual)
        {
            return _ritualService.IsPreferredRitual(soulId, ritual);
        }

        /// <summary>
        /// Toggle a soul between Saved and Abandoned (or back to Unselected).
        /// </summary>
        public void ToggleSoulSelection(string soulId)
        {
            if (_confirmed)
                return;

            if (!_localSelections.ContainsKey(soulId))
                return;

            var current = _localSelections[soulId];
            DaySelectionState next;

            switch (current)
            {
                case DaySelectionState.Unselected:
                    next = DaySelectionState.Saved;
                    break;
                case DaySelectionState.Saved:
                    next = DaySelectionState.Abandoned;
                    break;
                case DaySelectionState.Abandoned:
                    next = DaySelectionState.Unselected;
                    break;
                default:
                    next = DaySelectionState.Unselected;
                    break;
            }

            _localSelections[soulId] = next;

            _soulRepository.TrySetSelection(soulId, next, _phaseStateMachine.CurrentState);

            Revalidate();
        }

        /// <summary>
        /// Attempt to confirm the current selection. Returns true if confirmed.
        /// </summary>
        public bool TryConfirmSelection()
        {
            if (_confirmed)
                return false;

            if (LastValidation == null || !LastValidation.IsValid)
                return false;

            _confirmed = true;

            var savedSouls = _localSelections
                .Where(kvp => kvp.Value == DaySelectionState.Saved)
                .Select(kvp => kvp.Key)
                .ToList();

            var abandonedSoul = _localSelections
                .FirstOrDefault(kvp => kvp.Value == DaySelectionState.Abandoned);

            var ritualDict = new Dictionary<string, string>();
            foreach (var kvp in _ritualService.Assignments)
            {
                ritualDict[kvp.Key] = kvp.Value.ToString();
            }

            var payload = new SelectionConfirmedPayload
            {
                AbandonedSoulId = abandonedSoul.Key,
                SavedSoulIds = savedSouls,
                RitualAssignments = ritualDict
            };

            _lastPayload.Value = payload;

            _phaseStateMachine.TryTransition(PhaseState.ChoiceLock);

            return true;
        }

        public void Reset()
        {
            ResetLocalState();
            _confirmed = false;
            _lastPayload.Value = null;
            _isUIVisible.Value = false;
            _isConfirmEnabled.Value = false;
            _ritualService.Clear();
        }

        public void Dispose()
        {
            _phaseSubscription?.Dispose();
            _isUIVisible.Dispose();
            _lastPayload.Dispose();
            _isConfirmEnabled.Dispose();
        }

        // ── Private ──────────────────────────────────────────────────

        private void ResetLocalState()
        {
            _localSelections.Clear();
            foreach (var id in SoulIds)
            {
                _localSelections[id] = DaySelectionState.Unselected;
            }
            _confirmed = false;
            Revalidate();
        }

        private void Revalidate()
        {
            var selections = SoulIds
                .Select(id => new SoulSelectionState(id, _localSelections[id]))
                .ToList();

            LastValidation = _validator.Validate(selections);
            _isConfirmEnabled.Value = LastValidation.IsValid;
        }
    }
}

