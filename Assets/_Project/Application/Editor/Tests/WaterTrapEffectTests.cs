using NUnit.Framework;
using R3;
using SolarPhobia.Application.Consequences;
using SolarPhobia.Application.Consequences.WaterTrap;
using SolarPhobia.Application.Messages;
using SolarPhobia.Application.Phase.Flow;
using SolarPhobia.Application.Resources;
using SolarPhobia.Application.Services;
using SolarPhobia.Application.Phase.Reset;
using SolarPhobia.Domain.ValueObjects;
using UnityEngine;
namespace SolarPhobia.Application.Tests
{
    /// <summary>
    /// Tests for WaterTrapEffectService - validates AC-1 and AC-2.
    /// Validates: Story 002 - Water Trap Effect (Drag - Linh).
    /// </summary>
    [TestFixture]
    public class WaterTrapEffectTests
    {
        private WaterTrapEffectService _service;
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

            // Enter NightSurvival and receive Drag curse
            _phaseMachine.SetPhase(PhaseState.NightSurvival);
            _curseManager.ReceiveCurse(NightOutcomeState.Drag);

            _service = new WaterTrapEffectService(_curseManager, _wardTimer);
        }

        [TearDown]
        public void TearDown()
        {
            _service?.Dispose();
            _curseManager?.Dispose();
        }

        // ── AC-1: Water trap damage per second ─────────────────────

        [Test]
        public void AC1_WaterTrapApplies_3DamagePerSecond()
        {
            _curseManager.OnPlayerEnterHazard("water_zone_1");

            _service.Tick(1.0f);

            Assert.That(_wardTimer.TotalCostApplied, Is.EqualTo(3.0f).Within(0.001f));
        }

        [Test]
        public void AC1_NoDamage_WhenNotInWaterZone()
        {
            _service.Tick(1.0f);

            Assert.That(_wardTimer.TotalCostApplied, Is.EqualTo(0f));
        }

        [Test]
        public void AC1_IsActive_WhenInWaterZone()
        {
            _curseManager.OnPlayerEnterHazard("water_zone_1");

            Assert.That(_service.IsActive, Is.True);
        }

        [Test]
        public void AC1_IsNotActive_WhenNotInWaterZone()
        {
            Assert.That(_service.IsActive, Is.False);
        }

        // ── AC-2: DoT applies continuously ─────────────────────────

        [Test]
        public void AC2_DoTAppliesContinuously_OverMultipleSeconds()
        {
            _curseManager.OnPlayerEnterHazard("water_zone_1");

            _service.Tick(1.0f);
            _service.Tick(1.0f);
            _service.Tick(1.0f);

            Assert.That(_wardTimer.TotalCostApplied, Is.EqualTo(9.0f).Within(0.001f));
        }

        [Test]
        public void AC2_DoTAccumulates_OverPartialSeconds()
        {
            _curseManager.OnPlayerEnterHazard("water_zone_1");

            _service.Tick(0.5f);
            _service.Tick(0.5f);

            Assert.That(_wardTimer.TotalCostApplied, Is.EqualTo(3.0f).Within(0.001f));
        }

        [Test]
        public void AC2_DoTStops_WhenPlayerExitsWaterZone()
        {
            _curseManager.OnPlayerEnterHazard("water_zone_1");

            _service.Tick(1.0f);
            Assert.That(_wardTimer.TotalCostApplied, Is.EqualTo(3.0f).Within(0.001f));

            _curseManager.OnPlayerExitHazard("water_zone_1");

            _service.Tick(2.0f);

            Assert.That(_wardTimer.TotalCostApplied, Is.EqualTo(3.0f).Within(0.001f));
        }

        [Test]
        public void AC2_TotalDamageApplied_TracksAccumulatedDamage()
        {
            _curseManager.OnPlayerEnterHazard("water_zone_1");

            _service.Tick(1.0f);
            _service.Tick(0.5f);

            Assert.That(_service.TotalDamageApplied, Is.EqualTo(4.5f).Within(0.001f));
        }

        // ── Edge Cases ─────────────────────────────────────────────

        [Test]
        public void EdgeCase_OnlyDragCurse_TriggersWaterTrap()
        {
            // Switch to Block curse - water trap should not activate
            _curseManager.ReceiveCurse(NightOutcomeState.Block);
            _curseManager.OnPlayerEnterHazard("block_hazard_1");

            _service.Tick(1.0f);

            Assert.That(_wardTimer.TotalCostApplied, Is.EqualTo(0f));
            Assert.That(_service.IsActive, Is.False);
        }

        [Test]
        public void EdgeCase_ReenteringWaterZone_ContinuesDoT()
        {
            _curseManager.OnPlayerEnterHazard("water_zone_1");
            _service.Tick(1.0f);
            Assert.That(_wardTimer.TotalCostApplied, Is.EqualTo(3.0f).Within(0.001f));

            _curseManager.OnPlayerExitHazard("water_zone_1");
            _service.Tick(1.0f);

            // DoT stopped after exit
            Assert.That(_wardTimer.TotalCostApplied, Is.EqualTo(3.0f).Within(0.001f));

            _curseManager.OnPlayerEnterHazard("water_zone_2");
            _service.Tick(1.0f);

            // DoT resumes on re-entry
            Assert.That(_wardTimer.TotalCostApplied, Is.EqualTo(6.0f).Within(0.001f));
        }

        [Test]
        public void EdgeCase_TotalDamageReset_AfterAllZonesCleared()
        {
            _curseManager.OnPlayerEnterHazard("water_zone_1");
            _service.Tick(1.0f);
            Assert.That(_service.TotalDamageApplied, Is.EqualTo(3.0f).Within(0.001f));

            _curseManager.OnPlayerExitHazard("water_zone_1");

            Assert.That(_service.TotalDamageApplied, Is.EqualTo(0f));
        }

        [Test]
        public void EdgeCase_NoDamage_WhenWardCannotApply()
        {
            _wardTimer.SetCanApplyCost(false);
            _curseManager.OnPlayerEnterHazard("water_zone_1");

            _service.Tick(1.0f);

            Assert.That(_wardTimer.TotalCostApplied, Is.EqualTo(0f));
        }

        // ── Test Doubles ───────────────────────────────────────────

        private class TestWardTimerService : IWardTimerPort
        {
            public float CurrentWard { get; set; } = 100f;
            public float TotalCostApplied { get; private set; }
            public Observable<float> OnWardChanged => Observable.Empty<float>();

            private bool _canApply = true;

            public float GetCurrentWard() => CurrentWard;

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


