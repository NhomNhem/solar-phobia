// Assets/_Project/Application/Services/PlayerInputHandler.cs
using System;
using R3;
using SolarPhobia.Application.Phase.Flow;
using SolarPhobia.Domain.ValueObjects;
using VContainer;
using VContainer.Unity;

namespace SolarPhobia.Application.Player.Input
{
    /// <summary>
    /// Routes player input to the correct mode based on the current game phase.
    /// Implements TR-player-001, TR-player-008: Phase-gated input.
    ///
    /// Mode mapping:
    ///   NightSurvival                          â†’ NightMovement
    ///   DayService                             â†’ DayUI
    ///   ChoiceLock / EndingEvaluation / Boot   â†’ Disabled
    ///   All other phases (travel, dialogueâ€¦)   â†’ Disabled
    ///
    /// Mode switches synchronously on phase change â€” no frame delay.
    /// No combat inputs exist in any mode (flight-only survival design).
    /// </summary>
    public class PlayerInputHandler : IPlayerInputHandler, IInitializable, IDisposable
    {
        // â”€â”€ R3 Reactive State â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        private readonly ReactiveProperty<PlayerInputMode> _currentMode
            = new(PlayerInputMode.Disabled);

        // â”€â”€ Dependencies â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        private readonly IPhaseStateMachine _phaseStateMachine;

        // â”€â”€ Subscriptions â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        private IDisposable _phaseSubscription;

        // â”€â”€ Public Interface â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        /// <inheritdoc/>
        public ReadOnlyReactiveProperty<PlayerInputMode> CurrentMode => _currentMode;

        /// <inheritdoc/>
        public bool IsMovementEnabled => _currentMode.Value == PlayerInputMode.NightMovement;

        /// <inheritdoc/>
        public bool IsUIEnabled => _currentMode.Value == PlayerInputMode.DayUI;

        // â”€â”€ Constructor â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        [Inject]
        public PlayerInputHandler(IPhaseStateMachine phaseStateMachine)
        {
            _phaseStateMachine = phaseStateMachine;
        }

        // â”€â”€ IInitializable â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        /// <summary>
        /// Subscribes to phase changes and sets the initial mode from the current phase.
        /// </summary>
        public void Initialize()
        {
            // Set initial mode from current phase (handles hot-reload / late init)
            _currentMode.Value = ResolveModeForPhase(_phaseStateMachine.CurrentState);

            // Subscribe to future phase changes â€” synchronous update
            _phaseSubscription = _phaseStateMachine.CurrentPhase
                .Subscribe(phase => _currentMode.Value = ResolveModeForPhase(phase));
        }

        // â”€â”€ Private Methods â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
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

        // â”€â”€ IDisposable â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        public void Dispose()
        {
            _phaseSubscription?.Dispose();
            _currentMode.Dispose();
        }
    }
}

