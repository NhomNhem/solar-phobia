using System;
using R3;
using UnityEngine;
using SolarPhobia.Application.Repositories;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Services
{
    /// <summary>
    /// Implementation of Day Phase Mechanics Service.
    /// Enforces phase-gated swaps and shoves per ADR-0001 phase contracts.
    /// </summary>
    public class DayPhaseMechanicsService : IDayPhaseMechanicsService
    {
        // ── R3 Reactive Events ─────────────────────────────────────────
        private readonly Subject<SwapEvent> _swapSubject = new();
        private readonly Subject<ShoveEvent> _shoveSubject = new();

        // ── Dependencies ────────────────────────────────────────────────
        private readonly ISoulRepository _soulRepository;
        private readonly IAnimationService _animationService;
        private readonly IAudioService _audioService;

        // ── State ─────────────────────────────────────────────────────
        private string _sacrificedGhostId;
        private bool _isSwapInProgress;

        // ── Constants ────────────────────────────────────────────────
        private const float SwapAnimationDuration = 0.5f;
        private const float ShoveAnimationDuration = 1.0f;

        public Observable<SwapEvent> OnSwapInitiated => _swapSubject;
        public Observable<ShoveEvent> OnShoveExecuted => _shoveSubject;

        /// <summary>
        /// Initializes service with dependencies.
        /// </summary>
        public DayPhaseMechanicsService(ISoulRepository soulRepository,
                                      IAnimationService animationService,
                                      IAudioService audioService)
        {
            _soulRepository = soulRepository ?? throw new ArgumentNullException(nameof(soulRepository));
            _animationService = animationService ?? throw new ArgumentNullException(nameof(animationService));
            _audioService = audioService ?? throw new ArgumentNullException(nameof(audioService));
        }

        /// <summary>
        /// Attempts to swap player position with a soul at shadow edge.
        /// Only allowed during DayService phase (per ADR-0001 phase contracts).
        /// </summary>
        public bool TrySwap(string playerId, string soulId, PhaseState currentPhase)
        {
            if (currentPhase != PhaseState.DayService)
            {
                Debug.LogWarning($"DayPhaseMechanics: Swap not allowed in {currentPhase}. Only DayService permitted.");
                return false;
            }

            if (string.IsNullOrEmpty(soulId) || string.IsNullOrEmpty(playerId))
            {
                Debug.LogError("DayPhaseMechanics: Player ID and Soul ID must be non-empty.");
                return false;
            }

            if (!_soulRepository.IsAtShadowEdge(soulId))
            {
                Debug.LogWarning($"DayPhaseMechanics: Soul {soulId} is not at shadow edge.");
                return false;
            }

            if (_isSwapInProgress)
            {
                Debug.LogWarning("DayPhaseMechanics: Swap already in progress.");
                return false;
            }

            _isSwapInProgress = true;
            _soulRepository.SwapPositions(playerId, soulId);

            _animationService?.PlaySwapAnimation(playerId, soulId, SwapAnimationDuration);
            _audioService?.PlaySwapSound();

            var swapEvent = new SwapEvent(playerId, soulId);
            _swapSubject.OnNext(swapEvent);

            Debug.Log($"DayPhaseMechanics: Swap initiated between {playerId} and {soulId}. Animation: {SwapAnimationDuration}s");

            SimulateAnimationCompletion();

            return true;
        }

        /// <summary>
        /// Forces a soul to be shoved into sunlight (abandonment).
        /// Called at Collapse phase end or when timer expires.
        /// Writes sacrificied_ghost_id to SoulRepository before ChoiceLock transition.
        /// </summary>
        public bool TryShove(string soulId, PhaseState currentPhase)
        {
            if (currentPhase != PhaseState.DayService && currentPhase != PhaseState.ChoiceLock)
            {
                Debug.LogWarning($"DayPhaseMechanics: Shove not allowed in {currentPhase}. DayService/ChoiceLock required.");
                return false;
            }

            if (string.IsNullOrEmpty(soulId))
            {
                Debug.LogError("DayPhaseMechanics: Soul ID must be non-empty for shove.");
                return false;
            }

            var soul = _soulRepository.GetSoul(soulId);
            if (soul == null)
            {
                Debug.LogError($"DayPhaseMechanics: Soul {soulId} not found.");
                return false;
            }

            _soulRepository.MarkAbandoned(soulId);

            _sacrificedGhostId = soulId;
            _soulRepository.SetSacrificedGhostId(soulId);

            _animationService?.PlayShoveAnimation(null, soulId);
            _audioService?.PlayShoveImpact();
            _audioService?.PlaySoulBurn();

            var shoveEvent = new ShoveEvent(soulId, _sacrificedGhostId);
            _shoveSubject.OnNext(shoveEvent);

            Debug.Log($"DayPhaseMechanics: Soul {soulId} shoved into sunlight. SacrificedGhostId set: {_sacrificedGhostId}. Animation: {ShoveAnimationDuration}s");

            return true;
        }

        /// <summary>
        /// Returns the soul ID marked for sacrifice (sacrificied_ghost_id).
        /// This value persists to NightSurvival phase for karma hazards.
        /// </summary>
        public string GetSacrificedGhostId()
        {
            return _sacrificedGhostId;
        }

        /// <summary>
        /// Resets service state (called on PhaseState.Reset).
        /// Clears sacrificied_ghost_id and swap state.
        /// </summary>
        public void Reset()
        {
            _sacrificedGhostId = null;
            _isSwapInProgress = false;
            Debug.Log("DayPhaseMechanics: Service reset.");
        }

        // ── Private Methods ─────────────────────────────────────────

        /// <summary>
        /// Simulates animation completion (0.5s swap animation).
        /// In real implementation, this would be driven by Unity's Update loop or DOTween.
        /// </summary>
        private void SimulateAnimationCompletion()
        {
            _isSwapInProgress = false;
        }
    }
}