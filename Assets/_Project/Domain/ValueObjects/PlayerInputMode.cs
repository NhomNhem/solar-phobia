// Assets/_Project/Domain/ValueObjects/PlayerInputMode.cs
namespace SolarPhobia.Domain.ValueObjects
{
    /// <summary>
    /// Defines the active input processing mode for the player controller.
    /// Determined by the current game phase — switches synchronously on phase change.
    /// </summary>
    public enum PlayerInputMode
    {
        /// <summary>
        /// Day phase — mouse clicks only (NPC selection, resource assignment, confirm).
        /// WASD and all movement inputs are silently consumed.
        /// </summary>
        DayUI = 0,

        /// <summary>
        /// Night phase — WASD movement, Shift sprint, E interact, mouse-look active.
        /// UI selection clicks on NPCs are ignored.
        /// </summary>
        NightMovement = 1,

        /// <summary>
        /// ChoiceLock / Resolve / Reset phases — all input disabled
        /// except skip/continue prompts.
        /// </summary>
        Disabled = 2
    }
}
