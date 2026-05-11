// Assets/_Project/Application/Editor/Tests/EKeyInteractTests.cs
using System.Collections.Generic;
using NUnit.Framework;
using R3;
using SolarPhobia.Application.Services;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Tests
{
    /// <summary>
    /// Validates: TR-player-005 — E-Key Contextual Interact — Relic Pickup + Shrine Trigger.
    /// Story 005: E-Key Contextual Interact.
    ///
    /// Tests InteractHandler tag dispatch and phase gate in isolation.
    /// No Physics.Raycast, no scene, no Unity runtime required.
    /// </summary>
    [TestFixture]
    public class EKeyInteractTests
    {
        private InteractHandler _handler;
        private List<string> _events;

        [SetUp]
        public void Setup()
        {
            _handler = new InteractHandler();
            _events  = new List<string>();
            _handler.OnInteract.Subscribe(v => _events.Add(v));
        }

        // ── AC-1: CursedMound → "relic" ───────────────────────────

        [Test]
        public void AC1_CursedMound_NightMode_FiresRelicEvent()
        {
            _handler.TryInteract(InteractHandler.TagCursedMound, PlayerInputMode.NightMovement);

            Assert.AreEqual(1, _events.Count);
            Assert.AreEqual(InteractHandler.PayloadRelic, _events[0]);
        }

        [Test]
        public void AC1_CursedMound_FiresExactlyOnce_PerPress()
        {
            _handler.TryInteract(InteractHandler.TagCursedMound, PlayerInputMode.NightMovement);

            Assert.AreEqual(1, _events.Count, "Single E press must fire exactly one event");
        }

        // ── AC-2: EndShrine → "shrine" ────────────────────────────

        [Test]
        public void AC2_EndShrine_NightMode_FiresShrineEvent()
        {
            _handler.TryInteract(InteractHandler.TagEndShrine, PlayerInputMode.NightMovement);

            Assert.AreEqual(1, _events.Count);
            Assert.AreEqual(InteractHandler.PayloadShrine, _events[0]);
        }

        [Test]
        public void AC2_EndShrine_FiresExactlyOnce_PerPress()
        {
            _handler.TryInteract(InteractHandler.TagEndShrine, PlayerInputMode.NightMovement);

            Assert.AreEqual(1, _events.Count);
        }

        // ── AC-3: No interactable → silent ────────────────────────

        [Test]
        public void AC3_NullTag_NoEvent()
        {
            _handler.TryInteract(null, PlayerInputMode.NightMovement);

            Assert.AreEqual(0, _events.Count, "Null tag must produce no event");
        }

        [Test]
        public void AC3_EmptyTag_NoEvent()
        {
            _handler.TryInteract(string.Empty, PlayerInputMode.NightMovement);

            Assert.AreEqual(0, _events.Count);
        }

        [Test]
        public void AC3_UnknownTag_NoEvent()
        {
            _handler.TryInteract("SomeRandomObject", PlayerInputMode.NightMovement);

            Assert.AreEqual(0, _events.Count, "Unknown tag must be silently ignored");
        }

        [Test]
        public void AC3_FalseSafeMound_NoInteractEvent()
        {
            // FalseSafeMound triggers a warning tell (separate system) — no OnInteract
            _handler.TryInteract("FalseSafeMound", PlayerInputMode.NightMovement);

            Assert.AreEqual(0, _events.Count,
                "FalseSafeMound must not fire OnInteract — warning tell is handled elsewhere");
        }

        // ── AC-4: Phase gate — blocked outside NightSurvival ──────

        [Test]
        public void AC4_DayUI_CursedMound_NoEvent()
        {
            _handler.TryInteract(InteractHandler.TagCursedMound, PlayerInputMode.DayUI);

            Assert.AreEqual(0, _events.Count, "E-interact must be blocked in DayUI mode");
        }

        [Test]
        public void AC4_Disabled_CursedMound_NoEvent()
        {
            _handler.TryInteract(InteractHandler.TagCursedMound, PlayerInputMode.Disabled);

            Assert.AreEqual(0, _events.Count, "E-interact must be blocked in Disabled mode");
        }

        [Test]
        public void AC4_DayUI_EndShrine_NoEvent()
        {
            _handler.TryInteract(InteractHandler.TagEndShrine, PlayerInputMode.DayUI);

            Assert.AreEqual(0, _events.Count);
        }

        // ── AC-5: Pickup during strike telegraph ──────────────────

        [Test]
        public void AC5_StrikeTelegraphActive_RelicPickup_StillFires()
        {
            // Strike telegraph state is external — InteractHandler does NOT check it.
            // The relic pickup fires regardless; the strike system handles its own timing.
            // This test confirms InteractHandler has no strike-blocking logic.
            _handler.TryInteract(InteractHandler.TagCursedMound, PlayerInputMode.NightMovement);

            Assert.AreEqual(1, _events.Count,
                "Relic pickup must fire even during strike telegraph — strike is not cancelled");
            Assert.AreEqual(InteractHandler.PayloadRelic, _events[0]);
        }

        // ── Multiple presses ──────────────────────────────────────

        [Test]
        public void MultiplePresses_EachFiresIndependently()
        {
            _handler.TryInteract(InteractHandler.TagCursedMound, PlayerInputMode.NightMovement);
            _handler.TryInteract(InteractHandler.TagEndShrine,   PlayerInputMode.NightMovement);
            _handler.TryInteract(InteractHandler.TagCursedMound, PlayerInputMode.NightMovement);

            Assert.AreEqual(3, _events.Count);
            Assert.AreEqual(InteractHandler.PayloadRelic,  _events[0]);
            Assert.AreEqual(InteractHandler.PayloadShrine, _events[1]);
            Assert.AreEqual(InteractHandler.PayloadRelic,  _events[2]);
        }

        // ── Tag constants are correct ─────────────────────────────

        [Test]
        public void TagConstants_MatchExpectedUnityTagNames()
        {
            Assert.AreEqual("CursedMound", InteractHandler.TagCursedMound);
            Assert.AreEqual("EndShrine",   InteractHandler.TagEndShrine);
        }

        [Test]
        public void PayloadConstants_MatchExpectedValues()
        {
            Assert.AreEqual("relic",  InteractHandler.PayloadRelic);
            Assert.AreEqual("shrine", InteractHandler.PayloadShrine);
        }
    }
}
