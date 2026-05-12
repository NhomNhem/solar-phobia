using NUnit.Framework;
using R3;
using SolarPhobia.Application.Messages;
using SolarPhobia.Application.Services;
using SolarPhobia.Domain.ValueObjects;
using System;

namespace SolarPhobia.Application.Editor.Tests
{
    [TestFixture]
    public class ShrineArrivalWinConditionTests
    {
        private ShrineObjectiveService _service;
        private TestPhaseStateMachine _phaseMachine;

        [SetUp]
        public void SetUp()
        {
            _phaseMachine = new TestPhaseStateMachine(PhaseState.NightSurvival);
            _service = new ShrineObjectiveService(_phaseMachine);
        }

        [TearDown]
        public void TearDown()
        {
            _service?.Dispose();
            _phaseMachine?.Dispose();
        }

        // ── AC-1: Win Detection + Phase Transition ─────────────────

        [Test]
        public void AC1_WithinRange_NightSurvival_TransitionsToShrineArrival()
        {
            bool result = _service.TryTriggerShrineArrival(2.0f);

            Assert.That(result, Is.True);
            Assert.That(_phaseMachine.CurrentState, Is.EqualTo(PhaseState.ShrineArrival));
        }

        [Test]
        public void AC1_EmitsOnShrineReached()
        {
            Unit? received = null;
            using (_service.OnShrineReached.Subscribe(u => received = u))
            {
                _service.TryTriggerShrineArrival(2.0f);
            }

            Assert.That(received, Is.Not.Null);
        }

        // ── AC-3: Phase Gate ───────────────────────────────────────

        [Test]
        public void AC3_NonNightPhase_Ignored()
        {
            _phaseMachine.SetPhase(PhaseState.DayService);

            Assert.That(_service.TryTriggerShrineArrival(2.0f), Is.False);
            Assert.That(_phaseMachine.CurrentState, Is.EqualTo(PhaseState.DayService));
        }

        [Test]
        public void AC3_ShrineArrivalPhase_Ignored()
        {
            _phaseMachine.SetPhase(PhaseState.ShrineArrival);

            Assert.That(_service.TryTriggerShrineArrival(2.0f), Is.False);
        }

        // ── AC-4: Proximity Gate ───────────────────────────────────

        [Test]
        public void AC4_OutsideRange_Ignored()
        {
            bool result = _service.TryTriggerShrineArrival(5.0f);

            Assert.That(result, Is.False);
            Assert.That(_phaseMachine.CurrentState, Is.EqualTo(PhaseState.NightSurvival));
        }

        // ── AC-5: One-Shot Behavior ────────────────────────────────

        [Test]
        public void AC5_OnlyFiresOncePerRun()
        {
            Assert.That(_service.TryTriggerShrineArrival(2.0f), Is.True);
            Assert.That(_service.TryTriggerShrineArrival(2.0f), Is.False);
        }

        [Test]
        public void AC5_DuplicateAfterFire_Ignored()
        {
            _service.TryTriggerShrineArrival(2.0f);
            bool retry = _service.TryTriggerShrineArrival(1.0f);

            Assert.That(retry, Is.False);
        }

        // ── Edge Cases ─────────────────────────────────────────────

        [Test]
        public void EdgeCase_NegativeDistance_TriggersWin()
        {
            bool result = _service.TryTriggerShrineArrival(-1.0f);

            Assert.That(result, Is.True);
        }

        [Test]
        public void EdgeCase_Dispose_CleansSubscriptions()
        {
            _service.Dispose();

            Assert.DoesNotThrow(() => _service.TryTriggerShrineArrival(2.0f));
        }

        // ── Test Doubles ───────────────────────────────────────────

        private class TestPhaseStateMachine : IPhaseStateMachine, IDisposable
        {
            private readonly ReactiveProperty<PhaseState> _phase;
            private readonly Subject<PhaseChangedEvent> _phaseChangedSubject = new();
            private readonly Subject<DayStartEvent> _dayStartSubject = new();
            private readonly Subject<NightStartEvent> _nightStartSubject = new();
            private readonly Subject<ResolveEvent> _resolveSubject = new();

            public TestPhaseStateMachine(PhaseState initialPhase)
            {
                _phase = new ReactiveProperty<PhaseState>(initialPhase);
            }

            public PhaseState CurrentState => _phase.Value;
            public ReadOnlyReactiveProperty<PhaseState> CurrentPhase => _phase;
            public Observable<PhaseChangedEvent> OnPhaseChanged => _phaseChangedSubject;
            public Observable<DayStartEvent> OnDayStart => _dayStartSubject;
            public Observable<NightStartEvent> OnNightStart => _nightStartSubject;
            public Observable<ResolveEvent> OnResolve => _resolveSubject;

            public PhaseState? LastTransitionTarget { get; private set; }

            public bool TryTransition(PhaseState newPhase)
            {
                LastTransitionTarget = newPhase;
                PhaseState previousPhase = _phase.Value;
                _phase.Value = newPhase;
                _phaseChangedSubject.OnNext(new PhaseChangedEvent(previousPhase, newPhase));
                return true;
            }

            public bool IsActionAllowed(GameAction action) => true;
            public void Initialize() { }

            public void SetPhase(PhaseState phase)
            {
                PhaseState previousPhase = _phase.Value;
                _phase.Value = phase;
                _phaseChangedSubject.OnNext(new PhaseChangedEvent(previousPhase, phase));
            }

            public void Dispose()
            {
                _phase?.Dispose();
                _phaseChangedSubject?.Dispose();
                _dayStartSubject?.Dispose();
                _nightStartSubject?.Dispose();
                _resolveSubject?.Dispose();
            }
        }
    }
}
