// Assets/_Project/Application/Services/SprintController.cs
using System;
using R3;
using SolarPhobia.Domain.ValueObjects;
using VContainer;

namespace SolarPhobia.Application.Features.Player.Movement
{
    /// <summary>
    /// Manages sprint state for the player controller.
    /// Implements TR-player-003: Sprint — Shift key speed multiplier + stamina integration.
    ///
    /// Rules:
    ///   - Sprint activates when: mode == NightMovement AND sprintInput held AND stamina available
    ///   - Sprint deactivates when: input released OR mode changes OR stamina depleted
    ///   - OnSprintChanged fires only on state transitions (not every frame)
    ///   - NotifyStaminaDepleted() forces immediate sprint exit
    /// </summary>
    public class SprintController : ISprintController, IDisposable
    {
        // ── R3 Reactive State ──────────────────────────────────────
        private readonly Subject<bool> _onSprintChanged = new();

        // ── State ─────────────────────────────────────────────────
        private bool _isSprinting;
        private bool _staminaDepleted;

        // ── Public Interface ───────────────────────────────────────
        /// <inheritdoc/>
        public bool IsSprinting => _isSprinting;

        /// <inheritdoc/>
        public Observable<bool> OnSprintChanged => _onSprintChanged;

        // ── Constructor ────────────────────────────────────────────
        public SprintController()
        {
            _isSprinting = false;
            _staminaDepleted = false;
        }

        // ── ISprintController ──────────────────────────────────────
        /// <inheritdoc/>
        public void Tick(bool sprintInputHeld, PlayerInputMode mode)
        {
            // Sprint only valid in NightMovement mode
            bool canSprint = mode == PlayerInputMode.NightMovement
                             && sprintInputHeld
                             && !_staminaDepleted;

            SetSprinting(canSprint);
        }

        /// <inheritdoc/>
        public void NotifyStaminaDepleted()
        {
            _staminaDepleted = true;
            SetSprinting(false);
        }

        /// <inheritdoc/>
        public void NotifyStaminaRestored()
        {
            _staminaDepleted = false;
            // Sprint does not auto-restart — player must hold Shift again
        }

        // ── Private Methods ────────────────────────────────────────
        private void SetSprinting(bool value)
        {
            if (_isSprinting == value)
            {
                return; // No state change — do not fire event
            }

            _isSprinting = value;
            _onSprintChanged.OnNext(_isSprinting);
        }

        public void Dispose()
        {
            _onSprintChanged.Dispose();
        }
    }
}
