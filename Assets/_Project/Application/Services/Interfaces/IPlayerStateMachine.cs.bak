// Assets/_Project/Application/Services/Interfaces/IPlayerStateMachine.cs
using System;
using R3;
using SolarPhobia.Domain.Events;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Services
{
    /// <summary>
    /// Interface for the player Finite State Machine (FSM).
    /// Manages all player states and emits state-change events.
    /// Implements TR-player-009.
    /// </summary>
    /// <remarks>
    /// SOLID:ISP - Focused interface with minimal methods
    /// </remarks>
    public interface IPlayerStateMachine
    {
        /// <summary>Current FSM state value (immediate read).</summary>
        EPlayerState CurrentStateValue { get; }

        /// <summary>Current FSM state as a reactive observable.</summary>
        ReadOnlyReactiveProperty<EPlayerState> CurrentState { get; }

        /// <summary>Event stream for state transitions.</summary>
        Observable<PlayerStateChangedEvent> OnStateChanged { get; }

        /// <summary>
        /// Movement speed multiplier based on current state.
        /// AC-4: Exhausted state reduces movement speed by 50%.
        /// </summary>
        float MovementSpeedMultiplier { get; }

        /// <summary>
        /// Attempts to transition to the target state.
        /// Returns true if the transition is valid and executed.
        /// </summary>
        bool TryTransitionTo(EPlayerState targetState);

        /// <summary>
        /// Checks whether a transition to the target state is currently valid.
        /// </summary>
        bool CanTransitionTo(EPlayerState targetState);

        /// <summary>
        /// Whether the current phase is NightSurvival (full 2D movement enabled).
        /// Returns true during night phase, false during day or locked phases.
        /// </summary>
        bool IsNightPhase { get; }

        /// <summary>VContainer initialization entry point.</summary>
        void Initialize();
    }
}
