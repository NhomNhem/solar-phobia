// Assets/_Project/Application/Services/StrikeWarningController.cs
using System.Collections.Generic;
using R3;
using SolarPhobia.Domain.ValueObjects;
using UnityEngine;
using VContainer;

namespace SolarPhobia.Application.Services
{
    /// <summary>
    /// Manages the ordered set of active strike warnings and drives the Warning_Icon display.
    /// Implements TR-player-009: Strike Warning Integration (multi-warning priority selection).
    ///
    /// Rules:
    ///   - Maintains ordered List&lt;StrikeWarning&gt; of all active warnings (newest at end, LIFO resolution)
    ///   - Exposes IsWarningActive (ReactiveProperty&lt;bool&gt;) — true iff list is non-empty
    ///   - Phase-gated: only active in NightMovement mode; ClearAll() on any other mode
    ///   - ClearAll() called on phase exit
    /// </summary>
    public class StrikeWarningController : IStrikeWarningController
    {
        // ── R3 Reactive State ──────────────────────────────────────
        private readonly ReactiveProperty<bool> _isWarningActive = new(false);

        // ── State ─────────────────────────────────────────────────
        private readonly List<StrikeWarning> _activeWarnings = new();
        private int     _nextWarningId;
        private Vector2 _playerPosition;

        // ── Dependencies ───────────────────────────────────────────
        private readonly IMapSpawnDirector _mapDirector;

        // ── Public Interface ───────────────────────────────────────
        /// <inheritdoc/>
        public ReadOnlyReactiveProperty<bool> IsWarningActive => _isWarningActive;

        /// <inheritdoc/>
        public IReadOnlyList<StrikeWarning> ActiveWarnings => _activeWarnings;

        // ── Constructor ────────────────────────────────────────────
        [Inject]
        public StrikeWarningController(IMapSpawnDirector mapDirector)
        {
            _mapDirector = mapDirector;
        }

        // ── IStrikeWarningController ───────────────────────────────

        /// <inheritdoc/>
        public void OnStrikeWarningReceived(bool warningActive, PlayerInputMode mode, Vector2 playerPosition)
        {
            if (mode != PlayerInputMode.NightMovement)
            {
                ClearAll();
                return;
            }

            _playerPosition = playerPosition;

            if (warningActive)
            {
                _activeWarnings.Add(new StrikeWarning(_nextWarningId++, playerPosition));
            }
            else if (_activeWarnings.Count > 0)
            {
                // Resolve most-recently-registered warning (LIFO — matches single StrikeController)
                _activeWarnings.RemoveAt(_activeWarnings.Count - 1);
            }

            Reevaluate();
        }

        /// <inheritdoc/>
        public void ReportPlayerPosition(Vector2 position, Bounds bounds, PlayerInputMode mode)
        {
            if (mode != PlayerInputMode.NightMovement)
            {
                return;
            }

            if (_mapDirector == null)
            {
                Debug.LogWarning("[StrikeWarningController] MapDirector is null — skipping UpdatePlayerPosition.");
                return;
            }

            _playerPosition = position;
            _mapDirector.UpdatePlayerPosition(position, bounds);
        }

        /// <inheritdoc/>
        public void ClearAll()
        {
            _activeWarnings.Clear();
            SetWarning(false);
        }

        // ── Private ────────────────────────────────────────────────
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
