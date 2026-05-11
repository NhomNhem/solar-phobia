// Assets/_Project/Application/Services/Interfaces/IPlatformerFeelController.cs
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Services
{
    /// <summary>
    /// Manages precision platformer feel mechanics: Coyote Time and Jump Buffering.
    /// Implements Master GDD V5.0 Section 3.1 requirements.
    ///
    /// Coyote Time (0.1s):
    ///   Player can jump briefly after walking off a ledge.
    ///   Window starts when player becomes airborne without jumping.
    ///
    /// Jump Buffering (0.15s):
    ///   Jump input registered slightly before landing.
    ///   When player lands within the buffer window, jump fires immediately.
    ///
    /// Both only active during NightMovement mode.
    /// </summary>
    public interface IPlatformerFeelController
    {
        /// <summary>Coyote Time window in seconds. Default: 0.1s.</summary>
        float CoyoteTime { get; set; }

        /// <summary>Jump Buffer window in seconds. Default: 0.15s.</summary>
        float JumpBufferTime { get; set; }

        /// <summary>Whether a coyote jump is currently available.</summary>
        bool IsCoyoteJumpAvailable { get; }

        /// <summary>Whether a buffered jump is pending.</summary>
        bool IsJumpBuffered { get; }

        /// <summary>
        /// Updates coyote time and jump buffer each frame.
        /// </summary>
        /// <param name="isGrounded">Whether the player is currently on the ground.</param>
        /// <param name="jumpInput">Whether jump was pressed this frame.</param>
        /// <param name="mode">Current player input mode.</param>
        /// <param name="deltaTime">Frame delta time in seconds.</param>
        void Tick(bool isGrounded, bool jumpInput, PlayerInputMode mode, float deltaTime);

        /// <summary>
        /// Returns true if a jump should fire this frame.
        /// Consumes the coyote window and/or buffer on use.
        /// </summary>
        bool ShouldJump(bool jumpInput, bool isGrounded);

        /// <summary>Resets coyote window (call when player jumps intentionally).</summary>
        void ConsumeJump();
    }
}
