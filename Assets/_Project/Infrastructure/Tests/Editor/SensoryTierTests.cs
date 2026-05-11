// Assets/_Project/Infrastructure/Tests/Editor/SensoryTierTests.cs
using NUnit.Framework;
using R3;
using SolarPhobia.Domain;
using SolarPhobia.Domain.Events;
using SolarPhobia.Domain.ValueObjects;
using SolarPhobia.Infrastructure.Services;

namespace SolarPhobia.Infrastructure.Tests
{
    /// <summary>
    /// Validates: TR-state-005 — Sensory Tiers — Detection Threshold Feedback
    /// Story 009: Sensory Tiers — Detection Threshold Feedback
    /// </summary>
    public class SensoryTierTests
    {
        private SensoryTierService _sensoryTierService;
        private Subject<NightFailedEvent> _nightFailedSubject;
        private ReactiveProperty<float> _wardObservable;

        [SetUp]
        public void Setup()
        {
            _nightFailedSubject = new Subject<NightFailedEvent>();
            _sensoryTierService = new SensoryTierService(_nightFailedSubject);
            _wardObservable = new ReactiveProperty<float>(100f);
        }

        [TearDown]
        public void TearDown()
        {
            _nightFailedSubject?.Dispose();
            _wardObservable?.Dispose();
        }

        // ── AC-1: Tier 1 triggers at 75% Ward remaining ────────────
        // Given: Ward = 80 seconds (initial 100s max)
        // When: Ward drops to 75 seconds
        // Then: Tier 1 event fires, subtle UI pulse visible

        [Test]
        public void AC1_WardDropsTo75Percent_TriggersTier1_CreepingDread()
        {
            // Arrange: Ward = 80/100 = 80% (Stable)
            _wardObservable.Value = 80f;
            _sensoryTierService.Initialize(_wardObservable, 100f);

            SensoryTierChangedEvent? capturedEvent = null;
            _sensoryTierService.OnTierChanged.Subscribe(e => capturedEvent = e);

            // Act: Drop to 75/100 = 75% (crosses threshold)
            _wardObservable.Value = 75f;

            // Assert
            Assert.IsTrue(capturedEvent.HasValue, "Tier change event should fire");
            Assert.AreEqual(SensoryTier.CreepingDread, capturedEvent.Value.NewTier, "Should enter CreepingDread tier");
            Assert.AreEqual(SensoryTier.Stable, capturedEvent.Value.PreviousTier, "Should come from Stable");
        }

        [Test]
        public void AC1_WardAt76Percent_RemainsStable_NoEvent()
        {
            // Arrange
            _wardObservable.Value = 76f;
            _sensoryTierService.Initialize(_wardObservable, 100f);

            bool eventFired = false;
            _sensoryTierService.OnTierChanged.Subscribe(_ => eventFired = true);

            // Act: Still above 75%
            _wardObservable.Value = 76f;

            // Assert
            Assert.IsFalse(eventFired, "No event should fire when staying above threshold");
            Assert.AreEqual(SensoryTier.Stable, _sensoryTierService.CurrentTier.CurrentValue);
        }

        // ── AC-2: Tier 2 triggers at 50% Ward remaining ────────────
        // Given: Ward = 50 seconds
        // When: Ward drops to 50 seconds
        // Then: Tier 2 event fires, screen edge vignette turns red

        [Test]
        public void AC2_WardDropsTo50Percent_TriggersTier2_HeavyBurden()
        {
            // Arrange: Start at 51% (HeavyBurden)
            _wardObservable.Value = 51f;
            _sensoryTierService.Initialize(_wardObservable, 100f);

            SensoryTierChangedEvent? capturedEvent = null;
            _sensoryTierService.OnTierChanged.Subscribe(e => capturedEvent = e);

            // Act: Drop to 50/100 = 50% (crosses threshold)
            _wardObservable.Value = 50f;

            // Assert
            Assert.IsTrue(capturedEvent.HasValue);
            Assert.AreEqual(SensoryTier.HeavyBurden, capturedEvent.Value.NewTier);
        }

        // ── AC-3: Tier 3 triggers at 25% Ward remaining ────────────
        // Given: Ward = 25 seconds
        // When: Ward drops below 25 seconds
        // Then: Tier 3 event fires, heartbeat audio plays, screen shake occurs

