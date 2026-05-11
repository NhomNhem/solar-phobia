// Assets/_Project/Application/Services/Map/SweepExposureCalculator.cs
namespace SolarPhobia.Application.Services.Map
{
    /// <summary>
    /// Calculates whether the player is exposed during a boss searchlight sweep.
    /// Implements TR-map-003: Sweep Exposure Check.
    ///
    /// Formula: is_exposed = in_sweep_cone AND (not in_valid_cover)
    ///
    /// Rules:
    ///   - Player outside sweep cone → never exposed (regardless of cover)
    ///   - Player in cone + fully in valid cover → not exposed
    ///   - Player in cone + not in cover (or missing cover) → exposed
    ///   - Strike never applies inside shrine safe zone
    ///
    /// Pure static logic — no Unity scene state, fully testable in Editor.
    /// </summary>
    public static class SweepExposureCalculator
    {
        /// <summary>
        /// Returns true when the player is exposed to the boss searchlight.
        /// </summary>
        /// <param name="inSweepCone">Whether the player is inside the active sweep band.</param>
        /// <param name="inValidCover">
        /// Whether the player is fully inside a valid cover volume.
        /// Pass false when cover collider is missing (treat as exposed).
        /// </param>
        /// <param name="inShrineZone">
        /// Whether the player is inside the shrine safe zone.
        /// Strike is suppressed in the safe zone regardless of exposure.
        /// </param>
        public static bool IsExposed(bool inSweepCone, bool inValidCover, bool inShrineZone = false)
        {
            if (inShrineZone)
            {
                return false;
            }

            return inSweepCone && !inValidCover;
        }
    }
}
