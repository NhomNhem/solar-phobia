// Assets/_Project/Application/Editor/Tests/StrikeTests.cs
using System.Collections.Generic;
using NUnit.Framework;
using R3;
using SolarPhobia.Application.Services;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Tests
{
    /// <summary>
    /// Validates: TR-map-004 — Strike Telegraph + Penalty.
    /// Story 003: Strike telegraph fires OnStrikeWarning, resolves with -30s Ward cost.
    /// </summary>
    [TestFixture]
    public class StrikeTests
    {
        private StrikeController _strike;
        private List<bool>  _warnings;
        private List<float> _wardCosts;

        private const float DeltaSmall = 0.016f;

        [SetUp]
        public void Setup()
        {
            _strike    = new StrikeController();
            _warnings  = new List<bool>();
            _wardCosts = new List<float>();
            _strike.OnStrikeWarning.Subscribe(v    => _warnings.Add(v));
            _strike.OnWardCostIncurred.Subscribe(c => _wardCosts.Add(c));
        }

        // ── AC-1: Telegraph fires OnStrikeWarning ──────────────────

        [Test]
        public void AC1_Exposed_NightMode_StartsTelegraph()
        {
            _strike.Tick(isExposed: true, inShrineZone: false,
                         PlayerInputMode.NightMovement, DeltaSmall);

            Assert.IsTrue(_strike.IsTelegraphActive);
        }

        [Test]
        public void AC1_Exposed_FiresOnStrikeWarning_True()
        {
            _strike.Tick(true, false, PlayerInputMode.NightMovement, DeltaSmall);

            Assert.AreEqual(1, _warnings.Count);
            Assert.IsTrue(_warnings[0]);
        }

        // ── AC-2: Unresolved exposure → OnWardCostIncurred ─────────

        [Test]
        public void AC2_StillExposed_AfterTelegraph_FiresWardCost()
        {
            _strike.Tick(true, false, PlayerInputMode.NightMovement, DeltaSmall);

            // Advance past full telegraph duration
            _strike.Tick(true, false, PlayerInputMode.NightMovement,
                         StrikeController.DefaultStrikeTelegraphSec + 0.1f);

            Assert.AreEqual(1, _wardCosts.Count);
            Assert.AreEqual(StrikeController.DefaultStrikeTimePenaltySec,
                            _wardCosts[0], 0.001f);
        }

        [Test]
        public void AC2_DefaultPenalty_Is30Seconds()
        {
            Assert.AreEqual(30f, StrikeController.DefaultStrikeTimePenaltySec);
        }

        [Test]
        public void AC2_DefaultTelegraph_Is1p5Seconds()
        {
            Assert.AreEqual(1.5f, StrikeController.DefaultStrikeTelegraphSec);
        }

        // ── AC-3: Player takes cover → strike cancelled ────────────

        [Test]
        public void AC3_TookCover_BeforeTelegraphExpires_CancelsStrike()
        {
            _strike.Tick(true,  false, PlayerInputMode.NightMovement, DeltaSmall); // start
            _strike.Tick(false, false, PlayerInputMode.NightMovement, DeltaSmall); // took cover

            Assert.IsFalse(_strike.IsTelegraphActive);
            Assert.AreEqual(0, _wardCosts.Count, "No Ward cost when player takes cover in time");
        }

        [Test]
        public void AC3_TookCover_FiresOnStrikeWarning_False()
        {
            _strike.Tick(true,  false, PlayerInputMode.NightMovement, DeltaSmall);
            _strike.Tick(false, false, PlayerInputMode.NightMovement, DeltaSmall);

            Assert.AreEqual(2, _warnings.Count);
            Assert.IsFalse(_warnings[1], "Warning must clear when telegraph cancelled");
        }

        // ── AC-4: Strike never fires in shrine safe zone ───────────

        [Test]
        public void AC4_InShrineZone_Exposed_NoTelegraph()
        {
            _strike.Tick(true, inShrineZone: true,
                         PlayerInputMode.NightMovement, DeltaSmall);

            Assert.IsFalse(_strike.IsTelegraphActive);
            Assert.AreEqual(0, _warnings.Count);
        }

        [Test]
        public void AC4_ShrineZone_CancelsActiveTelegraph()
        {
            _strike.Tick(true, false, PlayerInputMode.NightMovement, DeltaSmall); // start
            _strike.Tick(true, true,  PlayerInputMode.NightMovement, DeltaSmall); // enter shrine

            Assert.IsFalse(_strike.IsTelegraphActive);
            Assert.AreEqual(0, _wardCosts.Count);
        }

        // ── AC-5: Strike only active in NightMovement ─────────────

        [Test]
        public void AC5_DayUI_Exposed_NoTelegraph()
        {
            _strike.Tick(true, false, PlayerInputMode.DayUI, DeltaSmall);

            Assert.IsFalse(_strike.IsTelegraphActive);
            Assert.AreEqual(0, _warnings.Count);
        }

        [Test]
        public void AC5_Disabled_Exposed_NoTelegraph()
        {
            _strike.Tick(true, false, PlayerInputMode.Disabled, DeltaSmall);

            Assert.IsFalse(_strike.IsTelegraphActive);
        }

        // ── AC-6: StrikeTimePenaltySec configurable ────────────────

        [Test]
        public void AC6_CustomPenalty_UsedOnStrike()
        {
            _strike.StrikeTimePenaltySec = 20f;
            _strike.Tick(true, false, PlayerInputMode.NightMovement, DeltaSmall);
            _strike.Tick(true, false, PlayerInputMode.NightMovement,
                         StrikeController.DefaultStrikeTelegraphSec + 0.1f);

            Assert.AreEqual(20f, _wardCosts[0], 0.001f);
        }

        [Test]
        public void AC6_TelegraphDuration_Configurable()
        {
            _strike.StrikeTelegraphSec = 2.0f;
            _strike.Tick(true, false, PlayerInputMode.NightMovement, DeltaSmall);

            // Advance 1.5s — should NOT have fired yet (duration is 2.0s)
            _strike.Tick(true, false, PlayerInputMode.NightMovement, 1.5f);

            Assert.IsTrue(_strike.IsTelegraphActive, "Telegraph must still be active at 1.5s with 2.0s duration");
            Assert.AreEqual(0, _wardCosts.Count);
        }

        [Test]
        public void AC6_TelegraphBelowMin_ClampedTo0p8()
        {
            _strike.StrikeTelegraphSec = 0.1f;

            Assert.AreEqual(StrikeController.MinTelegraphSec, _strike.StrikeTelegraphSec);
        }

        [Test]
        public void AC6_TelegraphAboveMax_ClampedTo2p5()
        {
            _strike.StrikeTelegraphSec = 99f;

            Assert.AreEqual(StrikeController.MaxTelegraphSec, _strike.StrikeTelegraphSec);
        }

        // ── TelegraphRemaining counts down ─────────────────────────

        [Test]
        public void TelegraphRemaining_CountsDown_WhileExposed()
        {
            _strike.Tick(true, false, PlayerInputMode.NightMovement, DeltaSmall);
            float initial = _strike.TelegraphRemaining;

            _strike.Tick(true, false, PlayerInputMode.NightMovement, 0.5f);

            Assert.Less(_strike.TelegraphRemaining, initial,
                "TelegraphRemaining must decrease each frame while exposed");
        }

        [Test]
        public void TelegraphRemaining_Zero_WhenNotActive()
        {
            Assert.AreEqual(0f, _strike.TelegraphRemaining);
        }
    }
}
