// Assets/_Project/Application/Services/Interfaces/IPlayerInputHandler.cs
using R3;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Services
{
    /// <summary>
    /// Manages player input mode routing based on game phase.
    /// Implements TR-player-001, TR-player-008: Phase-gated input —
    /// DayService = UI only, NightSurvival = movement + actions, all others = Disabled.
    /// Phase mode switches synchronously on phase change — no frame delay.
    /// </summary>
    public interface IPlayerInputHandler
    {
        /// <summary>
        /// Current input processing mode (reactive — updates synchronously on phase change).
        /// </summary>
        ReadOnlyReactiveProperty<PlayerInputMode> CurrentMode { get; }

        /// <summary>
        /// Whether movement input is currently being processed.
        /// True only when <see cref="CurrentMode"/> is <see cref="PlayerInputMode.NightMovement"/>.
        /// </summary>
        bool IsMovementEnabled { get; }

        /// <summary>
        /// Whether UI click input is currently being processed.
        /// True only when <see cref="CurrentMode"/> is <see cref="PlayerInputMode.DayUI"/>.
        /// </summary>
        bool IsUIEnabled { get; }
    }
}
