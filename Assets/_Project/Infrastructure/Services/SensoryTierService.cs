// Assets/_Project/Infrastructure/Services/SensoryTierService.cs
using System;
using R3;
using SolarPhobia.Domain;
using SolarPhobia.Domain.Events;
using SolarPhobia.Domain.ValueObjects;
using VContainer;
using VContainer.Unity;

namespace SolarPhobia.Infrastructure.Services
{
    /// <summary>
    /// Implementation of sensory tier feedback service.
    /// Implements TR-state-005: Sensory tiers trigger at 75%, 50%, 25%, ≤10s thresholds.
    /// Emits tier events once per downward threshold crossing (AC-6 compliance).
    /// Emits NightFailedEvent exactly once when Ward reaches 0 (AC-5 compliance).
    /// Implements IDisposable — dispose when the night phase ends to release the Ward subscription.
    /// </summary>
    public class SensoryTierService : ISensoryTierService, IInitializable, IDisposable
    {
        // ── R3 Reactive State ──────────────────────────────────────
        private readonly ReactiveProperty<SensoryTier> _currentTier = new(SensoryTier.Stable);
        private readonly Subject<SensoryTierChangedEvent> _onTierChanged = new();
        private readonly Subject<NightFailedEvent> _onNightFailed;

        // ── State ─────────────────────────────────────────────────
        private float _maxWard;
        private bool _isDepleted;
        private IDisposable _wardSubscription;

        // ── Constants ────────────────────────────────────────────
        private const float Tier1Threshold = 0.75f;  // Stable → CreepingDread
        private const float Tier2Threshold = 0.50f;  // CreepingDread → HeavyBurden
        private const float Tier3Threshold = 0.25f;  // HeavyBurden → Panic
        private const float Tier4Seconds   = 10f;    // Panic → DeathSpiral (absolute seconds)

        // ── Public Properties ─────────────────────────────────────
        public ReadOnlyReactiveProperty<SensoryTier> CurrentTier => _currentTier;
        public Observable<SensoryTierChangedEvent> OnTierChanged => _onTierChanged;
        public Observable<NightFailedEvent> OnNightFailed => _onNightFailed;

        // ── Constructor ───────────────────────────────────────────
        [Inject]
        public SensoryTierService(Subject<NightFailedEvent> onNightFailed)
        {
            _onNightFailed = onNightFailed;
            _maxWard = 0f;
            _isDepleted = false;
        }

        // ── IInitializable ────────────────────────────────────────
        public void Initialize()
        {
            // No-op: real initialization requires Ward observable — call Initialize(ward, max).
        }

        // ── ISensoryTierService ────────────────────────────────────
        /// <summary>
        /// Binds the service to a Ward timer observable and sets the max Ward for percentage calculations.
        /// Safe to call again on re-initialization — disposes the previous subscription first.
        /// </summary>
        public void Initialize(ReadOnlyReactiveProperty<float> wardObservable, float maxWard)
        {
            _maxWard = maxWard;
            _isDepleted = false;

            // Release previous subscription before re-binding
            _wardSubscription?.Dispose();
            _wardSubscription = wardObservable.Subscribe(OnWardChanged);

            // Sync tier to current Ward value immediately
            SensoryTier initialTier = CalculateTier(wardObservable.CurrentValue, maxWard);
            _currentTier.Value = initialTier;
        }

        // ── IDisposable ───────────────────────────────────────────
        /// <summary>
        /// Releases the Ward subscription. Call when the night phase ends.
        /// </summary>
        public void Dispose()
        {
            _wardSubscription?.Dispose();
            _wardSubscription = null;
        }

        // ── Private Methods ───────────────────────────────────────
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

        /// <summary>
        /// Calculates the sensory tier from current Ward value and max Ward.
        /// DeathSpiral (≤10s absolute) takes priority over percentage-based tiers.
        /// </summary>
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
