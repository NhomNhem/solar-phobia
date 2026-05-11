// Assets/_Project/Application/Services/Interfaces/IInteractHandler.cs
using R3;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Services
{
    /// <summary>
    /// Handles E-key contextual interaction dispatch.
    /// Implements TR-player-005: E-Key Contextual Interact — Relic Pickup + Shrine Trigger.
    ///
    /// The MonoBehaviour layer performs the Physics.Raycast and passes the hit tag here.
    /// This service owns the tag → interaction-type mapping and the phase gate.
    ///
    /// Interaction types:
    ///   "CursedMound" tag → fires <see cref="OnInteract"/> with payload <c>"relic"</c>
    ///   "EndShrine"   tag → fires <see cref="OnInteract"/> with payload <c>"shrine"</c>
    ///   Any other tag / null → silently ignored (no event, no log)
    ///
    /// Interaction is only processed when mode is <see cref="PlayerInputMode.NightMovement"/>.
    /// </summary>
    public interface IInteractHandler
    {
        /// <summary>
        /// Emits the interaction type string when a valid interactable is hit.
        /// Payload values: <c>"relic"</c> (CursedMound), <c>"shrine"</c> (EndShrine).
        /// </summary>
        Observable<string> OnInteract { get; }

        /// <summary>
        /// Processes an E-key press with the given raycast hit tag.
        /// Silently ignored when <paramref name="mode"/> is not NightMovement,
        /// or when <paramref name="hitTag"/> does not match a known interactable.
        /// </summary>
        /// <param name="hitTag">
        /// Tag of the collider hit by the forward raycast, or <c>null</c> / empty if nothing was hit.
        /// </param>
        /// <param name="mode">Current player input mode.</param>
        void TryInteract(string hitTag, PlayerInputMode mode);
    }
}
