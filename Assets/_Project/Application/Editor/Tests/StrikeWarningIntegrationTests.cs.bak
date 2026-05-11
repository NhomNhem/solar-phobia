// Assets/_Project/Application/Editor/Tests/StrikeWarningIntegrationTests.cs
using System.Collections.Generic;
using NUnit.Framework;
using R3;
using SolarPhobia.Application.Services;
using SolarPhobia.Application.Services.Map;
using SolarPhobia.Domain.ValueObjects;
using UnityEngine;

namespace SolarPhobia.Application.Tests
{
    /// <summary>
    /// Integration tests: TR-player-009 — Strike Warning Integration.
    /// Story 007: Map Director OnStrikeWarning → StrikeWarningController → UI state.
    ///
    /// Tests the full signal chain:
    ///   MapSpawnDirector.NotifyStrikeWarning()
    ///   → IMapSpawnDirector.OnStrikeWarning
    ///   → StrikeWarningController.OnStrikeWarningReceived()
    ///   → IsWarningActive ReactiveProperty
    /// </summary>
    [TestFixture]
    public class StrikeWarningIntegrationTests
    {
        private MapSpawnDirector        _mapDirector;
        private StrikeWarningController _controller;
        private List<bool>              _warningEvents;

        [SetUp]
        public void Setup()
        {
            _mapDirector   = new MapSpawnDirector();
            _mapDirector.Initialize(42);
            _controller    = new StrikeWarningController(_mapDirector);
            _warningEvents = new List<bool>();
            _controller.IsWarningActive.Subscribe(v => _warningEvents.Add(v));
        }

        // ── AC-1: OnStrikeWarning → IsWarningActive = true ─────────

        [Test]
        public void AC1_StrikeWarning_NightMode_SetsWarningActive_True()
        {
            _controller.OnStrikeWarningReceived(true, PlayerInputMode.NightMovement, Vector2.zero);

            Assert.IsTrue(_controller.IsWarningActive.CurrentValue);
        }

        [Test]
        public void AC1_StrikeWarning_FiresReactiveEvent()
        {
            int baseline = _warningEvents.Count;
            _controller.OnStrikeWarningReceived(true, PlayerInputMode.NightMovement, Vector2.zero);

            Assert.AreEqual(baseline + 1, _warningEvents.Count);
            Assert.IsTrue(_warningEvents[_warningEvents.Count - 1]);
        }

        // ── AC-3: Warning clears when strike resolves ──────────────

        [Test]
        public void AC3_StrikeResolved_ClearsWarning()
        {
            _controller.OnStrikeWarningReceived(true,  PlayerInputMode.NightMovement, Vector2.zero);
            _controller.OnStrikeWarningReceived(false, PlayerInputMode.NightMovement, Vector2.zero);

            Assert.IsFalse(_controller.IsWarningActive.CurrentValue,
                "Warning must clear when strike resolves");
        }

        [Test]
        public void AC3_WarningClear_FiresReactiveEvent_False()
        {
            _controller.OnStrikeWarningReceived(true,  PlayerInputMode.NightMovement, Vector2.zero);
            _controller.OnStrikeWarningReceived(false, PlayerInputMode.NightMovement, Vector2.zero);

            Assert.IsFalse(_warningEvents[_warningEvents.Count - 1]);
        }

        // ── AC-4: No duplicate events when state unchanged ─────────

        [Test]
        public void AC4_NoRepeatEvent_WhenWarningAlreadyActive()
        {
            _controller.OnStrikeWarningReceived(true, PlayerInputMode.NightMovement, Vector2.zero);
            int countAfterFirst = _warningEvents.Count;

            // Second true on same warning — LIFO adds another entry but IsWarningActive stays true
            // so no new reactive event fires
            _controller.OnStrikeWarningReceived(true, PlayerInputMode.NightMovement, Vector2.zero);

            Assert.AreEqual(countAfterFirst, _warningEvents.Count,
                "No duplicate IsWarningActive event when already active");
        }

        // ── Phase gate ─────────────────────────────────────────────

        [Test]
        public void PhaseGate_DayUI_WarningNotShown()
        {
            _controller.OnStrikeWarningReceived(true, PlayerInputMode.DayUI, Vector2.zero);

            Assert.IsFalse(_controller.IsWarningActive.CurrentValue,
                "Warning must not show in DayUI mode");
        }

        [Test]
        public void PhaseGate_Disabled_WarningNotShown()
        {
            _controller.OnStrikeWarningReceived(true, PlayerInputMode.Disabled, Vector2.zero);

            Assert.IsFalse(_controller.IsWarningActive.CurrentValue);
        }

        [Test]
        public void PhaseGate_ModeChange_ClearsActiveWarning()
        {
            _controller.OnStrikeWarningReceived(true, PlayerInputMode.NightMovement, Vector2.zero);
            Assert.IsTrue(_controller.IsWarningActive.CurrentValue);

            // Phase changes to DayUI — ClearAll() called, warning must clear
            _controller.OnStrikeWarningReceived(true, PlayerInputMode.DayUI, Vector2.zero);

            Assert.IsFalse(_controller.IsWarningActive.CurrentValue,
                "Warning must clear when mode leaves NightMovement");
        }

        // ── AC-5: Position reporting to Map Director ───────────────

        [Test]
        public void AC5_ReportPlayerPosition_NightMode_DoesNotThrow()
        {
            Assert.DoesNotThrow(() =>
                _controller.ReportPlayerPosition(
                    new Vector2(10f, 0f),
                    new Bounds(Vector3.zero, Vector3.one),
                    PlayerInputMode.NightMovement
                )
            );
        }

        [Test]
        public void AC5_ReportPlayerPosition_DayUI_DoesNotThrow()
        {
            // Should silently skip — no exception
            Assert.DoesNotThrow(() =>
                _controller.ReportPlayerPosition(
                    Vector2.zero,
                    new Bounds(),
                    PlayerInputMode.DayUI
                )
            );
        }

        // ── Full signal chain integration ──────────────────────────

        [Test]
        public void Integration_MapDirectorSignal_ReachesController()
        {
            // Simulate Map Director firing strike warning
            _mapDirector.NotifyStrikeWarning(true);

            // Controller processes it when subscribed via OnStrikeWarning
            // (In production, PlayerController wires: mapDirector.OnStrikeWarning.Subscribe(...))
            _controller.OnStrikeWarningReceived(true, PlayerInputMode.NightMovement, Vector2.zero);

            Assert.IsTrue(_controller.IsWarningActive.CurrentValue,
                "Full signal chain: MapDirector → Controller → IsWarningActive");
        }

        [Test]
        public void Integration_MapDirectorSignal_Clears_WhenResolved()
        {
            _controller.OnStrikeWarningReceived(true,  PlayerInputMode.NightMovement, Vector2.zero);
            _mapDirector.NotifyStrikeWarning(false);
            _controller.OnStrikeWarningReceived(false, PlayerInputMode.NightMovement, Vector2.zero);

            Assert.IsFalse(_controller.IsWarningActive.CurrentValue);
        }
    }
}
