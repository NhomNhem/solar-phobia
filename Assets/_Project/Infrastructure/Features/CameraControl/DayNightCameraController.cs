using System;
using DG.Tweening;
using NhemDangFugBixs.NhemLogging;
using R3;
using SolarPhobia.Application.Messages;
using SolarPhobia.Application.Features.Phase.Flow;
using SolarPhobia.Application.Features.Resources;
using SolarPhobia.Application.Features.Phase;
using SolarPhobia.Domain;
using SolarPhobia.Domain.Events;
using SolarPhobia.Domain.ValueObjects;
using SolarPhobia.Shared.InputActions;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using VContainer;
using VContainer.Unity;
using Camera = UnityEngine.Camera;

namespace SolarPhobia.Infrastructure.Features.CameraControl
{
    public class DayNightCameraController : IDayNightCameraController, IInitializable, ITickable, IDisposable
    {
        // ── Constants ──────────────────────────────────────────────
        public const float MaxMouseYAngle = 30f;
        public const float MinDayDistance = 3f;
        public const float MaxDayDistance = 8f;
        public const float MinNightDistance = 8f;
        public const float MaxNightDistance = 15f;
        public const float MinTransitionDuration = 0.3f;
        public const float MaxTransitionDuration = 1.0f;
        public const float MinVignetteAlpha = 0.3f;
        public const float MaxVignetteAlpha = 0.8f;
        public const float MinFollowSmooth = 0.1f;
        public const float MaxFollowSmooth = 0.5f;

        public static event Action<float> OnNightTransitionComplete;
        public static event Action<float> OnDayTransitionComplete;

        private readonly INhemLogger _logger;
        private readonly IPhaseStateMachine _phaseStateMachine;
        private readonly IObjectResolver _resolver;
        private readonly SolarPhobiaInputActions _inputActions;

        private Camera _camera;
        private Transform _cameraTransform;
        private Vector3 _dayPosition;
        private Vector3 _nightOffset;
        private float _currentYaw;
        private float _currentPitch;
        private float _targetPitch;

        private Vignette _vignette;
        private ColorAdjustments _colorAdjustments;

        private Tweener _transitionTweener;
        private Tweener _vignetteTweener;
        private Sequence _nightFailedSequence;

        private float _dayCameraDistance = 5f;
        private float _nightCameraDistance = 12f;
        private float _transitionDurationDayToNight = 0.8f;
        private float _transitionDurationNightToDay = 0.5f;
        private float _vignetteMaxAlpha = 0.6f;
        private float _cameraFollowSmooth = 0.3f;

        private bool _isNight;
        private bool _isInChoiceLock;
        private bool _isDisposed;

        public Camera TestCamera { private get; set; }

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

        public bool IsNight => _isNight;
        public bool IsInChoiceLock => _isInChoiceLock;

        public ISensoryTierService SensoryTierService { private get; set; }

        public DayNightCameraController(
            INhemLogger logger,
            IPhaseStateMachine phaseState,
            IObjectResolver resolver,
            SolarPhobiaInputActions inputActions)
        {
            _logger = logger;
            _phaseStateMachine = phaseState;
            _resolver = resolver;
            _inputActions = inputActions;
        }

        public DayNightCameraController(
            INhemLogger logger,
            IPhaseStateMachine phaseState,
            IObjectResolver resolver)
            : this(logger, phaseState, resolver, new SolarPhobiaInputActions())
        {
        }

        public void Initialize()
        {
            _camera = Camera.main;
            if (_camera == null)
            {
                _logger.LogError("[DayNightCameraController] Camera.main not found");
                return;
            }

            _cameraTransform = _camera.transform;
            _dayPosition = _cameraTransform.position;
            _nightOffset = new Vector3(0, 5, -10);

            var volume = _camera.GetComponent<Volume>();
            if (volume != null && volume.profile != null)
            {
                volume.profile.TryGet(out _vignette);
                volume.profile.TryGet(out _colorAdjustments);
            }

            _phaseStateMachine.CurrentPhase.Subscribe(OnPhaseChanged);
        }

