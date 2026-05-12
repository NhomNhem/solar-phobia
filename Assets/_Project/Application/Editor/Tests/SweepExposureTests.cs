// Assets/_Project/Application/Editor/Tests/SweepExposureTests.cs
using NUnit.Framework;
using SolarPhobia.Application.Map.Analysis;

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

namespace SolarPhobia.Application.Tests
{
    /// <summary>
    /// Validates: TR-map-003 â€” Sweep Exposure Check.
    /// Story 002: is_exposed = in_sweep_cone AND (not in_valid_cover).
    /// </summary>
    [TestFixture]
    public class SweepExposureTests
    {
        // â”€â”€ AC-1: In cone + not in cover â†’ exposed â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [Test]
        public void AC1_InCone_NotInCover_IsExposed_True()
        {
            bool result = SweepExposureCalculator.IsExposed(
                inSweepCone: true, inValidCover: false);

            Assert.IsTrue(result);
        }

        // â”€â”€ AC-2: In cone + in cover â†’ not exposed â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [Test]
        public void AC2_InCone_InCover_IsExposed_False()
        {
            bool result = SweepExposureCalculator.IsExposed(
                inSweepCone: true, inValidCover: true);

            Assert.IsFalse(result, "Player fully in cover must not be exposed");
        }

        // â”€â”€ AC-3: Outside cone â†’ never exposed â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

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

        // â”€â”€ AC-4: Missing cover treated as not in cover â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [Test]
        public void AC4_MissingCover_InCone_IsExposed_True()
        {
            // inValidCover = false represents missing cover collider
            bool result = SweepExposureCalculator.IsExposed(
                inSweepCone: true, inValidCover: false);

            Assert.IsTrue(result,
                "Missing cover collider must be treated as exposed");
        }

        // â”€â”€ AC-5: Shrine safe zone suppresses exposure â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

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
            // Default inShrineZone = false â€” normal exposure rules apply
            bool result = SweepExposureCalculator.IsExposed(
                inSweepCone: true, inValidCover: false);

            Assert.IsTrue(result);
        }
    }
}

