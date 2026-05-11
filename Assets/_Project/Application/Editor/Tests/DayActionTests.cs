// Assets/_Project/Application/Editor/Tests/DayActionTests.cs
using System.Collections.Generic;
using NUnit.Framework;
using R3;
using SolarPhobia.Application.Services;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Tests
{
    /// <summary>
    /// Validates: Master GDD V5.0 Section 2.1 — Day Phase Swap + Shove.
    /// Story 007-v2: Day Phase Swap (Space) + Shove (F).
    /// </summary>
    [TestFixture]
    public class DayActionTests
    {
        private DayActionController _ctrl;
        private List<bool> _swapEvents;
        private List<bool> _shoveEvents;

        [SetUp]
        public void Setup()
        {
            _ctrl = new DayActionController();
            _swapEvents  = new List<bool>();
            _shoveEvents = new List<bool>();
            _ctrl.OnSwap.Subscribe(v  => _swapEvents.Add(v));
            _ctrl.OnShove.Subscribe(v => _shoveEvents.Add(v));
        }

        // ── Swap (Space) ──────────────────────────────────────────

        [Test]
        public void Swap_DayUI_SpaceInput_FiresSwapEvent()
        {
            _ctrl.TryActions(swapInput: true, shoveInput: false, PlayerInputMode.DayUI);

            Assert.AreEqual(1, _swapEvents.Count);
        }

        [Test]
        public void Swap_DayUI_NoInput_NoEvent()
        {
            _ctrl.TryActions(false, false, PlayerInputMode.DayUI);

            Assert.AreEqual(0, _swapEvents.Count);
        }

        [Test]
        public void Swap_NightMovement_Blocked()
        {
            _ctrl.TryActions(true, false, PlayerInputMode.NightMovement);

            Assert.AreEqual(0, _swapEvents.Count,
                "Swap must be blocked in NightMovement mode");
        }

        [Test]
        public void Swap_Disabled_Blocked()
        {
            _ctrl.TryActions(true, false, PlayerInputMode.Disabled);

            Assert.AreEqual(0, _swapEvents.Count);
        }

        [Test]
        public void Swap_MultiplePresses_FiresEachTime()
        {
            _ctrl.TryActions(true, false, PlayerInputMode.DayUI);
            _ctrl.TryActions(true, false, PlayerInputMode.DayUI);

            Assert.AreEqual(2, _swapEvents.Count,
                "Each Space press fires a separate Swap event");
        }

        // ── Shove (F) ─────────────────────────────────────────────

        [Test]
        public void Shove_DayUI_FInput_FiresShoveEvent()
        {
            _ctrl.TryActions(swapInput: false, shoveInput: true, PlayerInputMode.DayUI);

            Assert.AreEqual(1, _shoveEvents.Count);
        }

        [Test]
        public void Shove_DayUI_NoInput_NoEvent()
        {
            _ctrl.TryActions(false, false, PlayerInputMode.DayUI);

            Assert.AreEqual(0, _shoveEvents.Count);
        }

        [Test]
        public void Shove_NightMovement_Blocked()
        {
            _ctrl.TryActions(false, true, PlayerInputMode.NightMovement);

            Assert.AreEqual(0, _shoveEvents.Count);
        }

        [Test]
        public void Shove_Disabled_Blocked()
        {
            _ctrl.TryActions(false, true, PlayerInputMode.Disabled);

            Assert.AreEqual(0, _shoveEvents.Count);
        }

        // ── Both actions same frame ───────────────────────────────

        [Test]
        public void BothInputs_SameFrame_FiresBothEvents()
        {
            _ctrl.TryActions(true, true, PlayerInputMode.DayUI);

            Assert.AreEqual(1, _swapEvents.Count,  "Swap must fire");
            Assert.AreEqual(1, _shoveEvents.Count, "Shove must fire");
        }

        // ── Swap does not fire Shove and vice versa ───────────────

        [Test]
        public void SwapInput_DoesNotFireShove()
        {
            _ctrl.TryActions(true, false, PlayerInputMode.DayUI);

            Assert.AreEqual(0, _shoveEvents.Count);
        }

        [Test]
        public void ShoveInput_DoesNotFireSwap()
        {
            _ctrl.TryActions(false, true, PlayerInputMode.DayUI);

            Assert.AreEqual(0, _swapEvents.Count);
        }
    }
}
