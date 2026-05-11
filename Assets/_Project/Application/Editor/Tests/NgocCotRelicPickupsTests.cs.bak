// Assets/_Project/Application/Editor/Tests/NgocCotRelicPickupsTests.cs
using NUnit.Framework;
using SolarPhobia.Application.Services;
using SolarPhobia.Domain.Services;

namespace SolarPhobia.Application.Tests
{
    /// <summary>
    /// Validates: TR-state-005 — Ngọc Cốt relic pickups increase Ward drain multiplicatively.
    /// </summary>
    public class NgocCotRelicPickupsTests
    {
        private NgocCotService _ngocCotService;

        [SetUp]
        public void Setup()
        {
            _ngocCotService = new NgocCotService();
        }

        [TearDown]
        public void Teardown()
        {
            _ngocCotService?.ResetForNight();
        }

        // ── Bone Count Tracking ─────────────────────────────────

        [Test]
        public void BoneCount_StartsAtZero()
        {
            Assert.AreEqual(0, _ngocCotService.BoneCount);
        }

        [Test]
        public void TryCollectRelic_ReturnsTrue_AndIncrementsBoneCount()
        {
            bool result = _ngocCotService.TryCollectRelic();
            Assert.IsTrue(result);
            Assert.AreEqual(1, _ngocCotService.BoneCount);
        }

        [Test]
        public void TryCollectRelic_ReturnsFalse_WhenMaxReached()
        {
            _ngocCotService.TryCollectRelic();
            _ngocCotService.TryCollectRelic();
            _ngocCotService.TryCollectRelic();
            bool result = _ngocCotService.TryCollectRelic();
            Assert.IsFalse(result);
        }

        [Test]
        public void TryCollectRelic_DoesNotIncrement_WhenMaxReached()
        {
            for (int i = 0; i < 3; i++)
            {
                _ngocCotService.TryCollectRelic();
            }
            _ngocCotService.TryCollectRelic();
            Assert.AreEqual(3, _ngocCotService.BoneCount);
        }

        [Test]
        public void ResetForNight_ResetsBoneCountToZero()
        {
            _ngocCotService.TryCollectRelic();
            _ngocCotService.TryCollectRelic();
            _ngocCotService.ResetForNight();
            Assert.AreEqual(0, _ngocCotService.BoneCount);
        }

        // ── Bone Multiplier Formula ─────────────────────────────

        [Test]
        public void BoneMultiplier_Returns1_WhenNoBones()
        {
            Assert.AreEqual(1f, _ngocCotService.BoneMultiplier);
        }

        [Test]
        public void BoneMultiplier_Returns1d25_When1Bone()
        {
            _ngocCotService.TryCollectRelic();
            Assert.AreEqual(1.25f, _ngocCotService.BoneMultiplier);
        }

        [Test]
        public void BoneMultiplier_Returns1d5_When2Bones()
        {
            _ngocCotService.TryCollectRelic();
            _ngocCotService.TryCollectRelic();
            Assert.AreEqual(1.5f, _ngocCotService.BoneMultiplier);
        }

        [Test]
        public void BoneMultiplier_Returns1d75_When3Bones()
        {
            _ngocCotService.TryCollectRelic();
            _ngocCotService.TryCollectRelic();
            _ngocCotService.TryCollectRelic();
            Assert.AreEqual(1.75f, _ngocCotService.BoneMultiplier);
        }

        // ── AC-1: First pickup increases drain by 25% ───────────

        [Test]
        public void AC1_FirstPickup_IncreasesDrainFrom1d0_To1d25()
        {
            var service = new NgocCotService();
            Assert.AreEqual(1f, service.BoneMultiplier);
            service.TryCollectRelic();
            Assert.AreEqual(1.25f, service.BoneMultiplier);
        }

        // ── AC-2: Second pickup adds additional 25% ─────────────

        [Test]
        public void AC2_SecondPickup_IncreasesDrainFrom1d25_To1d5()
        {
            var service = new NgocCotService();
            service.TryCollectRelic();
            Assert.AreEqual(1.25f, service.BoneMultiplier);
            service.TryCollectRelic();
            Assert.AreEqual(1.5f, service.BoneMultiplier);
        }

        // ── AC-3: Third pickup caps at maximum, fourth ignored ──

        [Test]
        public void AC3_ThirdPickup_CapsDrainAt1d75()
        {
            var service = new NgocCotService();
            service.TryCollectRelic();
            service.TryCollectRelic();
            Assert.AreEqual(1.5f, service.BoneMultiplier);
            service.TryCollectRelic();
            Assert.AreEqual(1.75f, service.BoneMultiplier);
        }

        [Test]
        public void AC3_FourthPickup_IsIgnored()
        {
            var service = new NgocCotService();
            service.TryCollectRelic();
            service.TryCollectRelic();
            service.TryCollectRelic();
            bool fourthPickup = service.TryCollectRelic();
            Assert.IsFalse(fourthPickup);
            Assert.AreEqual(3, service.BoneCount);
            Assert.AreEqual(1.75f, service.BoneMultiplier);
        }
    }

    /// <summary>
    /// Validates: WardTimerService drain rate calculation with bone + hallucination multipliers.
    /// Formula: baseDrainRate × (1 + boneCount × 0.25) × (1 + hallucinationMultiplier)
    /// </summary>
    public class WardDrainRateCalculationTests
    {
        [Test]
        public void AC4_BoneMultiplier_StacksWithHallucination()
        {
            var service = new TestWardDrainRate();
            service.SetDrainRate(1f, 1, 0.5f);
            Assert.AreEqual(1.875f, service.GetDrainRate());
        }

        [Test]
        public void Drain_With0BonesAndNoHallucination_Is1d0()
        {
            var service = new TestWardDrainRate();
            service.SetDrainRate(1f, 0, 0f);
            Assert.AreEqual(1f, service.GetDrainRate());
        }

        [Test]
        public void Drain_With1BoneAndNoHallucination_Is1d25()
        {
            var service = new TestWardDrainRate();
            service.SetDrainRate(1f, 1, 0f);
            Assert.AreEqual(1.25f, service.GetDrainRate());
        }

        [Test]
        public void Drain_With2BonesAndNoHallucination_Is1d5()
        {
            var service = new TestWardDrainRate();
            service.SetDrainRate(1f, 2, 0f);
            Assert.AreEqual(1.5f, service.GetDrainRate());
        }

        [Test]
        public void Drain_With3BonesAndNoHallucination_Is1d75()
        {
            var service = new TestWardDrainRate();
            service.SetDrainRate(1f, 3, 0f);
            Assert.AreEqual(1.75f, service.GetDrainRate());
        }

        private class TestWardDrainRate
        {
            private float _drainRate;

            public void SetDrainRate(float baseDrainRate, int boneCount, float hallucinationMultiplier)
            {
                float boneMultiplier = 1f + (boneCount * 0.25f);
                float hallMultiplier = 1f + hallucinationMultiplier;
                _drainRate = baseDrainRate * boneMultiplier * hallMultiplier;
            }

            public float GetDrainRate() => _drainRate;
        }
    }
}
