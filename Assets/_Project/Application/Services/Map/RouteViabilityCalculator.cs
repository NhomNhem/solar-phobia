// Assets/_Project/Application/Services/Map/RouteViabilityCalculator.cs
using UnityEngine;

namespace SolarPhobia.Application.Services.Map
{
    /// <summary>
    /// Evaluates whether the player has enough Ward time to reach the end shrine.
    /// Implements TR-map-005: Route Viability Check (fairness guard).
    ///
    /// Formula:
    ///   is_viable = ward_remaining > (distance_to_goal / effective_move_speed) + safety_buffer_sec
    ///
    /// If false immediately after first relic pickup, reduce active TimeDrain one tier.
    /// Pure static logic — no Unity scene state.
    /// </summary>
    public static class RouteViabilityCalculator
    {
        /// <summary>Default safety buffer in seconds (per GDD Tuning Knobs).</summary>
        public const float DefaultSafetyBufferSec = 12f;

        /// <summary>Minimum safety buffer.</summary>
        public const float MinSafetyBufferSec = 5f;

        /// <summary>Maximum safety buffer.</summary>
        public const float MaxSafetyBufferSec = 30f;

        /// <summary>
        /// Returns true when the player has sufficient Ward to reach the goal.
        /// </summary>
        /// <param name="wardRemaining">Current Ward Timer value in seconds.</param>
        /// <param name="distanceToGoal">Remaining distance to end shrine in units.</param>
        /// <param name="effectiveMoveSpeed">Current movement speed in units/second.</param>
        /// <param name="safetyBufferSec">Fairness margin in seconds.</param>
        public static bool IsRouteViable(
            float wardRemaining,
            float distanceToGoal,
            float effectiveMoveSpeed,
            float safetyBufferSec = DefaultSafetyBufferSec)
        {
            if (effectiveMoveSpeed <= 0f)
            {
                return false; // Cannot move — not viable
            }

            float travelTime = distanceToGoal / effectiveMoveSpeed;
            return wardRemaining > travelTime + safetyBufferSec;
        }
    }
}
