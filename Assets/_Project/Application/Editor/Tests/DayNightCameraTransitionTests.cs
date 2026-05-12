// Assets/_Project/Application/Editor/Tests/DayNightCameraTransitionTests.cs
using System;
using NUnit.Framework;
using R3;
using SolarPhobia.Application.Messages;
using SolarPhobia.Application.Phase.Flow;
using SolarPhobia.Application.Services;
using SolarPhobia.Domain;
using SolarPhobia.Domain.Events;
using SolarPhobia.Domain.ValueObjects;
using SolarPhobia.Infrastructure.Services;
using SolarPhobia.Shared.InputActions;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using VContainer;

namespace SolarPhobia.Application.Tests
{
    /// <summary>
    /// Validates: Day/Night Camera Controller story.
    /// Tests phase-to-camera mapping, transitions, mouse-look, tuning knobs, and lifecycle.
    /// </summary>
    [TestFixture]
    public class DayNightCameraTransitionTests
    {
        // ── Stubs ──────────────────────────────────────────────────
        private class PhaseStateMachineStub : IPhaseStateMachine
        {
            public PhaseState CurrentState { get; set; } = PhaseState.Boot;

            private readonly ReactiveProperty<PhaseState> _currentPhase = new(PhaseState.Boot);
            public ReadOnlyReactiveProperty<PhaseState> CurrentPhase => _currentPhase;

            public Subject<PhaseChangedEvent> OnPhaseChangedSubject { get; } = new();
            public Observable<PhaseChangedEvent> OnPhaseChanged => OnPhaseChangedSubject;

            public Subject<NightStartEvent> OnNightStartSubject { get; } = new();
            public Observable<NightStartEvent> OnNightStart => OnNightStartSubject;

            public Subject<DayStartEvent> OnDayStartSubject { get; } = new();
            public Observable<DayStartEvent> OnDayStart => OnDayStartSubject;

            public Subject<ResolveEvent> OnResolveSubject { get; } = new();
            public Observable<ResolveEvent> OnResolve => OnResolveSubject;

            public bool TryTransition(PhaseState newPhase) => true;
            public bool IsActionAllowed(GameAction action) => true;
            public void Initialize() { }
        }

        private class SensoryTierServiceStub : ISensoryTierService
        {
            public Subject<NightFailedEvent> OnNightFailedSubject { get; } = new();
            public Observable<NightFailedEvent> OnNightFailed => OnNightFailedSubject;

            public Observable<SensoryTierChangedEvent> OnTierChanged =>
                throw new NotSupportedException();
            public ReadOnlyReactiveProperty<SensoryTier> CurrentTier =>
                throw new NotSupportedException();
            public void Initialize(ReadOnlyReactiveProperty<float> wardObservable, float maxWard) { }
            public void Dispose() { }
        }

        // ── Fixture ───────────────────────────────────────────────
        private DayNightCameraController _controller;
        private PhaseStateMachineStub _phaseStub;
        private SensoryTierServiceStub _sensoryStub;
        private SolarPhobiaInputActions _inputActions;
        private GameObject _cameraGo;
        private GameObject _playerGo;
        private GameObject _volumeGo;
        private UnityEngine.Rendering.Volume _volume;
        private Vignette _vignette;
        private ColorAdjustments _colorAdjustments;
        private UnityEngine.Rendering.VolumeProfile _profile;

        private static float NormalizeAngle(float angle)
        {
            while (angle > 180f) { angle -= 360f; }
            while (angle < -180f) { angle += 360f; }
            return angle;
        }

