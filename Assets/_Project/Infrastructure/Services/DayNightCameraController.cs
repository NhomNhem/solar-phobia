using System;
using DG.Tweening;
using NhemDangFugBixs.NhemLogging;
using R3;
using SolarPhobia.Application.Messages;
using SolarPhobia.Application.Phase.Flow;
using SolarPhobia.Application.Resources;
using SolarPhobia.Application.Services;
using SolarPhobia.Domain;
using SolarPhobia.Domain.Events;
using SolarPhobia.Domain.ValueObjects;
using SolarPhobia.Shared.InputActions;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using VContainer;
using VContainer.Unity;
namespace SolarPhobia.Infrastructure.Services
{
    /// <summary>
    /// Phase-driven camera controller subscribing to <see cref="IPhaseStateMachine"/>
    /// via R3. Handles day fixed 2.5D top-down, night side-scroll follow,
    /// DOTween transitions with vignette/color grading, and resolve cinematics.
    /// Implements ADR-0010: Phase-driven camera controller.
    /// </summary>
    public class DayNightCameraController : IDayNightCameraController, IInitializable, ITickable, IDisposable
    {


        // ── Tuning Knob Ranges ──────────────────────────────────────
        public const float MinDayDistance = 3f;
        public const float MaxDayDistance = 8f;
        public const float MinNightDistance = 8f;
        public const float MaxNightDistance = 15f;
        public const float MinTransitionDuration = 0.2f;
        public const float MaxTransitionDuration = 1.0f;
        public const float MinVignetteAlpha = 0.3f;
        public const float MaxVignetteAlpha = 0.8f;
        public const float MinFollowSmooth = 0.1f;
        public const float MaxFollowSmooth = 0.5f;
        public const float MaxMouseYAngle = 30f;

        // ── Constants ───────────────────────────────────────────────
        private const float MouseSensitivity = 1.0f;
        private const string PlayerTag = "Player";

        // ── Post-Processing Defaults ─────────────────────────────────
        private const float DayColorTemperature = 30f;
        private const float NightColorTemperature = -30f;
        private const float FadeToBlackExposure = -10f;

        // ── Dependencies ───────────────────────────────────────────
        private readonly INhemLogger _logger;
        private readonly IPhaseStateMachine _phaseState;
        private readonly IObjectResolver _resolver;
        private readonly SolarPhobiaInputActions _inputActions;
        private ISensoryTierService _sensoryTierService;

        // ── Tuning Knob Backing Fields ──────────────────────────────
        private float _dayCameraDistance = 5.0f;
        private float _nightCameraDistance = 10.0f;
        private float _transitionDurationDayToNight = 0.5f;
        private float _transitionDurationNightToDay = 0.3f;
        private float _vignetteMaxAlpha = 0.6f;
        private float _cameraFollowSmooth = 0.3f;

        // ── Public Properties ──────────────────────────────────────
        public float DayCameraDistance
        {
            get => _dayCameraDistance;
            set => _dayCameraDistance = Mathf.Clamp(value, MinDayDistance, MaxDayDistance);
        }

        public float NightCameraDistance
        {
            get => _nightCameraDistance;
            set => _nightCameraDistance = Mathf.Clamp(value, MinNightDistance, MaxNightDistance);
        }

        public float TransitionDurationDayToNight
        {
            get => _transitionDurationDayToNight;
            set => _transitionDurationDayToNight = Mathf.Clamp(value, MinTransitionDuration, MaxTransitionDuration);
        }

        public float TransitionDurationNightToDay
        {
            get => _transitionDurationNightToDay;
            set => _transitionDurationNightToDay = Mathf.Clamp(value, MinTransitionDuration, MaxTransitionDuration);
        }

        public float VignetteMaxAlpha
        {
            get => _vignetteMaxAlpha;
            set => _vignetteMaxAlpha = Mathf.Clamp(value, MinVignetteAlpha, MaxVignetteAlpha);
        }

        public float CameraFollowSmooth
        {
            get => _cameraFollowSmooth;
            set => _cameraFollowSmooth = Mathf.Clamp(value, MinFollowSmooth, MaxFollowSmooth);
        }

        // ── State ──────────────────────────────────────────────────
        public bool IsNight { get; private set; }
        public bool IsInChoiceLock { get; private set; }

        /// <summary>Sensory tier service for NightFailedEvent subscription. May be null if not registered.</summary>
        public ISensoryTierService SensoryTierService
        {
            get => _sensoryTierService;
            set => _sensoryTierService = value;
        }

        /// <summary>Override camera reference for testing. When set, Initialize skips Camera.main lookup.</summary>
        public global::UnityEngine.Camera TestCamera
        {
            set
            {
                _camera = value;
                _cameraTransform = value != null ? value.transform : null;
            }
        }