        [Test]
        public void AC3_WardDropsTo25Percent_TriggersTier3_Panic()
        {
            // Arrange: Start at 26% (HeavyBurden)
            _wardObservable.Value = 26f;
            _sensoryTierService.Initialize(_wardObservable, 100f);

            SensoryTierChangedEvent? capturedEvent = null;
            _sensoryTierService.OnTierChanged.Subscribe(e => capturedEvent = e);

            // Act: Drop to 25/100 = 25% (crosses threshold)
            _wardObservable.Value = 25f;

            // Assert
            Assert.IsTrue(capturedEvent.HasValue);
            Assert.AreEqual(SensoryTier.Panic, capturedEvent.Value.NewTier);
        }

        // ── AC-4: Tier 4 triggers at ≤10 seconds ────────────────────
        // Given: Ward = 10 seconds
        // When: Ward drops to 10 or below
        // Then: Tier 4 event fires, intense audio, full screen flash

        [Test]
        public void AC4_WardDropsTo10Seconds_TriggersTier4_DeathSpiral()
        {
            // Arrange: Start at 11 seconds (Panic at 11/100 = 11%)
            _wardObservable.Value = 11f;
            _sensoryTierService.Initialize(_wardObservable, 100f);

            SensoryTierChangedEvent? capturedEvent = null;
            _sensoryTierService.OnTierChanged.Subscribe(e => capturedEvent = e);

            // Act: Drop to 10 seconds (crosses threshold)
            _wardObservable.Value = 10f;

            // Assert
            Assert.IsTrue(capturedEvent.HasValue);
            Assert.AreEqual(SensoryTier.DeathSpiral, capturedEvent.Value.NewTier);
        }

        [Test]
        public void AC4_WardDropsBelow10Seconds_TriggersTier4_DeathSpiral()
        {
            // Arrange: Start at 10 seconds
            _wardObservable.Value = 10f;
            _sensoryTierService.Initialize(_wardObservable, 100f);

            SensoryTierChangedEvent? capturedEvent = null;
            _sensoryTierService.OnTierChanged.Subscribe(e => capturedEvent = e);

            // Act: Drop to 5 seconds
            _wardObservable.Value = 5f;

            // Assert - should NOT fire again (already in DeathSpiral)
            Assert.IsFalse(capturedEvent.HasValue, "Should not fire event when already in DeathSpiral");
        }

        // ── AC-5: Ward reaching 0 triggers death ────────────────────
        // Given: Ward = 1 second
        // When: 1 second passes and Ward reaches 0
        // Then: NightFailedEvent emitted, phase transitions to NightOutcomeState with Death outcome

        [Test]
        public void AC5_WardReachesZero_EmitsNightFailedEvent()
        {
            // Arrange
            _wardObservable.Value = 1f;
            _sensoryTierService.Initialize(_wardObservable, 100f);

            NightFailedEvent? capturedEvent = null;
            _sensoryTierService.OnNightFailed.Subscribe(e => capturedEvent = e);

            // Act: Ward reaches 0
            _wardObservable.Value = 0f;

            // Assert
            Assert.IsTrue(capturedEvent.HasValue, "NightFailedEvent should fire");
            Assert.AreEqual(0f, capturedEvent.Value.FinalWard, "Final Ward should be 0");
        }

        [Test]
        public void AC5_WardReachesZero_FinalWardIsZero()
        {
            // Arrange
            _wardObservable.Value = 5f;
            _sensoryTierService.Initialize(_wardObservable, 100f);

            NightFailedEvent capturedEvent = default;
            _sensoryTierService.OnNightFailed.Subscribe(e => capturedEvent = e);

            // Act
            _wardObservable.Value = 0f;

            // Assert
            Assert.AreEqual(0f, capturedEvent.FinalWard);
        }

        // ── AC-6: Tier events fire only once per threshold crossing ──
        // Given: Ward crossed from 76 to 74 (Tier 1)
        // When: Ward stays at 73 then goes to 75 then back to 74
        // Then: No additional Tier 1 event (only fires on crossing down)

