// Assets/_Project/Application/Services/Interfaces/IDayActionController.cs
using R3;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Services
{
    /// <summary>
    /// Manages Day phase special actions: Swap (Space) and Shove (F).
    /// Implements Master GDD V5.0 Section 2.1 — X-axis soul management.
    ///
    /// Swap (Space): Exchange positions with a nearby soul.
    ///   Fires OnSwap — Soul system subscribes and executes the swap.
    ///
    /// Shove (F): Push a soul away from the player (out of shadow).
    ///   Fires OnShove — Soul system subscribes and applies the push.
    ///
    /// Both actions only active during DayUI mode.
    /// </summary>
    public interface IDayActionController
    {
        /// <summary>Emits when Swap is triggered (Space key in DayUI mode).</summary>
        Observable<bool> OnSwap { get; }

        /// <summary>Emits when Shove is triggered (F key in DayUI mode).</summary>
        Observable<bool> OnShove { get; }

        /// <summary>
        /// Processes Swap and Shove inputs for the current frame.
        /// Silently ignored when mode != DayUI.
        /// </summary>
        void TryActions(bool swapInput, bool shoveInput, PlayerInputMode mode);
    }
}
