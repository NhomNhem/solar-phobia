// Assets/_Project/Application/Editor/Tests/EKeyInteractTests.cs
using System.Collections.Generic;
using NUnit.Framework;
using R3;
using SolarPhobia.Application.Services;
using SolarPhobia.Domain.ValueObjects;

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
    /// Validates: TR-player-005 â€” E-Key Contextual Interact â€” Relic Pickup + Shrine Trigger.
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

        // â”€â”€ AC-1: CursedMound â†’ "relic" â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

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

        // â”€â”€ AC-2: EndShrine â†’ "shrine" â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

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

        // â”€â”€ AC-3: No interactable â†’ silent â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

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
            // FalseSafeMound triggers a warning tell (separate system) â€” no OnInteract
            _handler.TryInteract("FalseSafeMound", PlayerInputMode.NightMovement);

            Assert.AreEqual(0, _events.Count,
                "FalseSafeMound must not fire OnInteract â€” warning tell is handled elsewhere");
        }

        // â”€â”€ AC-4: Phase gate â€” blocked outside NightSurvival â”€â”€â”€â”€â”€â”€

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

        // â”€â”€ AC-5: Pickup during strike telegraph â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [Test]
        public void AC5_StrikeTelegraphActive_RelicPickup_StillFires()
        {
            // Strike telegraph state is external â€” InteractHandler does NOT check it.
            // The relic pickup fires regardless; the strike system handles its own timing.
            // This test confirms InteractHandler has no strike-blocking logic.
            _handler.TryInteract(InteractHandler.TagCursedMound, PlayerInputMode.NightMovement);

            Assert.AreEqual(1, _events.Count,
                "Relic pickup must fire even during strike telegraph â€” strike is not cancelled");
            Assert.AreEqual(InteractHandler.PayloadRelic, _events[0]);
        }

        // â”€â”€ Multiple presses â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

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

        // â”€â”€ Tag constants are correct â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

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

