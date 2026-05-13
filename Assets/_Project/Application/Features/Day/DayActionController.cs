// Assets/_Project/Application/Services/DayActionController.cs
using System;
using R3;
using SolarPhobia.Domain.ValueObjects;
using VContainer;

namespace SolarPhobia.Application.Features.Day
{
    /// <summary>
    /// Manages Day phase special actions: Swap (Space) and Shove (F).
    /// Implements Master GDD V5.0 Section 2.1.
    ///
    /// Both actions are phase-gated to DayUI mode only.
    /// Events fire once per button press — not held.
    /// </summary>
    public class DayActionController : IDayActionController, IDisposable
    {
        // ── R3 Reactive State ──────────────────────────────────────
        private readonly Subject<bool> _onSwap  = new();
        private readonly Subject<bool> _onShove = new();

        // ── Public Interface ───────────────────────────────────────
        /// <inheritdoc/>
        public Observable<bool> OnSwap  => _onSwap;

        /// <inheritdoc/>
        public Observable<bool> OnShove => _onShove;

        // ── Constructor ────────────────────────────────────────────
        public DayActionController() { }

        // ── IDayActionController ───────────────────────────────────
        /// <inheritdoc/>
        public void TryActions(bool swapInput, bool shoveInput, PlayerInputMode mode)
        {
            if (mode != PlayerInputMode.DayUI)
            {
                return;
            }

            if (swapInput)
            {
                _onSwap.OnNext(true);
            }

            if (shoveInput)
            {
                _onShove.OnNext(true);
            }
        }

        public void Dispose()
        {
            _onSwap.Dispose();
            _onShove.Dispose();
        }
    }
}


