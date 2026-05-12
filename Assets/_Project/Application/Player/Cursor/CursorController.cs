// Assets/_Project/Application/Services/CursorController.cs
using SolarPhobia.Domain.ValueObjects;
using VContainer;

namespace SolarPhobia.Application.Player.Cursor
{
    /// <summary>
    /// Determines the desired cursor state for each game phase.
    /// Implements TR-player-007: Cursor Visibility â€” Phase-Driven Show/Hide.
    ///
    /// Phase â†’ cursor mapping:
    ///   NightSurvival â†’ hidden + Locked   (mouselook active)
    ///   DayService    â†’ visible + None    (UI interaction)
    ///   all others    â†’ visible + None    (ChoiceLock, EndingEvaluation, etc.)
    ///
    /// The MonoBehaviour layer applies the result to Unity's Cursor API:
    ///   Cursor.visible   = GetCursorVisible(phase)
    ///   Cursor.lockState = MapToUnity(GetCursorLockState(phase))
    ///
    /// Edge case (Alt+Tab): Unity auto-unlocks cursor on focus loss.
    /// Re-apply on OnApplicationFocus(true) if phase is NightSurvival.
    /// </summary>
    public class CursorController : ICursorController
    {
        // â”€â”€ Constructor â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        [Inject]
        public CursorController() { }

        // â”€â”€ ICursorController â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        /// <inheritdoc/>
        public bool GetCursorVisible(PhaseState phase)
        {
            return phase != PhaseState.NightSurvival;
        }

        /// <inheritdoc/>
        public CursorLockState GetCursorLockState(PhaseState phase)
        {
            return phase == PhaseState.NightSurvival
                ? CursorLockState.Locked
                : CursorLockState.None;
        }
    }
}

