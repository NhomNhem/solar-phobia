// Assets/_Project/Application/Services/Interfaces/IMovement2DCalculator.cs
using UnityEngine;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Services
{
    /// <summary>
    /// Calculates 2D horizontal movement displacement for the player each frame.
    /// Implements Master GDD V5.0 — A/D movement for both Day and Night phases.
    ///
    /// Day phase:  slow X-axis walk — displacement applied via Transform.position.
    /// Night phase: sprint X-axis movement — velocity applied via Rigidbody2D.
    ///
    /// Gravity is handled by Rigidbody2D in Night phase (not calculated here).
    /// This service is pure math — no Unity scene state, fully testable in Editor.
    /// </summary>
    public interface IMovement2DCalculator
    {
        /// <summary>
        /// Night phase base movement speed (units/second).
        /// Default: 5.0, safe range: 2.0–8.0.
        /// </summary>
        float NightMoveSpeed { get; set; }

        /// <summary>
        /// Day phase movement speed (units/second) — slower than night.
        /// Default: 2.0, safe range: 1.0–4.0.
        /// </summary>
        float DayMoveSpeed { get; set; }

        /// <summary>
        /// Sprint speed multiplier applied when sprinting is active.
        /// Default: 1.8, safe range: 1.5–3.0 (per GDD Tuning Knobs).
        /// </summary>
        float SprintMultiplier { get; set; }

        /// <summary>
        /// Calculates the horizontal X-axis displacement for one frame.
        /// Returns 0 when <paramref name="mode"/> is <see cref="PlayerInputMode.Disabled"/>.
        /// </summary>
        /// <param name="inputX">
        /// Horizontal axis input: -1.0 (A/left) to +1.0 (D/right).
        /// </param>
        /// <param name="deltaTime">Frame delta time in seconds.</param>
        /// <param name="mode">Current player input mode.</param>
        /// <returns>
        /// Signed X displacement in units.
        /// Positive = right, negative = left.
        /// </returns>
        float CalculateHorizontalDisplacement(float inputX, float deltaTime, PlayerInputMode mode);

        /// <summary>
        /// Calculates the target horizontal velocity for Rigidbody2D (Night phase only).
        /// Returns 0 when mode is not NightMovement.
        /// </summary>
        float CalculateNightVelocityX(float inputX, PlayerInputMode mode);

        /// <summary>
        /// Calculates the target horizontal velocity for Rigidbody2D with optional sprint multiplier.
        /// Returns 0 when mode is not NightMovement.
        /// </summary>
        float CalculateNightVelocityX(float inputX, PlayerInputMode mode, bool isSprinting);
    }
}
