// Assets/_Project/Application/Editor/Tests/Movement2DTests.cs
using NUnit.Framework;
using SolarPhobia.Application.Services;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Tests
{
    /// <summary>
    /// Validates: Master GDD V5.0 — A/D Movement (2D side-scroller).
    /// Story 002-v2: A/D Movement — 2D Rigidbody Night Traversal + Day Walk.
    ///
    /// Tests Movement2DCalculator formula in isolation.
    /// No Rigidbody2D, no scene, no Unity physics required.
    /// </summary>
    [TestFixture]
    public class Movement2DTests
    {
        private Movement2DCalculator _calc;
        private const float DeltaTime1s  = 1.0f;
        private const float DeltaTime0p5 = 0.5f;

        [SetUp]
        public void Setup()
        {
            _calc = new Movement2DCalculator();
        }

        // ── AC-1: Night A/D drives horizontal velocity ─────────────

        [Test]
        public void AC1_Night_RightInput_ProducesPositiveVelocityX()
        {
            // D key = +1.0 input, NightMovement
            float velocity = _calc.CalculateNightVelocityX(1f, PlayerInputMode.NightMovement);

            Assert.AreEqual(Movement2DCalculator.DefaultNightMoveSpeed, velocity, 0.001f,
                "D key should produce +velocityX at night speed");
        }

        [Test]
        public void AC1_Night_LeftInput_ProducesNegativeVelocityX()
        {
            float velocity = _calc.CalculateNightVelocityX(-1f, PlayerInputMode.NightMovement);

            Assert.AreEqual(-Movement2DCalculator.DefaultNightMoveSpeed, velocity, 0.001f,
                "A key should produce -velocityX");
        }

        [Test]
        public void AC1_Night_ZeroInput_ProducesZeroVelocity()
        {
            float velocity = _calc.CalculateNightVelocityX(0f, PlayerInputMode.NightMovement);

            Assert.AreEqual(0f, velocity, 0.001f);
        }

        [Test]
        public void AC1_Night_Displacement_ScalesWithDeltaTime()
        {
            float full = _calc.CalculateHorizontalDisplacement(1f, DeltaTime1s,  PlayerInputMode.NightMovement);
            float half = _calc.CalculateHorizontalDisplacement(1f, DeltaTime0p5, PlayerInputMode.NightMovement);

            Assert.AreEqual(full * 0.5f, half, 0.001f,
                "Displacement must scale linearly with deltaTime");
        }

        // ── AC-2: Night effective_speed = base_move_speed ──────────

        [Test]
        public void AC2_Night_DefaultSpeed_Is5()
        {
            Assert.AreEqual(Movement2DCalculator.DefaultNightMoveSpeed, _calc.NightMoveSpeed);
        }

        [Test]
        public void AC2_Night_Displacement_UsesNightSpeed()
        {
            _calc.NightMoveSpeed = 5.0f;

            float disp = _calc.CalculateHorizontalDisplacement(1f, DeltaTime1s, PlayerInputMode.NightMovement);

            Assert.AreEqual(5.0f, disp, 0.001f);
        }

        // ── AC-3: Night movement blocked outside NightMovement ──────

        [Test]
        public void AC3_DayUI_NightVelocity_ReturnsZero()
        {
            float velocity = _calc.CalculateNightVelocityX(1f, PlayerInputMode.DayUI);

            Assert.AreEqual(0f, velocity, 0.001f,
                "Night velocity must be 0 in DayUI mode");
        }

        [Test]
        public void AC3_Disabled_NightVelocity_ReturnsZero()
        {
            float velocity = _calc.CalculateNightVelocityX(1f, PlayerInputMode.Disabled);

            Assert.AreEqual(0f, velocity, 0.001f);
        }

        // ── AC-4: Day A/D slow walk on X-axis ──────────────────────

        [Test]
        public void AC4_Day_RightInput_ProducesPositiveDisplacement()
        {
            float disp = _calc.CalculateHorizontalDisplacement(1f, DeltaTime1s, PlayerInputMode.DayUI);

            Assert.AreEqual(Movement2DCalculator.DefaultDayMoveSpeed, disp, 0.001f,
                "D key in Day should produce +X displacement at day speed");
        }

        [Test]
        public void AC4_Day_LeftInput_ProducesNegativeDisplacement()
        {
            float disp = _calc.CalculateHorizontalDisplacement(-1f, DeltaTime1s, PlayerInputMode.DayUI);

            Assert.AreEqual(-Movement2DCalculator.DefaultDayMoveSpeed, disp, 0.001f);
        }

        [Test]
        public void AC4_Day_ZeroInput_ProducesZeroDisplacement()
        {
            float disp = _calc.CalculateHorizontalDisplacement(0f, DeltaTime1s, PlayerInputMode.DayUI);

            Assert.AreEqual(0f, disp, 0.001f);
        }

        // ── AC-5: Day speed configurable, slower than night ─────────

        [Test]
        public void AC5_Day_DefaultSpeed_Is2()
        {
            Assert.AreEqual(Movement2DCalculator.DefaultDayMoveSpeed, _calc.DayMoveSpeed);
        }

        [Test]
        public void AC5_Day_DefaultSpeed_SlowerThanNight()
        {
            Assert.Less(_calc.DayMoveSpeed, _calc.NightMoveSpeed,
                "Day speed must be slower than night speed");
        }

        [Test]
        public void AC5_Day_CustomSpeed_ProducesCorrectDisplacement()
        {
            _calc.DayMoveSpeed = 1.5f;

            float disp = _calc.CalculateHorizontalDisplacement(1f, DeltaTime1s, PlayerInputMode.DayUI);

            Assert.AreEqual(1.5f, disp, 0.001f);
        }

        // ── AC-6: base_move_speed configurable ──────────────────────

        [Test]
        public void AC6_NightSpeed_Configurable()
        {
            _calc.NightMoveSpeed = 7.0f;

            float velocity = _calc.CalculateNightVelocityX(1f, PlayerInputMode.NightMovement);

            Assert.AreEqual(7.0f, velocity, 0.001f);
        }

        [Test]
        public void AC6_NightSpeed_BelowMin_ClampedTo2()
        {
            _calc.NightMoveSpeed = 0.1f;

            Assert.AreEqual(Movement2DCalculator.MinNightMoveSpeed, _calc.NightMoveSpeed);
        }

        [Test]
        public void AC6_NightSpeed_AboveMax_ClampedTo8()
        {
            _calc.NightMoveSpeed = 99f;

            Assert.AreEqual(Movement2DCalculator.MaxNightMoveSpeed, _calc.NightMoveSpeed);
        }

        [Test]
        public void AC6_DaySpeed_BelowMin_ClampedTo1()
        {
            _calc.DayMoveSpeed = 0f;

            Assert.AreEqual(Movement2DCalculator.MinDayMoveSpeed, _calc.DayMoveSpeed);
        }

        [Test]
        public void AC6_DaySpeed_AboveMax_ClampedTo4()
        {
            _calc.DayMoveSpeed = 99f;

            Assert.AreEqual(Movement2DCalculator.MaxDayMoveSpeed, _calc.DayMoveSpeed);
        }

        // ── Disabled mode ────────────────────────────────────────────

        [Test]
        public void Disabled_Displacement_ReturnsZero()
        {
            float disp = _calc.CalculateHorizontalDisplacement(1f, DeltaTime1s, PlayerInputMode.Disabled);

            Assert.AreEqual(0f, disp, 0.001f,
                "Disabled mode must produce zero displacement");
        }

        // ── Day vs Night speed difference ────────────────────────────

        [Test]
        public void NightDisplacement_GreaterThan_DayDisplacement_SameInput()
        {
            float night = _calc.CalculateHorizontalDisplacement(1f, DeltaTime1s, PlayerInputMode.NightMovement);
            float day   = _calc.CalculateHorizontalDisplacement(1f, DeltaTime1s, PlayerInputMode.DayUI);

            Assert.Greater(night, day,
                "Night movement must be faster than day movement");
        }

        // ── DeltaTime scaling ─────────────────────────────────────────

        [Test]
        public void Day_Displacement_ScalesWithDeltaTime()
        {
            float full = _calc.CalculateHorizontalDisplacement(1f, DeltaTime1s,  PlayerInputMode.DayUI);
            float half = _calc.CalculateHorizontalDisplacement(1f, DeltaTime0p5, PlayerInputMode.DayUI);

            Assert.AreEqual(full * 0.5f, half, 0.001f);
        }
    }
}
