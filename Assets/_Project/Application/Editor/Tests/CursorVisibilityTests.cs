// Assets/_Project/Application/Editor/Tests/CursorVisibilityTests.cs
using NUnit.Framework;
using SolarPhobia.Application.Resources;
using SolarPhobia.Domain.ValueObjects;


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

using SolarPhobia.Application.Combat;

using SolarPhobia.Application.Phase.Reset;

namespace SolarPhobia.Application.Tests
{
    /// <summary>
    /// Validates: TR-player-007 — Cursor Visibility — Phase-Driven Show/Hide.
    /// Story 006: Cursor Visibility.
    ///
    /// Tests CursorController phase → cursor state mapping in isolation.
    /// Actual Cursor.visible / Cursor.lockState calls are applied by the
    /// MonoBehaviour layer and verified via manual walkthrough.
    /// </summary>
    [TestFixture]
    public class CursorVisibilityTests
    {
        private CursorController _controller;

        [SetUp]
        public void Setup()
        {
            _controller = new CursorController();
        }

        // ── AC-1: DayService → cursor visible ─────────────────────

        [Test]
        public void AC1_DayService_CursorVisible_True()
        {
            Assert.IsTrue(_controller.GetCursorVisible(PhaseState.DayService));
        }

        [Test]
        public void AC1_DayService_LockState_None()
        {
            Assert.AreEqual(CursorLockState.None, _controller.GetCursorLockState(PhaseState.DayService));
        }

        // ── AC-2: NightSurvival → cursor hidden + locked ──────────

        [Test]
        public void AC2_NightSurvival_CursorVisible_False()
        {
            Assert.IsFalse(_controller.GetCursorVisible(PhaseState.NightSurvival));
        }

        [Test]
        public void AC2_NightSurvival_LockState_Locked()
        {
            Assert.AreEqual(CursorLockState.Locked, _controller.GetCursorLockState(PhaseState.NightSurvival));
        }

        // ── AC-3: ChoiceLock / EndingEvaluation → cursor visible ──

        [Test]
        public void AC3_ChoiceLock_CursorVisible_True()
        {
            Assert.IsTrue(_controller.GetCursorVisible(PhaseState.ChoiceLock));
        }

        [Test]
        public void AC3_ChoiceLock_LockState_None()
        {
            Assert.AreEqual(CursorLockState.None, _controller.GetCursorLockState(PhaseState.ChoiceLock));
        }

        [Test]
        public void AC3_EndingEvaluation_CursorVisible_True()
        {
            Assert.IsTrue(_controller.GetCursorVisible(PhaseState.EndingEvaluation));
        }

        [Test]
        public void AC3_EndingEvaluation_LockState_None()
        {
            Assert.AreEqual(CursorLockState.None, _controller.GetCursorLockState(PhaseState.EndingEvaluation));
        }

        // ── All intermediate phases → cursor visible ──────────────

        [Test]
        public void IntermediatePhases_AllShowCursor()
        {
            var phases = new[]
            {
                PhaseState.Boot,
                PhaseState.Dialogue,
                PhaseState.Order,
                PhaseState.SunsetWarning,
                PhaseState.NightTravel,
                PhaseState.ShrineArrival
            };

            foreach (var phase in phases)
            {
                Assert.IsTrue(
                    _controller.GetCursorVisible(phase),
                    $"Phase {phase} should show cursor"
                );
                Assert.AreEqual(
                    CursorLockState.None,
                    _controller.GetCursorLockState(phase),
                    $"Phase {phase} should use CursorLockState.None"
                );
            }
        }

        // ── NightSurvival is the only phase that hides cursor ──────

        [Test]
        public void OnlyNightSurvival_HidesCursor()
        {
            var allPhases = System.Enum.GetValues(typeof(PhaseState));
            foreach (PhaseState phase in allPhases)
            {
                bool visible = _controller.GetCursorVisible(phase);
                if (phase == PhaseState.NightSurvival)
                {
                    Assert.IsFalse(visible, "NightSurvival must hide cursor");
                }
                else
                {
                    Assert.IsTrue(visible, $"Phase {phase} must show cursor");
                }
            }
        }
    }
}


