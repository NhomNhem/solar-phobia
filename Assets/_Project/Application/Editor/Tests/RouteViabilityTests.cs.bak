// Assets/_Project/Application/Editor/Tests/RouteViabilityTests.cs
using NUnit.Framework;
using SolarPhobia.Application.Services.Map;

namespace SolarPhobia.Application.Tests
{
    /// <summary>
    /// Validates: TR-map-005 — Route Viability Check (fairness guard).
    /// Story 004: is_viable = ward_remaining > (distance / speed) + safety_buffer.
    /// </summary>
    [TestFixture]
    public class RouteViabilityTests
    {
        // ── AC-1: Viable when Ward > travel time + buffer ──────────

        [Test]
        public void AC1_SufficientWard_IsViable_True()
        {
            // distance=100, speed=5 → travel=20s; buffer=12; need >32s; ward=50 ✓
            bool result = RouteViabilityCalculator.IsRouteViable(
                wardRemaining: 50f, distanceToGoal: 100f,
                effectiveMoveSpeed: 5f, safetyBufferSec: 12f);

            Assert.IsTrue(result);
        }

        [Test]
        public void AC1_ExactlyAtThreshold_IsViable_False()
        {
            // ward == travel + buffer → NOT viable (need strictly greater)
            bool result = RouteViabilityCalculator.IsRouteViable(
                wardRemaining: 32f, distanceToGoal: 100f,
                effectiveMoveSpeed: 5f, safetyBufferSec: 12f);

            Assert.IsFalse(result, "Ward exactly equal to threshold must not be viable");
        }

        // ── AC-2: Not viable when Ward insufficient ────────────────

        [Test]
        public void AC2_InsufficientWard_IsViable_False()
        {
            // ward=10, travel=20, buffer=12 → need >32, have 10 → not viable
            bool result = RouteViabilityCalculator.IsRouteViable(
                wardRemaining: 10f, distanceToGoal: 100f,
                effectiveMoveSpeed: 5f, safetyBufferSec: 12f);

            Assert.IsFalse(result);
        }

        [Test]
        public void AC2_ZeroWard_IsViable_False()
        {
            bool result = RouteViabilityCalculator.IsRouteViable(
                wardRemaining: 0f, distanceToGoal: 50f,
                effectiveMoveSpeed: 5f);

            Assert.IsFalse(result);
        }

        // ── AC-3: SafetyBufferSec configurable ────────────────────

        [Test]
        public void AC3_DefaultSafetyBuffer_Is12Seconds()
        {
            Assert.AreEqual(12f, RouteViabilityCalculator.DefaultSafetyBufferSec);
        }

        [Test]
        public void AC3_LargerBuffer_MakesRouteHarderToBeViable()
        {
            // With buffer=12: ward=33 > 20+12=32 → viable
            bool withSmallBuffer = RouteViabilityCalculator.IsRouteViable(
                33f, 100f, 5f, safetyBufferSec: 12f);

            // With buffer=20: ward=33 > 20+20=40 → not viable
            bool withLargeBuffer = RouteViabilityCalculator.IsRouteViable(
                33f, 100f, 5f, safetyBufferSec: 20f);

            Assert.IsTrue(withSmallBuffer);
            Assert.IsFalse(withLargeBuffer);
        }

        // ── AC-4: Zero speed → not viable ─────────────────────────

        [Test]
        public void AC4_ZeroSpeed_IsViable_False()
        {
            bool result = RouteViabilityCalculator.IsRouteViable(
                wardRemaining: 999f, distanceToGoal: 100f,
                effectiveMoveSpeed: 0f);

            Assert.IsFalse(result, "Zero speed means player cannot move — not viable");
        }

        // ── Formula verification ───────────────────────────────────

        [Test]
        public void Formula_TravelTime_IsDistanceDividedBySpeed()
        {
            // distance=50, speed=10 → travel=5s; buffer=12; need >17s
            bool result = RouteViabilityCalculator.IsRouteViable(
                wardRemaining: 18f, distanceToGoal: 50f,
                effectiveMoveSpeed: 10f, safetyBufferSec: 12f);

            Assert.IsTrue(result, "18 > 5+12=17 → viable");
        }
    }
}