        // ── Private State ──────────────────────────────────────────
        private global::UnityEngine.Camera _camera;
        private Transform _cameraTransform;
        private Transform _playerTransform;
        private Vector3 _dayPosition;
        private float _mouseYRotation;
        private readonly CompositeDisposable _subscriptions = new();

        // ── Post-Processing ─────────────────────────────────────────
        private Volume _postProcessVolume;
        private Vignette _vignette;
        private ColorAdjustments _colorAdjustments;
        private WhiteBalance _whiteBalance;

        // ── Current Tweens ──────────────────────────────────────────
        private Tween _activeTween;

        [Inject]
        public DayNightCameraController(INhemLogger logger, IPhaseStateMachine phaseState, IObjectResolver resolver, SolarPhobiaInputActions inputActions)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _phaseState = phaseState;
            _resolver = resolver;
            _inputActions = inputActions;
        }

        public DayNightCameraController(INhemLogger logger, IPhaseStateMachine phaseState, IObjectResolver resolver)
            : this(logger, phaseState, resolver, new SolarPhobiaInputActions())
        {
        }

        public void Initialize()
        {
            if (_camera == null)
            {
                _camera = global::UnityEngine.Camera.main;
            }

            if (_camera == null)
            {
                _logger.LogError("[DayNightCameraController] Camera.main not found");
                return;
            }

            if (_cameraTransform == null)
            {
                _cameraTransform = _camera.transform;
            }

            _dayPosition = _cameraTransform.position;

            var player = GameObject.FindWithTag(PlayerTag);
            if (player != null)
            {
                _playerTransform = player.transform;
            }

            if (_resolver.TryResolve<ISensoryTierService>(out var sensoryService))
            {
                _sensoryTierService = sensoryService;
            }

            _postProcessVolume = UnityEngine.Object.FindFirstObjectByType<Volume>();
            if (_postProcessVolume != null && _postProcessVolume.profile != null)
            {
                _postProcessVolume.profile.TryGet(out _vignette);
                _postProcessVolume.profile.TryGet(out _colorAdjustments);
                _postProcessVolume.profile.TryGet(out _whiteBalance);
            }

            _phaseState.OnPhaseChanged
                .Subscribe(OnPhaseChanged)
                .AddTo(_subscriptions);

            _phaseState.OnNightStart
                .Subscribe(_ => OnNightStarted())
                .AddTo(_subscriptions);

            _phaseState.OnDayStart
                .Subscribe(_ => OnDayStarted())
                .AddTo(_subscriptions);

            _phaseState.OnResolve
                .Subscribe(OnResolve)
                .AddTo(_subscriptions);

            if (_sensoryTierService != null)
            {
                _sensoryTierService.OnNightFailed
                    .Subscribe(HandleNightFailed)
                    .AddTo(_subscriptions);
            }

            ApplyPhaseState(_phaseState.CurrentState);
        }

        public void Tick()
        {
            if (!IsNight || _playerTransform == null || _cameraTransform == null)
            {
                return;
            }

            Vector3 targetPos = _cameraTransform.position;
            targetPos.x = _playerTransform.position.x;
            _cameraTransform.position = Vector3.Lerp(
                _cameraTransform.position,
                targetPos,
                _cameraFollowSmooth);

            float mouseY = _inputActions.Player.Look.ReadValue<Vector2>().y * MouseSensitivity;
            if (Mathf.Abs(mouseY) > 0.001f)
            {
                ApplyMouseLook(mouseY);
            }
        }

        public void Dispose()
        {
            _activeTween?.Kill();
            _subscriptions.Dispose();
        }

        public void ApplyMouseLook(float mouseDeltaY)
        {
            if (!IsNight || _cameraTransform == null)
            {
                return;
            }

            _mouseYRotation -= mouseDeltaY;
            _mouseYRotation = Mathf.Clamp(_mouseYRotation, -MaxMouseYAngle, MaxMouseYAngle);

            Vector3 currentEuler = _cameraTransform.localEulerAngles;
            currentEuler.x = _mouseYRotation;
            currentEuler.z = 0f;
            _cameraTransform.localEulerAngles = currentEuler;
        }

        public void HandleNightFailed(NightFailedEvent evt)
        {
            if (_cameraTransform == null)
            {
                return;
            }

            _activeTween?.Kill();

            Sequence failSeq = DOTween.Sequence();
            failSeq.Append(_cameraTransform.DOShakePosition(0.5f, 0.5f, 10));
            failSeq.Join(DOVirtual.Float(
                _vignette != null ? _vignette.intensity.value : 0f,
                _vignetteMaxAlpha,
                0.4f,
                v =>
                {
                    if (_vignette != null)
                    {
                        _vignette.intensity.value = v;
                    }
                }));
            failSeq.Join(DOVirtual.Float(
                _colorAdjustments != null ? _colorAdjustments.postExposure.value : 0f,
                FadeToBlackExposure,
                0.6f,
                v =>
                {
                    if (_colorAdjustments != null)
                    {
                        _colorAdjustments.postExposure.value = v;
                    }
                }));
            failSeq.Play();

            _activeTween = failSeq;
        }

