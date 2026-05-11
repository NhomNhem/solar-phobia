// Assets/_Project/Application/Services/PlayerStateMachine.cs
using System;
using System.Collections.Generic;
using R3;
using SolarPhobia.Domain.Events;
using SolarPhobia.Domain.ValueObjects;
using VContainer;
using VContainer.Unity;

namespace SolarPhobia.Application.Services
{
    /// <summary>
    /// Centralized Finite State Machine for player states during night survival.
    /// Pure C# logic — fully testable without Unity scene dependencies.
    /// Implements TR-player-009, ADR-0003-v2.
    /// </summary>
    /// <remarks>
    /// SOLID Compliance:
    /// - SRP: Single responsibility - manages player state transitions only
    /// - OCP: Open for extension - add new states/transitions without modifying existing
    /// - LSP: Interface-based design allows substitution
    /// - ISP: Focused interface with minimal methods
    /// - DIP: Depends on abstractions (IPlayerStateMachine interface)
    /// </remarks>
    public sealed class PlayerStateMachine : IPlayerStateMachine, IInitializable, IDisposable
    {
        private readonly IPhaseStateMachine _phaseStateMachine;
        private readonly ReactiveProperty<EPlayerState> _currentState = new(EPlayerState.Idle);
        private readonly Subject<PlayerStateChangedEvent> _stateChangedSubject = new();
        private readonly List<IDisposable> _disposables = new();
        private bool _isNightMovement;

        public ReadOnlyReactiveProperty<EPlayerState> CurrentState => _currentState;
        public EPlayerState CurrentStateValue => _currentState.Value;
        public Observable<PlayerStateChangedEvent> OnStateChanged => _stateChangedSubject;
        public bool IsNightPhase => _isNightMovement;

        /// <summary>
        /// Returns movement speed multiplier based on current state.
        /// AC-4: Exhausted state reduces movement speed by 50%.
        /// </summary>
        public float MovementSpeedMultiplier => _currentState.Value == EPlayerState.Exhausted ? 0.5f : 1.0f;

        // ── Constructor ───────────────────────────────────────────
        [Inject]
        public PlayerStateMachine(IPhaseStateMachine phaseStateMachine)
        {
            _phaseStateMachine = phaseStateMachine;
        }

        // ── IInitializable ─────────────────────────────────────────
        public void Initialize()
        {
            _currentState.Value = EPlayerState.Idle;
            _isNightMovement = _phaseStateMachine.CurrentState == PhaseState.NightSurvival;

            _phaseStateMachine.OnPhaseChanged
                .Subscribe(e => OnPhaseChanged(e.NewPhase))
                .AddTo(_disposables);
        }

        private void OnPhaseChanged(PhaseState phase)
        {
            _currentState.Value = EPlayerState.Idle;

            _isNightMovement = phase == PhaseState.NightSurvival;
        }

        // ── IPlayerStateMachine Implementation ─────────────────────
        public bool TryTransitionTo(EPlayerState targetState)
        {
            if (!CanTransitionTo(targetState))
                return false;

            var previous = _currentState.Value;
            _currentState.Value = targetState;
            _stateChangedSubject.OnNext(new PlayerStateChangedEvent(previous, targetState));
            return true;
        }

        public bool CanTransitionTo(EPlayerState targetState)
        {
            if (_isNightMovement && IsNightSkillState(targetState))
            {
                return true;
            }

            var current = _currentState.Value;
            return IsValidTransition(current, targetState);
        }

        // ── IDisposable ───────────────────────────────────────────
        public void Dispose()
        {
            _currentState.Dispose();
            _stateChangedSubject.Dispose();
            foreach (var disposable in _disposables)
            {
                disposable?.Dispose();
            }
            _disposables.Clear();
        }

        // ── SOLID Compliance ───────────────────────────────────────
        // SRP: Manages only state transitions
        // OCP: Add new states/transitions without modifying existing
        // LSP: Interface-based design allows substitution
        // ISP: Focused interface with minimal methods
        // DIP: Depends on IPlayerStateMachine abstraction

