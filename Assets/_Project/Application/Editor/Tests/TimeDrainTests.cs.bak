// Assets/_Project/Application/Editor/Tests/TimeDrainTests.cs
using NUnit.Framework;
using SolarPhobia.Application.Services.Map;

namespace SolarPhobia.Application.Tests
{
    /// <summary>
    /// Validates: TR-map-008 — Bone Relic Time Drain.
    /// Story 007: effective_drain = base * (1 + bones * hallucination_multiplier).
    /// </summary>
    [TestFixture]
    public class TimeDrainTests
    {
        // ── AC-1: Formula correctness ──────────────────────────────

        [Test]
        public void AC1_NoBones_DrainEqualsBase()
        {
            float drain = TimeDrainCalculator.CalculateEffectiveDrain(
                baseDrainRate: 1.0f, bonesCarried: 0);

            Assert.AreEqual(1.0f, drain, 0.001f,
                "With 0 bones, drain must equal base rate");
        }

        [Test]
        public void AC1_OneBone_DefaultMultiplier_Drain1p5()
        {
            // 1.0 * (1 + 1 * 0.5) = 1.5
            float drain = TimeDrainCalculator.CalculateEffectiveDrain(
                baseDrainRate: 1.0f, bonesCarried: 1,
                hallucinationMultiplier: 0.5f);

            Assert.AreEqual(1.5f, drain, 0.001f);
        }

        [Test]
        public void AC1_TwoBones_DefaultMultiplier_Drain2p0()
        {
            // 1.0 * (1 + 2 * 0.5) = 2.0
            float drain = TimeDrainCalculator.CalculateEffectiveDrain(
                baseDrainRate: 1.0f, bonesCarried: 2,
                hallucinationMultiplier: 0.5f);

            Assert.AreEqual(2.0f, drain, 0.001f);
        }

        [Test]
        public void AC1_ThreeBones_DefaultMultiplier_Drain2p5()
        {
            // 1.0 * (1 + 3 * 0.5) = 2.5
            float drain = TimeDrainCalculator.CalculateEffectiveDrain(
                baseDrainRate: 1.0f, bonesCarried: 3,
                hallucinationMultiplier: 0.5f);

            Assert.AreEqual(2.5f, drain, 0.001f);
        }

        // ── AC-2: Configurable parameters ─────────────────────────

        [Test]
        public void AC2_DefaultBaseDrainRate_Is1()
        {
            Assert.AreEqual(1.0f, TimeDrainCalculator.DefaultBaseDrainRate);
        }

        [Test]
        public void AC2_DefaultHallucinationMultiplier_Is0p5()
        {
            Assert.AreEqual(0.5f, TimeDrainCalculator.DefaultHallucinationMultiplier);
        }

        [Test]
        public void AC2_CustomBaseDrainRate_ScalesResult()
        {
            // base=2.0, bones=1, mult=0.5 → 2.0 * 1.5 = 3.0
            float drain = TimeDrainCalculator.CalculateEffectiveDrain(
                baseDrainRate: 2.0f, bonesCarried: 1,
                hallucinationMultiplier: 0.5f);

            Assert.AreEqual(3.0f, drain, 0.001f);
        }

        [Test]
        public void AC2_CustomMultiplier_AffectsResult()
        {
            // base=1.0, bones=1, mult=1.0 → 1.0 * 2.0 = 2.0
            float drain = TimeDrainCalculator.CalculateEffectiveDrain(
                baseDrainRate: 1.0f, bonesCarried: 1,
                hallucinationMultiplier: 1.0f);

            Assert.AreEqual(2.0f, drain, 0.001f);
        }

        // ── AC-3: Clamped to max safe cap ─────────────────────────

        [Test]
        public void AC3_ExtremeValues_ClampedToMaxSafeDrainRate()
        {
            // base=10, bones=3, mult=3.0 → 10 * (1+9) = 100 → clamped to 20
            float drain = TimeDrainCalculator.CalculateEffectiveDrain(
                baseDrainRate: 10f, bonesCarried: 3,
                hallucinationMultiplier: 3.0f);

            Assert.AreEqual(TimeDrainCalculator.MaxSafeDrainRate, drain, 0.001f,
                "Effective drain must be clamped to MaxSafeDrainRate");
        }

        [Test]
        public void AC3_MaxSafeDrainRate_Is20()
        {
            Assert.AreEqual(20f, TimeDrainCalculator.MaxSafeDrainRate);
        }

        // ── Default parameter usage ────────────────────────────────

        [Test]
        public void DefaultMultiplier_UsedWhenNotSpecified()
        {
            float withDefault  = TimeDrainCalculator.CalculateEffectiveDrain(1f, 1);
            float withExplicit = TimeDrainCalculator.CalculateEffectiveDrain(
                1f, 1, TimeDrainCalculator.DefaultHallucinationMultiplier);

            Assert.AreEqual(withExplicit, withDefault, 0.001f);
        }
    }
}
