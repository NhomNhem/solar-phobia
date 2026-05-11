using System;
using NUnit.Framework;
using R3;
using SolarPhobia.Application.Messages;
using SolarPhobia.Application.Services;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Tests
{
    [TestFixture]
    public class ShrineObjectiveTests
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
            _phaseMachine?.Dispose();
        }

        // ── AC-1: Proximity — within range ─────────────────────────

        [Test]
        public void AC1_WithinRange_NightSurvival_TransitionsToEndingEvaluation()
        {
            bool result = _service.TryTriggerShrineArrival(2.0f);

            Assert.That(result, Is.True);
            Assert.That(_phaseMachine.CurrentState, Is.EqualTo(PhaseState.EndingEvaluation));
        }

        [Test]
        public void AC1_AtExactTriggerRadius_TriggersWin()
        {
            bool result = _service.TryTriggerShrineArrival(3.0f);

            Assert.That(result, Is.True);
            Assert.That(_phaseMachine.CurrentState, Is.EqualTo(PhaseState.EndingEvaluation));
        }

        [Test]
        public void AC1_ZeroDistance_TriggersWin()
        {
            bool result = _service.TryTriggerShrineArrival(0f);

            Assert.That(result, Is.True);
        }

        // ── AC-2: Proximity — outside range ────────────────────────

        [Test]
        public void AC2_OutsideRange_Ignored()
        {
            bool result = _service.TryTriggerShrineArrival(5.0f);

            Assert.That(result, Is.False);
            Assert.That(_phaseMachine.CurrentState, Is.EqualTo(PhaseState.NightSurvival));
        }

        [Test]
        public void AC2_SlightlyAboveRadius_Ignored()
        {
            bool result = _service.TryTriggerShrineArrival(3.01f);

            Assert.That(result, Is.False);
        }

        // ── AC-3: Phase gating — only NightSurvival ────────────────

        [Test]
        public void AC3_DayService_Ignored()
        {
            _phaseMachine.SetPhase(PhaseState.DayService);

            Assert.That(_service.TryTriggerShrineArrival(2.0f), Is.False);
            Assert.That(_phaseMachine.CurrentState, Is.EqualTo(PhaseState.DayService));
        }

        [Test]
        public void AC3_NightTravel_Ignored()
        {
            _phaseMachine.SetPhase(PhaseState.NightTravel);

            Assert.That(_service.TryTriggerShrineArrival(2.0f), Is.False);
        }

        [Test]
        public void AC3_ShrineArrival_Ignored()
        {
            _phaseMachine.SetPhase(PhaseState.ShrineArrival);

            Assert.That(_service.TryTriggerShrineArrival(2.0f), Is.False);
        }

        [Test]
        public void AC3_EndingEvaluation_Ignored()
        {
            _phaseMachine.SetPhase(PhaseState.EndingEvaluation);

            Assert.That(_service.TryTriggerShrineArrival(2.0f), Is.False);
        }

        [Test]
        public void AC3_ChoiceLock_Ignored()
        {
            _phaseMachine.SetPhase(PhaseState.ChoiceLock);

            Assert.That(_service.TryTriggerShrineArrival(2.0f), Is.False);
        }

        // ── AC-4: Debounce ─────────────────────────────────────────

        [Test]
        public void AC4_RapidDuplicatePresses_OnlyFirstSucceeds()
        {
            bool first = _service.TryTriggerShrineArrival(2.0f);
            bool second = _service.TryTriggerShrineArrival(2.0f);

            Assert.That(first, Is.True);
            Assert.That(second, Is.False);
        }

        // ── AC-5: State transition verification ────────────────────

        [Test]
        public void AC5_TryTransitionCalled_WithEndingEvaluation()
        {
            _service.TryTriggerShrineArrival(2.0f);

            Assert.That(_phaseMachine.LastTransitionTarget, Is.EqualTo(PhaseState.EndingEvaluation));
        }

        // ── AC-6: Edge cases ───────────────────────────────────────

        [Test]
        public void AC6_NegativeDistance_TriggersWin()
        {
            bool result = _service.TryTriggerShrineArrival(-1.0f);

            Assert.That(result, Is.True);
        }

        [Test]
        public void AC6_AfterWin_DuplicateCallIsDebounced()
        {
            _service.TryTriggerShrineArrival(2.0f);
            bool retry = _service.TryTriggerShrineArrival(2.0f);

            Assert.That(retry, Is.False);
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
