using R3;

namespace SolarPhobia.Application.Services
{
    /// <summary>
    /// Types of resources that can be picked up.
    /// </summary>
    public enum ResourceType
    {
        NgocCot, // Bone Relic
        // Other resource types can be added here
    }

    /// <summary>
    /// Manages resource effects including Time Drain modifiers from relic pickups.
    /// </summary>
    public interface IResourceEffectsService
    {
        /// <summary>
        /// Event stream for when resources are picked up.
        /// </summary>
        ReadOnlyReactiveProperty<ResourceType> OnResourcePickedUp { get; }

        /// <summary>
        /// Gets whether Time Drain modifier is currently active.
        /// </summary>
        bool IsTimeDrainActive { get; }

        /// <summary>
        /// Gets the current Time Drain multiplier.
        /// </summary>
        float TimeDrainMultiplier { get; }

        /// <summary>
        /// Handles resource pickup event.
        /// </summary>
        void HandleResourcePickup(ResourceType resourceType);
    }
}