        public void Tick()
        {
            if (_camera == null || _isDisposed) return;

            if (_isNight && !_isInChoiceLock)
            {
                float mouseY = _inputActions.Player.Look.ReadValue<Vector2>().y;
                ApplyMouseLook(mouseY);

                FollowPlayerSmooth();
            }
        }

        public void ApplyMouseLook(float mouseDeltaY)
        {
            _targetPitch = Mathf.Clamp(
                _targetPitch - mouseDeltaY * _cameraFollowSmooth,
                -MaxMouseYAngle,
                MaxMouseYAngle
            );
        }

        public void HandleNightFailed(NightFailedEvent evt)
        {
            PlayLoseCinematic();
        }

        private void OnPhaseChanged(PhaseState phase)
        {
            if (_isDisposed) return;

            switch (phase)
            {
                case PhaseState.DayService:
                case PhaseState.Dialogue:
                case PhaseState.Order:
                    TransitionToDay(phase);
                    break;

                case PhaseState.NightTravel:
                case PhaseState.NightSurvival:
                    TransitionToNight(phase);
                    break;

                case PhaseState.SunsetWarning:
                    PrepareForNight();
                    break;

                case PhaseState.ShrineArrival:
                    OnShrineArrival();
                    break;

                case PhaseState.EndingEvaluation:
                    OnEndingEvaluation();
                    break;

                case PhaseState.ChoiceLock:
                    _isInChoiceLock = true;
                    break;
            }
        }

        private void TransitionToDay(PhaseState phase)
        {
            if (_cameraTransform == null) return;
            _isNight = false;
            _isInChoiceLock = false;
            _targetPitch = 0;

            _transitionTweener?.Kill();
            _transitionTweener = _cameraTransform.DOMove(_dayPosition, _transitionDurationNightToDay)
                .SetEase(Ease.OutCubic);

            SetVignetteIntensity(0f, _transitionDurationNightToDay);

            OnDayTransitionComplete?.Invoke(_transitionDurationNightToDay);
        }

        private void TransitionToNight(PhaseState phase)
        {
            if (_cameraTransform == null) return;
            _isNight = true;

            Vector3 targetPos = _dayPosition + _nightOffset;
            _transitionTweener?.Kill();
            _transitionTweener = _cameraTransform.DOMove(targetPos, _transitionDurationDayToNight)
                .SetEase(Ease.InCubic);

            SetVignetteIntensity(_vignetteMaxAlpha, _transitionDurationDayToNight);

            OnNightTransitionComplete?.Invoke(_transitionDurationDayToNight);
        }

        private void PrepareForNight()
        {
        }

        private void OnShrineArrival()
        {
        }

        private void OnEndingEvaluation()
        {
        }

        private void FollowPlayerSmooth()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return;

            Vector3 targetPos = new Vector3(
                player.transform.position.x,
                player.transform.position.y + _nightOffset.y,
                player.transform.position.z + _nightOffset.z
            );

            _cameraTransform.position = Vector3.Lerp(
                _cameraTransform.position,
                targetPos,
                _cameraFollowSmooth * Time.deltaTime
            );
        }

        private void SetVignetteIntensity(float targetAlpha, float duration)
        {
            if (_vignette == null) return;

            _vignetteTweener?.Kill();
            _vignetteTweener = DOTween.To(
                () => _vignette.intensity.value,
                x => _vignette.intensity.value = x,
                targetAlpha,
                duration
            );
        }

        private void PlayLoseCinematic()
        {
            if (_cameraTransform == null) return;

            _nightFailedSequence?.Kill();
            _nightFailedSequence = DOTween.Sequence();

            _nightFailedSequence.Append(
                DOTween.To(() => _vignette.intensity.value, x => _vignette.intensity.value = x, 0.9f, 0.3f)
            );

            _nightFailedSequence.Join(
                _cameraTransform.DOShakePosition(0.5f, 0.5f)
            );

            _nightFailedSequence.Append(
                DOTween.To(() => _vignette.intensity.value, x => _vignette.intensity.value = x, 1f, 0.5f)
            );
        }

        public void Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;

            _transitionTweener?.Kill();
            _vignetteTweener?.Kill();
            _nightFailedSequence?.Kill();
        }
    }
}


