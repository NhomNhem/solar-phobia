// Assets/_Project/Infrastructure/Tests/Editor/SensoryTierIntegrationTests.cs
using NUnit.Framework;
using R3;
using SolarPhobia.Application.Messages;
using SolarPhobia.Application.Services;
using SolarPhobia.Domain;
using SolarPhobia.Domain.Events;
using SolarPhobia.Domain.ValueObjects;
using SolarPhobia.Infrastructure.Services;

namespace SolarPhobia.Infrastructure.Tests
{
    /// <summary>
    /// Integration tests for Sensory Tiers with Ward Timer and HUD coordination.
    /// Story 009: Sensory Tiers — Detection Threshold Feedback
    /// </summary>
    public class SensoryTierIntegrationTests
    {
        private TestPhaseStateMachine _phaseStateMachine;
        private WardTimerService _wardTimerService;
        private SensoryTierService _sensoryTierService;
        private Subject<NightFailedEvent> _nightFailedSubject;

        [SetUp]
        public void Setup()
        {
            _phaseStateMachine = new TestPhaseStateMachine();
            _phaseStateMachine.SetState(PhaseState.NightSurvival);
            _wardTimerService = new WardTimerService(_phaseStateMachine);
            _nightFailedSubject = new Subject<NightFailedEvent>();
            _sensoryTierService = new SensoryTierService(_nightFailedSubject);
        }

        [TearDown]
        public void TearDown()
        {
            _nightFailedSubject?.Dispose();
        }

        // ── Integration: Ward Timer + Sensory Tier Service ───────────

        [Test]
        public void Integration_WardTimerInitialization_TriggersCorrectInitialTier()
        {
            // Arrange: Initialize Ward with 100 seconds
            _wardTimerService.Initialize(
                ghostsSaved: 3,  // 10 + 90 = 100 seconds
                failedLightInterrupts: 0,
                soulPanicEvents: 0);

            // Act: Link SensoryTierService to WardTimerService
            _sensoryTierService.Initialize(_wardTimerService.CurrentWardObservable, _wardTimerService.MaxWard);

            // Assert: Initial tier should be Stable (>75%)
            Assert.AreEqual(SensoryTier.Stable, _sensoryTierService.CurrentTier.CurrentValue);
            Assert.AreEqual(100f, _wardTimerService.CurrentWard);
        }

        [Test]
        public void Integration_WardDrain_CausesTierProgression()
        {
            // Arrange: Initialize Ward with 100 seconds
            _wardTimerService.Initialize(
                ghostsSaved: 3,
                failedLightInterrupts: 0,
                soulPanicEvents: 0);
            _sensoryTierService.Initialize(_wardTimerService.CurrentWardObservable, _wardTimerService.MaxWard);

            var tierChanges = new System.Collections.Generic.List<SensoryTierChangedEvent>();
            _sensoryTierService.OnTierChanged.Subscribe(e => tierChanges.Add(e));

            // Act: Simulate Ward drain through direct updates
            // Drop to 74% (Tier 1: CreepingDread)
            _wardTimerService.ApplyPenalty(26f); // 100 -> 74

            // Assert
            Assert.AreEqual(1, tierChanges.Count, "Should have crossed into CreepingDread");
            Assert.AreEqual(SensoryTier.CreepingDread, tierChanges[0].NewTier);
        }

        [Test]
        public void Integration_AllTierTransitions_SequenceCorrectly()
        {
            // Arrange: Initialize Ward with 100 seconds
            _wardTimerService.Initialize(
                ghostsSaved: 3,
                failedLightInterrupts: 0,
                soulPanicEvents: 0);
            _sensoryTierService.Initialize(_wardTimerService.CurrentWardObservable, _wardTimerService.MaxWard);

            var tierChanges = new System.Collections.Generic.List<SensoryTierChangedEvent>();
            _sensoryTierService.OnTierChanged.Subscribe(e => tierChanges.Add(e));

            // Act: Progress through all tiers
            // 100 -> 74 (Stable -> CreepingDread at 74%)
            _wardTimerService.ApplyPenalty(26f);
            Assert.AreEqual(SensoryTier.CreepingDread, _sensoryTierService.CurrentTier.CurrentValue);

            // 74 -> 49 (CreepingDread -> HeavyBurden at 49%)
            _wardTimerService.ApplyPenalty(25f);
            Assert.AreEqual(SensoryTier.HeavyBurden, _sensoryTierService.CurrentTier.CurrentValue);

            // 49 -> 24 (HeavyBurden -> Panic at 24%)
            _wardTimerService.ApplyPenalty(25f);
            Assert.AreEqual(SensoryTier.Panic, _sensoryTierService.CurrentTier.CurrentValue);

            // 24 -> 9 (Panic -> DeathSpiral at ≤10s)
            _wardTimerService.ApplyPenalty(15f);
            Assert.AreEqual(SensoryTier.DeathSpiral, _sensoryTierService.CurrentTier.CurrentValue);

            // Assert: All 4 tier transitions recorded
            Assert.AreEqual(4, tierChanges.Count);
            Assert.AreEqual(SensoryTier.CreepingDread, tierChanges[0].NewTier);
            Assert.AreEqual(SensoryTier.HeavyBurden, tierChanges[1].NewTier);
            Assert.AreEqual(SensoryTier.Panic, tierChanges[2].NewTier);
            Assert.AreEqual(SensoryTier.DeathSpiral, tierChanges[3].NewTier);
        }

