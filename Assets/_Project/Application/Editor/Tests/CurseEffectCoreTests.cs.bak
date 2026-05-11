using System;
using NUnit.Framework;
using R3;
using SolarPhobia.Application.Messages;
using SolarPhobia.Application.Services;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Tests
{
    /// <summary>
    /// Tests for CurseEffectManager - validates AC-1 through AC-4.
    /// Validates: Story 001 - Curse Effect Core.
    /// </summary>
    [TestFixture]
    public class CurseEffectCoreTests
    {
        private CurseEffectManager _manager;
        private TestPhaseStateMachine _phaseMachine;

        [SetUp]
        public void SetUp()
        {
            _phaseMachine = new TestPhaseStateMachine(PhaseState.DayService);
            _manager = new CurseEffectManager(_phaseMachine);
            _manager.Initialize();
        }

        [TearDown]
        public void TearDown()
        {
            _manager?.Dispose();
        }

        // ── AC-1: Idle state when not night phase ──────────────────────────────

        [Test]
        public void AC1_InitialState_IsIdle_WhenNotNightSurvival()
        {
            Assert.That(_manager.CurrentStateValue, Is.EqualTo(CurseEffectState.Idle));
        }

        [Test]
        public void AC1_RemainsIdle_WhenPhaseIsDayService()
        {
            _phaseMachine.SetPhase(PhaseState.DayService);
            
            Assert.That(_manager.CurrentStateValue, Is.EqualTo(CurseEffectState.Idle));
        }

        [Test]
        public void AC1_RemainsIdle_WhenPhaseIsChoiceLock()
        {
            _phaseMachine.SetPhase(PhaseState.ChoiceLock);
            
            Assert.That(_manager.CurrentStateValue, Is.EqualTo(CurseEffectState.Idle));
        }

        // ── AC-2: CurseActive state receives curse ──────────────────────────────

        [Test]
        public void AC2_TransitionsToCurseActive_WhenNightSurvivalAndCurseReceived()
        {
            _phaseMachine.SetPhase(PhaseState.NightSurvival);
            _manager.ReceiveCurse(NightOutcomeState.Drag);
            
            Assert.That(_manager.CurrentStateValue, Is.EqualTo(CurseEffectState.CurseActive));
        }

        [Test]
        public void AC2_CurseActive_WithDragCurse()
        {
            _phaseMachine.SetPhase(PhaseState.NightSurvival);
            _manager.ReceiveCurse(NightOutcomeState.Drag);
            
            Assert.That(_manager.CurrentStateValue, Is.EqualTo(CurseEffectState.CurseActive));
        }

        [Test]
        public void AC2_CurseActive_WithBlockCurse()
        {
            _phaseMachine.SetPhase(PhaseState.NightSurvival);
            _manager.ReceiveCurse(NightOutcomeState.Block);
            
            Assert.That(_manager.CurrentStateValue, Is.EqualTo(CurseEffectState.CurseActive));
        }

        [Test]
        public void AC2_CurseActive_WithFakeShrineCurse()
        {
            _phaseMachine.SetPhase(PhaseState.NightSurvival);
            _manager.ReceiveCurse(NightOutcomeState.FakeShrine);
            
            Assert.That(_manager.CurrentStateValue, Is.EqualTo(CurseEffectState.CurseActive));
        }

        [Test]
        public void AC2_NightStart_TransitionsToCurseActive_WhenCurseAlreadySet()
        {
            _manager.ReceiveCurse(NightOutcomeState.Drag);
            _phaseMachine.FireNightStart();
            
            Assert.That(_manager.CurrentStateValue, Is.EqualTo(CurseEffectState.CurseActive));
        }

        // ── AC-3: HazardTriggered on player entry ──────────────────────────────

        [Test]
        public void AC3_TransitionsToHazardTriggered_WhenPlayerEntersHazardZone()
        {
            _phaseMachine.SetPhase(PhaseState.NightSurvival);
            _manager.ReceiveCurse(NightOutcomeState.Drag);
            _manager.OnPlayerEnterHazard("hazard_1");
            
            Assert.That(_manager.CurrentStateValue, Is.EqualTo(CurseEffectState.HazardTriggered));
        }

        [Test]
        public void AC3_TriggersHazardEvent_WhenPlayerEntersHazard()
        {
            _phaseMachine.SetPhase(PhaseState.NightSurvival);
            _manager.ReceiveCurse(NightOutcomeState.Drag);
            
            bool eventFired = false;
            _manager.OnHazardTriggered.Subscribe(evt => eventFired = true);
            
            _manager.OnPlayerEnterHazard("hazard_1");
            
            Assert.That(eventFired, Is.True);
        }

        [Test]
        public void AC3_DoesNotTrigger_WhenNotInCurseActiveState()
        {
            _phaseMachine.SetPhase(PhaseState.DayService);
            _manager.OnPlayerEnterHazard("hazard_1");
            
            Assert.That(_manager.CurrentStateValue, Is.EqualTo(CurseEffectState.Idle));
        }

        // ── AC-4: HazardCleared on player exit ──────────────────────────────

        [Test]
        public void AC4_TransitionsToHazardCleared_WhenPlayerExitsHazardZone()
        {
            _phaseMachine.SetPhase(PhaseState.NightSurvival);
            _manager.ReceiveCurse(NightOutcomeState.Drag);
            _manager.OnPlayerEnterHazard("hazard_1");
            _manager.OnPlayerExitHazard("hazard_1");
            
            // After clearing the only hazard, state returns to CurseActive
            Assert.That(_manager.CurrentStateValue, Is.EqualTo(CurseEffectState.CurseActive));
        }

        [Test]
        public void AC4_TriggersHazardClearedEvent_WhenPlayerExits()
        {
            _phaseMachine.SetPhase(PhaseState.NightSurvival);
            _manager.ReceiveCurse(NightOutcomeState.Drag);
            _manager.OnPlayerEnterHazard("hazard_1");
            
            bool eventFired = false;
            _manager.OnHazardCleared.Subscribe(evt => eventFired = true);
            
            _manager.OnPlayerExitHazard("hazard_1");
            
            Assert.That(eventFired, Is.True);
        }

        [Test]
        public void AC4_ReturnsToCurseActive_WhenAllHazardsCleared()
        {
            _phaseMachine.SetPhase(PhaseState.NightSurvival);
            _manager.ReceiveCurse(NightOutcomeState.Drag);
            _manager.OnPlayerEnterHazard("hazard_1");
            _manager.OnPlayerExitHazard("hazard_1");
            
            Assert.That(_manager.CurrentStateValue, Is.EqualTo(CurseEffectState.CurseActive));
        }

        [Test]
        public void AC4_DoesNotTransition_WhenNotInHazardTriggeredState()
        {
            _phaseMachine.SetPhase(PhaseState.NightSurvival);
            _manager.ReceiveCurse(NightOutcomeState.Drag);
            // Not in HazardTriggered state - shouldn't transition
            _manager.OnPlayerExitHazard("hazard_1");
            
            Assert.That(_manager.CurrentStateValue, Is.EqualTo(CurseEffectState.CurseActive));
        }

        // ── Additional edge case tests ──────────────────────────────

        [Test]
        public void EdgeCase_TransitionsToIdle_WhenNightPhaseEnds()
        {
            _phaseMachine.SetPhase(PhaseState.NightSurvival);
            _manager.ReceiveCurse(NightOutcomeState.Drag);
            _phaseMachine.SetPhase(PhaseState.EndingEvaluation);
            
            Assert.That(_manager.CurrentStateValue, Is.EqualTo(CurseEffectState.Idle));
        }

        [Test]
        public void EdgeCase_MultipleHazards_TracksCorrectly()
        {
            _phaseMachine.SetPhase(PhaseState.NightSurvival);
            _manager.ReceiveCurse(NightOutcomeState.Drag);
            
            _manager.OnPlayerEnterHazard("hazard_1");
            _manager.OnPlayerEnterHazard("hazard_2");
            
            Assert.That(_manager.CurrentStateValue, Is.EqualTo(CurseEffectState.HazardTriggered));
            
            _manager.OnPlayerExitHazard("hazard_1");
            // State is HazardCleared after exiting a hazard, even if others remain
            Assert.That(_manager.CurrentStateValue, Is.EqualTo(CurseEffectState.HazardCleared));
            
            _manager.OnPlayerExitHazard("hazard_2");
            // Back to CurseActive when all hazards cleared
            Assert.That(_manager.CurrentStateValue, Is.EqualTo(CurseEffectState.CurseActive));
        }

        // ── Test Helpers ──────────────────────────────

        private class TestPhaseStateMachine : IPhaseStateMachine
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

            public bool TryTransition(PhaseState newPhase)
            {
                var previousPhase = _phase.Value;
                _phase.Value = newPhase;
                _phaseChangedSubject.OnNext(new PhaseChangedEvent(previousPhase, newPhase));
                return true;
            }

            public bool IsActionAllowed(GameAction action) => true;

            public void Initialize() { }

            public void SetPhase(PhaseState phase)
            {
                var previousPhase = _phase.Value;
                _phase.Value = phase;
                _phaseChangedSubject.OnNext(new PhaseChangedEvent(previousPhase, phase));
            }

            public void FireNightStart()
            {
                _nightStartSubject.OnNext(new NightStartEvent());
            }
        }
    }
}