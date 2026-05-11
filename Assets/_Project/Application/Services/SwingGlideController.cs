// Assets/_Project/Application/Services/SwingGlideController.cs
using R3;
using SolarPhobia.Domain.ValueObjects;
using UnityEngine;
using VContainer;

namespace SolarPhobia.Application.Services
{
    /// <summary>
    /// Manages Swing and Glide skills — Khăn Tang cloth movement abilities.
    /// Implements Master GDD V5.0 Section 3.1.
    ///
    /// Swing:  Left Click near anchor → attach, costs -2.0s Ward once on attach.
    ///         ReleaseSwing() detaches.
    ///
    /// Glide:  Hold input while airborne → reduced gravity, costs -1.0s/sec Ward.
    ///         Auto-cancels when Ward reaches 0 or player lands.
    /// </summary>
    public class SwingGlideController : ISwingGlideController
    {
        // ── Constants ─────────────────────────────────────────────
        /// <summary>Default Ward cost for Swing activation (seconds).</summary>
        public const float DefaultSwingWardCost = 2.0f;

        /// <summary>Default Ward cost per second while Gliding.</summary>
        public const float DefaultGlideWardCostPerSec = 1.0f;

        // ── R3 Reactive State ──────────────────────────────────────
        private readonly Subject<float> _onWardCostIncurred = new();

        // ── State ─────────────────────────────────────────────────
        private float _swingWardCost       = DefaultSwingWardCost;
        private float _glideWardCostPerSec = DefaultGlideWardCostPerSec;
        private bool _isSwinging;
        private bool _isGliding;

        // ── Public Interface ───────────────────────────────────────
        /// <inheritdoc/>
        public bool IsSwinging => _isSwinging;

        /// <inheritdoc/>
        public bool IsGliding => _isGliding;

        /// <inheritdoc/>
        public Observable<float> OnWardCostIncurred => _onWardCostIncurred;

        /// <inheritdoc/>
        public float SwingWardCost
        {
            get => _swingWardCost;
            set => _swingWardCost = Mathf.Max(0f, value);
        }

        /// <inheritdoc/>
        public float GlideWardCostPerSec
        {
            get => _glideWardCostPerSec;
            set => _glideWardCostPerSec = Mathf.Max(0f, value);
        }

        // ── Constructor ────────────────────────────────────────────
        [Inject]
        public SwingGlideController() { }

        // ── ISwingGlideController ──────────────────────────────────
        /// <inheritdoc/>
        public void TrySwing(bool swingInput, PlayerInputMode mode, float currentWard)
        {
            if (!swingInput || mode != PlayerInputMode.NightMovement)
            {
                return;
            }

            if (_isSwinging)
            {
                return; // Already attached
            }

            if (currentWard <= _swingWardCost)
            {
                return; // Cannot afford
            }

            _isSwinging = true;
            _onWardCostIncurred.OnNext(_swingWardCost);
        }

        /// <inheritdoc/>
        public void ReleaseSwing()
        {
            _isSwinging = false;
        }

        /// <inheritdoc/>
        public void TickGlide(bool glideInput, bool isAirborne, PlayerInputMode mode, float currentWard, float deltaTime)
        {
            bool shouldGlide = glideInput
                               && isAirborne
                               && mode == PlayerInputMode.NightMovement
                               && currentWard > 0f;

            if (!shouldGlide)
            {
                _isGliding = false;
                return;
            }

            _isGliding = true;
            float cost = _glideWardCostPerSec * deltaTime;
            _onWardCostIncurred.OnNext(cost);
        }
    }
}