        [Test]
        public void AC6_TierEventFiresOnlyOncePerCrossing_DoesNotRefire()
        {
            // Arrange: Start at Stable (80%)
            _wardObservable.Value = 80f;
            _sensoryTierService.Initialize(_wardObservable, 100f);

            int eventCount = 0;
            _sensoryTierService.OnTierChanged.Subscribe(_ => eventCount++);

            // Act: Cross to CreepingDread (76 → 74)
            _wardObservable.Value = 76f; // Still Stable
            _wardObservable.Value = 74f; // Cross to CreepingDread
            Assert.AreEqual(1, eventCount, "First crossing should fire event");

            // Stay in CreepingDread
            _wardObservable.Value = 73f;
            _wardObservable.Value = 72f;
            Assert.AreEqual(1, eventCount, "Staying in tier should not fire");

            // Go back up to Stable (76% — above the 75% threshold)
            _wardObservable.Value = 76f;
            Assert.AreEqual(2, eventCount, "Crossing back up to Stable should fire");

            // Go down again to CreepingDread
            _wardObservable.Value = 74f;
            Assert.AreEqual(3, eventCount, "Re-crossing down should fire again");
        }

        [Test]
        public void AC6_MultipleValueChangesInSameTier_NoRefire()
        {
            // Arrange: Start in CreepingDread (70%)
            _wardObservable.Value = 70f;
            _sensoryTierService.Initialize(_wardObservable, 100f);

            int eventCount = 0;
            _sensoryTierService.OnTierChanged.Subscribe(_ => eventCount++);

            // Act: Multiple changes within same tier
            _wardObservable.Value = 69f;
            _wardObservable.Value = 68f;
            _wardObservable.Value = 65f;
            _wardObservable.Value = 60f;

            // Assert
            Assert.AreEqual(0, eventCount, "No events should fire when staying in same tier");
        }

        // ── Additional Edge Case Tests ──────────────────────────────

        [Test]
        public void Initialization_SetsCorrectInitialTier_Stable()
        {
            // Arrange & Act: 100/100 = 100% (Stable)
            _wardObservable.Value = 100f;
            _sensoryTierService.Initialize(_wardObservable, 100f);

            // Assert
            Assert.AreEqual(SensoryTier.Stable, _sensoryTierService.CurrentTier.CurrentValue);
        }

        [Test]
        public void Initialization_SetsCorrectInitialTier_CreepingDread()
        {
            // Arrange & Act: 70/100 = 70% (CreepingDread)
            _wardObservable.Value = 70f;
            _sensoryTierService.Initialize(_wardObservable, 100f);

            // Assert
            Assert.AreEqual(SensoryTier.CreepingDread, _sensoryTierService.CurrentTier.CurrentValue);
        }

        [Test]
        public void Initialization_SetsCorrectInitialTier_DeathSpiral()
        {
            // Arrange & Act: 5/100 = 5%, but ≤10s rule applies
            _wardObservable.Value = 5f;
            _sensoryTierService.Initialize(_wardObservable, 100f);

            // Assert
            Assert.AreEqual(SensoryTier.DeathSpiral, _sensoryTierService.CurrentTier.CurrentValue);
        }

        [Test]
        public void TierCalculation_LowMaxWard_DeathSpiralBySeconds()
        {
            // Arrange: Max ward is 20, current is 8
            // 8/20 = 40% (would be Panic by %), but ≤10s rule takes precedence
            _wardObservable.Value = 20f;
            _sensoryTierService.Initialize(_wardObservable, 20f);

            SensoryTierChangedEvent? capturedEvent = null;
            _sensoryTierService.OnTierChanged.Subscribe(e => capturedEvent = e);

            // Act: Drop to 8 seconds
            _wardObservable.Value = 8f;

            // Assert
            Assert.IsTrue(capturedEvent.HasValue);
            Assert.AreEqual(SensoryTier.DeathSpiral, capturedEvent.Value.NewTier);
        }

        [Test]
        public void TierCalculation_HighMaxWard_UsesPercentage()
        {
            // Arrange: Max ward is 200, current is 100
            // 100/200 = 50% (HeavyBurden)
            _wardObservable.Value = 200f;
            _sensoryTierService.Initialize(_wardObservable, 200f);

            SensoryTierChangedEvent? capturedEvent = null;
            _sensoryTierService.OnTierChanged.Subscribe(e => capturedEvent = e);

            // Act: Drop to 100 seconds (50%)
            _wardObservable.Value = 100f;

            // Assert
            Assert.IsTrue(capturedEvent.HasValue);
            Assert.AreEqual(SensoryTier.HeavyBurden, capturedEvent.Value.NewTier);
        }
    }
}
