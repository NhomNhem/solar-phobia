using System;

namespace SolarPhobia.Application.Consequences.WaterTrap
{
    /// <summary>
    /// Service for Water Trap curse effect (Drag - Linh abandoned).
    /// Applies continuous DoT when player stands in water hazard zones.
    /// </summary>
    public interface IWaterTrapEffectService : IDisposable
    {
        /// <summary>Whether water trap DoT is currently active.</summary>
        bool IsActive { get; }

        /// <summary>Total damage applied during current activation (for testing).</summary>
        float TotalDamageApplied { get; }

        /// <summary>
        /// Tick the service each frame to apply continuous damage when active.
        /// </summary>
        void Tick(float deltaTime);
    }
}

