// Assets/_Project/Application/Services/Interfaces/ICoverDetector2D.cs
using R3;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Services
{
    /// <summary>
    /// Detects whether the player is hiding behind a Mộ Gió (tombstone) cover object.
    /// Implements Master GDD V5.0 Section 3.2 — 2D trigger overlap, not 3D bounds containment.
    ///
    /// The MonoBehaviour layer calls NotifyOverlapEnter/Exit from OnTriggerEnter2D/OnTriggerExit2D.
    /// This service owns the tag → cover state mapping and the phase gate.
    ///
    /// Cover tags:
    ///   "MoThuong"       → IsInCover = true (safe cover)
    ///   "FalseSafeMound" → IsInCover = true + fires OnFalseSafeMoundEntered warning
    ///   anything else    → ignored
    ///
    /// Cover check only active during NightMovement mode.
    /// </summary>
    public interface ICoverDetector2D
    {
        /// <summary>
        /// Whether the player is currently inside a cover trigger.
        /// Reactive — fires only on state change.
        /// </summary>
        ReadOnlyReactiveProperty<bool> IsInCover { get; }

        /// <summary>
        /// Emits when player enters a FalseSafeMound trigger.
        /// Subscribers should play the warning tell.
        /// </summary>
        Observable<bool> OnFalseSafeMoundEntered { get; }

        /// <summary>
        /// Called by MonoBehaviour from OnTriggerEnter2D.
        /// Ignored when mode != NightMovement.
        /// </summary>
        void NotifyOverlapEnter(string coverTag, PlayerInputMode mode);

        /// <summary>
        /// Called by MonoBehaviour from OnTriggerExit2D.
        /// Ignored when mode != NightMovement.
        /// </summary>
        void NotifyOverlapExit(string coverTag, PlayerInputMode mode);
    }
}
