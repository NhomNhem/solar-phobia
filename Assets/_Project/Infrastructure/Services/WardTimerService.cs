using R3;
using SolarPhobia.Application.Phase.Flow;
using SolarPhobia.Application.Ward;
using SolarPhobia.Domain.ValueObjects;
using SolarPhobia.Shared.Configuration;
using UnityEngine;
using VContainer;
using VContainer.Unity;
namespace SolarPhobia.Infrastructure.Services
{
    /// <summary>
    /// Implementation of Ward Timer service.
    /// Implements TR-state-005: Ward Timer Initialization — Base + (Saved × 30) Formula
    /// Also implements Application-layer IWardTimerPort for cross-layer cost operations.
    /// </summary>
    public class WardTimerService : Domain.IWardTimerService, IWardTimerPort, IInitializable, ITickable
    {
        // ── R3 Reactive State ──────────────────────────────────────
        private readonly ReactiveProperty<float> _currentWard = new(0f);
        private readonly ReactiveProperty<SensoryTier> _currentTier = new(SensoryTier.Stable);
        private readonly Subject<Unit> _onDepleted = new();

        // ── Dependencies ────────────────────────────────────────
        private readonly IPhaseStateMachine _phaseStateMachine;
        private readonly GameplayBalanceConfig _balanceConfig;

        // ── State ────────────────────────────────────────────────
        private float _maxWard;
        private float _drainRate;
        private bool _isDepleted;

        // ── Public Properties ─────────────────────────────────────
        public float CurrentWard => _currentWard.Value;

        public ReadOnlyReactiveProperty<float> CurrentWardObservable => _currentWard;

        public ReadOnlyReactiveProperty<SensoryTier> CurrentTier => _currentTier;

        public Observable<Unit> OnDepleted => _onDepleted;

        public float MaxWard => _maxWard;

        // ── Application-layer IWardTimerPort ─────────────────────

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

        // ── Constructor ───────────────────────────────────────────
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

            // Subscribe to ward changes to update sensory tier
            _currentWard.Subscribe(HandleWardChanged);
        }

        // ── IInitializable ────────────────────────────────────────
        public void Initialize()
        {
            // Initialization happens via Initialize(int, int, int) at day→night transition
        }

        // ── ITickable ─────────────────────────────────────────────
        public void Tick()
        {
            if (_phaseStateMachine.CurrentState != PhaseState.NightSurvival)
            {
                return;
            }

            // Apply time-based drain
            if (_drainRate > 0 && _currentWard.Value > 0)
            {
                float newValue = _currentWard.Value - (_drainRate * Time.deltaTime);
                _currentWard.Value = Mathf.Max(0f, newValue);
            }
        }

        // ── Public Methods ─────────────────────────────────────────
        /// <summary>
        /// Initialize Ward timer for night phase.
        /// Formula: InitialWard = 10 + (GhostsSaved × 30) - DayPenalties
        /// DayPenalties = (FailedLightInterrupts × 10) + (SoulPanicEvents × 5), capped at 30s
        /// </summary>
        public void Initialize(int ghostsSaved, int failedLightInterrupts, int soulPanicEvents)
        {
            WardTimerBalanceConfig config = _balanceConfig.WardTimer;

            // Calculate day penalties (capped at 30s)
            float dayPenalties = (failedLightInterrupts * config.FailedLightInterruptPenalty)
                               + (soulPanicEvents * config.SoulPanicPenalty);
            dayPenalties = Mathf.Min(dayPenalties, config.MaxDayPenalties);

            // Calculate initial ward (can go negative, clamped to 0 for gameplay)
            float initialWard = config.BaseWardSec + (ghostsSaved * config.WardPerGhostSec) - dayPenalties;

            _maxWard = Mathf.Max(0f, initialWard);
            _currentWard.Value = _maxWard;
            _isDepleted = false;

            // Update tier based on initial value
            UpdateSensoryTier();
        }

        /// <inheritdoc/>
        public void ApplyPenalty(float amount)
        {
            if (_currentWard.Value <= 0) return;

            _currentWard.Value = Mathf.Max(0f, _currentWard.Value - amount);
        }

        /// <summary>
        /// Set the passive drain rate.
        /// Formula: baseDrain + (boneCount × hallucinationMultiplier)
        /// </summary>
        public void SetDrainRate(float baseDrain, int boneCount, float hallucinationMultiplier)
        {
            // Additive formula as per Story-008: base + (bones × multiplier)
            _drainRate = baseDrain + (boneCount * hallucinationMultiplier);
        }

        // ── Private Methods ───────────────────────────────────────
        private void HandleWardChanged(float newValue)
        {
            UpdateSensoryTier();

            // Check for depletion
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

