// Assets/_Project/Application/Services/StrikeController.cs
using R3;
using SolarPhobia.Domain.ValueObjects;
using UnityEngine;
using VContainer;

namespace SolarPhobia.Application.Services
{
    /// <summary>
    /// Manages the boss searchlight strike telegraph and Ward penalty.
    /// Implements TR-map-004: Strike Telegraph + Penalty.
    ///
    /// State machine:
    ///   Idle → (exposed) → Telegraphing → (still exposed at end) → Strike fires → Idle
    ///                                   → (took cover before end) → Cancelled → Idle
    ///
    /// Strike is suppressed in shrine safe zone and outside NightMovement mode.
    /// </summary>
    public class StrikeController : IStrikeController
    {
        // ── Constants ─────────────────────────────────────────────
        /// <summary>Default Ward penalty per strike (seconds).</summary>
        public const float DefaultStrikeTimePenaltySec = 30f;

        /// <summary>Default telegraph duration (seconds).</summary>
        public const float DefaultStrikeTelegraphSec = 1.5f;

        /// <summary>Minimum telegraph duration.</summary>
        public const float MinTelegraphSec = 0.8f;

        /// <summary>Maximum telegraph duration.</summary>
        public const float MaxTelegraphSec = 2.5f;

        // ── R3 Reactive State ──────────────────────────────────────
        private readonly Subject<bool>  _onStrikeWarning     = new();
        private readonly Subject<float> _onWardCostIncurred  = new();

        // ── State ─────────────────────────────────────────────────
        private float _strikeTimePenalty  = DefaultStrikeTimePenaltySec;
        private float _telegraphDuration  = DefaultStrikeTelegraphSec;
        private float _telegraphRemaining;
        private bool  _isTelegraphActive;

        // ── Public Interface ───────────────────────────────────────
        /// <inheritdoc/>
        public bool  IsTelegraphActive   => _isTelegraphActive;

        /// <inheritdoc/>
        public float TelegraphRemaining  => _telegraphRemaining;

        /// <inheritdoc/>
        public Observable<bool>  OnStrikeWarning    => _onStrikeWarning;

        /// <inheritdoc/>
        public Observable<float> OnWardCostIncurred => _onWardCostIncurred;

        /// <inheritdoc/>
        public float StrikeTimePenaltySec
        {
            get => _strikeTimePenalty;
            set => _strikeTimePenalty = Mathf.Max(0f, value);
        }

        /// <inheritdoc/>
        public float StrikeTelegraphSec
        {
            get => _telegraphDuration;
            set => _telegraphDuration = Mathf.Clamp(value, MinTelegraphSec, MaxTelegraphSec);
        }

        // ── Constructor ────────────────────────────────────────────
        [Inject]
        public StrikeController() { }

        // ── IStrikeController ──────────────────────────────────────
        /// <inheritdoc/>
        public void Tick(bool isExposed, bool inShrineZone, PlayerInputMode mode, float deltaTime)
        {
            // Strike suppressed outside NightMovement or in shrine safe zone
            if (mode != PlayerInputMode.NightMovement || inShrineZone)
            {
                CancelTelegraph();
                return;
            }

            if (!_isTelegraphActive)
            {
                // Start telegraph when player becomes exposed
                if (isExposed)
                {
                    _isTelegraphActive  = true;
                    _telegraphRemaining = _telegraphDuration;
                    _onStrikeWarning.OnNext(true);
                }
            }
            else
            {
                if (!isExposed)
                {
                    // Player took cover — cancel telegraph
                    CancelTelegraph();
                }
                else
                {
                    // Still exposed — count down
                    _telegraphRemaining -= deltaTime;

                    if (_telegraphRemaining <= 0f)
                    {
                        // Telegraph expired while exposed — strike fires
                        _telegraphRemaining = 0f;
                        _isTelegraphActive  = false;
                        _onStrikeWarning.OnNext(false);
                        _onWardCostIncurred.OnNext(_strikeTimePenalty);
                    }
                }
            }
        }

        // ── Private ────────────────────────────────────────────────
        private void CancelTelegraph()
        {
            if (_isTelegraphActive)
            {
                _isTelegraphActive  = false;
                _telegraphRemaining = 0f;
                _onStrikeWarning.OnNext(false);
            }
        }
    }
}
