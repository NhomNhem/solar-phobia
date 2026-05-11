// Assets/_Project/Domain/ValueObjects/SensoryTier.cs
namespace SolarPhobia.Domain.ValueObjects
{
    /// <summary>
    /// Sensory degradation tiers for HUD-less Ward feedback.
    /// Based on GDD Section: Sensory Readability Tiers
    /// </summary>
    public enum SensoryTier
    {
        /// <summary>Ward > 75% - Clear, stable perception</summary>
        Stable = 1,
        
        /// <summary>Ward ≤ 75% - Vignette appears, lowpass filter on audio</summary>
        CreepingDread = 2,
        
        /// <summary>Ward ≤ 50% - Heavy breathing, increased BPM, dash penalty</summary>
        HeavyBurden = 3,
        
        /// <summary>Ward ≤ 25% - Chromatic aberration, controller rumble, whispers</summary>
        Panic = 4,
        
        /// <summary>Ward ≤ 10s - Tunnel vision, tinnitus, distorted sounds</summary>
        DeathSpiral = 5
    }
}
