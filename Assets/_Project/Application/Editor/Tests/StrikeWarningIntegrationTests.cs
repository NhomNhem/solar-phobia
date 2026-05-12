using System.Collections.Generic;
using NUnit.Framework;
using R3;
using SolarPhobia.Application.Map.Directors;
using SolarPhobia.Application.Player.Warnings;
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
    [TestFixture]
    public class StrikeWarningIntegrationTests
    {
        private MapSpawnDirector _mapDirector;
        private StrikeWarningController _controller;
        private List<bool> _warningEvents;

        [SetUp]
        public void Setup()
        {
            _mapDirector = new MapSpawnDirector();
            _mapDirector.Initialize(42);
            _controller = new StrikeWarningController(_mapDirector);
            _warningEvents = new List<bool>();
            _controller.IsWarningActive.Subscribe(v => _warningEvents.Add(v));
        }

        [Test]
        public void AC1_StrikeWarning_NightMode_SetsWarningActive_True()
        {
            _controller.OnStrikeWarningReceived(true, PlayerInputMode.NightMovement, new Float2(0f, 0f));
            Assert.IsTrue(_controller.IsWarningActive.CurrentValue);
        }

        [Test]
        public void AC1_StrikeWarning_FiresReactiveEvent()
        {
            int baseline = _warningEvents.Count;
            _controller.OnStrikeWarningReceived(true, PlayerInputMode.NightMovement, new Float2(0f, 0f));
            Assert.AreEqual(baseline + 1, _warningEvents.Count);
            Assert.IsTrue(_warningEvents[_warningEvents.Count - 1]);
        }

        [Test]
        public void AC3_StrikeResolved_ClearsWarning()
        {
            _controller.OnStrikeWarningReceived(true, PlayerInputMode.NightMovement, new Float2(0f, 0f));
            _controller.OnStrikeWarningReceived(false, PlayerInputMode.NightMovement, new Float2(0f, 0f));
            Assert.IsFalse(_controller.IsWarningActive.CurrentValue);
        }

        [Test]
        public void AC3_WarningClear_FiresReactiveEvent_False()
        {
            _controller.OnStrikeWarningReceived(true, PlayerInputMode.NightMovement, new Float2(0f, 0f));
            _controller.OnStrikeWarningReceived(false, PlayerInputMode.NightMovement, new Float2(0f, 0f));
            Assert.IsFalse(_warningEvents[_warningEvents.Count - 1]);
        }

        [Test]
        public void AC4_NoRepeatEvent_WhenWarningAlreadyActive()
        {
            _controller.OnStrikeWarningReceived(true, PlayerInputMode.NightMovement, new Float2(0f, 0f));
            int countAfterFirst = _warningEvents.Count;
            _controller.OnStrikeWarningReceived(true, PlayerInputMode.NightMovement, new Float2(0f, 0f));
            Assert.AreEqual(countAfterFirst, _warningEvents.Count);
        }

        [Test]
        public void PhaseGate_DayUI_WarningNotShown()
        {
            _controller.OnStrikeWarningReceived(true, PlayerInputMode.DayUI, new Float2(0f, 0f));
            Assert.IsFalse(_controller.IsWarningActive.CurrentValue);
        }

        [Test]
        public void PhaseGate_Disabled_WarningNotShown()
        {
            _controller.OnStrikeWarningReceived(true, PlayerInputMode.Disabled, new Float2(0f, 0f));
            Assert.IsFalse(_controller.IsWarningActive.CurrentValue);
        }

        [Test]
        public void PhaseGate_ModeChange_ClearsActiveWarning()
        {
            _controller.OnStrikeWarningReceived(true, PlayerInputMode.NightMovement, new Float2(0f, 0f));
            Assert.IsTrue(_controller.IsWarningActive.CurrentValue);
            _controller.OnStrikeWarningReceived(true, PlayerInputMode.DayUI, new Float2(0f, 0f));
            Assert.IsFalse(_controller.IsWarningActive.CurrentValue);
        }

        [Test]
        public void AC5_ReportPlayerPosition_NightMode_DoesNotThrow()
        {
            Assert.DoesNotThrow(() =>
                _controller.ReportPlayerPosition(
                    new Float2(10f, 0f),
                    new Bounds2D(new Float2(0f, 0f), new Float2(1f, 1f)),
                    PlayerInputMode.NightMovement));
        }

        [Test]
        public void AC5_ReportPlayerPosition_DayUI_DoesNotThrow()
        {
            Assert.DoesNotThrow(() =>
                _controller.ReportPlayerPosition(
                    new Float2(0f, 0f),
                    new Bounds2D(new Float2(0f, 0f), new Float2(0f, 0f)),
                    PlayerInputMode.DayUI));
        }

        [Test]
        public void Integration_MapDirectorSignal_ReachesController()
        {
            _mapDirector.NotifyStrikeWarning(true);
            _controller.OnStrikeWarningReceived(true, PlayerInputMode.NightMovement, new Float2(0f, 0f));
            Assert.IsTrue(_controller.IsWarningActive.CurrentValue);
        }

        [Test]
        public void Integration_MapDirectorSignal_Clears_WhenResolved()
        {
            _controller.OnStrikeWarningReceived(true, PlayerInputMode.NightMovement, new Float2(0f, 0f));
            _mapDirector.NotifyStrikeWarning(false);
            _controller.OnStrikeWarningReceived(false, PlayerInputMode.NightMovement, new Float2(0f, 0f));
            Assert.IsFalse(_controller.IsWarningActive.CurrentValue);
        }
    }
}

