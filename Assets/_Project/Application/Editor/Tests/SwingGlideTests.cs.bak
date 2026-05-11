// Assets/_Project/Application/Editor/Tests/SwingGlideTests.cs
using System.Collections.Generic;
using NUnit.Framework;
using R3;
using SolarPhobia.Application.Services;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Tests
{
    /// <summary>
    /// Validates: Master GDD V5.0 Section 3.1 — Swing + Glide skills.
    /// Story 004-v2: Swing (-2s Ward) + Glide (-1s/sec Ward).
    /// </summary>
    [TestFixture]
    public class SwingGlideTests
    {
        private SwingGlideController _ctrl;
        private List<float> _wardCosts;

        private const float FullWard  = 100f;
        private const float ZeroWard  = 0f;
        private const float DeltaTime = 1.0f;

        [SetUp]
        public void Setup()
        {
            _ctrl = new SwingGlideController();
            _wardCosts = new List<float>();
            _ctrl.OnWardCostIncurred.Subscribe(c => _wardCosts.Add(c));
        }

        // ── Swing: AC-1 — Left Click near anchor activates Swing ──

        [Test]
        public void Swing_Input_NightMode_SufficientWard_SetsSwinging_True()
        {
            _ctrl.TrySwing(true, PlayerInputMode.NightMovement, FullWard);

            Assert.IsTrue(_ctrl.IsSwinging);
        }

        [Test]
        public void Swing_Activation_FiresWardCost_2Seconds()
        {
            _ctrl.TrySwing(true, PlayerInputMode.NightMovement, FullWard);

            Assert.AreEqual(1, _wardCosts.Count);
            Assert.AreEqual(SwingGlideController.DefaultSwingWardCost, _wardCosts[0], 0.001f);
        }

        [Test]
        public void Swing_DefaultCost_Is2Seconds()
        {
            Assert.AreEqual(2.0f, SwingGlideController.DefaultSwingWardCost);
        }

        [Test]
        public void Swing_BlockedInDayUI()
        {
            _ctrl.TrySwing(true, PlayerInputMode.DayUI, FullWard);

            Assert.IsFalse(_ctrl.IsSwinging);
            Assert.AreEqual(0, _wardCosts.Count);
        }

        [Test]
        public void Swing_BlockedInDisabled()
        {
            _ctrl.TrySwing(true, PlayerInputMode.Disabled, FullWard);

            Assert.IsFalse(_ctrl.IsSwinging);
        }

        [Test]
        public void Swing_BlockedWhenWardInsufficient()
        {
            float insufficientWard = SwingGlideController.DefaultSwingWardCost; // exactly equal = blocked

            _ctrl.TrySwing(true, PlayerInputMode.NightMovement, insufficientWard);

            Assert.IsFalse(_ctrl.IsSwinging);
            Assert.AreEqual(0, _wardCosts.Count);
        }

        [Test]
        public void Swing_BlockedWhenAlreadySwinging()
        {
            _ctrl.TrySwing(true, PlayerInputMode.NightMovement, FullWard); // first attach
            int costsBefore = _wardCosts.Count;

            _ctrl.TrySwing(true, PlayerInputMode.NightMovement, FullWard); // second attempt

            Assert.AreEqual(costsBefore, _wardCosts.Count, "No additional cost when already swinging");
        }

        [Test]
        public void Swing_NoInput_NoActivation()
        {
            _ctrl.TrySwing(false, PlayerInputMode.NightMovement, FullWard);

            Assert.IsFalse(_ctrl.IsSwinging);
        }

        [Test]
        public void ReleaseSwing_SetsSwinging_False()
        {
            _ctrl.TrySwing(true, PlayerInputMode.NightMovement, FullWard);
            _ctrl.ReleaseSwing();

            Assert.IsFalse(_ctrl.IsSwinging);
        }

        [Test]
        public void ReleaseSwing_AfterRelease_CanSwingAgain()
        {
            _ctrl.TrySwing(true, PlayerInputMode.NightMovement, FullWard);
            _ctrl.ReleaseSwing();
            _ctrl.TrySwing(true, PlayerInputMode.NightMovement, FullWard);

            Assert.IsTrue(_ctrl.IsSwinging);
            Assert.AreEqual(2, _wardCosts.Count);
        }

        // ── Glide: AC-2 — Hold while airborne activates Glide ─────

        [Test]
        public void Glide_HoldInput_Airborne_NightMode_SufficientWard_SetsGliding_True()
        {
            _ctrl.TickGlide(true, true, PlayerInputMode.NightMovement, FullWard, DeltaTime);

            Assert.IsTrue(_ctrl.IsGliding);
        }

        [Test]
        public void Glide_Tick_FiresWardCost_PerSecond()
        {
            _ctrl.TickGlide(true, true, PlayerInputMode.NightMovement, FullWard, DeltaTime);

            Assert.AreEqual(1, _wardCosts.Count);
            Assert.AreEqual(SwingGlideController.DefaultGlideWardCostPerSec * DeltaTime,
                _wardCosts[0], 0.001f);
        }

        [Test]
        public void Glide_DefaultCostPerSec_Is1Second()
        {
            Assert.AreEqual(1.0f, SwingGlideController.DefaultGlideWardCostPerSec);
        }

        [Test]
        public void Glide_CostScalesWithDeltaTime()
        {
            float dt = 0.5f;
            _ctrl.TickGlide(true, true, PlayerInputMode.NightMovement, FullWard, dt);

            Assert.AreEqual(0.5f, _wardCosts[0], 0.001f,
                "Glide cost must scale with deltaTime");
        }

        [Test]
        public void Glide_BlockedWhenNotAirborne()
        {
            _ctrl.TickGlide(true, false, PlayerInputMode.NightMovement, FullWard, DeltaTime);

            Assert.IsFalse(_ctrl.IsGliding);
            Assert.AreEqual(0, _wardCosts.Count);
        }

        [Test]
        public void Glide_BlockedInDayUI()
        {
            _ctrl.TickGlide(true, true, PlayerInputMode.DayUI, FullWard, DeltaTime);

            Assert.IsFalse(_ctrl.IsGliding);
        }

        [Test]
        public void Glide_AutoCancels_WhenWardZero()
        {
            _ctrl.TickGlide(true, true, PlayerInputMode.NightMovement, ZeroWard, DeltaTime);

            Assert.IsFalse(_ctrl.IsGliding,
                "Glide must auto-cancel when Ward reaches 0");
            Assert.AreEqual(0, _wardCosts.Count);
        }

        [Test]
        public void Glide_StopsWhenInputReleased()
        {
            _ctrl.TickGlide(true,  true, PlayerInputMode.NightMovement, FullWard, DeltaTime);
            _ctrl.TickGlide(false, true, PlayerInputMode.NightMovement, FullWard, DeltaTime);

            Assert.IsFalse(_ctrl.IsGliding);
        }

        [Test]
        public void Glide_MultipleFrames_AccumulatesCost()
        {
            _ctrl.TickGlide(true, true, PlayerInputMode.NightMovement, FullWard, 0.5f);
            _ctrl.TickGlide(true, true, PlayerInputMode.NightMovement, FullWard, 0.5f);

            Assert.AreEqual(2, _wardCosts.Count);
            float total = _wardCosts[0] + _wardCosts[1];
            Assert.AreEqual(1.0f, total, 0.001f, "Two 0.5s frames = 1.0s total cost");
        }
    }
}
