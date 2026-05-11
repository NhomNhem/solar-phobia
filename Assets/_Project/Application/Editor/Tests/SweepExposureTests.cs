// Assets/_Project/Application/Editor/Tests/SweepExposureTests.cs
using NUnit.Framework;
using SolarPhobia.Application.Services.Map;

namespace SolarPhobia.Application.Tests
{
    /// <summary>
    /// Validates: TR-map-003 — Sweep Exposure Check.
    /// Story 002: is_exposed = in_sweep_cone AND (not in_valid_cover).
    /// </summary>
    [TestFixture]
    public class SweepExposureTests
    {
        // ── AC-1: In cone + not in cover → exposed ─────────────────

        [Test]
        public void AC1_InCone_NotInCover_IsExposed_True()
        {
            bool result = SweepExposureCalculator.IsExposed(
                inSweepCone: true, inValidCover: false);

            Assert.IsTrue(result);
        }

        // ── AC-2: In cone + in cover → not exposed ─────────────────

        [Test]
        public void AC2_InCone_InCover_IsExposed_False()
        {
            bool result = SweepExposureCalculator.IsExposed(
                inSweepCone: true, inValidCover: true);

            Assert.IsFalse(result, "Player fully in cover must not be exposed");
        }

        // ── AC-3: Outside cone → never exposed ─────────────────────

        [Test]
        public void AC3_OutsideCone_NotInCover_IsExposed_False()
        {
            bool result = SweepExposureCalculator.IsExposed(
                inSweepCone: false, inValidCover: false);

            Assert.IsFalse(result, "Player outside sweep cone must never be exposed");
        }

        [Test]
        public void AC3_OutsideCone_InCover_IsExposed_False()
        {
            bool result = SweepExposureCalculator.IsExposed(
                inSweepCone: false, inValidCover: true);

            Assert.IsFalse(result);
        }

        // ── AC-4: Missing cover treated as not in cover ────────────

        [Test]
        public void AC4_MissingCover_InCone_IsExposed_True()
        {
            // inValidCover = false represents missing cover collider
            bool result = SweepExposureCalculator.IsExposed(
                inSweepCone: true, inValidCover: false);

            Assert.IsTrue(result,
                "Missing cover collider must be treated as exposed");
        }

        // ── AC-5: Shrine safe zone suppresses exposure ─────────────

        [Test]
        public void AC5_InShrineZone_InCone_NotInCover_IsExposed_False()
        {
            bool result = SweepExposureCalculator.IsExposed(
                inSweepCone: true, inValidCover: false, inShrineZone: true);

            Assert.IsFalse(result,
                "Strike must never apply inside shrine safe zone");
        }

        [Test]
        public void AC5_InShrineZone_InCone_InCover_IsExposed_False()
        {
            bool result = SweepExposureCalculator.IsExposed(
                inSweepCone: true, inValidCover: true, inShrineZone: true);

            Assert.IsFalse(result);
        }

        [Test]
        public void AC5_NotInShrineZone_Default_BehaviorUnchanged()
        {
            // Default inShrineZone = false — normal exposure rules apply
            bool result = SweepExposureCalculator.IsExposed(
                inSweepCone: true, inValidCover: false);

            Assert.IsTrue(result);
        }
    }
}