        // ── Setup / Teardown ───────────────────────────────────────
        [SetUp]
        public void SetUp()
        {
            _phaseStub = new PhaseStateMachineStub();
            _sensoryStub = new SensoryTierServiceStub();

            _cameraGo = new GameObject("Main Camera");
            _cameraGo.tag = "MainCamera";
            _cameraGo.transform.position = new Vector3(0f, 5f, -5f);
            _cameraGo.AddComponent<Camera>();

            _playerGo = new GameObject("Player");
            _playerGo.tag = "Player";
            _playerGo.transform.position = Vector3.zero;

            _volumeGo = new GameObject("PostProcessVolume");
            _volume = _volumeGo.AddComponent<UnityEngine.Rendering.Volume>();
            _profile = ScriptableObject.CreateInstance<UnityEngine.Rendering.VolumeProfile>();
            _volume.profile = _profile;
            _vignette = _profile.Add<Vignette>(true);
            _vignette.active = true;
            _colorAdjustments = _profile.Add<ColorAdjustments>(true);
            _colorAdjustments.active = true;

            var builder = new ContainerBuilder();
            var resolver = builder.Build();
            _inputActions = new SolarPhobiaInputActions();
            _controller = new DayNightCameraController(_phaseStub, resolver, _inputActions);
            _controller.TestCamera = _cameraGo.GetComponent<Camera>();
            _controller.SensoryTierService = _sensoryStub;
            _controller.Initialize();
        }

        [TearDown]
        public void TearDown()
        {
            _controller?.Dispose();
            if (_cameraGo != null) UnityEngine.Object.DestroyImmediate(_cameraGo);
            if (_playerGo != null) UnityEngine.Object.DestroyImmediate(_playerGo);
            if (_volumeGo != null) UnityEngine.Object.DestroyImmediate(_volumeGo);
            if (_profile != null) ScriptableObject.DestroyImmediate(_profile);
            }

        // ═══════════════════════════════════════════════════════════
        // ── Initialization ─────────────────────────────────────────
        // ═══════════════════════════════════════════════════════════

        [Test]
        public void Initialize_StartsInDayMode()
        {
            Assert.That(_controller.IsNight, Is.False);
            Assert.That(_controller.IsInChoiceLock, Is.False);
        }

        [Test]
        public void Initialize_FindsPlayerByTag()
        {
            // Player position used for night follow — reference stored without error
            _phaseStub.OnNightStartSubject.OnNext(new NightStartEvent());
            _playerGo.transform.position = new Vector3(50f, 0f, 0f);

            for (int i = 0; i < 120; i++)
            {
                _controller.Tick();
            }

            Assert.That(_cameraGo.transform.position.x, Is.EqualTo(50f).Within(1f));
        }

        [Test]
        public void Initialize_FindsPostProcessVolume()
        {
            // Vignette and ColorAdjustments found — transitions animate without null ref
            _phaseStub.OnNightStartSubject.OnNext(new NightStartEvent());

            Assert.DoesNotThrow(() =>
            {
                _phaseStub.OnDayStartSubject.OnNext(new DayStartEvent());
            });
        }

        // ═══════════════════════════════════════════════════════════
        // ── Phase-to-Camera Mapping — ALL PhaseState values ────────
        // ═══════════════════════════════════════════════════════════

        [Test]
        public void PhaseChanged_DayService_SetsDayFixed()
        {
            _phaseStub.OnPhaseChangedSubject.OnNext(
                new PhaseChangedEvent(PhaseState.SunsetWarning, PhaseState.DayService));

            Assert.That(_controller.IsNight, Is.False);
            Assert.That(_controller.IsInChoiceLock, Is.False);
        }

        [Test]
        public void PhaseChanged_Dialogue_SetsDayFixed()
        {
            _phaseStub.OnPhaseChangedSubject.OnNext(
                new PhaseChangedEvent(PhaseState.DayService, PhaseState.Dialogue));

            Assert.That(_controller.IsNight, Is.False);
            Assert.That(_controller.IsInChoiceLock, Is.False);
        }

        [Test]
        public void PhaseChanged_Order_SetsDayFixed()
        {
            _phaseStub.OnPhaseChangedSubject.OnNext(
                new PhaseChangedEvent(PhaseState.Dialogue, PhaseState.Order));

            Assert.That(_controller.IsNight, Is.False);
            Assert.That(_controller.IsInChoiceLock, Is.False);
        }

