using System;
using R3;
using SolarPhobia.Domain;
using SolarPhobia.Domain.Events;
using SolarPhobia.Domain.ValueObjects;
using VContainer;
using VContainer.Unity;

namespace SolarPhobia.Infrastructure.Features.Sensory
{
    public class SensoryTierService : ISensoryTierService, IInitializable, IDisposable
    {
        private readonly ReactiveProperty<SensoryTier> _currentTier = new(SensoryTier.Stable);
        private readonly Subject<SensoryTierChangedEvent> _onTierChanged = new();
        private readonly Subject<NightFailedEvent> _onNightFailed;

        private float _maxWard;
        private bool _isDepleted;
        private IDisposable _wardSubscription;

        private const float Tier1Threshold = 0.75f;
        private const float Tier2Threshold = 0.50f;
        private const float Tier3Threshold = 0.25f;
        private const float Tier4Seconds   = 10f;

        public ReadOnlyReactiveProperty<SensoryTier> CurrentTier => _currentTier;
        public Observable<SensoryTierChangedEvent> OnTierChanged => _onTierChanged;
        public Observable<NightFailedEvent> OnNightFailed => _onNightFailed;

        public SensoryTierService(Subject<NightFailedEvent> onNightFailed)
        {
            _onNightFailed = onNightFailed;
            _maxWard = 0f;
            _isDepleted = false;
        }

        public void Initialize()
        {
        }

        public void Initialize(ReadOnlyReactiveProperty<float> wardObservable, float maxWard)
        {
            _maxWard = maxWard;
            _isDepleted = false;

            _wardSubscription?.Dispose();
            _wardSubscription = wardObservable.Subscribe(OnWardChanged);

            SensoryTier initialTier = CalculateTier(wardObservable.CurrentValue, maxWard);
            _currentTier.Value = initialTier;
        }

        public void Dispose()
        {
            _wardSubscription?.Dispose();
            _wardSubscription = null;
        }

        private void OnWardChanged(float wardValue)
        {
            if (wardValue <= 0f)
            {
                if (!_isDepleted)
                {
                    _isDepleted = true;
                    _onNightFailed.OnNext(new NightFailedEvent(0f, 0f));
                }
                return;
            }

            SensoryTier newTier = CalculateTier(wardValue, _maxWard);

            if (newTier == _currentTier.Value)
            {
                return;
            }

            SensoryTier previousTier = _currentTier.Value;
            _currentTier.Value = newTier;

            _onTierChanged.OnNext(new SensoryTierChangedEvent(
                newTier,
                previousTier,
                wardValue,
                _maxWard
            ));
        }

        private SensoryTier CalculateTier(float wardValue, float maxWard)
        {
            if (maxWard <= 0f)
            {
                return SensoryTier.DeathSpiral;
            }

            if (wardValue <= Tier4Seconds)
            {
                return SensoryTier.DeathSpiral;
            }

            float percentage = wardValue / maxWard;

            if (percentage <= Tier3Threshold)
            {
                return SensoryTier.Panic;
            }

            if (percentage <= Tier2Threshold)
            {
                return SensoryTier.HeavyBurden;
            }

            if (percentage <= Tier1Threshold)
            {
                return SensoryTier.CreepingDread;
            }

            return SensoryTier.Stable;
        }
    }
}
