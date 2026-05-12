using System;
using NhemDangFugBixs.NhemLogging;
using R3;
using SolarPhobia.Application.Audio;
using SolarPhobia.Application.Player.Events;
using SolarPhobia.Application.Repositories;
using SolarPhobia.Domain.ValueObjects;
using VContainer;

namespace SolarPhobia.Application.Phase.Day
{
    /// <summary>
    /// Implements day-phase swap and shove interactions.
    /// </summary>
    public class DayPhaseMechanicsService : IDayPhaseMechanicsService
    {
        private readonly INhemLogger _logger;

        private readonly Subject<SwapEvent> _swapSubject = new();
        private readonly Subject<ShoveEvent> _shoveSubject = new();
        private readonly ISoulRepository _soulRepository;
        private readonly IAnimationService _animationService;
        private readonly IAudioCueService _audioService;
        private string _sacrificedGhostId;
        private bool _isSwapInProgress;

        private const float SwapAnimationDuration = 0.5f;
        private const float ShoveAnimationDuration = 1.0f;

        public Observable<SwapEvent> OnSwapInitiated => _swapSubject;
        public Observable<ShoveEvent> OnShoveExecuted => _shoveSubject;

        [Inject]
        public DayPhaseMechanicsService(
            INhemLogger logger,
            ISoulRepository soulRepository,
            IAnimationService animationService,
            IAudioCueService audioService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _soulRepository = soulRepository ?? throw new ArgumentNullException(nameof(soulRepository));
            _animationService = animationService ?? throw new ArgumentNullException(nameof(animationService));
            _audioService = audioService ?? throw new ArgumentNullException(nameof(audioService));
        }

        public DayPhaseMechanicsService(
            ISoulRepository soulRepository,
            IAnimationService animationService,
            IAudioCueService audioService)
            : this(new NhemUnityLogger(), soulRepository, animationService, audioService)
        {
        }

        public bool TrySwap(string playerId, string soulId, PhaseState currentPhase)
        {
            if (currentPhase != PhaseState.DayService)
            {
                _logger.LogWarning($"DayPhaseMechanics: Swap not allowed in {currentPhase}. Only DayService permitted.");
                return false;
            }

            if (string.IsNullOrEmpty(soulId) || string.IsNullOrEmpty(playerId))
            {
                _logger.LogError("DayPhaseMechanics: Player ID and Soul ID must be non-empty.");
                return false;
            }

            if (!_soulRepository.IsAtShadowEdge(soulId))
            {
                _logger.LogWarning($"DayPhaseMechanics: Soul {soulId} is not at shadow edge.");
                return false;
            }

            if (_isSwapInProgress)
            {
                _logger.LogWarning("DayPhaseMechanics: Swap already in progress.");
                return false;
            }

            _isSwapInProgress = true;
            _soulRepository.SwapPositions(playerId, soulId);

            _animationService?.PlaySwapAnimation(playerId, soulId, SwapAnimationDuration);
            _audioService?.PlaySwapSound();

            _swapSubject.OnNext(new SwapEvent(playerId, soulId));
            _logger.Log($"DayPhaseMechanics: Swap initiated between {playerId} and {soulId}. Animation: {SwapAnimationDuration}s");

            SimulateAnimationCompletion();
            return true;
        }

        public bool TryShove(string soulId, PhaseState currentPhase)
        {
            if (currentPhase != PhaseState.DayService && currentPhase != PhaseState.ChoiceLock)
            {
                _logger.LogWarning($"DayPhaseMechanics: Shove not allowed in {currentPhase}. DayService/ChoiceLock required.");
                return false;
            }

            if (string.IsNullOrEmpty(soulId))
            {
                _logger.LogError("DayPhaseMechanics: Soul ID must be non-empty for shove.");
                return false;
            }

            var soul = _soulRepository.GetSoul(soulId);
            if (soul == null)
            {
                _logger.LogError($"DayPhaseMechanics: Soul {soulId} not found.");
                return false;
            }

            _soulRepository.MarkAbandoned(soulId);

            _sacrificedGhostId = soulId;
            _soulRepository.SetSacrificedGhostId(soulId);

            _animationService?.PlayShoveAnimation(null, soulId);
            _audioService?.PlayShoveImpact();
            _audioService?.PlaySoulBurn();

            _shoveSubject.OnNext(new ShoveEvent(soulId, _sacrificedGhostId));
            _logger.Log($"DayPhaseMechanics: Soul {soulId} shoved into sunlight. SacrificedGhostId set: {_sacrificedGhostId}. Animation: {ShoveAnimationDuration}s");

            return true;
        }

        public string GetSacrificedGhostId()
        {
            return _sacrificedGhostId;
        }

        public void Reset()
        {
            _sacrificedGhostId = null;
            _isSwapInProgress = false;
            _logger.Log("DayPhaseMechanics: Service reset.");
        }

        private void SimulateAnimationCompletion()
        {
            _isSwapInProgress = false;
        }
    }
}
