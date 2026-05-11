// Assets/_Project/Application/Editor/Tests/SprintTests.cs
using System.Collections.Generic;
using NUnit.Framework;
using R3;
using UnityEngine;
using SolarPhobia.Application.Services;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Tests
{
    /// <summary>
    /// Validates: TR-player-003 — Sprint — Shift Key Speed Multiplier + Stamina Integration.
    /// Story 003-v2: Sprint (2D Rigidbody).
    ///
    /// Tests SprintController state machine and Movement2DCalculator sprint formula.
    /// </summary>
    [TestFixture]
    public class SprintTests
    {
        private SprintController       _sprint;
        private Movement2DCalculator   _calc;
        private List<bool>             _sprintEvents;

        private const float DeltaTime1s = 1.0f;

        [SetUp]
        public void Setup()
        {
            _sprint       = new SprintController();
            _calc         = new Movement2DCalculator();
            _sprintEvents = new List<bool>();
            _sprint.OnSprintChanged.Subscribe(v => _sprintEvents.Add(v));
        }

        // ── AC-1: Shift activates sprint speed ────────────────────

        [Test]
        public void AC1_SprintInput_NightMode_SetsSprinting_True()
        {
            _sprint.Tick(sprintInputHeld: true, PlayerInputMode.NightMovement);

            Assert.IsTrue(_sprint.IsSprinting);
        }

        [Test]
        public void AC1_SprintInput_FiresOnSprintChanged_True()
        {
            _sprint.Tick(sprintInputHeld: true, PlayerInputMode.NightMovement);

            Assert.AreEqual(1, _sprintEvents.Count);
            Assert.IsTrue(_sprintEvents[0]);
        }

        [Test]
        public void AC1_SprintSpeed_IsBase_Times_Multiplier()
        {
            // base=5.0, multiplier=1.8 → effective=9.0
            _calc.NightMoveSpeed   = 5.0f;
            _calc.SprintMultiplier = 1.8f;

            float velocity = _calc.CalculateNightVelocityX(
                inputX: 1f,
                PlayerInputMode.NightMovement,
                isSprinting: true
            );

            Assert.AreEqual(9.0f, velocity, 0.001f,
                "Sprint speed must be nightMoveSpeed * sprintMultiplier");
        }

        // ── AC-2: Releasing Shift returns to base speed ───────────

        [Test]
        public void AC2_ReleaseSprint_SetsSprinting_False()
        {
            _sprint.Tick(true,  PlayerInputMode.NightMovement); // start
            _sprint.Tick(false, PlayerInputMode.NightMovement); // release

            Assert.IsFalse(_sprint.IsSprinting);
        }

        [Test]
        public void AC2_ReleaseSprint_FiresOnSprintChanged_False()
        {
            _sprint.Tick(true,  PlayerInputMode.NightMovement);
            _sprint.Tick(false, PlayerInputMode.NightMovement);

            Assert.AreEqual(2, _sprintEvents.Count);
            Assert.IsFalse(_sprintEvents[1]);
        }

        [Test]
        public void AC2_BaseSpeed_WhenNotSprinting()
        {
            _calc.NightMoveSpeed = 5.0f;

            float velocity = _calc.CalculateNightVelocityX(
                inputX: 1f,
                PlayerInputMode.NightMovement,
                isSprinting: false
            );

            Assert.AreEqual(5.0f, velocity, 0.001f,
                "Base speed must be used when not sprinting");
        }

        // ── AC-3: OnStaminaDepleted forces sprint exit ────────────

        [Test]
        public void AC3_StaminaDepleted_WhileSprinting_StopsSprint()
        {
            _sprint.Tick(true, PlayerInputMode.NightMovement); // start sprint
            _sprint.NotifyStaminaDepleted();

            Assert.IsFalse(_sprint.IsSprinting);
        }

        [Test]
        public void AC3_StaminaDepleted_FiresOnSprintChanged_False()
        {
            _sprint.Tick(true, PlayerInputMode.NightMovement);
            _sprint.NotifyStaminaDepleted();

            Assert.AreEqual(2, _sprintEvents.Count);
            Assert.IsFalse(_sprintEvents[1]);
        }

        [Test]
        public void AC3_StaminaDepleted_SpeedReturnsToBase()
        {
            _sprint.Tick(true, PlayerInputMode.NightMovement);
            _sprint.NotifyStaminaDepleted();

            // After depletion, isSprinting = false → base speed
            float velocity = _calc.CalculateNightVelocityX(
                inputX: 1f,
                PlayerInputMode.NightMovement,
                isSprinting: _sprint.IsSprinting
            );

            Assert.AreEqual(5.0f, velocity, 0.001f);
        }

        // ── AC-4: Sprint blocked with 0 stamina ───────────────────

        [Test]
        public void AC4_SprintInput_AfterStaminaDepleted_DoesNotActivate()
        {
            _sprint.NotifyStaminaDepleted();
            _sprint.Tick(sprintInputHeld: true, PlayerInputMode.NightMovement);

            Assert.IsFalse(_sprint.IsSprinting);
        }

        [Test]
        public void AC4_SprintInput_AfterStaminaDepleted_NoEvent()
        {
            _sprint.NotifyStaminaDepleted();
            int countBefore = _sprintEvents.Count;

            _sprint.Tick(sprintInputHeld: true, PlayerInputMode.NightMovement);

            Assert.AreEqual(countBefore, _sprintEvents.Count,
                "No OnSprintChanged event when stamina is depleted");
        }

        [Test]
        public void AC4_StaminaRestored_AllowsSprintAgain()
        {
            _sprint.NotifyStaminaDepleted();
            _sprint.NotifyStaminaRestored();
            _sprint.Tick(sprintInputHeld: true, PlayerInputMode.NightMovement);

            Assert.IsTrue(_sprint.IsSprinting,
                "Sprint must be re-activatable after stamina is restored");
        }

        // ── AC-5: Sprint only in NightSurvival ────────────────────

        [Test]
        public void AC5_DayUI_SprintInput_DoesNotActivate()
        {
            _sprint.Tick(sprintInputHeld: true, PlayerInputMode.DayUI);

            Assert.IsFalse(_sprint.IsSprinting);
        }

        [Test]
        public void AC5_Disabled_SprintInput_DoesNotActivate()
        {
            _sprint.Tick(sprintInputHeld: true, PlayerInputMode.Disabled);

            Assert.IsFalse(_sprint.IsSprinting);
        }

        [Test]
        public void AC5_DayUI_SprintInput_NoEvent()
        {
            _sprint.Tick(sprintInputHeld: true, PlayerInputMode.DayUI);

            Assert.AreEqual(0, _sprintEvents.Count,
                "No OnSprintChanged event in DayUI mode");
        }

        [Test]
        public void AC5_ModeChange_NightToDay_StopsSprint()
        {
            _sprint.Tick(true, PlayerInputMode.NightMovement); // start
            _sprint.Tick(true, PlayerInputMode.DayUI);         // mode changes mid-sprint

            Assert.IsFalse(_sprint.IsSprinting,
                "Sprint must stop when mode leaves NightMovement");
        }

        // ── AC-6: sprint_multiplier is configurable ───────────────

        [Test]
        public void AC6_DefaultMultiplier_Is1d8()
        {
            Assert.AreEqual(Movement2DCalculator.DefaultSprintMultiplier, _calc.SprintMultiplier);
        }

        [Test]
        public void AC6_CustomMultiplier_2d0_ProducesCorrectSpeed()
        {
            _calc.NightMoveSpeed   = 5.0f;
            _calc.SprintMultiplier = 2.0f;

            float velocity = _calc.CalculateNightVelocityX(
                inputX: 1f,
                PlayerInputMode.NightMovement,
                isSprinting: true
            );

            Assert.AreEqual(10.0f, velocity, 0.001f);
        }

        [Test]
        public void AC6_MultiplierBelowMin_ClampedTo1d5()
        {
            _calc.SprintMultiplier = 0.5f;

            Assert.AreEqual(Movement2DCalculator.MinSprintMultiplier, _calc.SprintMultiplier);
        }

        [Test]
        public void AC6_MultiplierAboveMax_ClampedTo3d0()
        {
            _calc.SprintMultiplier = 99f;

            Assert.AreEqual(Movement2DCalculator.MaxSprintMultiplier, _calc.SprintMultiplier);
        }

        // ── No duplicate events ───────────────────────────────────

        [Test]
        public void NoEvent_WhenSprintStateUnchanged()
        {
            _sprint.Tick(true, PlayerInputMode.NightMovement); // start → event
            int countAfterFirst = _sprintEvents.Count;

            _sprint.Tick(true, PlayerInputMode.NightMovement); // still sprinting → no event
            _sprint.Tick(true, PlayerInputMode.NightMovement);

            Assert.AreEqual(countAfterFirst, _sprintEvents.Count,
                "OnSprintChanged must not fire when state is unchanged");
        }

        [Test]
        public void NoEvent_WhenNotSprintingAndInputReleased()
        {
            // Already not sprinting — releasing input should not fire
            _sprint.Tick(false, PlayerInputMode.NightMovement);

            Assert.AreEqual(0, _sprintEvents.Count);
        }
    }
}
