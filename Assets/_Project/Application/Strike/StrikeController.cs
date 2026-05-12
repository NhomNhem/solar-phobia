// Assets/_Project/Application/Services/StrikeController.cs
using R3;
using SolarPhobia.Domain.ValueObjects;
using SolarPhobia.Shared.Configuration;
using VContainer;
using System;

namespace SolarPhobia.Application.Strike
{
    /// <summary>
    /// Manages the boss searchlight strike telegraph and Ward penalty.
    /// Implements TR-map-004: Strike Telegraph + Penalty.
    ///
    /// State machine:
    ///   Idle â†’ (exposed) â†’ Telegraphing â†’ (still exposed at end) â†’ Strike fires â†’ Idle
    ///                                   â†’ (took cover before end) â†’ Cancelled â†’ Idle
    ///
    /// Strike is suppressed in shrine safe zone and outside NightMovement mode.
    /// </summary>
    public class StrikeController : IStrikeController
    {
        public static float DefaultStrikeTimePenaltySec => GameplayBalanceConfig.CreateDefault().Strike.DefaultStrikeTimePenaltySec;
        public static float DefaultStrikeTelegraphSec => GameplayBalanceConfig.CreateDefault().Strike.DefaultStrikeTelegraphSec;
        public static float MinTelegraphSec => GameplayBalanceConfig.CreateDefault().Strike.MinTelegraphSec;
        public static float MaxTelegraphSec => GameplayBalanceConfig.CreateDefault().Strike.MaxTelegraphSec;

        // â”€â”€ R3 Reactive State â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        private readonly Subject<bool>  _onStrikeWarning     = new();
        private readonly Subject<float> _onWardCostIncurred  = new();

        // â”€â”€ State â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        private float _strikeTimePenalty;
        private float _telegraphDuration;
        private float _telegraphRemaining;
        private bool  _isTelegraphActive;
        private readonly GameplayBalanceConfig _balanceConfig;

        // â”€â”€ Public Interface â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
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
            set => _strikeTimePenalty = Math.Max(0f, value);
        }

        /// <inheritdoc/>
        public float StrikeTelegraphSec
        {
            get => _telegraphDuration;
            set => _telegraphDuration = Math.Clamp(value, _balanceConfig.Strike.MinTelegraphSec, _balanceConfig.Strike.MaxTelegraphSec);
        }

        // â”€â”€ Constructor â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        public StrikeController()
            : this(GameplayBalanceConfig.CreateDefault())
        {
        }

        [Inject]
        public StrikeController(GameplayBalanceConfig balanceConfig)
        {
            _balanceConfig = balanceConfig ?? GameplayBalanceConfig.CreateDefault();
            _strikeTimePenalty = _balanceConfig.Strike.DefaultStrikeTimePenaltySec;
            _telegraphDuration = Math.Clamp(
                _balanceConfig.Strike.DefaultStrikeTelegraphSec,
                _balanceConfig.Strike.MinTelegraphSec,
                _balanceConfig.Strike.MaxTelegraphSec);
        }

        // â”€â”€ IStrikeController â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
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
                    // Player took cover â€” cancel telegraph
                    CancelTelegraph();
                }
                else
                {
                    // Still exposed â€” count down
                    _telegraphRemaining -= deltaTime;

                    if (_telegraphRemaining <= 0f)
                    {
                        // Telegraph expired while exposed â€” strike fires
                        _telegraphRemaining = 0f;
                        _isTelegraphActive  = false;
                        _onStrikeWarning.OnNext(false);
                        _onWardCostIncurred.OnNext(_strikeTimePenalty);
                    }
                }
            }
        }

        // â”€â”€ Private â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
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

