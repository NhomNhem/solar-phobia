// Assets/_Project/Application/Services/PlatformerFeelController.cs
using SolarPhobia.Domain.ValueObjects;
using UnityEngine;
using VContainer;

namespace SolarPhobia.Application.Services
{
    /// <summary>
    /// Manages Coyote Time and Jump Buffering for precision platformer feel.
    /// Implements Master GDD V5.0 Section 3.1.
    ///
    /// Coyote Time:
    ///   When player walks off a ledge (was grounded, now airborne without jumping),
    ///   a window of CoyoteTime seconds opens. ShouldJump() returns true during this window.
    ///
    /// Jump Buffering:
    ///   When jump is pressed while airborne, the input is buffered for JumpBufferTime seconds.
    ///   If the player lands within that window, ShouldJump() returns true on landing.
    /// </summary>
    public class PlatformerFeelController : IPlatformerFeelController
    {
        // ── Constants ─────────────────────────────────────────────
        /// <summary>Default coyote time window (seconds).</summary>
        public const float DefaultCoyoteTime = 0.1f;

        /// <summary>Default jump buffer window (seconds).</summary>
        public const float DefaultJumpBufferTime = 0.15f;

        // ── State ─────────────────────────────────────────────────
        private float _coyoteTime      = DefaultCoyoteTime;
        private float _jumpBufferTime  = DefaultJumpBufferTime;
        private float _coyoteRemaining;
        private float _jumpBufferRemaining;
        private bool  _wasGrounded;
        private bool  _jumpConsumed;

        // ── Public Interface ───────────────────────────────────────
        /// <inheritdoc/>
        public float CoyoteTime
        {
            get => _coyoteTime;
            set => _coyoteTime = Mathf.Max(0f, value);
        }

        /// <inheritdoc/>
        public float JumpBufferTime
        {
            get => _jumpBufferTime;
            set => _jumpBufferTime = Mathf.Max(0f, value);
        }

        /// <inheritdoc/>
        public bool IsCoyoteJumpAvailable => _coyoteRemaining > 0f && !_jumpConsumed;

        /// <inheritdoc/>
        public bool IsJumpBuffered => _jumpBufferRemaining > 0f;

        // ── Constructor ────────────────────────────────────────────
        [Inject]
        public PlatformerFeelController() { }

        // ── IPlatformerFeelController ──────────────────────────────
        /// <inheritdoc/>
        public void Tick(bool isGrounded, bool jumpInput, PlayerInputMode mode, float deltaTime)
        {
            if (mode != PlayerInputMode.NightMovement)
            {
                // Reset everything outside Night phase
                _coyoteRemaining    = 0f;
                _jumpBufferRemaining = 0f;
                _jumpConsumed       = false;
                _wasGrounded        = false;
                return;
            }

            // ── Coyote Time ────────────────────────────────────────
            if (isGrounded)
            {
                // Reset coyote window when grounded
                _coyoteRemaining = _coyoteTime;
                _jumpConsumed    = false;
            }
            else if (_wasGrounded && !_jumpConsumed)
            {
                // Just left the ground without jumping — coyote window already set
                // (it was set last frame when grounded)
            }

            // Count down coyote window while airborne
            if (!isGrounded && _coyoteRemaining > 0f)
            {
                _coyoteRemaining = Mathf.Max(0f, _coyoteRemaining - deltaTime);
            }

            // ── Jump Buffer ────────────────────────────────────────
            if (jumpInput)
            {
                _jumpBufferRemaining = _jumpBufferTime;
            }
            else if (_jumpBufferRemaining > 0f)
            {
                _jumpBufferRemaining = Mathf.Max(0f, _jumpBufferRemaining - deltaTime);
            }

            _wasGrounded = isGrounded;
        }

        /// <inheritdoc/>
        public bool ShouldJump(bool jumpInput, bool isGrounded)
        {
            // Direct jump: grounded + jump input
            if (isGrounded && jumpInput)
            {
                return true;
            }

            // Coyote jump: airborne within coyote window + jump input
            if (!isGrounded && jumpInput && IsCoyoteJumpAvailable)
            {
                return true;
            }

            // Buffered jump: just landed + had buffered jump input
            if (isGrounded && IsJumpBuffered)
            {
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public void ConsumeJump()
        {
            _coyoteRemaining     = 0f;
            _jumpBufferRemaining = 0f;
            _jumpConsumed        = true;
        }
    }
}
