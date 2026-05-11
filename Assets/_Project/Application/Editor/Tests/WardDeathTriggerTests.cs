using NUnit.Framework;
using R3;
using SolarPhobia.Application.Messages;
using SolarPhobia.Application.Services;
using SolarPhobia.Application.Services.Objective;
using SolarPhobia.Domain.ValueObjects;
using System;

namespace SolarPhobia.Application.Editor.Tests
{
    public class WardDeathTriggerTests
    {
        private WardDeathTriggerService _service;
        private TestWardTimer _wardTimer;
        private TestPhaseStateMachine _phaseMachine;

        [SetUp]
        public void SetUp()
        {
            _wardTimer = new TestWardTimer();
            _phaseMachine = new TestPhaseStateMachine(PhaseState.NightSurvival);
            _service = new WardDeathTriggerService(_wardTimer, _phaseMachine);
        }

        [TearDown]
        public void TearDown()
        {
            _service?.Dispose();
            _phaseMachine?.Dispose();
        }

        [Test]
        public void AC1_DepletedDuringNightSurvival_TransitionsToEndingEvaluation()
        {
            _wardTimer.FireDepleted();

            Assert.That(_phaseMachine.CurrentState, Is.EqualTo(PhaseState.EndingEvaluation));
            Assert.That(_service.HasTriggeredDeath, Is.True);
        }

        [Test]
        public void AC1_Depleted_TryTransitionCalledWithEndingEvaluation()
        {
            _wardTimer.FireDepleted();

            Assert.That(_phaseMachine.LastTransitionTarget, Is.EqualTo(PhaseState.EndingEvaluation));
        }

        [Test]
        public void AC2_DoesNotDoubleTrigger()
        {
            _wardTimer.FireDepleted();
            _wardTimer.FireDepleted();

            Assert.That(_service.HasTriggeredDeath, Is.True);
            Assert.That(_phaseMachine.TransitionCount, Is.EqualTo(1));
        }

        [Test]
        public void AC3_AlreadyInEndingEvaluation_DoesNotTransition()
        {
            _phaseMachine.SetPhase(PhaseState.EndingEvaluation);
            _wardTimer.FireDepleted();

            Assert.That(_service.HasTriggeredDeath, Is.False);
        }

        [Test]
        public void AC4_DepletedInDayService_TransitionsToEndingEvaluation()
        {
            _phaseMachine.SetPhase(PhaseState.DayService);
            _wardTimer.FireDepleted();

            Assert.That(_service.HasTriggeredDeath, Is.True);
            Assert.That(_phaseMachine.LastTransitionTarget, Is.EqualTo(PhaseState.EndingEvaluation));
        }

        [Test]
        public void AC5_OnDepletedSubscription_IsActive()
        {
            Assert.DoesNotThrow(() => _wardTimer.FireDepleted());
        }

        [Test]
        public void EdgeCase_InitialState_HasNotTriggered()
        {
            Assert.That(_service.HasTriggeredDeath, Is.False);
        }

        [Test]
        public void EdgeCase_Dispose_CleansUpSubscription()
        {
            _service.Dispose();
            _wardTimer.FireDepleted();

            Assert.That(_service.HasTriggeredDeath, Is.False);
        }

        [Test]
        public void EdgeCase_MultipleDepletions_AfterTrigger_Ignored()
        {
            _wardTimer.FireDepleted();
            Assert.That(_service.HasTriggeredDeath, Is.True);

            _wardTimer.FireDepleted();
            _wardTimer.FireDepleted();

            Assert.That(_phaseMachine.TransitionCount, Is.EqualTo(1));
        }

        // ── Test Doubles ───────────────────────────────────────────

        private class TestWardTimer : SolarPhobia.Domain.IWardTimerService
        {
            private readonly Subject<Unit> _depletedSubject = new();

            public float CurrentWard { get; set; } = 100f;
            public ReadOnlyReactiveProperty<float> CurrentWardObservable => new ReactiveProperty<float>(CurrentWard);
            public ReadOnlyReactiveProperty<SensoryTier> CurrentTier => new ReactiveProperty<SensoryTier>(SensoryTier.Stable);
            public Observable<Unit> OnDepleted => _depletedSubject;
            public float MaxWard => 100f;

            public void Initialize(int ghostsSaved, int failedLightInterrupts, int soulPanicEvents) { }
            public void ApplyPenalty(float amount) { }
            public void SetDrainRate(float baseDrain, int boneCount, float hallucinationMultiplier) { }

            public bool TryApplyCost(float amount) => true;

            public void FireDepleted()
            {
                _depletedSubject.OnNext(Unit.Default);
            }
        }

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
                public int TransitionCount { get; private set; }

                public bool TryTransition(PhaseState newPhase)
                {
                    LastTransitionTarget = newPhase;
                    TransitionCount++;
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
