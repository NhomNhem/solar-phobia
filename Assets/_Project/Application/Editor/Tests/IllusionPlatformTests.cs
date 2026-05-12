using NUnit.Framework;
using R3;
using SolarPhobia.Application.Consequences;
using SolarPhobia.Application.Messages;
using SolarPhobia.Application.Phase.Flow;
using SolarPhobia.Application.Services;
using SolarPhobia.Application.Services.Interfaces;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Editor.Tests
{
    public class IllusionPlatformTests
    {
        private IllusionPlatformEffectService _service;
        private CurseEffectManager _curseManager;
        private TestPhaseStateMachine _phaseMachine;

        [SetUp]
        public void SetUp()
        {
            _phaseMachine = new TestPhaseStateMachine(PhaseState.DayService);
            _curseManager = new CurseEffectManager(_phaseMachine);
            _curseManager.Initialize();

            _phaseMachine.SetPhase(PhaseState.NightSurvival);
            _curseManager.ReceiveCurse(NightOutcomeState.FakeShrine);

            _service = new IllusionPlatformEffectService(_curseManager);
        }

        [TearDown]
        public void TearDown()
        {
            _service?.Dispose();
            _curseManager?.Dispose();
        }

        // ── AC-1: Platform collapses after 0.2s ───────────────────

        [Test]
        public void AC1_AfterExactCollapseDelay_PlatformCollapses()
        {
            _curseManager.OnPlayerEnterHazard("platform_1");

            _service.Tick(0.2f);

            Assert.That(_service.IsPlatformCollapsed("platform_1"), Is.True);
            Assert.That(_service.IsCollapseTimerActive("platform_1"), Is.False);
        }

        [Test]
        public void AC1_BeforeCollapseDelay_PlatformRemainsIntact()
        {
            _curseManager.OnPlayerEnterHazard("platform_1");

            _service.Tick(0.1f);

            Assert.That(_service.IsPlatformCollapsed("platform_1"), Is.False);
            Assert.That(_service.IsCollapseTimerActive("platform_1"), Is.True);
        }

        [Test]
        public void AC1_CollapseTimer_StartsOnEntry()
        {
            _curseManager.OnPlayerEnterHazard("platform_1");

            Assert.That(_service.IsCollapseTimerActive("platform_1"), Is.True);
            Assert.That(_service.IsPlatformCollapsed("platform_1"), Is.False);
        }

        // ── AC-2: Immediate collapse when standing at end ─────────

        [Test]
        public void AC2_AfterQuarterSecond_PlatformCollapsed()
        {
            _curseManager.OnPlayerEnterHazard("platform_1");

            _service.Tick(0.25f);

            Assert.That(_service.IsPlatformCollapsed("platform_1"), Is.True);
        }

        [Test]
        public void AC2_AfterMultipleSmallTicks_PlatformCollapses()
        {
            _curseManager.OnPlayerEnterHazard("platform_1");

            _service.Tick(0.05f);
            _service.Tick(0.05f);
            _service.Tick(0.05f);
            _service.Tick(0.05f);

            Assert.That(_service.IsPlatformCollapsed("platform_1"), Is.True);
        }

        // ── AC-3: Platform stays collapsed ────────────────────────

        [Test]
        public void AC3_AfterCollapse_PlatformStaysCollapsed()
        {
            _curseManager.OnPlayerEnterHazard("platform_1");
            _service.Tick(0.2f);

            _service.Tick(10.0f);

            Assert.That(_service.IsPlatformCollapsed("platform_1"), Is.True);
        }

        [Test]
        public void AC3_ExitBeforeCollapse_PreventsCollapse()
        {
            _curseManager.OnPlayerEnterHazard("platform_1");
            _service.Tick(0.1f);

            _curseManager.OnPlayerExitHazard("platform_1");

            _service.Tick(0.2f);

            Assert.That(_service.IsPlatformCollapsed("platform_1"), Is.False);
            Assert.That(_service.IsCollapseTimerActive("platform_1"), Is.False);
        }

        // ── Edge Cases ────────────────────────────────────────────

        [Test]
        public void EdgeCase_OnlyFakeShrineCurse_TriggersCollapse()
        {
            _curseManager.ReceiveCurse(NightOutcomeState.Drag);
            _curseManager.OnPlayerEnterHazard("drag_hazard_1");

            _service.Tick(0.2f);

            Assert.That(_service.IsPlatformCollapsed("drag_hazard_1"), Is.False);
        }

        [Test]
        public void EdgeCase_MultiplePlatforms_TrackedIndependently()
        {
            _curseManager.OnPlayerEnterHazard("platform_1");
            _curseManager.OnPlayerEnterHazard("platform_2");
            _curseManager.OnPlayerEnterHazard("platform_3");

            _service.Tick(0.15f);

            _curseManager.OnPlayerExitHazard("platform_2");
            _service.Tick(0.1f);

            Assert.That(_service.IsPlatformCollapsed("platform_1"), Is.True);
            Assert.That(_service.IsPlatformCollapsed("platform_2"), Is.False);
            Assert.That(_service.IsPlatformCollapsed("platform_3"), Is.True);
        }

        [Test]
        public void EdgeCase_ReEntry_CollapsedPlatform_DoesNotRestartTimer()
        {
            _curseManager.OnPlayerEnterHazard("platform_1");
            _service.Tick(0.2f);
            Assert.That(_service.IsPlatformCollapsed("platform_1"), Is.True);

            _curseManager.OnPlayerEnterHazard("platform_1");
            _service.Tick(0.2f);

            Assert.That(_service.IsPlatformCollapsed("platform_1"), Is.True);
        }

        [Test]
        public void EdgeCase_NoHazards_NothingCollapsed()
        {
            Assert.That(_service.IsPlatformCollapsed("nonexistent"), Is.False);
            Assert.That(_service.IsCollapseTimerActive("nonexistent"), Is.False);
        }

        // ── Test Doubles ───────────────────────────────────────────

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
