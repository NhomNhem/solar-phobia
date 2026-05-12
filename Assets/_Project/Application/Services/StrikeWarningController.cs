using System.Collections.Generic;
using NhemDangFugBixs.NhemLogging;
using R3;
using SolarPhobia.Domain.ValueObjects;
using VContainer;

namespace SolarPhobia.Application.Services
{
    /// <summary>
    /// Manages the ordered set of active strike warnings and drives the Warning_Icon display.
    /// Implements TR-player-009: Strike Warning Integration (multi-warning priority selection).
    /// </summary>
    public class StrikeWarningController : IStrikeWarningController
    {
        [Inject] internal INhemLogger _logger = new NhemUnityLogger();

        // ── R3 Reactive State ──────────────────────────────────────
        private readonly ReactiveProperty<bool> _isWarningActive = new(false);

        // ── State ─────────────────────────────────────────────────
        private readonly List<StrikeWarning> _activeWarnings = new();
        private int _nextWarningId;

        // ── Dependencies ───────────────────────────────────────────
        private readonly IMapSpawnDirector _mapDirector;

        // ── Public Interface ───────────────────────────────────────
        public ReadOnlyReactiveProperty<bool> IsWarningActive => _isWarningActive;
        public IReadOnlyList<StrikeWarning> ActiveWarnings => _activeWarnings;

        [Inject]
        public StrikeWarningController(IMapSpawnDirector mapDirector)
        {
            _mapDirector = mapDirector;
        }

        public void OnStrikeWarningReceived(bool warningActive, PlayerInputMode mode, Float2 playerPosition)
        {
            if (mode != PlayerInputMode.NightMovement)
            {
                ClearAll();
                return;
            }

            if (warningActive)
            {
                _activeWarnings.Add(new StrikeWarning(_nextWarningId++, playerPosition));
            }
            else if (_activeWarnings.Count > 0)
            {
                _activeWarnings.RemoveAt(_activeWarnings.Count - 1);
            }

            Reevaluate();
        }

        public void ReportPlayerPosition(Float2 position, Bounds2D bounds, PlayerInputMode mode)
        {
            if (mode != PlayerInputMode.NightMovement)
            {
                return;
            }

            if (_mapDirector == null)
            {
                _logger.LogWarning("[StrikeWarningController] MapDirector is null — skipping UpdatePlayerPosition.");
                return;
            }

            _mapDirector.UpdatePlayerPosition(position, bounds);
        }

        public void ClearAll()
        {
            _activeWarnings.Clear();
            SetWarning(false);
        }

        private void Reevaluate()
        {
            SetWarning(_activeWarnings.Count > 0);
        }

        private void SetWarning(bool value)
        {
            if (_isWarningActive.Value != value)
            {
                _isWarningActive.Value = value;
            }
        }
    }
}
