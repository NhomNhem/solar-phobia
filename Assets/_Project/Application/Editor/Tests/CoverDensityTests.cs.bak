// Assets/_Project/Application/Editor/Tests/CoverDensityTests.cs
using NUnit.Framework;
using SolarPhobia.Application.Services.Map;

namespace SolarPhobia.Application.Tests
{
    /// <summary>
    /// Validates: TR-map-006 — Cover Density Validation.
    /// Story 005: cover_density = mo_thuong_count / lane_length (target 0.02–0.08).
    /// </summary>
    [TestFixture]
    public class CoverDensityTests
    {
        // ── AC-1: Valid density range ──────────────────────────────

        [Test]
        public void AC1_DensityInRange_IsValid_True()
        {
            // 14 mounds / 200 units = 0.07 → in range [0.02, 0.08]
            bool result = CoverDensityValidator.IsValid(14, 200f);

            Assert.IsTrue(result);
        }

        [Test]
        public void AC1_DefaultCount_OnTypicalLane_IsValid()
        {
            // Default 14 mounds on 200-unit lane = 0.07 ✓
            bool result = CoverDensityValidator.IsValid(
                CoverDensityValidator.DefaultMoThuongCount, 200f);

            Assert.IsTrue(result);
        }

        // ── AC-2: Below minimum → invalid ─────────────────────────

        [Test]
        public void AC2_BelowMinDensity_IsValid_False()
        {
            // 1 mound / 200 units = 0.005 < 0.02 → invalid
            bool result = CoverDensityValidator.IsValid(1, 200f);

            Assert.IsFalse(result, "Density below minimum must be invalid");
        }

        [Test]
        public void AC2_ZeroMounds_IsValid_False()
        {
            bool result = CoverDensityValidator.IsValid(0, 200f);

            Assert.IsFalse(result);
        }

        // ── AC-3: Above maximum → invalid ─────────────────────────

        [Test]
        public void AC3_AboveMaxDensity_IsValid_False()
        {
            // 100 mounds / 200 units = 0.5 > 0.08 → invalid
            bool result = CoverDensityValidator.IsValid(100, 200f);

            Assert.IsFalse(result, "Density above maximum must be invalid");
        }

        // ── AC-4: Edge cases ───────────────────────────────────────

        [Test]
        public void AC4_ZeroLaneLength_IsValid_False()
        {
            bool result = CoverDensityValidator.IsValid(14, 0f);

            Assert.IsFalse(result, "Zero lane length must be invalid");
        }

        [Test]
        public void AC4_ExactlyAtMinDensity_IsValid_True()
        {
            // 4 mounds / 200 units = 0.02 = MinDensity → valid (inclusive)
            bool result = CoverDensityValidator.IsValid(4, 200f);

            Assert.IsTrue(result, "Exactly at minimum density must be valid");
        }

        [Test]
        public void AC4_ExactlyAtMaxDensity_IsValid_True()
        {
            // 16 mounds / 200 units = 0.08 = MaxDensity → valid (inclusive)
            bool result = CoverDensityValidator.IsValid(16, 200f);

            Assert.IsTrue(result, "Exactly at maximum density must be valid");
        }

        // ── CalculateDensity ───────────────────────────────────────

        [Test]
        public void CalculateDensity_ReturnsCorrectValue()
        {
            float density = CoverDensityValidator.CalculateDensity(14, 200f);

            Assert.AreEqual(0.07f, density, 0.001f);
        }

        [Test]
        public void CalculateDensity_ZeroLane_ReturnsZero()
        {
            float density = CoverDensityValidator.CalculateDensity(14, 0f);

            Assert.AreEqual(0f, density);
        }

        // ── MinCountForLane ────────────────────────────────────────

        [Test]
        public void MinCountForLane_Returns4_For200Units()
        {
            // 0.02 * 200 = 4
            int min = CoverDensityValidator.MinCountForLane(200f);

            Assert.AreEqual(4, min);
        }

        [Test]
        public void DefaultMoThuongCount_Is14()
        {
            Assert.AreEqual(14, CoverDensityValidator.DefaultMoThuongCount);
        }
    }
}
