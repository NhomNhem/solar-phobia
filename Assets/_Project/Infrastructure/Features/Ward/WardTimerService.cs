using System;
using R3;
using SolarPhobia.Application.Features.Phase.Flow;
using SolarPhobia.Application.Features.Ward;
using SolarPhobia.Domain.Features.Ward;
using SolarPhobia.Domain.ValueObjects;
using SolarPhobia.Shared.Configuration;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace SolarPhobia.Infrastructure.Features.Ward
{
    public class WardTimerService : IWardTimerService, IWardTimerPort, IInitializable, ITickable, IDisposable
    {
        private readonly ReactiveProperty<float> _currentWard = new(0f);
        private readonly ReactiveProperty<SensoryTier> _currentTier = new(SensoryTier.Stable);
        private readonly Subject<Unit> _onDepleted = new();

        private readonly IPhaseStateMachine _phaseStateMachine;
        private readonly GameplayBalanceConfig _balanceConfig;

        private float _maxWard;
        private float _drainRate;
        private bool _isDepleted;
        private IDisposable _wardSubscription;

        public float CurrentWard
        {
            get => _currentWard.Value;
            set => _currentWard.Value = value;
        }
        public ReadOnlyReactiveProperty<float> CurrentWardObservable => _currentWard;
        public ReadOnlyReactiveProperty<SensoryTier> CurrentTier => _currentTier;
        public Observable<Unit> OnDepleted => _onDepleted;
        public float MaxWard => _maxWard;

        float IWardTimerPort.CurrentWard
        {
            get => _currentWard.Value;
            set => _currentWard.Value = value;
        }

        public float GetCurrentWard()
        {
            return _currentWard.Value;
        }

        public bool TryApplyCost(float cost)
        {
            if (_currentWard.Value <= 0f) return false;

            float newValue = _currentWard.Value - cost;
            _currentWard.Value = Mathf.Max(0f, newValue);
            return true;
        }

        public Observable<float> OnWardChanged => _currentWard;

        public WardTimerService(IPhaseStateMachine phaseStateMachine)
            : this(GameplayBalanceConfig.CreateDefault(), phaseStateMachine)
        {
        }

        [Inject]
        public WardTimerService(GameplayBalanceConfig balanceConfig, IPhaseStateMachine phaseStateMachine)
        {
            _balanceConfig = balanceConfig ?? GameplayBalanceConfig.CreateDefault();
            _phaseStateMachine = phaseStateMachine;
            _maxWard = 0f;
            _drainRate = _balanceConfig.WardTimer.DefaultBaseDrain;
            _isDepleted = false;

            _wardSubscription = _currentWard.Subscribe(HandleWardChanged);
        }

        public void Initialize()
        {
        }

        public void Tick()
        {
            if (_phaseStateMachine.CurrentState != PhaseState.NightSurvival)
            {
                return;
            }

            if (_drainRate > 0 && _currentWard.Value > 0)
            {
                float newValue = _currentWard.Value - (_drainRate * Time.deltaTime);
                _currentWard.Value = Mathf.Max(0f, newValue);
            }
        }

        public void Initialize(int ghostsSaved, int failedLightInterrupts, int soulPanicEvents)
        {
            WardTimerBalanceConfig config = _balanceConfig.WardTimer;

            float dayPenalties = (failedLightInterrupts * config.FailedLightInterruptPenalty)
                               + (soulPanicEvents * config.SoulPanicPenalty);
            dayPenalties = Mathf.Min(dayPenalties, config.MaxDayPenalties);

            float initialWard = config.BaseWardSec + (ghostsSaved * config.WardPerGhostSec) - dayPenalties;

            _maxWard = Mathf.Max(0f, initialWard);
            _currentWard.Value = _maxWard;
            _isDepleted = false;

            UpdateSensoryTier();
        }

        public void ApplyPenalty(float amount)
        {
            if (_currentWard.Value <= 0) return;

            _currentWard.Value = Mathf.Max(0f, _currentWard.Value - amount);
        }

        public void SetDrainRate(float baseDrain, int boneCount, float hallucinationMultiplier)
        {
            _drainRate = baseDrain + (boneCount * hallucinationMultiplier);
        }

        public void Dispose()
        {
            _wardSubscription?.Dispose();
            _currentWard.Dispose();
            _currentTier.Dispose();
            _onDepleted.Dispose();
        }

        private void HandleWardChanged(float newValue)
        {
            UpdateSensoryTier();

            if (newValue <= 0 && !_isDepleted)
            {
                _isDepleted = true;
                _onDepleted.OnNext(Unit.Default);
            }
        }

        private void UpdateSensoryTier()
        {
            if (_maxWard <= 0)
            {
                _currentTier.Value = SensoryTier.DeathSpiral;
                return;
            }

            float percentage = _currentWard.Value / _maxWard;
            float secondsRemaining = _currentWard.Value;

            SensoryTier newTier;
            if (secondsRemaining <= 10f)
            {
                newTier = SensoryTier.DeathSpiral;
            }
            else if (percentage <= 0.25f)
            {
                newTier = SensoryTier.Panic;
            }
            else if (percentage <= 0.50f)
            {
                newTier = SensoryTier.HeavyBurden;
            }
            else if (percentage <= 0.75f)
            {
                newTier = SensoryTier.CreepingDread;
            }
            else
            {
                newTier = SensoryTier.Stable;
            }

            _currentTier.Value = newTier;
        }
    }
}


