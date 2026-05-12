using R3;
using SolarPhobia.Shared.Configuration;
using VContainer;

namespace SolarPhobia.Application.Resources
{
    /// <summary>
    /// Manages resource effects including Time Drain modifiers from relic pickups.
    /// </summary>
    public class ResourceEffectsService : IResourceEffectsService
    {
        private readonly GameplayBalanceConfig _balanceConfig;

        // Reactive properties
        private readonly ReactiveProperty<ResourceType> _resourcePickedUp = new(ResourceType.NgocCot);
        private readonly ReactiveProperty<bool> _isTimeDrainActive = new(false);
        private readonly ReactiveProperty<float> _timeDrainMultiplier = new(1.0f);

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceEffectsService"/> class.
        /// </summary>
        public ResourceEffectsService()
            : this(GameplayBalanceConfig.CreateDefault())
        {
        }

        [Inject]
        public ResourceEffectsService(GameplayBalanceConfig balanceConfig)
        {
            _balanceConfig = balanceConfig ?? GameplayBalanceConfig.CreateDefault();
            _timeDrainMultiplier.Value = _balanceConfig.ResourceEffects.DefaultNgocCotDrainMultiplier;
        }

        // IResourceEffectsService implementation
        public ReadOnlyReactiveProperty<ResourceType> OnResourcePickedUp => _resourcePickedUp;
        public bool IsTimeDrainActive => _isTimeDrainActive.Value;
        public float TimeDrainMultiplier => _timeDrainMultiplier.Value;

        /// <summary>
        /// Handles resource pickup event.
        /// </summary>
        public void HandleResourcePickup(ResourceType resourceType)
        {
            if (resourceType == ResourceType.NgocCot)
            {
                _resourcePickedUp.Value = resourceType;
                
                // Activate Time Drain
                _isTimeDrainActive.Value = true;
                // Multiplier will be provided by NgocCotService in future integration
                _timeDrainMultiplier.Value = _balanceConfig.ResourceEffects.DefaultNgocCotDrainMultiplier;
            }
        }
    }
}