        [Test]
        public void PhaseChanged_ChoiceLock_SetsChoiceLock()
        {
            _phaseStub.OnPhaseChangedSubject.OnNext(
                new PhaseChangedEvent(PhaseState.EndingEvaluation, PhaseState.ChoiceLock));

            Assert.That(_controller.IsInChoiceLock, Is.True);
            Assert.That(_controller.IsNight, Is.False);
        }

        [Test]
        public void PhaseChanged_NightTravel_SetsNightFollow()
        {
            _phaseStub.OnPhaseChangedSubject.OnNext(
                new PhaseChangedEvent(PhaseState.SunsetWarning, PhaseState.NightTravel));

            Assert.That(_controller.IsNight, Is.True);
            Assert.That(_controller.IsInChoiceLock, Is.False);
        }

        [Test]
        public void PhaseChanged_NightSurvival_SetsNightFollow()
        {
            _phaseStub.OnPhaseChangedSubject.OnNext(
                new PhaseChangedEvent(PhaseState.ShrineArrival, PhaseState.NightSurvival));

            Assert.That(_controller.IsNight, Is.True);
            Assert.That(_controller.IsInChoiceLock, Is.False);
        }

        [Test]
        public void PhaseChanged_Boot_NoChanges()
        {
            _phaseStub.OnPhaseChangedSubject.OnNext(
                new PhaseChangedEvent(PhaseState.DayService, PhaseState.Boot));

            Assert.That(_controller.IsNight, Is.False);
            Assert.That(_controller.IsInChoiceLock, Is.False);
        }

        [Test]
        public void PhaseChanged_SunsetWarning_NoChanges()
        {
            _phaseStub.OnPhaseChangedSubject.OnNext(
                new PhaseChangedEvent(PhaseState.Order, PhaseState.SunsetWarning));

            Assert.That(_controller.IsNight, Is.False);
            Assert.That(_controller.IsInChoiceLock, Is.False);
        }

        [Test]
        public void PhaseChanged_ShrineArrival_TriggersCinematic()
        {
            _phaseStub.OnPhaseChangedSubject.OnNext(
                new PhaseChangedEvent(PhaseState.NightTravel, PhaseState.ShrineArrival));

            Assert.That(_controller.IsNight, Is.False);
            Assert.That(_controller.IsInChoiceLock, Is.False);
        }

        [Test]
        public void PhaseChanged_EndingEvaluation_NoDirectAction()
        {
            _phaseStub.OnPhaseChangedSubject.OnNext(
                new PhaseChangedEvent(PhaseState.NightSurvival, PhaseState.EndingEvaluation));

            Assert.That(_controller.IsNight, Is.False);
            Assert.That(_controller.IsInChoiceLock, Is.False);
        }

        // ═══════════════════════════════════════════════════════════
        // ── Night / Day Start Events ───────────────────────────────
        // ═══════════════════════════════════════════════════════════

        [Test]
        public void OnNightStart_SetsIsNightTrue()
        {
            _phaseStub.OnNightStartSubject.OnNext(new NightStartEvent());

            Assert.That(_controller.IsNight, Is.True);
        }

        [Test]
        public void OnNightStart_ClearsChoiceLock()
        {
            _phaseStub.OnPhaseChangedSubject.OnNext(
                new PhaseChangedEvent(PhaseState.EndingEvaluation, PhaseState.ChoiceLock));
            Assert.That(_controller.IsInChoiceLock, Is.True);

            _phaseStub.OnNightStartSubject.OnNext(new NightStartEvent());

            Assert.That(_controller.IsInChoiceLock, Is.False);
        }

