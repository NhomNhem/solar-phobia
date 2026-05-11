using System;
using System.Collections.Generic;
using R3;
using SolarPhobia.Application.Messages;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Services
{
    /// <summary>
    /// Interface for the game phase state machine.
    /// </summary>
    public interface IPhaseStateMachine
    {
        PhaseState CurrentState { get; }
        ReadOnlyReactiveProperty<PhaseState> CurrentPhase { get; }
        Observable<PhaseChangedEvent> OnPhaseChanged { get; }
        Observable<DayStartEvent> OnDayStart { get; }
        Observable<NightStartEvent> OnNightStart { get; }
        Observable<ResolveEvent> OnResolve { get; }
        bool TryTransition(PhaseState newPhase);
        bool IsActionAllowed(GameAction action);
        void Initialize();
    }
}