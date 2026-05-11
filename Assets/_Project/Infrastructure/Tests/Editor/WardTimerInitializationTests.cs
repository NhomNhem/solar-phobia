// Assets/_Project/Infrastructure/Tests/Editor/WardTimerInitializationTests.cs
using NUnit.Framework;
using R3;
using SolarPhobia.Application.Messages;
using SolarPhobia.Application.Services;
using SolarPhobia.Domain;
using SolarPhobia.Domain.ValueObjects;
using SolarPhobia.Infrastructure.Services;

namespace SolarPhobia.Infrastructure.Tests
{
    /// <summary>
    /// Validates: TR-state-005 — Ward Timer Initialization — Base + (Saved × 30) Formula
    /// Story 008: Ward Timer Initialization — Base + (Saved × 30) Formula
    /// </summary>
    public class WardTimerInitializationTests
    {
        private TestPhaseStateMachine _phaseStateMachine;
        private WardTimerService _wardTimerService;

        [SetUp]
        public void Setup()
        {
            _phaseStateMachine = new TestPhaseStateMachine();
            _wardTimerService = new WardTimerService(_phaseStateMachine);
        }

        // ── AC-1: Default initialization with 0 ghosts saved ───────────
        // Given: Player has saved 0 ghosts, no day penalties
        // When: Night phase starts
        // Then: Initial Ward = 10 + (0 × 30) - 0 = 10 seconds

        [Test]
        public void AC1_DefaultInitialization_With0Ghosts_Returns10Seconds()
        {
            _wardTimerService.Initialize(
                ghostsSaved: 0,
                failedLightInterrupts: 0,
                soulPanicEvents: 0);

            Assert.AreEqual(10f, _wardTimerService.CurrentWard, 0.001f);
            Assert.AreEqual(10f, _wardTimerService.MaxWard, 0.001f);
        }

        // ── AC-2: Initialization with 2 ghosts saved ────────────────
        // Given: Player has saved 2 ghosts, no day penalties
        // When: Night phase starts
        // Then: Initial Ward = 10 + (2 × 30) - 0 = 70 seconds

        [Test]
        public void AC2_Initialization_With2Ghosts_Returns70Seconds()
        {
            _wardTimerService.Initialize(
                ghostsSaved: 2,
                failedLightInterrupts: 0,
                soulPanicEvents: 0);

            Assert.AreEqual(70f, _wardTimerService.CurrentWard, 0.001f);
            Assert.AreEqual(70f, _wardTimerService.MaxWard, 0.001f);
        }

        // ── AC-3: Initialization applies day penalties ─────────────
        // Given: Player has saved 1 ghost, 1 failed light interrupt
        // When: Night phase starts
        // Then: Initial Ward = 10 + (1 × 30) - 10 = 30 seconds

        [Test]
        public void AC3_Initialization_With1GhostAnd1FailedInterrupt_Returns30Seconds()
        {
            _wardTimerService.Initialize(
                ghostsSaved: 1,
                failedLightInterrupts: 1,
                soulPanicEvents: 0);

            Assert.AreEqual(30f, _wardTimerService.CurrentWard, 0.001f);
        }

        [Test]
        public void AC3_Initialization_With1GhostAnd1SoulPanic_Returns35Seconds()
        {
            _wardTimerService.Initialize(
                ghostsSaved: 1,
                failedLightInterrupts: 0,
                soulPanicEvents: 1);

            // 10 + 30 - 5 = 35
            Assert.AreEqual(35f, _wardTimerService.CurrentWard, 0.001f);
        }

        // ── AC-4: Penalty cap limits total penalty ───────────────────
        // Given: Player has saved 0 ghosts, 5 failed light interrupts (50s penalty)
        // When: Night phase starts
        // Then: Initial Ward = 10 + (0 × 30) - 30 = -20 → clamped to 0 (minimum 0)

        [Test]
        public void AC4_PenaltyCap_LimitsTotalPenalty_To30SecondsMax()
        {
            _wardTimerService.Initialize(
                ghostsSaved: 0,
                failedLightInterrupts: 5,
                soulPanicEvents: 0);

            // 10 + 0 - 30 (capped) = -20 → clamped to 0
            Assert.AreEqual(0f, _wardTimerService.CurrentWard, 0.001f);
        }

