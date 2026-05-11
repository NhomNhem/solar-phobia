using R3;
using SolarPhobia.Application.Services.Interfaces;
using SolarPhobia.Domain.ValueObjects;
using VContainer;

namespace SolarPhobia.Application.Services.Movement
{
    public class SprintController : ISprintController
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
        [Inject]
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
    }
}