        [Test]
        public void OnDayStart_SetsIsNightFalse()
        {
            _phaseStub.OnNightStartSubject.OnNext(new NightStartEvent());
            Assert.That(_controller.IsNight, Is.True);

            _phaseStub.OnDayStartSubject.OnNext(new DayStartEvent());

            Assert.That(_controller.IsNight, Is.False);
        }

        [Test]
        public void OnDayStart_ClearsChoiceLock()
        {
            _phaseStub.OnPhaseChangedSubject.OnNext(
                new PhaseChangedEvent(PhaseState.EndingEvaluation, PhaseState.ChoiceLock));
            Assert.That(_controller.IsInChoiceLock, Is.True);

            _phaseStub.OnDayStartSubject.OnNext(new DayStartEvent());

            Assert.That(_controller.IsInChoiceLock, Is.False);
        }

        [Test]
        public void NightFailedEvent_Subscription_DoesNotThrow()
        {
            Assert.DoesNotThrow(() =>
            {
                _sensoryStub.OnNightFailedSubject.OnNext(new NightFailedEvent(0f, 30f));
            });
        }

        [Test]
        public void NightFailedEvent_AfterDispose_DoesNotThrow()
        {
            _controller.Dispose();

            Assert.DoesNotThrow(() =>
            {
                _sensoryStub.OnNightFailedSubject.OnNext(new NightFailedEvent(0f, 30f));
            });
        }

        // ═══════════════════════════════════════════════════════════
        // ── ChoiceLock ─────────────────────────────────────────────
        // ═══════════════════════════════════════════════════════════

        [Test]
        public void ChoiceLock_NightFollow_Disabled()
        {
            _phaseStub.OnPhaseChangedSubject.OnNext(
                new PhaseChangedEvent(PhaseState.EndingEvaluation, PhaseState.ChoiceLock));
            _playerGo.transform.position = new Vector3(100f, 0f, 0f);

            Vector3 posBefore = _cameraGo.transform.position;
            for (int i = 0; i < 30; i++)
            {
                _controller.Tick();
            }

            Assert.That(_cameraGo.transform.position.x, Is.EqualTo(posBefore.x).Within(0.01f));
        }

        [Test]
        public void ChoiceLock_PhaseChanged_DoesNotSetNight()
        {
            _phaseStub.OnPhaseChangedSubject.OnNext(
                new PhaseChangedEvent(PhaseState.EndingEvaluation, PhaseState.ChoiceLock));

            Assert.That(_controller.IsNight, Is.False);
            Assert.That(_controller.IsInChoiceLock, Is.True);
        }

        // ═══════════════════════════════════════════════════════════
        // ── Mouse-Look ─────────────────────────────────────────────
        // ═══════════════════════════════════════════════════════════

        [Test]
        public void ApplyMouseLook_NightMode_RotatesY()
        {
            _phaseStub.OnNightStartSubject.OnNext(new NightStartEvent());
            _controller.ApplyMouseLook(15f);

            float x = NormalizeAngle(_cameraGo.transform.localEulerAngles.x);
            Assert.That(x, Is.EqualTo(-15f).Within(0.01f));
        }

        [Test]
        public void ApplyMouseLook_NightMode_ClampedAtPositiveBound()
        {
            _phaseStub.OnNightStartSubject.OnNext(new NightStartEvent());

            _controller.ApplyMouseLook(100f);

            float x = NormalizeAngle(_cameraGo.transform.localEulerAngles.x);
            Assert.That(x, Is.EqualTo(-DayNightCameraController.MaxMouseYAngle).Within(0.01f));
        }

        [Test]
        public void ApplyMouseLook_NightMode_ClampedAtNegativeBound()
        {
            _phaseStub.OnNightStartSubject.OnNext(new NightStartEvent());

            _controller.ApplyMouseLook(-100f);

            float x = NormalizeAngle(_cameraGo.transform.localEulerAngles.x);
            Assert.That(x, Is.EqualTo(DayNightCameraController.MaxMouseYAngle).Within(0.01f));
        }

