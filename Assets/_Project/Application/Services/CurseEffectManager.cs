using System;
using System.Collections.Generic;
using R3;
using VContainer;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Services
{
    /// <summary>
    /// Implementation of curse effect management - core state machine for hazard spawning.
    /// 
    /// State transitions:
    /// - Idle → CurseActive: When NightSurvival phase starts AND curse received
    /// - CurseActive → HazardTriggered: When player enters hazard zone
    /// - HazardTriggered → HazardCleared: When player exits hazard zone
    /// - HazardCleared → CurseActive: When hazard effect ends (for next hazard)
    /// </summary>
    public class CurseEffectManager : ICurseEffectManager
    {
        // ── Dependencies ──────────────────────────────
        private readonly IPhaseStateMachine _phaseStateMachine;
        private IDisposable _phaseSubscription;
        private IDisposable _nightStartSubscription;

        // ── State ──────────────────────────────
        private readonly ReactiveProperty<CurseEffectState> _currentState = new(CurseEffectState.Idle);
        private readonly Subject<HazardEvent> _hazardTriggeredSubject = new();
        private readonly Subject<HazardEvent> _hazardClearedSubject = new();
        
        private NightOutcomeState _currentCurseType = NightOutcomeState.None;
        private readonly HashSet<string> _activeHazards = new();
        private string _currentHazardId = string.Empty;

        /// <summary>
        /// Current curse effect state value.
        /// </summary>
        public CurseEffectState CurrentStateValue => _currentState.Value;

        /// <summary>
        /// Current curse effect state observable.
        /// </summary>
        public ReadOnlyReactiveProperty<CurseEffectState> CurrentState => _currentState;

        /// <summary>
        /// Observable that triggers when a hazard is triggered.
        /// </summary>
        public Observable<HazardEvent> OnHazardTriggered => _hazardTriggeredSubject;

        /// <summary>
        /// Observable that triggers when a hazard is cleared.
        /// </summary>
        public Observable<HazardEvent> OnHazardCleared => _hazardClearedSubject;

        /// <summary>
        /// Initializes a new instance of the CurseEffectManager class.
        /// </summary>
        [Inject]
        public CurseEffectManager(IPhaseStateMachine phaseStateMachine)
        {
            _phaseStateMachine = phaseStateMachine;
        }

        /// <inheritdoc/>
        public void Initialize()
        {
            _phaseSubscription = _phaseStateMachine.CurrentPhase
                .Subscribe(OnPhaseChanged);

            _nightStartSubscription = _phaseStateMachine.OnNightStart
                .Subscribe(_ => OnNightStart());
        }

        /// <inheritdoc/>
        public void ReceiveCurse(NightOutcomeState curseType)
        {
            _currentCurseType = curseType;
            
            if (_phaseStateMachine.CurrentState == PhaseState.NightSurvival)
            {
                TransitionTo(CurseEffectState.CurseActive);
            }
        }

        /// <inheritdoc/>
        public void OnPlayerEnterHazard(string hazardId)
        {
            if (_currentState.Value != CurseEffectState.CurseActive && 
                _currentState.Value != CurseEffectState.HazardCleared &&
                _currentState.Value != CurseEffectState.HazardTriggered)
            {
                return;
            }

            if (_activeHazards.Contains(hazardId))
            {
                return;
            }

            _activeHazards.Add(hazardId);
            _currentHazardId = hazardId;

            if (_currentState.Value != CurseEffectState.HazardTriggered)
            {
                TransitionTo(CurseEffectState.HazardTriggered);
            }

            var evt = new HazardEvent
            {
                HazardId = hazardId,
                CurseType = _currentCurseType
            };
            _hazardTriggeredSubject.OnNext(evt);
        }

        /// <inheritdoc/>
        public void OnPlayerExitHazard(string hazardId)
        {
            if (_currentState.Value != CurseEffectState.HazardTriggered &&
                _currentState.Value != CurseEffectState.HazardCleared)
            {
                return;
            }

            _activeHazards.Remove(hazardId);

            TransitionTo(CurseEffectState.HazardCleared);

            var evt = new HazardEvent
            {
                HazardId = hazardId,
                CurseType = _currentCurseType
            };
            _hazardClearedSubject.OnNext(evt);

            // If no more active hazards, return to CurseActive
            if (_activeHazards.Count == 0)
            {
                TransitionTo(CurseEffectState.CurseActive);
            }
        }

        // ── Private Methods ──────────────────────────────

        private void OnPhaseChanged(PhaseState newPhase)
        {
            if (newPhase != PhaseState.NightSurvival && _currentState.Value != CurseEffectState.Idle)
            {
                TransitionTo(CurseEffectState.Idle);
                _currentCurseType = NightOutcomeState.None;
                _activeHazards.Clear();
            }
        }

        private void OnNightStart()
        {
            // If we already have a curse type (from earlier), activate it
            if (_currentCurseType != NightOutcomeState.None)
            {
                TransitionTo(CurseEffectState.CurseActive);
            }
        }

        private void TransitionTo(CurseEffectState newState)
        {
            if (_currentState.Value != newState)
            {
                _currentState.Value = newState;
            }
        }

        /// <summary>
        /// Dispose all subscriptions and resources.
        /// </summary>
        public void Dispose()
        {
            _phaseSubscription?.Dispose();
            _nightStartSubscription?.Dispose();
            _currentState?.Dispose();
            _hazardTriggeredSubject?.Dispose();
            _hazardClearedSubject?.Dispose();
            _activeHazards.Clear();
        }
    }
}
