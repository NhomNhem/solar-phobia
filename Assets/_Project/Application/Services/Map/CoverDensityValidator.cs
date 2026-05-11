// Assets/_Project/Application/Services/Map/CoverDensityValidator.cs
using UnityEngine;

namespace SolarPhobia.Application.Services.Map
{
    /// <summary>
    /// Validates that a generated lane has sufficient cover density.
    /// Implements TR-map-006: Cover Density Validation.
    ///
    /// Formula: cover_density = mo_thuong_count / lane_length
    /// Target range: 0.02–0.08 per unit length (per GDD).
    ///
    /// If below minimum, regenerate placement once.
    /// Force-spawn one SafeMound near start if opening segment has no cover.
    /// Pure static logic — no Unity scene state.
    /// </summary>
    public static class CoverDensityValidator
    {
        /// <summary>Minimum cover density (SafeMounds per unit length).</summary>
        public const float MinDensity = 0.02f;

        /// <summary>Maximum cover density.</summary>
        public const float MaxDensity = 0.08f;

        /// <summary>Default MoThuong count per lane (per GDD Tuning Knobs).</summary>
        public const int DefaultMoThuongCount = 14;

        /// <summary>
        /// Returns true when cover density is within the valid range.
        /// </summary>
        /// <param name="moThuongCount">Number of SafeMound objects in the lane.</param>
        /// <param name="laneLength">Total traversable lane length in units.</param>
        public static bool IsValid(int moThuongCount, float laneLength)
        {
            if (laneLength <= 0f)
            {
                return false;
            }

            float density = moThuongCount / laneLength;
            return density >= MinDensity && density <= MaxDensity;
        }

        /// <summary>
        /// Returns the cover density value for a given count and lane length.
        /// </summary>
        public static float CalculateDensity(int moThuongCount, float laneLength)
        {
            if (laneLength <= 0f)
            {
                return 0f;
            }

            return moThuongCount / laneLength;
        }

        /// <summary>
        /// Returns the minimum SafeMound count needed for a given lane length
        /// to meet the minimum density requirement.
        /// </summary>
        public static int MinCountForLane(float laneLength)
        {
            return Mathf.CeilToInt(MinDensity * laneLength);
        }
    }
}
