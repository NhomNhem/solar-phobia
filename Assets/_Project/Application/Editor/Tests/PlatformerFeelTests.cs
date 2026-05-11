// Assets/_Project/Application/Editor/Tests/PlatformerFeelTests.cs
using NUnit.Framework;
using SolarPhobia.Application.Services;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Tests
{
    /// <summary>
    /// Validates: Master GDD V5.0 Section 3.1 — Coyote Time (0.1s) + Jump Buffering (0.15s).
    /// Story 005-v2: Platformer Feel.
    /// </summary>
    [TestFixture]
    public class PlatformerFeelTests
    {
        private PlatformerFeelController _ctrl;

        private const float DeltaSmall = 0.016f; // ~60fps frame
        private const float Night = 0f; // used as PlayerInputMode.NightMovement shorthand

        [SetUp]
        public void Setup()
        {
            _ctrl = new PlatformerFeelController();
        }

        // ── Defaults ──────────────────────────────────────────────

        [Test]
        public void DefaultCoyoteTime_Is0p1Seconds()
        {
            Assert.AreEqual(0.1f, PlatformerFeelController.DefaultCoyoteTime);
        }

        [Test]
        public void DefaultJumpBufferTime_Is0p15Seconds()
        {
            Assert.AreEqual(0.15f, PlatformerFeelController.DefaultJumpBufferTime);
        }

        // ── Coyote Time ───────────────────────────────────────────

        [Test]
        public void CoyoteJump_Available_JustAfterLeavingGround()
        {
            // Frame 1: grounded
            _ctrl.Tick(isGrounded: true, jumpInput: false, PlayerInputMode.NightMovement, DeltaSmall);
            // Frame 2: just left ground (airborne, within coyote window)
            _ctrl.Tick(isGrounded: false, jumpInput: false, PlayerInputMode.NightMovement, DeltaSmall);

            Assert.IsTrue(_ctrl.IsCoyoteJumpAvailable,
                "Coyote jump must be available immediately after leaving ground");
        }

        [Test]
        public void CoyoteJump_ShouldJump_ReturnsTrue_WithinWindow()
        {
            _ctrl.Tick(true,  false, PlayerInputMode.NightMovement, DeltaSmall); // grounded
            _ctrl.Tick(false, false, PlayerInputMode.NightMovement, DeltaSmall); // left ground

            bool shouldJump = _ctrl.ShouldJump(jumpInput: true, isGrounded: false);

            Assert.IsTrue(shouldJump, "ShouldJump must return true within coyote window");
        }

        [Test]
        public void CoyoteJump_Expires_AfterCoyoteTime()
        {
            _ctrl.Tick(true,  false, PlayerInputMode.NightMovement, DeltaSmall);
            _ctrl.Tick(false, false, PlayerInputMode.NightMovement, DeltaSmall);

            // Advance past coyote window
            _ctrl.Tick(false, false, PlayerInputMode.NightMovement, _ctrl.CoyoteTime + 0.01f);

            Assert.IsFalse(_ctrl.IsCoyoteJumpAvailable,
                "Coyote window must expire after CoyoteTime seconds");
        }

        [Test]
        public void CoyoteJump_NotAvailable_WhenNeverGrounded()
        {
            // Start airborne — no coyote window
            _ctrl.Tick(false, false, PlayerInputMode.NightMovement, DeltaSmall);

            Assert.IsFalse(_ctrl.IsCoyoteJumpAvailable);
        }

        [Test]
        public void CoyoteJump_Consumed_AfterUse()
        {
            _ctrl.Tick(true,  false, PlayerInputMode.NightMovement, DeltaSmall);
            _ctrl.Tick(false, false, PlayerInputMode.NightMovement, DeltaSmall);

            _ctrl.ConsumeJump();

            Assert.IsFalse(_ctrl.IsCoyoteJumpAvailable,
                "Coyote window must be consumed after jump");
        }

        [Test]
        public void CoyoteJump_Resets_OnLanding()
        {
            _ctrl.Tick(true,  false, PlayerInputMode.NightMovement, DeltaSmall); // grounded
            _ctrl.Tick(false, false, PlayerInputMode.NightMovement, DeltaSmall); // airborne
            _ctrl.ConsumeJump();

            // Land again
            _ctrl.Tick(true, false, PlayerInputMode.NightMovement, DeltaSmall);
            _ctrl.Tick(false, false, PlayerInputMode.NightMovement, DeltaSmall);

            Assert.IsTrue(_ctrl.IsCoyoteJumpAvailable,
                "Coyote window must reset after landing");
        }

        // ── Jump Buffering ────────────────────────────────────────

        [Test]
        public void JumpBuffer_Active_WhenJumpPressedAirborne()
        {
            _ctrl.Tick(false, jumpInput: true, PlayerInputMode.NightMovement, DeltaSmall);

            Assert.IsTrue(_ctrl.IsJumpBuffered,
                "Jump buffer must activate when jump pressed while airborne");
        }

        [Test]
        public void JumpBuffer_ShouldJump_ReturnsTrue_OnLanding()
        {
            // Press jump while airborne
            _ctrl.Tick(false, true, PlayerInputMode.NightMovement, DeltaSmall);

            // Land within buffer window
            bool shouldJump = _ctrl.ShouldJump(jumpInput: false, isGrounded: true);

            Assert.IsTrue(shouldJump,
                "Buffered jump must fire when player lands within buffer window");
        }

        [Test]
        public void JumpBuffer_Expires_AfterBufferTime()
        {
            _ctrl.Tick(false, true, PlayerInputMode.NightMovement, DeltaSmall);

            // Advance past buffer window
            _ctrl.Tick(false, false, PlayerInputMode.NightMovement, _ctrl.JumpBufferTime + 0.01f);

            Assert.IsFalse(_ctrl.IsJumpBuffered,
                "Jump buffer must expire after JumpBufferTime seconds");
        }

        [Test]
        public void JumpBuffer_Consumed_AfterUse()
        {
            _ctrl.Tick(false, true, PlayerInputMode.NightMovement, DeltaSmall);
            _ctrl.ConsumeJump();

            Assert.IsFalse(_ctrl.IsJumpBuffered);
        }

        // ── Phase gate ────────────────────────────────────────────

        [Test]
        public void CoyoteAndBuffer_Reset_OutsideNightMovement()
        {
            // Build up state in Night mode
            _ctrl.Tick(true,  false, PlayerInputMode.NightMovement, DeltaSmall);
            _ctrl.Tick(false, true,  PlayerInputMode.NightMovement, DeltaSmall);

            // Switch to DayUI
            _ctrl.Tick(false, true, PlayerInputMode.DayUI, DeltaSmall);

            Assert.IsFalse(_ctrl.IsCoyoteJumpAvailable, "Coyote must reset outside Night");
            Assert.IsFalse(_ctrl.IsJumpBuffered, "Buffer must reset outside Night");
        }

        // ── Direct jump (grounded + input) ────────────────────────

        [Test]
        public void DirectJump_Grounded_JumpInput_ShouldJump_True()
        {
            bool shouldJump = _ctrl.ShouldJump(jumpInput: true, isGrounded: true);

            Assert.IsTrue(shouldJump, "Direct jump must work when grounded");
        }

        [Test]
        public void NoJump_Airborne_NoBuffer_NoCoyote()
        {
            bool shouldJump = _ctrl.ShouldJump(jumpInput: false, isGrounded: false);

            Assert.IsFalse(shouldJump);
        }

        // ── Configurable windows ──────────────────────────────────

        [Test]
        public void CoyoteTime_Configurable()
        {
            _ctrl.CoyoteTime = 0.2f;

            _ctrl.Tick(true,  false, PlayerInputMode.NightMovement, DeltaSmall);
            _ctrl.Tick(false, false, PlayerInputMode.NightMovement, 0.15f); // within 0.2s window

            Assert.IsTrue(_ctrl.IsCoyoteJumpAvailable);
        }

        [Test]
        public void JumpBufferTime_Configurable()
        {
            _ctrl.JumpBufferTime = 0.3f;

            _ctrl.Tick(false, true, PlayerInputMode.NightMovement, DeltaSmall);
            _ctrl.Tick(false, false, PlayerInputMode.NightMovement, 0.25f); // within 0.3s window

            Assert.IsTrue(_ctrl.IsJumpBuffered);
        }
    }
}
