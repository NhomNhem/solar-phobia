using System;
using System.Collections.Generic;
using R3;
using SolarPhobia.Application.Messages;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Services
{
    /// <summary>
    /// Implementation of the game phase state machine.
    /// </summary>
    public class PhaseStateMachine : IPhaseStateMachine
    {
        private readonly ReactiveProperty<PhaseState> _currentPhase = new(PhaseState.Boot);
        private readonly Subject<PhaseChangedEvent> _phaseChangedSubject = new();
        private readonly Subject<DayStartEvent> _dayStartSubject = new();
        private readonly Subject<NightStartEvent> _nightStartSubject = new();
        private readonly Subject<ResolveEvent> _resolveSubject = new();
        private readonly Dictionary<PhaseState, HashSet<GameAction>> _phaseContracts;

        public PhaseState CurrentState => _currentPhase.Value;
        public ReadOnlyReactiveProperty<PhaseState> CurrentPhase => _currentPhase;
        public Observable<PhaseChangedEvent> OnPhaseChanged => _phaseChangedSubject;
        public Observable<DayStartEvent> OnDayStart => _dayStartSubject;
        public Observable<NightStartEvent> OnNightStart => _nightStartSubject;
        public Observable<ResolveEvent> OnResolve => _resolveSubject;

        public PhaseStateMachine()
        {
            _phaseContracts = BuildPhaseContracts();
        }

        public void Initialize()
        {
            TransitionTo(PhaseState.DayService);
        }

        public bool TryTransition(PhaseState newPhase)
        {
            if (!IsValidTransition(_currentPhase.Value, newPhase))
            {
                return false;
            }

            TransitionTo(newPhase);
            return true;
        }

        private void TransitionTo(PhaseState newPhase)
        {
            var previousPhase = _currentPhase.Value;
            _currentPhase.Value = newPhase;

            _phaseChangedSubject.OnNext(new PhaseChangedEvent(previousPhase, newPhase));

            switch (newPhase)
            {
                case PhaseState.DayService:
                    _dayStartSubject.OnNext(new DayStartEvent());
                    break;
                case PhaseState.NightSurvival:
                    _nightStartSubject.OnNext(new NightStartEvent());
                    break;
                case PhaseState.EndingEvaluation:
                    _resolveSubject.OnNext(new ResolveEvent());
                    break;
            }
        }

        public bool IsActionAllowed(GameAction action)
        {
            return _phaseContracts.TryGetValue(_currentPhase.Value, out var allowed)
                   && allowed.Contains(action);
        }

        private bool IsValidTransition(PhaseState from, PhaseState to)
        {
            return to switch
            {
                PhaseState.DayService => from == PhaseState.Boot || from == PhaseState.ChoiceLock,
                PhaseState.Dialogue => from == PhaseState.DayService,
                PhaseState.Order => from == PhaseState.Dialogue,
                PhaseState.SunsetWarning => from == PhaseState.Order,
                PhaseState.NightTravel => from == PhaseState.SunsetWarning,
                PhaseState.ShrineArrival => from == PhaseState.NightTravel,
                PhaseState.NightSurvival => from == PhaseState.ShrineArrival,
                PhaseState.EndingEvaluation => from == PhaseState.NightSurvival,
                PhaseState.ChoiceLock => from == PhaseState.EndingEvaluation,
                _ => false
            };
        }

        private Dictionary<PhaseState, HashSet<GameAction>> BuildPhaseContracts()
        {
            return new Dictionary<PhaseState, HashSet<GameAction>>
            {
                [PhaseState.DayService] = new()
                {
                    GameAction.StartGame,
                    GameAction.CompleteDayPrep,
                    GameAction.FinishDialogue,
                    GameAction.CompleteOrder
                },
                [PhaseState.Dialogue] = new()
                {
                    GameAction.SunsetWarningTimeout
                },
                [PhaseState.Order] = new()
                {
                    GameAction.ArriveAtShrine
                },
                [PhaseState.SunsetWarning] = new()
                {
                    GameAction.CompleteEndingEvaluation
                },
                [PhaseState.NightTravel] = new()
                {
                    GameAction.SurviveNight
                },
                [PhaseState.ShrineArrival] = new()
                {
                    GameAction.MakeChoice
                },
                [PhaseState.NightSurvival] = new()
                {
                    GameAction.ResetGame
                },
                [PhaseState.EndingEvaluation] = new(),
                [PhaseState.ChoiceLock] = new()
            };
        }
    }
}