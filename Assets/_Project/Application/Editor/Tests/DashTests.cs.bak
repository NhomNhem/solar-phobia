// Assets/_Project/Application/Editor/Tests/DashTests.cs
using System.Collections.Generic;
using NUnit.Framework;
using R3;
using SolarPhobia.Application.Services;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Tests
{
    /// <summary>
    /// Validates: Master GDD V5.0 Section 3.1 — Spirit Dash (Khăn Tang burst skill).
    /// Story 003-v2: Spirit Dash — Ward cost -5.0s, cooldown, phase gate.
    /// </summary>
    [TestFixture]
    public class DashTests
    {
        private DashController _dash;
        private List<float> _wardCosts;

        private const float FullWard    = 100f;
        private const float ExactCost   = DashController.DefaultDashWardCost;      // 5.0f
        private const float BelowCost   = DashController.DefaultDashWardCost - 0.1f; // 4.9f
        private const float AboveCost   = DashController.DefaultDashWardCost + 0.1f; // 5.1f

        [SetUp]
        public void Setup()
        {
            _dash = new DashController();
            _wardCosts = new List<float>();
            _dash.OnWardCostIncurred.Subscribe(c => _wardCosts.Add(c));
        }

        // ── AC-1: Shift triggers Spirit Dash ──────────────────────

        [Test]
        public void AC1_DashInput_NightMode_SufficientWard_SetsDashing_True()
        {
            _dash.TryDash(dashInput: true, PlayerInputMode.NightMovement, FullWard);

            Assert.IsTrue(_dash.IsDashing);
        }

        [Test]
        public void AC1_DashInput_FiresWardCostEvent()
        {
            _dash.TryDash(true, PlayerInputMode.NightMovement, FullWard);

            Assert.AreEqual(1, _wardCosts.Count);
        }

        // ── AC-2: Spirit Dash costs -5.0s Ward ────────────────────

        [Test]
        public void AC2_WardCostEvent_Payload_Is5Seconds()
        {
            _dash.TryDash(true, PlayerInputMode.NightMovement, FullWard);

            Assert.AreEqual(DashController.DefaultDashWardCost, _wardCosts[0], 0.001f,
                "Ward cost must be exactly 5.0s per dash");
        }

        [Test]
        public void AC2_DefaultDashWardCost_Is5()
        {
            Assert.AreEqual(5.0f, DashController.DefaultDashWardCost);
        }

        // ── AC-3: OnWardCostIncurred fires on each dash ────────────

        [Test]
        public void AC3_TwoDashes_FiresTwoEvents()
        {
            _dash.TryDash(true, PlayerInputMode.NightMovement, FullWard);
            _dash.Tick(DashController.DefaultDashCooldown + 0.1f); // expire cooldown
            _dash.TryDash(true, PlayerInputMode.NightMovement, FullWard);

            Assert.AreEqual(2, _wardCosts.Count);
        }

        [Test]
        public void AC3_NoDashInput_NoEvent()
        {
            _dash.TryDash(false, PlayerInputMode.NightMovement, FullWard);

            Assert.AreEqual(0, _wardCosts.Count);
        }

        // ── AC-4: Dash only active in NightSurvival ───────────────

        [Test]
        public void AC4_DayUI_DashInput_NoActivation()
        {
            _dash.TryDash(true, PlayerInputMode.DayUI, FullWard);

            Assert.IsFalse(_dash.IsDashing);
            Assert.AreEqual(0, _wardCosts.Count);
        }

        [Test]
        public void AC4_Disabled_DashInput_NoActivation()
        {
            _dash.TryDash(true, PlayerInputMode.Disabled, FullWard);

            Assert.IsFalse(_dash.IsDashing);
        }

        // ── AC-5: Cooldown prevents spam ──────────────────────────

        [Test]
        public void AC5_DefaultCooldown_Is0p5Seconds()
        {
            Assert.AreEqual(0.5f, DashController.DefaultDashCooldown);
        }

        [Test]
        public void AC5_SecondDash_BlockedDuringCooldown()
        {
            _dash.TryDash(true, PlayerInputMode.NightMovement, FullWard); // first dash
            _dash.Tick(0.1f); // cooldown not expired yet

            _dash.TryDash(true, PlayerInputMode.NightMovement, FullWard); // second attempt

            Assert.AreEqual(1, _wardCosts.Count, "Second dash must be blocked during cooldown");
        }

        [Test]
        public void AC5_DashAllowed_AfterCooldownExpires()
        {
            _dash.TryDash(true, PlayerInputMode.NightMovement, FullWard);
            _dash.Tick(DashController.DefaultDashCooldown + 0.01f); // expire cooldown

            Assert.IsTrue(_dash.CanDash);

            _dash.TryDash(true, PlayerInputMode.NightMovement, FullWard);

            Assert.AreEqual(2, _wardCosts.Count, "Dash must be allowed after cooldown expires");
        }

        [Test]
        public void AC5_CanDash_True_Initially()
        {
            Assert.IsTrue(_dash.CanDash, "Dash should be available at start (no cooldown)");
        }

        [Test]
        public void AC5_CanDash_False_DuringCooldown()
        {
            _dash.TryDash(true, PlayerInputMode.NightMovement, FullWard);

            Assert.IsFalse(_dash.CanDash, "CanDash must be false immediately after dashing");
        }

        [Test]
        public void AC5_CooldownConfigurable()
        {
            _dash.DashCooldown = 1.0f;
            _dash.TryDash(true, PlayerInputMode.NightMovement, FullWard);
            _dash.Tick(0.5f); // half of 1.0s cooldown

            Assert.IsFalse(_dash.CanDash);

            _dash.Tick(0.6f); // now past 1.0s

            Assert.IsTrue(_dash.CanDash);
        }

        // ── AC-6: Dash blocked when Ward insufficient ─────────────

        [Test]
        public void AC6_Ward_ExactlyEqualToCost_BlocksDash()
        {
            // Ward <= cost → blocked (need strictly more)
            _dash.TryDash(true, PlayerInputMode.NightMovement, ExactCost);

            Assert.IsFalse(_dash.IsDashing,
                "Dash must be blocked when Ward == cost (cannot afford)");
            Assert.AreEqual(0, _wardCosts.Count);
        }

        [Test]
        public void AC6_Ward_BelowCost_BlocksDash()
        {
            _dash.TryDash(true, PlayerInputMode.NightMovement, BelowCost);

            Assert.IsFalse(_dash.IsDashing);
            Assert.AreEqual(0, _wardCosts.Count);
        }

        [Test]
        public void AC6_Ward_SlightlyAboveCost_AllowsDash()
        {
            _dash.TryDash(true, PlayerInputMode.NightMovement, AboveCost);

            Assert.IsTrue(_dash.IsDashing);
            Assert.AreEqual(1, _wardCosts.Count);
        }

        [Test]
        public void AC6_Ward_Zero_BlocksDash()
        {
            _dash.TryDash(true, PlayerInputMode.NightMovement, 0f);

            Assert.IsFalse(_dash.IsDashing);
        }

        // ── IsDashing is one-frame flag ───────────────────────────

        [Test]
        public void IsDashing_ResetToFalse_OnNextTryDash_WithNoInput()
        {
            _dash.TryDash(true, PlayerInputMode.NightMovement, FullWard);
            Assert.IsTrue(_dash.IsDashing);

            // Next frame — no input
            _dash.TryDash(false, PlayerInputMode.NightMovement, FullWard);

            Assert.IsFalse(_dash.IsDashing, "IsDashing must reset when no dash input");
        }

        // ── Cooldown clamping ─────────────────────────────────────

        [Test]
        public void CooldownBelowMin_ClampedTo0p1()
        {
            _dash.DashCooldown = 0f;

            Assert.AreEqual(DashController.MinDashCooldown, _dash.DashCooldown);
        }

        [Test]
        public void CooldownAboveMax_ClampedTo2()
        {
            _dash.DashCooldown = 99f;

            Assert.AreEqual(DashController.MaxDashCooldown, _dash.DashCooldown);
        }
    }
}