        private void OnPhaseChanged(PhaseChangedEvent e)
        {
            ApplyPhaseState(e.NewPhase);
        }

        private void OnNightStarted()
        {
            IsNight = true;
            IsInChoiceLock = false;
            TransitionToNight();
        }

        private void OnDayStarted()
        {
            IsNight = false;
            IsInChoiceLock = false;
            TransitionToDay();
        }

        private void OnResolve(ResolveEvent e)
        {
        }

        private void ApplyPhaseState(PhaseState phase)
        {
            switch (phase)
            {
                case PhaseState.DayService:
                case PhaseState.Dialogue:
                case PhaseState.Order:
                    SetDayFixed();
                    break;

                case PhaseState.ChoiceLock:
                    SetChoiceLock();
                    break;

                case PhaseState.NightTravel:
                case PhaseState.NightSurvival:
                    SetNightFollow();
                    break;

                case PhaseState.ShrineArrival:
                    OnShrineArrival();
                    break;

                case PhaseState.EndingEvaluation:
                    break;

                case PhaseState.Boot:
                case PhaseState.SunsetWarning:
                default:
                    break;
            }
        }

        private void SetDayFixed()
        {
            IsNight = false;
            IsInChoiceLock = false;

            if (_cameraTransform == null)
            {
                return;
            }

            _activeTween?.Kill();
            _cameraTransform.DOMove(_dayPosition, _transitionDurationNightToDay);
        }

        private void SetChoiceLock()
        {
            IsInChoiceLock = true;
            IsNight = false;

            if (_cameraTransform == null)
            {
                return;
            }

            _activeTween?.Kill();
            _cameraTransform.DOMove(_dayPosition, _transitionDurationNightToDay);
        }

        private void SetNightFollow()
        {
            IsNight = true;
            IsInChoiceLock = false;
        }

        private void OnShrineArrival()
        {
            IsNight = false;
            IsInChoiceLock = false;

            if (_cameraTransform == null || _playerTransform == null)
            {
                return;
            }

            _activeTween?.Kill();

            Vector3 shrineTarget = new Vector3(
                _playerTransform.position.x,
                _dayPosition.y,
                _dayPosition.z);

            _activeTween = _cameraTransform.DOMove(shrineTarget, 2f)
                .SetEase(Ease.InOutSine);
        }

        private void TransitionToNight()
        {
            if (_cameraTransform == null)
            {
                return;
            }

            _activeTween?.Kill();

            float playerX = _playerTransform != null
                ? _playerTransform.position.x
                : _dayPosition.x;

            Vector3 nightPos = new Vector3(
                playerX,
                _dayPosition.y,
                _dayPosition.z - (_nightCameraDistance - _dayCameraDistance));

            Sequence nightSeq = DOTween.Sequence();

            nightSeq.Join(_cameraTransform.DOMove(nightPos, _transitionDurationDayToNight)
                .SetEase(Ease.OutQuad));

            if (_vignette != null)
            {
                _vignette.active = true;
                nightSeq.Join(DOTween.To(
                    () => _vignette.intensity.value,
                    v => _vignette.intensity.value = v,
                    _vignetteMaxAlpha,
                    _transitionDurationDayToNight));
            }

            if (_whiteBalance != null)
            {
                _whiteBalance.active = true;
                nightSeq.Join(DOTween.To(
                    () => _whiteBalance.temperature.value,
                    v => _whiteBalance.temperature.value = v,
                    NightColorTemperature,
                    _transitionDurationDayToNight));
            }

            nightSeq.Play();
            _activeTween = nightSeq;
        }

        private void TransitionToDay()
        {
            if (_cameraTransform == null)
            {
                return;
            }

            _activeTween?.Kill();

            Sequence daySeq = DOTween.Sequence();

            daySeq.Join(_cameraTransform.DOMove(_dayPosition, _transitionDurationNightToDay)
                .SetEase(Ease.InQuad));

            if (_vignette != null)
            {
                daySeq.Join(DOTween.To(
                    () => _vignette.intensity.value,
                    v => _vignette.intensity.value = v,
                    0f,
                    _transitionDurationNightToDay));
            }

            if (_whiteBalance != null)
            {
                daySeq.Join(DOTween.To(
                    () => _whiteBalance.temperature.value,
                    v => _whiteBalance.temperature.value = v,
                    DayColorTemperature,
                    _transitionDurationNightToDay));
            }

            daySeq.Play();
            _activeTween = daySeq;
        }
    }
}