        // ── Private ───────────────────────────────────────────────
        private static bool IsValidTransition(EPlayerState from, EPlayerState to)
        {
            if (from == to)
                return false;

            return (from, to) switch
            {
                // Idle
                (EPlayerState.Idle, EPlayerState.Moving) => true,
                (EPlayerState.Idle, EPlayerState.Jumping) => true,
                (EPlayerState.Idle, EPlayerState.Crouching) => true,
                (EPlayerState.Idle, EPlayerState.Interacting) => true,
                (EPlayerState.Idle, EPlayerState.Exhausted) => true,

                // Moving
                (EPlayerState.Moving, EPlayerState.Idle) => true,
                (EPlayerState.Moving, EPlayerState.Sprinting) => true,
                (EPlayerState.Moving, EPlayerState.Jumping) => true,
                (EPlayerState.Moving, EPlayerState.Falling) => true,
                (EPlayerState.Moving, EPlayerState.Dashing) => true,
                (EPlayerState.Moving, EPlayerState.Swinging) => true,
                (EPlayerState.Moving, EPlayerState.Crouching) => true,
                (EPlayerState.Moving, EPlayerState.Interacting) => true,
                (EPlayerState.Moving, EPlayerState.Exhausted) => true,

                // Sprinting
                (EPlayerState.Sprinting, EPlayerState.Moving) => true,
                (EPlayerState.Sprinting, EPlayerState.Idle) => true,
                (EPlayerState.Sprinting, EPlayerState.Jumping) => true,
                (EPlayerState.Sprinting, EPlayerState.Crouching) => true,
                (EPlayerState.Sprinting, EPlayerState.Exhausted) => true,

                // Jumping
                (EPlayerState.Jumping, EPlayerState.Falling) => true,
                (EPlayerState.Jumping, EPlayerState.Gliding) => true,
                (EPlayerState.Jumping, EPlayerState.Idle) => true,
                (EPlayerState.Jumping, EPlayerState.Exhausted) => true,

                // Falling
                (EPlayerState.Falling, EPlayerState.Gliding) => true,
                (EPlayerState.Falling, EPlayerState.Idle) => true,
                (EPlayerState.Falling, EPlayerState.Moving) => true,
                (EPlayerState.Falling, EPlayerState.Swinging) => true,
                (EPlayerState.Falling, EPlayerState.Exhausted) => true,

                // Gliding
                (EPlayerState.Gliding, EPlayerState.Falling) => true,
                (EPlayerState.Gliding, EPlayerState.Idle) => true,
                (EPlayerState.Gliding, EPlayerState.Exhausted) => true,

                // Dashing
                (EPlayerState.Dashing, EPlayerState.Idle) => true,
                (EPlayerState.Dashing, EPlayerState.Moving) => true,
                (EPlayerState.Dashing, EPlayerState.Falling) => true,
                (EPlayerState.Dashing, EPlayerState.Exhausted) => true,

                // Swinging
                (EPlayerState.Swinging, EPlayerState.Falling) => true,
                (EPlayerState.Swinging, EPlayerState.Idle) => true,
                (EPlayerState.Swinging, EPlayerState.Exhausted) => true,

                // Interacting
                (EPlayerState.Interacting, _) => true,

                // Exhausted — only allow movement restoration
                (EPlayerState.Exhausted, EPlayerState.Idle) => true,
                (EPlayerState.Exhausted, EPlayerState.Moving) => true,
                (EPlayerState.Exhausted, EPlayerState.Crouching) => true,

                // Crouching
                (EPlayerState.Crouching, EPlayerState.Idle) => true,
                (EPlayerState.Crouching, EPlayerState.Moving) => true,

                _ => false
            };
        }

        private static bool IsNightSkillState(EPlayerState state)
        {
            return state == EPlayerState.Dashing
                   || state == EPlayerState.Swinging
                   || state == EPlayerState.Jumping
                   || state == EPlayerState.Sprinting;
        }
    }
}
