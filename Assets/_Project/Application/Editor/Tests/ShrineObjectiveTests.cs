using NUnit.Framework;
using R3;
using SolarPhobia.Application.Messages;
using SolarPhobia.Application.Features.Phase.Flow;
using SolarPhobia.Application.Features.Phase.Reset;
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
using SolarPhobia.Domain.ValueObjects;
using System;

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
            _service?.Dispose();
            _phaseMachine?.Dispose();
        }

        // ── AC-1: Within range during NightSurvival ────────────────

        [Test]
        public void AC1_WithinRange_NightSurvival_TransitionsToShrineArrival()
        {
            bool result = _service.TryTriggerShrineArrival(2.0f);

            Assert.That(result, Is.True);
            Assert.That(_phaseMachine.CurrentState, Is.EqualTo(PhaseState.ShrineArrival));
        }

        [Test]
        public void AC1_AtExactTriggerRadius_TriggersWin()
        {
            bool result = _service.TryTriggerShrineArrival(3.0f);

            Assert.That(result, Is.True);
            Assert.That(_phaseMachine.CurrentState, Is.EqualTo(PhaseState.ShrineArrival));
        }

        [Test]
        public void AC1_ZeroDistance_TriggersWin()
        {
            bool result = _service.TryTriggerShrineArrival(0f);

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

        // ── AC-2: Outside range ────────────────────────────────────

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

        // ── AC-4: One-shot — fires only once per run ───────────────

        [Test]
        public void AC4_AfterFirstFire_SubsequentCallsFail()
        {
            Assert.That(_service.TryTriggerShrineArrival(2.0f), Is.True);
            Assert.That(_service.TryTriggerShrineArrival(2.0f), Is.False);
            Assert.That(_service.TryTriggerShrineArrival(1.0f), Is.False);
        }

        [Test]
        public void AC4_DebounceWindow_Ignored()
        {
            bool first = _service.TryTriggerShrineArrival(2.0f);
            bool second = _service.TryTriggerShrineArrival(2.0f);

            Assert.That(first, Is.True);
            Assert.That(second, Is.False);
        }

        // ── AC-5: Transition to ShrineArrival phase ────────────────

        [Test]
        public void AC5_TryTransitionCalled_WithShrineArrival()
        {
            _service.TryTriggerShrineArrival(2.0f);

            Assert.That(_phaseMachine.LastTransitionTarget, Is.EqualTo(PhaseState.ShrineArrival));
        }

        // ── Edge cases ─────────────────────────────────────────────

        [Test]
        public void EdgeCase_NegativeDistance_TriggersWin()
        {
            bool result = _service.TryTriggerShrineArrival(-1.0f);

            Assert.That(result, Is.True);
        }

        [Test]
        public void EdgeCase_Dispose_CleansUpSubscription()
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






