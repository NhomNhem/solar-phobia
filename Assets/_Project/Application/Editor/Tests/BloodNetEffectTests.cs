using NUnit.Framework;
using R3;
using SolarPhobia.Application.Consequences;
using SolarPhobia.Application.Messages;
using SolarPhobia.Application.Phase.Flow;
using SolarPhobia.Application.Services;
using SolarPhobia.Application.Services.Combat;
using SolarPhobia.Domain.ValueObjects;

using SolarPhobia.Application.Resources;

using SolarPhobia.Application.Strike;

using SolarPhobia.Application.Consequences.WaterTrap;

using SolarPhobia.Application.Rituals;

using SolarPhobia.Application.Shrines;

using SolarPhobia.Application.Day;

using SolarPhobia.Application.Flow;

using SolarPhobia.Application.Player.State;

using SolarPhobia.Application.Player.Input;

using SolarPhobia.Application.Player.Interactions;

using SolarPhobia.Application.Player.Cursor;

using SolarPhobia.Application.Player.Events;

namespace SolarPhobia.Application.Editor.Tests
{
    public class BloodNetEffectTests
    {
        private BloodNetEffectService _service;
        private CurseEffectManager _curseManager;
        private TestWardTimerService _wardTimer;
        private TestPhaseStateMachine _phaseMachine;

        [SetUp]
        public void SetUp()
        {
            _wardTimer = new TestWardTimerService();
            _phaseMachine = new TestPhaseStateMachine(PhaseState.DayService);
            _curseManager = new CurseEffectManager(_phaseMachine);
            _curseManager.Initialize();

            _phaseMachine.SetPhase(PhaseState.NightSurvival);
            _curseManager.ReceiveCurse(NightOutcomeState.Block);

            _service = new BloodNetEffectService(_curseManager, _wardTimer);
        }

        [TearDown]
        public void TearDown()
        {
            _service?.Dispose();
            _curseManager?.Dispose();
        }

        // â”€â”€ AC-1: Immediate penalty on contact â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [Test]
        public void AC1_BloodNetContact_Applies5WardPenalty()
        {
            _curseManager.OnPlayerEnterHazard("blood_net_1");

            Assert.That(_wardTimer.TotalCostApplied, Is.EqualTo(5.0f).Within(0.001f));
        }

        [Test]
        public void AC1_NoPenalty_WhenNotInBloodNet()
        {
            Assert.That(_wardTimer.TotalCostApplied, Is.EqualTo(0f));
        }

        [Test]
        public void AC1_TotalPenaltyApplied_TracksAccumulated()
        {
            _curseManager.OnPlayerEnterHazard("blood_net_1");

            Assert.That(_service.TotalPenaltyApplied, Is.EqualTo(5.0f).Within(0.001f));
        }

        // â”€â”€ AC-2: Slow applied for 3 seconds â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [Test]
        public void AC2_BloodNetContact_StartsSlow()
        {
            _curseManager.OnPlayerEnterHazard("blood_net_1");

            Assert.That(_service.IsSlowed, Is.True);
            Assert.That(_service.SlowTimeRemaining, Is.EqualTo(3.0f).Within(0.001f));
        }

        [Test]
        public void AC2_NoContact_NotSlowed()
        {
            Assert.That(_service.IsSlowed, Is.False);
        }

        [Test]
        public void AC2_SlowDuration_DecreasesOverTime()
        {
            _curseManager.OnPlayerEnterHazard("blood_net_1");

            _service.Tick(1.0f);

            Assert.That(_service.SlowTimeRemaining, Is.EqualTo(2.0f).Within(0.001f));
        }

        // â”€â”€ AC-3: Slow restores after 3 seconds â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [Test]
        public void AC3_SlowEnds_After3Seconds()
        {
            _curseManager.OnPlayerEnterHazard("blood_net_1");

            _service.Tick(3.0f);

            Assert.That(_service.IsSlowed, Is.False);
            Assert.That(_service.SlowTimeRemaining, Is.EqualTo(0f));
        }

        [Test]
        public void AC3_SlowEnds_AfterPartialTicks()
        {
            _curseManager.OnPlayerEnterHazard("blood_net_1");

            _service.Tick(1.5f);
            Assert.That(_service.IsSlowed, Is.True);

            _service.Tick(1.5f);
            Assert.That(_service.IsSlowed, Is.False);
        }

        // â”€â”€ Edge Cases â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [Test]
        public void EdgeCase_OnlyBlockCurse_TriggersBloodNet()
        {
            _curseManager.ReceiveCurse(NightOutcomeState.Drag);
            _curseManager.OnPlayerEnterHazard("drag_hazard_1");

            _service.Tick(1.0f);

            Assert.That(_wardTimer.TotalCostApplied, Is.EqualTo(0f));
            Assert.That(_service.IsSlowed, Is.False);
        }

        [Test]
        public void EdgeCase_Reentry_RenewsSlow()
        {
            _curseManager.OnPlayerEnterHazard("blood_net_1");
            _service.Tick(2.0f);
            Assert.That(_service.SlowTimeRemaining, Is.EqualTo(1.0f).Within(0.001f));

            _curseManager.OnPlayerEnterHazard("blood_net_2");
            Assert.That(_service.SlowTimeRemaining, Is.EqualTo(3.0f).Within(0.001f));
        }

        [Test]
        public void EdgeCase_NoPenalty_WhenWardCannotApply()
        {
            _wardTimer.SetCanApplyCost(false);
            _curseManager.OnPlayerEnterHazard("blood_net_1");

            Assert.That(_wardTimer.TotalCostApplied, Is.EqualTo(0f));
        }

        // â”€â”€ Test Doubles â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        private class TestWardTimerService : IWardTimerService
        {
            public float CurrentWard { get; set; } = 100f;
            public float TotalCostApplied { get; private set; }
            public float GetCurrentWard() => CurrentWard;
            public Observable<float> OnWardChanged => Observable.Empty<float>();

            private bool _canApply = true;

            public void Initialize(int ghostsSaved, int failedLightInterrupts, int soulPanicEvents) { }
            public void ApplyPenalty(float amount) { TryApplyCost(amount); }
            public void SetDrainRate(float baseDrain, int boneCount, float hallucinationMultiplier) { }

            public bool TryApplyCost(float cost)
            {
                if (!_canApply) return false;
                TotalCostApplied += cost;
                CurrentWard -= cost;
                return true;
            }

            public void SetCanApplyCost(bool canApply)
            {
                _canApply = canApply;
            }
        }

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

