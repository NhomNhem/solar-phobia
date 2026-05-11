using R3;
using SolarPhobia.Application.Services.Interfaces;
using SolarPhobia.Domain.ValueObjects;
using VContainer;

namespace SolarPhobia.Application.Services.Phase
{
    public class DayActionController : IDayActionController
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
        [Inject]
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
    }
}