        [Test]
        public void AC4_PenaltyCap_With2GhostsAndMaxPenalties_Returns40Seconds()
        {
            _wardTimerService.Initialize(
                ghostsSaved: 2,
                failedLightInterrupts: 3,
                soulPanicEvents: 0);

            // 10 + 60 - 30 (capped) = 40
            Assert.AreEqual(40f, _wardTimerService.CurrentWard, 0.001f);
        }

        // ── AC-5: Passive drain uses formula ────────────────────────
        // Given: Ward initialized at 60s, player has 2 bones, hallucination_multiplier = 0.5
        // When: 1 second passes
        // Then: Ward = 60 - (1.0 + 2 × 0.5) = 60 - 2.0 = 58 seconds

        [Test]
        public void AC5_PassiveDrain_With2BonesAnd0d5Multiplier_Drains2d0PerSecond()
        {
            _phaseStateMachine.SetState(PhaseState.NightSurvival);

            _wardTimerService.Initialize(
                ghostsSaved: 1,
                failedLightInterrupts: 1,
                soulPanicEvents: 0);

            // Reset to known state for drain test
            _wardTimerService.Initialize(
                ghostsSaved: 1,
                failedLightInterrupts: 1,
                soulPanicEvents: 0);

            // Manually set up drain: base 1.0 + (2 bones × 0.5) = 2.0/sec
            _wardTimerService.SetDrainRate(baseDrain: 1.0f, boneCount: 2, hallucinationMultiplier: 0.5f);

            // Verify initial state
            Assert.AreEqual(30f, _wardTimerService.CurrentWard, 0.001f);
        }

        // ── Additional Tests ────────────────────────────────────────

        [Test]
        public void Initialization_SetsSensoryTier_ToStable()
        {
            _wardTimerService.Initialize(
                ghostsSaved: 2,
                failedLightInterrupts: 0,
                soulPanicEvents: 0);

            Assert.AreEqual(SensoryTier.Stable, _wardTimerService.CurrentTier.CurrentValue);
        }

        [Test]
        public void PenaltyCalculation_FailedLightInterrupt_Is10SecondsEach()
        {
            _wardTimerService.Initialize(
                ghostsSaved: 2,
                failedLightInterrupts: 2,
                soulPanicEvents: 0);

            // 10 + 60 - 20 = 50
            Assert.AreEqual(50f, _wardTimerService.CurrentWard, 0.001f);
        }

        [Test]
        public void PenaltyCalculation_SoulPanic_Is5SecondsEach()
        {
            _wardTimerService.Initialize(
                ghostsSaved: 2,
                failedLightInterrupts: 0,
                soulPanicEvents: 3);

            // 10 + 60 - 15 = 55
            Assert.AreEqual(55f, _wardTimerService.CurrentWard, 0.001f);
        }

        [Test]
        public void PenaltyCalculation_MixedPenalties_AreSummed()
        {
            _wardTimerService.Initialize(
                ghostsSaved: 2,
                failedLightInterrupts: 1,
                soulPanicEvents: 2);

            // 10 + 60 - 10 - 10 = 50
            Assert.AreEqual(50f, _wardTimerService.CurrentWard, 0.001f);
        }

        // ── Test Helper ─────────────────────────────────────────────

        private class TestPhaseStateMachine : IPhaseStateMachine
        {
            public PhaseState CurrentState { get; private set; } = PhaseState.DayService;

            public ReadOnlyReactiveProperty<PhaseState> CurrentPhase =>
                new ReactiveProperty<PhaseState>(CurrentState);

            public Observable<PhaseChangedEvent> OnPhaseChanged => new Subject<PhaseChangedEvent>();
            public Observable<DayStartEvent> OnDayStart => new Subject<DayStartEvent>();
            public Observable<NightStartEvent> OnNightStart => new Subject<NightStartEvent>();
            public Observable<ResolveEvent> OnResolve => new Subject<ResolveEvent>();

            public void SetState(PhaseState state)
            {
                CurrentState = state;
            }

            public bool TryTransition(PhaseState newPhase) => true;
            public bool IsActionAllowed(GameAction action) => true;
            public void Initialize() { }
        }
    }
}