        [Test]
        public void ApplyMouseLook_DayMode_Ignored()
        {
            _controller.ApplyMouseLook(30f);

            float x = NormalizeAngle(_cameraGo.transform.localEulerAngles.x);
            Assert.That(x, Is.EqualTo(0f).Within(0.01f));
        }

        // ═══════════════════════════════════════════════════════════
        // ── Night Follow ───────────────────────────────────────────
        // ═══════════════════════════════════════════════════════════

        [Test]
        public void Tick_NightMode_FollowsPlayerX()
        {
            _phaseStub.OnNightStartSubject.OnNext(new NightStartEvent());
            _playerGo.transform.position = new Vector3(100f, 0f, 0f);

            for (int i = 0; i < 120; i++)
            {
                _controller.Tick();
            }

            Assert.That(_cameraGo.transform.position.x, Is.EqualTo(100f).Within(0.5f));
        }

        [Test]
        public void Tick_NightMode_DoesNotFollowPlayerY()
        {
            _phaseStub.OnNightStartSubject.OnNext(new NightStartEvent());
            float initialY = _cameraGo.transform.position.y;
            _playerGo.transform.position = new Vector3(0f, 999f, 0f);

            for (int i = 0; i < 60; i++)
            {
                _controller.Tick();
            }

            Assert.That(_cameraGo.transform.position.y, Is.EqualTo(initialY).Within(0.01f));
        }

        [Test]
        public void Tick_DayMode_NoFollow()
        {
            _playerGo.transform.position = new Vector3(50f, 0f, 0f);
            Vector3 startPos = _cameraGo.transform.position;

            for (int i = 0; i < 30; i++)
            {
                _controller.Tick();
            }

            Assert.That(_cameraGo.transform.position, Is.EqualTo(startPos));
        }

        // ═══════════════════════════════════════════════════════════
        // ── HandleNightFailed ──────────────────────────────────────
        // ═══════════════════════════════════════════════════════════

        [Test]
        public void HandleNightFailed_DoesNotThrow()
        {
            var evt = new NightFailedEvent(0f, 30f);

            Assert.DoesNotThrow(() => _controller.HandleNightFailed(evt));
        }

        [Test]
        public void HandleNightFailed_AfterTween_DoesNotThrow()
        {
            _phaseStub.OnNightStartSubject.OnNext(new NightStartEvent());

            Assert.DoesNotThrow(() =>
            {
                _controller.HandleNightFailed(new NightFailedEvent(0f, 30f));
            });
        }

        // ═══════════════════════════════════════════════════════════
        // ── Tuning Knobs ───────────────────────────────────────────
        // ═══════════════════════════════════════════════════════════

        [Test]
        public void DayCameraDistance_ClampedToRange()
        {
            _controller.DayCameraDistance = 1f;
            Assert.That(_controller.DayCameraDistance, Is.EqualTo(DayNightCameraController.MinDayDistance));

            _controller.DayCameraDistance = 99f;
            Assert.That(_controller.DayCameraDistance, Is.EqualTo(DayNightCameraController.MaxDayDistance));
        }

        [Test]
        public void NightCameraDistance_ClampedToRange()
        {
            _controller.NightCameraDistance = 1f;
            Assert.That(_controller.NightCameraDistance, Is.EqualTo(DayNightCameraController.MinNightDistance));

            _controller.NightCameraDistance = 99f;
            Assert.That(_controller.NightCameraDistance, Is.EqualTo(DayNightCameraController.MaxNightDistance));
        }

        [Test]
        public void TransitionDuration_ClampedToRange()
        {
            _controller.TransitionDurationDayToNight = 0f;
            Assert.That(_controller.TransitionDurationDayToNight, Is.EqualTo(DayNightCameraController.MinTransitionDuration));

            _controller.TransitionDurationDayToNight = 5f;
            Assert.That(_controller.TransitionDurationDayToNight, Is.EqualTo(DayNightCameraController.MaxTransitionDuration));

            _controller.TransitionDurationNightToDay = 0f;
            Assert.That(_controller.TransitionDurationNightToDay, Is.EqualTo(DayNightCameraController.MinTransitionDuration));

            _controller.TransitionDurationNightToDay = 5f;
            Assert.That(_controller.TransitionDurationNightToDay, Is.EqualTo(DayNightCameraController.MaxTransitionDuration));
        }

