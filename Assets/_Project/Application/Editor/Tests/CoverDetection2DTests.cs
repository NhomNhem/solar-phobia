// Assets/_Project/Application/Editor/Tests/CoverDetection2DTests.cs
using System.Collections.Generic;
using NUnit.Framework;
using R3;
using SolarPhobia.Application.Services;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Tests
{
    /// <summary>
    /// Validates: Master GDD V5.0 Section 3.2 — Cover Detection 2D (Mộ Gió trigger overlap).
    /// Story 006-v2: Cover Detection 2D.
    /// </summary>
    [TestFixture]
    public class CoverDetection2DTests
    {
        private CoverDetector2D _detector;
        private List<bool> _coverEvents;
        private List<bool> _falseSafeMoundEvents;

        [SetUp]
        public void Setup()
        {
            _detector = new CoverDetector2D();
            _coverEvents = new List<bool>();
            _falseSafeMoundEvents = new List<bool>();
            _detector.IsInCover.Subscribe(v => _coverEvents.Add(v));
            _detector.OnFalseSafeMoundEntered.Subscribe(v => _falseSafeMoundEvents.Add(v));
        }

        // ── AC-1: MoThuong overlap → IsInCover = true ─────────────

        [Test]
        public void AC1_MoThuong_Enter_NightMode_SetsInCover_True()
        {
            _detector.NotifyOverlapEnter(CoverDetector2D.TagMoThuong, PlayerInputMode.NightMovement);

            Assert.IsTrue(_detector.IsInCover.CurrentValue);
        }

        [Test]
        public void AC1_MoThuong_Exit_SetsInCover_False()
        {
            _detector.NotifyOverlapEnter(CoverDetector2D.TagMoThuong, PlayerInputMode.NightMovement);
            _detector.NotifyOverlapExit(CoverDetector2D.TagMoThuong, PlayerInputMode.NightMovement);

            Assert.IsFalse(_detector.IsInCover.CurrentValue);
        }

        [Test]
        public void AC1_MoThuong_Enter_FiresCoverEvent_True()
        {
            int baseline = _coverEvents.Count;
            _detector.NotifyOverlapEnter(CoverDetector2D.TagMoThuong, PlayerInputMode.NightMovement);

            Assert.AreEqual(baseline + 1, _coverEvents.Count);
            Assert.IsTrue(_coverEvents[_coverEvents.Count - 1]);
        }

        // ── AC-2: Exit trigger → IsInCover = false ────────────────

        [Test]
        public void AC2_Exit_FiresCoverEvent_False()
        {
            _detector.NotifyOverlapEnter(CoverDetector2D.TagMoThuong, PlayerInputMode.NightMovement);
            int baseline = _coverEvents.Count;

            _detector.NotifyOverlapExit(CoverDetector2D.TagMoThuong, PlayerInputMode.NightMovement);

            Assert.AreEqual(baseline + 1, _coverEvents.Count);
            Assert.IsFalse(_coverEvents[_coverEvents.Count - 1]);
        }

        // ── AC-3: ReactiveProperty fires on state change only ──────

        [Test]
        public void AC3_StayInCover_NoRepeatEvent()
        {
            _detector.NotifyOverlapEnter(CoverDetector2D.TagMoThuong, PlayerInputMode.NightMovement);
            int baseline = _coverEvents.Count;

            // Enter again while already in cover — no state change
            _detector.NotifyOverlapEnter(CoverDetector2D.TagMoThuong, PlayerInputMode.NightMovement);

            Assert.AreEqual(baseline, _coverEvents.Count, "No event when state unchanged");
        }

        // ── AC-4: Cover check disabled outside NightSurvival ──────

        [Test]
        public void AC4_DayUI_Enter_NoStateChange()
        {
            _detector.NotifyOverlapEnter(CoverDetector2D.TagMoThuong, PlayerInputMode.DayUI);

            Assert.IsFalse(_detector.IsInCover.CurrentValue,
                "Cover must not activate in DayUI mode");
        }

        [Test]
        public void AC4_Disabled_Enter_NoStateChange()
        {
            _detector.NotifyOverlapEnter(CoverDetector2D.TagMoThuong, PlayerInputMode.Disabled);

            Assert.IsFalse(_detector.IsInCover.CurrentValue);
        }

        [Test]
        public void AC4_DayUI_DoesNotClearExistingCoverState()
        {
            // Enter cover in Night
            _detector.NotifyOverlapEnter(CoverDetector2D.TagMoThuong, PlayerInputMode.NightMovement);
            Assert.IsTrue(_detector.IsInCover.CurrentValue);

            // Phase changes — cover state must not be cleared
            _detector.NotifyOverlapEnter(CoverDetector2D.TagMoThuong, PlayerInputMode.DayUI);

            Assert.IsTrue(_detector.IsInCover.CurrentValue,
                "Cover state must not change when check is skipped");
        }

        // ── AC-5: FalseSafeMound → cover + warning ─────────────────

        [Test]
        public void AC5_FalseSafeMound_Enter_SetsInCover_True()
        {
            _detector.NotifyOverlapEnter(CoverDetector2D.TagFalseSafeMound, PlayerInputMode.NightMovement);

            Assert.IsTrue(_detector.IsInCover.CurrentValue);
        }

        [Test]
        public void AC5_FalseSafeMound_Enter_FiresWarningEvent()
        {
            _detector.NotifyOverlapEnter(CoverDetector2D.TagFalseSafeMound, PlayerInputMode.NightMovement);

            Assert.AreEqual(1, _falseSafeMoundEvents.Count,
                "FalseSafeMound must fire warning event");
        }

        [Test]
        public void AC5_MoThuong_DoesNotFireWarningEvent()
        {
            _detector.NotifyOverlapEnter(CoverDetector2D.TagMoThuong, PlayerInputMode.NightMovement);

            Assert.AreEqual(0, _falseSafeMoundEvents.Count,
                "MoThuong must not fire FalseSafeMound warning");
        }

        [Test]
        public void AC5_FalseSafeMound_Exit_SetsInCover_False()
        {
            _detector.NotifyOverlapEnter(CoverDetector2D.TagFalseSafeMound, PlayerInputMode.NightMovement);
            _detector.NotifyOverlapExit(CoverDetector2D.TagFalseSafeMound, PlayerInputMode.NightMovement);

            Assert.IsFalse(_detector.IsInCover.CurrentValue);
        }

        // ── Unknown tags silently ignored ─────────────────────────

        [Test]
        public void UnknownTag_Enter_NoStateChange()
        {
            _detector.NotifyOverlapEnter("SomeRandomObject", PlayerInputMode.NightMovement);

            Assert.IsFalse(_detector.IsInCover.CurrentValue);
        }

        [Test]
        public void NullTag_Enter_NoStateChange()
        {
            _detector.NotifyOverlapEnter(null, PlayerInputMode.NightMovement);

            Assert.IsFalse(_detector.IsInCover.CurrentValue);
        }

        // ── Tag constants ─────────────────────────────────────────

        [Test]
        public void TagConstants_MatchExpectedUnityTagNames()
        {
            Assert.AreEqual("MoThuong",       CoverDetector2D.TagMoThuong);
            Assert.AreEqual("FalseSafeMound", CoverDetector2D.TagFalseSafeMound);
        }
    }
}
