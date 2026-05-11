// Assets/_Project/Application/Services/Map/TimeDrainCalculator.cs
using UnityEngine;

namespace SolarPhobia.Application.Services.Map
{
    /// <summary>
    /// Calculates the effective Ward drain rate when carrying Bone Relics.
    /// Implements TR-map-008: Time Drain Under Bone Relic.
    ///
    /// Formula:
    ///   effective_drain = base_drain_rate * (1 + bones_carried * hallucination_multiplier)
    ///
    /// Owned by Map &amp; Spawn Director (per GDD Interface Ownership section).
    /// Pure static logic — no Unity scene state.
    /// </summary>
    public static class TimeDrainCalculator
    {
        /// <summary>Default base drain rate (Ward seconds per second).</summary>
        public const float DefaultBaseDrainRate = 1.0f;

        /// <summary>Default hallucination multiplier per bone relic.</summary>
        public const float DefaultHallucinationMultiplier = 0.5f;

        /// <summary>Maximum safe drain cap (per GDD edge case).</summary>
        public const float MaxSafeDrainRate = 20.0f;

        /// <summary>
        /// Calculates the effective Ward drain rate for the current frame.
        /// Result is clamped to MaxSafeDrainRate.
        /// </summary>
        /// <param name="baseDrainRate">Base drain rate (default 1.0 s/s).</param>
        /// <param name="bonesCarried">Number of Bone Relics currently carried (0–3).</param>
        /// <param name="hallucinationMultiplier">Extra drain multiplier per bone (default 0.5).</param>
        public static float CalculateEffectiveDrain(
            float baseDrainRate,
            int   bonesCarried,
            float hallucinationMultiplier = DefaultHallucinationMultiplier)
        {
            float effective = baseDrainRate * (1f + bonesCarried * hallucinationMultiplier);
            return Mathf.Min(effective, MaxSafeDrainRate);
        }
    }
}