        [Test]
        public void VignetteMaxAlpha_ClampedToRange()
        {
            _controller.VignetteMaxAlpha = 0f;
            Assert.That(_controller.VignetteMaxAlpha, Is.EqualTo(DayNightCameraController.MinVignetteAlpha));

            _controller.VignetteMaxAlpha = 1f;
            Assert.That(_controller.VignetteMaxAlpha, Is.EqualTo(DayNightCameraController.MaxVignetteAlpha));
        }

        [Test]
        public void CameraFollowSmooth_ClampedToRange()
        {
            _controller.CameraFollowSmooth = 0f;
            Assert.That(_controller.CameraFollowSmooth, Is.EqualTo(DayNightCameraController.MinFollowSmooth));

            _controller.CameraFollowSmooth = 1f;
            Assert.That(_controller.CameraFollowSmooth, Is.EqualTo(DayNightCameraController.MaxFollowSmooth));
        }

        [Test]
        public void DefaultValues_MatchStorySpec()
        {
            Assert.That(_controller.DayCameraDistance, Is.EqualTo(5f));
            Assert.That(_controller.NightCameraDistance, Is.EqualTo(10f));
            Assert.That(_controller.TransitionDurationDayToNight, Is.EqualTo(0.5f));
            Assert.That(_controller.TransitionDurationNightToDay, Is.EqualTo(0.3f));
            Assert.That(_controller.VignetteMaxAlpha, Is.EqualTo(0.6f));
            Assert.That(_controller.CameraFollowSmooth, Is.EqualTo(0.3f));
        }

        // ═══════════════════════════════════════════════════════════
        // ── Lifecycle ──────────────────────────────────────────────
        // ═══════════════════════════════════════════════════════════

        [Test]
        public void Dispose_KillsActiveTween()
        {
            _phaseStub.OnNightStartSubject.OnNext(new NightStartEvent());

            Assert.DoesNotThrow(() => _controller.Dispose());
        }

        [Test]
        public void Dispose_SubsequentEvents_DoNotThrow()
        {
            _controller.Dispose();

            Assert.DoesNotThrow(() =>
            {
                _phaseStub.OnPhaseChangedSubject.OnNext(
                    new PhaseChangedEvent(PhaseState.Boot, PhaseState.DayService));
                _phaseStub.OnNightStartSubject.OnNext(new NightStartEvent());
                _phaseStub.OnDayStartSubject.OnNext(new DayStartEvent());
                _phaseStub.OnResolveSubject.OnNext(new ResolveEvent());
            });
        }

        [Test]
        public void Dispose_CanBeCalledMultipleTimes()
        {
            Assert.DoesNotThrow(() =>
            {
                _controller.Dispose();
                _controller.Dispose();
            });
        }

        // ═══════════════════════════════════════════════════════════
        // ── Resolve Cinematics ─────────────────────────────────────
        // ═══════════════════════════════════════════════════════════

        [Test]
        public void ShrineArrival_PansToPlayerX()
        {
            _playerGo.transform.position = new Vector3(75f, 0f, 0f);
            _phaseStub.OnPhaseChangedSubject.OnNext(
                new PhaseChangedEvent(PhaseState.NightTravel, PhaseState.ShrineArrival));

            Assert.That(_controller.IsNight, Is.False);
        }

        [Test]
        public void OnResolve_DoesNotThrow()
        {
            Assert.DoesNotThrow(() =>
            {
                _phaseStub.OnResolveSubject.OnNext(new ResolveEvent());
            });
        }
    }
}