        [Test]
        public void Integration_WardReachesZero_BothServicesEmitEvents()
        {
            // Arrange: Initialize with minimal ward
            _wardTimerService.Initialize(
                ghostsSaved: 0,  // 10 seconds only
                failedLightInterrupts: 0,
                soulPanicEvents: 0);
            _sensoryTierService.Initialize(_wardTimerService.CurrentWardObservable, _wardTimerService.MaxWard);

            NightFailedEvent? nightFailedEvent = null;
            _sensoryTierService.OnNightFailed.Subscribe(e => nightFailedEvent = e);

            // Act: Drain all Ward
            _wardTimerService.ApplyPenalty(10f); // 10 -> 0

            // Assert
            Assert.IsTrue(nightFailedEvent.HasValue, "NightFailedEvent should be emitted");
            Assert.AreEqual(0f, nightFailedEvent.Value.FinalWard);
            Assert.AreEqual(SensoryTier.DeathSpiral, _sensoryTierService.CurrentTier.CurrentValue);
        }

        [Test]
        public void Integration_TierEventData_ContainsCorrectValues()
        {
            // Arrange
            _wardTimerService.Initialize(
                ghostsSaved: 3,
                failedLightInterrupts: 0,
                soulPanicEvents: 0);
            _sensoryTierService.Initialize(_wardTimerService.CurrentWardObservable, _wardTimerService.MaxWard);

            SensoryTierChangedEvent? capturedEvent = null;
            _sensoryTierService.OnTierChanged.Subscribe(e => capturedEvent = e);

            // Act: Drop to trigger tier change
            _wardTimerService.ApplyPenalty(26f); // 100 -> 74

            // Assert: Event data is correct
            Assert.IsTrue(capturedEvent.HasValue);
            Assert.AreEqual(SensoryTier.CreepingDread, capturedEvent.Value.NewTier);
            Assert.AreEqual(SensoryTier.Stable, capturedEvent.Value.PreviousTier);
            Assert.AreEqual(74f, capturedEvent.Value.WardValue, 0.01f);
            Assert.AreEqual(100f, capturedEvent.Value.MaxWard);
        }

        [Test]
        public void Integration_Reinitialization_ResetsState()
        {
            // Arrange: First night with 100 seconds
            _wardTimerService.Initialize(
                ghostsSaved: 3,
                failedLightInterrupts: 0,
                soulPanicEvents: 0);
            _sensoryTierService.Initialize(_wardTimerService.CurrentWardObservable, _wardTimerService.MaxWard);

            // Drain to DeathSpiral
            _wardTimerService.ApplyPenalty(91f); // 100 -> 9
            Assert.AreEqual(SensoryTier.DeathSpiral, _sensoryTierService.CurrentTier.CurrentValue);

            // Act: Reinitialize for new night (60 seconds)
            _wardTimerService.Initialize(
                ghostsSaved: 1,  // 10 + 30 = 40 seconds
                failedLightInterrupts: 0,
                soulPanicEvents: 0);
            _sensoryTierService.Initialize(_wardTimerService.CurrentWardObservable, _wardTimerService.MaxWard);

            // Assert: Should reset to appropriate tier for new Ward
            // 40 seconds is >10s, but 40/40 = 100% = Stable
            Assert.AreEqual(SensoryTier.Stable, _sensoryTierService.CurrentTier.CurrentValue);
        }

        [Test]
        public void Integration_NightFailedEvent_CanTriggerPhaseTransition()
        {
            // Arrange: Setup with minimal ward
            _wardTimerService.Initialize(
                ghostsSaved: 0,  // 10 seconds
                failedLightInterrupts: 0,
                soulPanicEvents: 0);
            _sensoryTierService.Initialize(_wardTimerService.CurrentWardObservable, _wardTimerService.MaxWard);

            bool phaseTransitionTriggered = false;
            _sensoryTierService.OnNightFailed.Subscribe(_ => phaseTransitionTriggered = true);

            // Act: Simulate Ward reaching 0 (player death)
            _wardTimerService.ApplyPenalty(10f);

            // Assert
            Assert.IsTrue(phaseTransitionTriggered, "NightFailedEvent should trigger phase transition logic");
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
