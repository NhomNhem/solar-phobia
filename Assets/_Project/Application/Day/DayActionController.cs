// Assets/_Project/Application/Services/DayActionController.cs
using R3;
using SolarPhobia.Domain.ValueObjects;
using VContainer;

namespace SolarPhobia.Application.Day
{
    /// <summary>
    /// Manages Day phase special actions: Swap (Space) and Shove (F).
    /// Implements Master GDD V5.0 Section 2.1.
    ///
    /// Both actions are phase-gated to DayUI mode only.
    /// Events fire once per button press â€” not held.
    /// </summary>
    public class DayActionController : IDayActionController
    {
        // â”€â”€ R3 Reactive State â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        private readonly Subject<bool> _onSwap  = new();
        private readonly Subject<bool> _onShove = new();

        // â”€â”€ Public Interface â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        /// <inheritdoc/>
        public Observable<bool> OnSwap  => _onSwap;

        /// <inheritdoc/>
        public Observable<bool> OnShove => _onShove;

        // â”€â”€ Constructor â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        [Inject]
        public DayActionController() { }

        // â”€â”€ IDayActionController â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
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

