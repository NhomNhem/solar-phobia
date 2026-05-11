// Assets/_Project/Application/Services/Movement2DCalculator.cs
using UnityEngine;
using SolarPhobia.Domain.ValueObjects;
using VContainer;

namespace SolarPhobia.Application.Services
{
    /// <summary>
    /// Calculates 2D horizontal movement for the player each frame.
    /// Implements Master GDD V5.0 — A/D movement for Day and Night phases.
    ///
    /// Day phase formula:
    ///   displacement = inputX * dayMoveSpeed * deltaTime
    ///   Applied via: transform.position += new Vector3(displacement, 0, 0)
    ///
    /// Night phase formula:
    ///   velocityX = inputX * nightMoveSpeed
    ///   Applied via: rigidbody2D.velocity = new Vector2(velocityX, rb.velocity.y)
    ///
    /// Pure calculation service — no Unity scene state, fully testable in Editor.
    /// </summary>
    public class Movement2DCalculator : IMovement2DCalculator
    {
        // ── Constants ─────────────────────────────────────────────
        /// <summary>Default night movement speed (units/second).</summary>
        public const float DefaultNightMoveSpeed = 5.0f;

        /// <summary>Default day movement speed — slower than night (units/second).</summary>
        public const float DefaultDayMoveSpeed = 2.0f;

        /// <summary>Minimum night speed (per GDD Tuning Knobs).</summary>
        public const float MinNightMoveSpeed = 2.0f;

        /// <summary>Maximum night speed (per GDD Tuning Knobs).</summary>
        public const float MaxNightMoveSpeed = 8.0f;

        /// <summary>Minimum day speed.</summary>
        public const float MinDayMoveSpeed = 1.0f;

        /// <summary>Maximum day speed.</summary>
        public const float MaxDayMoveSpeed = 4.0f;

        /// <summary>Default sprint speed multiplier (per GDD Tuning Knobs).</summary>
        public const float DefaultSprintMultiplier = 1.8f;

        /// <summary>Minimum allowed sprint multiplier (per GDD Tuning Knobs).</summary>
        public const float MinSprintMultiplier = 1.5f;

        /// <summary>Maximum allowed sprint multiplier (per GDD Tuning Knobs).</summary>
        public const float MaxSprintMultiplier = 3.0f;

        // ── State ─────────────────────────────────────────────────
        private float _nightMoveSpeed    = DefaultNightMoveSpeed;
        private float _dayMoveSpeed      = DefaultDayMoveSpeed;
        private float _sprintMultiplier  = DefaultSprintMultiplier;

        // ── Public Interface ───────────────────────────────────────
        /// <inheritdoc/>
        public float NightMoveSpeed
        {
            get => _nightMoveSpeed;
            set => _nightMoveSpeed = Mathf.Clamp(value, MinNightMoveSpeed, MaxNightMoveSpeed);
        }

        /// <inheritdoc/>
        public float DayMoveSpeed
        {
            get => _dayMoveSpeed;
            set => _dayMoveSpeed = Mathf.Clamp(value, MinDayMoveSpeed, MaxDayMoveSpeed);
        }

        /// <inheritdoc/>
        public float SprintMultiplier
        {
            get => _sprintMultiplier;
            set => _sprintMultiplier = Mathf.Clamp(value, MinSprintMultiplier, MaxSprintMultiplier);
        }

        // ── Constructor ────────────────────────────────────────────
        [Inject]
        public Movement2DCalculator() { }

        // ── IMovement2DCalculator ──────────────────────────────────
        /// <inheritdoc/>
        public float CalculateHorizontalDisplacement(float inputX, float deltaTime, PlayerInputMode mode)
        {
            return mode switch
            {
                PlayerInputMode.DayUI         => inputX * _dayMoveSpeed * deltaTime,
                PlayerInputMode.NightMovement => inputX * _nightMoveSpeed * deltaTime,
                _                             => 0f
            };
        }

        /// <inheritdoc/>
        public float CalculateNightVelocityX(float inputX, PlayerInputMode mode)
        {
            if (mode != PlayerInputMode.NightMovement)
            {
                return 0f;
            }

            return inputX * _nightMoveSpeed;
        }

        /// <inheritdoc/>
        public float CalculateNightVelocityX(float inputX, PlayerInputMode mode, bool isSprinting)
        {
            if (mode != PlayerInputMode.NightMovement)
            {
                return 0f;
            }

            float speed = _nightMoveSpeed * (isSprinting ? _sprintMultiplier : 1f);
            return inputX * speed;
        }
    }
}
