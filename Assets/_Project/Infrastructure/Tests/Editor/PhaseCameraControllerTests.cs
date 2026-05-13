using System;
using NUnit.Framework;
using R3;
using SolarPhobia.Application.Messages;
using SolarPhobia.Application.Features.Phase.Flow;
using SolarPhobia.Application.Features.Resources;
using SolarPhobia.Application.Features.Strike;
using SolarPhobia.Application.Features.Consequences.WaterTrap;
using SolarPhobia.Application.Features.Rituals;
using SolarPhobia.Application.Features.Shrines;
using SolarPhobia.Application.Features.Day;
using SolarPhobia.Application.Features.Player.State;
using SolarPhobia.Application.Features.Player.Input;
using SolarPhobia.Application.Features.Player.Interactions;
using SolarPhobia.Application.Features.Player.Cursor;
using SolarPhobia.Application.Features.Player.Events;
using SolarPhobia.Application.Features.Combat;
using SolarPhobia.Application.Features.Phase.Reset;
using SolarPhobia.Domain.ValueObjects;
using SolarPhobia.Infrastructure.Features.CameraControl;
using Unity.Cinemachine;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SolarPhobia.Infrastructure.Tests
{
    public class PhaseCameraControllerTests
    {
        private GameObject _go;
        private PhaseCameraController _controller;
        private TestPhaseStateMachine _phaseStateMachine;
        private CinemachineCamera _dayCam;
        private CinemachineCamera _nightCam;

        [SetUp]
        public void SetUp()
        {
            _go = new GameObject("TestController");
            _controller = _go.AddComponent<PhaseCameraController>();
            
            _dayCam = new GameObject("DayCam").AddComponent<CinemachineCamera>();
            _nightCam = new GameObject("NightCam").AddComponent<CinemachineCamera>();

            // Use reflection to set private serializable fields for the test
            var type = typeof(PhaseCameraController);
            var dayCamField = type.GetField("_dayFixedCamera", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var nightCamField = type.GetField("_nightFollowCamera", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            dayCamField.SetValue(_controller, _dayCam);
            nightCamField.SetValue(_controller, _nightCam);

            _phaseStateMachine = new TestPhaseStateMachine();
            
            // Use reflection to call internal Construct method
            var constructMethod = type.GetMethod("Construct", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            constructMethod.Invoke(_controller, new object[] { _phaseStateMachine });
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_go);
            Object.DestroyImmediate(_dayCam.gameObject);
            Object.DestroyImmediate(_nightCam.gameObject);
            _phaseStateMachine.Dispose();
        }

        [Test]
        public void InitialState_SetsDayCameraActive()
        {
            // Trigger Start manually
            var startMethod = typeof(PhaseCameraController).GetMethod("Start", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            startMethod.Invoke(_controller, null);

            Assert.AreEqual(10, _dayCam.Priority.Value);
            Assert.AreEqual(0, _nightCam.Priority.Value);
        }

        [Test]
        public void TransitionToNight_SwitchesPriorities()
        {
            var startMethod = typeof(PhaseCameraController).GetMethod("Start", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            startMethod.Invoke(_controller, null);

            _phaseStateMachine.TriggerPhaseChange(PhaseState.DayService, PhaseState.NightSurvival);

            Assert.AreEqual(0, _dayCam.Priority.Value);
            Assert.AreEqual(10, _nightCam.Priority.Value);
        }

        [Test]
        public void TransitionBackToDay_SwitchesPriorities()
        {
            var startMethod = typeof(PhaseCameraController).GetMethod("Start", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            startMethod.Invoke(_controller, null);

            _phaseStateMachine.TriggerPhaseChange(PhaseState.DayService, PhaseState.NightSurvival);
            _phaseStateMachine.TriggerPhaseChange(PhaseState.NightSurvival, PhaseState.DayService);

            Assert.AreEqual(10, _dayCam.Priority.Value);
            Assert.AreEqual(0, _nightCam.Priority.Value);
        }

        private class TestPhaseStateMachine : IPhaseStateMachine, IDisposable
        {
            public PhaseState CurrentState { get; set; } = PhaseState.DayService;
            public ReadOnlyReactiveProperty<PhaseState> CurrentPhase => throw new NotImplementedException();

            private readonly Subject<PhaseChangedEvent> _phaseChangedSubject = new();
            public Observable<PhaseChangedEvent> OnPhaseChanged => _phaseChangedSubject;

            public Observable<DayStartEvent> OnDayStart => throw new NotImplementedException();
            public Observable<NightStartEvent> OnNightStart => throw new NotImplementedException();
            public Observable<ResolveEvent> OnResolve => throw new NotImplementedException();

            public void TriggerPhaseChange(PhaseState oldPhase, PhaseState newPhase)
            {
                CurrentState = newPhase;
                _phaseChangedSubject.OnNext(new PhaseChangedEvent(oldPhase, newPhase));
            }

            public bool IsActionAllowed(GameAction action) => true;
            public void Initialize() { }
            public bool TryTransition(PhaseState newPhase) => true;

            public void Dispose()
            {
                _phaseChangedSubject.Dispose();
            }
        }
    }
}





