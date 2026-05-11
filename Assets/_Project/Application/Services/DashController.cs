// Assets/_Project/Application/Services/DashController.cs
using R3;
using SolarPhobia.Domain.ValueObjects;
using UnityEngine;
using VContainer;

namespace SolarPhobia.Application.Services
{
    /// <summary>
    /// Manages Spirit Dash — the Khăn Tang burst skill.
    /// Implements Master GDD V5.0 Section 3.1: Spirit Dash costs -5.0s Ward.
    ///
    /// On successful dash:
    ///   1. Fires OnWardCostIncurred(dashWardCost) — Ward Timer deducts the cost
    ///   2. Sets IsDashing = true for one frame (MonoBehaviour applies burst velocity)
    ///   3. Starts cooldown timer
    ///
    /// Dash is blocked when:
    ///   - mode != NightMovement
    ///   - cooldown has not expired
    ///   - currentWard &lt;= dashWardCost (cannot afford)
    /// </summary>
    public class DashController : IDashController
    {
        // ── Constants ─────────────────────────────────────────────
        /// <summary>Default Ward cost per Spirit Dash (seconds).</summary>
        public const float DefaultDashWardCost = 5.0f;

        /// <summary>Default cooldown between dashes (seconds).</summary>
        public const float DefaultDashCooldown = 0.5f;

        /// <summary>Minimum configurable cooldown.</summary>
        public const float MinDashCooldown = 0.1f;

        /// <summary>Maximum configurable cooldown.</summary>
        public const float MaxDashCooldown = 2.0f;

        // ── R3 Reactive State ──────────────────────────────────────
        private readonly Subject<float> _onWardCostIncurred = new();

        // ── State ─────────────────────────────────────────────────
        private float _dashWardCost = DefaultDashWardCost;
        private float _dashCooldown = DefaultDashCooldown;
        private float _cooldownRemaining;
        private bool _isDashing;

        // ── Public Interface ───────────────────────────────────────
        /// <inheritdoc/>
        public bool IsDashing => _isDashing;

        /// <inheritdoc/>
        public bool CanDash => _cooldownRemaining <= 0f;

        /// <inheritdoc/>
        public Observable<float> OnWardCostIncurred => _onWardCostIncurred;

        /// <inheritdoc/>
        public float DashWardCost
        {
            get => _dashWardCost;
            set => _dashWardCost = Mathf.Max(0f, value);
        }

        /// <inheritdoc/>
        public float DashCooldown
        {
            get => _dashCooldown;
            set => _dashCooldown = Mathf.Clamp(value, MinDashCooldown, MaxDashCooldown);
        }

        // ── Constructor ────────────────────────────────────────────
        [Inject]
        public DashController()
        {
            _cooldownRemaining = 0f;
            _isDashing = false;
        }

        // ── IDashController ────────────────────────────────────────
        /// <inheritdoc/>
        public void TryDash(bool dashInput, PlayerInputMode mode, float currentWard)
        {
            // Reset dash flag each call — dash is a one-frame impulse
            _isDashing = false;

            if (!dashInput)
            {
                return;
            }

            // Phase gate
            if (mode != PlayerInputMode.NightMovement)
            {
                return;
            }

            // Cooldown gate
            if (_cooldownRemaining > 0f)
            {
                return;
            }

            // Ward affordability gate — need strictly more than cost
            if (currentWard <= _dashWardCost)
            {
                return;
            }

            // Dash activates
            _isDashing = true;
            _cooldownRemaining = _dashCooldown;
            _onWardCostIncurred.OnNext(_dashWardCost);
        }

        /// <inheritdoc/>
        public void Tick(float deltaTime)
        {
            if (_cooldownRemaining > 0f)
            {
                _cooldownRemaining = Mathf.Max(0f, _cooldownRemaining - deltaTime);
            }
        }
    }
}
