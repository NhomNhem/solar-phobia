// Assets/_Project/Application/Services/PlayerInputHandler.cs
using System;
using R3;
using SolarPhobia.Domain.ValueObjects;
using VContainer;
using VContainer.Unity;

namespace SolarPhobia.Application.Services
{
    /// <summary>
    /// Routes player input to the correct mode based on the current game phase.
    /// Implements TR-player-001, TR-player-008: Phase-gated input.
    ///
    /// Mode mapping:
    ///   NightSurvival                          → NightMovement
    ///   DayService                             → DayUI
    ///   ChoiceLock / EndingEvaluation / Boot   → Disabled
    ///   All other phases (travel, dialogue…)   → Disabled
    ///
    /// Mode switches synchronously on phase change — no frame delay.
    /// No combat inputs exist in any mode (flight-only survival design).
    /// </summary>
    public class PlayerInputHandler : IPlayerInputHandler, IInitializable, IDisposable
    {
        // ── R3 Reactive State ──────────────────────────────────────
        private readonly ReactiveProperty<PlayerInputMode> _currentMode
            = new(PlayerInputMode.Disabled);

        // ── Dependencies ───────────────────────────────────────────
        private readonly IPhaseStateMachine _phaseStateMachine;

        // ── Subscriptions ──────────────────────────────────────────
        private IDisposable _phaseSubscription;

        // ── Public Interface ───────────────────────────────────────
        /// <inheritdoc/>
        public ReadOnlyReactiveProperty<PlayerInputMode> CurrentMode => _currentMode;

        /// <inheritdoc/>
        public bool IsMovementEnabled => _currentMode.Value == PlayerInputMode.NightMovement;

        /// <inheritdoc/>
        public bool IsUIEnabled => _currentMode.Value == PlayerInputMode.DayUI;

        // ── Constructor ────────────────────────────────────────────
        [Inject]
        public PlayerInputHandler(IPhaseStateMachine phaseStateMachine)
        {
            _phaseStateMachine = phaseStateMachine;
        }

        // ── IInitializable ─────────────────────────────────────────
        /// <summary>
        /// Subscribes to phase changes and sets the initial mode from the current phase.
        /// </summary>
        public void Initialize()
        {
            // Set initial mode from current phase (handles hot-reload / late init)
            _currentMode.Value = ResolveModeForPhase(_phaseStateMachine.CurrentState);

            // Subscribe to future phase changes — synchronous update
            _phaseSubscription = _phaseStateMachine.CurrentPhase
                .Subscribe(phase => _currentMode.Value = ResolveModeForPhase(phase));
        }

        // ── Private Methods ────────────────────────────────────────
        /// <summary>
        /// Maps a <see cref="PhaseState"/> to the corresponding <see cref="PlayerInputMode"/>.
        /// </summary>
        private static PlayerInputMode ResolveModeForPhase(PhaseState phase)
        {
            return phase switch
            {
                PhaseState.NightSurvival => PlayerInputMode.NightMovement,
                PhaseState.DayService    => PlayerInputMode.DayUI,
                _                        => PlayerInputMode.Disabled
            };
        }

        // ── IDisposable ───────────────────────────────────────────────
        public void Dispose()
        {
            _phaseSubscription?.Dispose();
            _currentMode.Dispose();
        }
    }
}
