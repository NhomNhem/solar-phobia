// Assets/_Project/Application/Services/Interfaces/ISprintController.cs
using System;
using R3;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Services
{
    /// <summary>
    /// Manages sprint state for the player controller.
    /// Implements TR-player-003: Sprint — Shift key speed multiplier + stamina integration.
    ///
    /// Sprint is only active when:
    ///   - <see cref="PlayerInputMode"/> is <see cref="PlayerInputMode.NightMovement"/>
    ///   - Sprint input is held
    ///   - Stamina has not been depleted
    ///
    /// Fires <see cref="OnSprintChanged"/> only on state transitions (not every frame).
    /// Immediately exits sprint when <see cref="NotifyStaminaDepleted"/> is called.
    /// </summary>
    public interface ISprintController
    {
        /// <summary>Whether sprint is currently active.</summary>
        bool IsSprinting { get; }

        /// <summary>
        /// Emits when sprint state changes: <c>true</c> = sprint started, <c>false</c> = sprint ended.
        /// Fires only on transitions — not every frame.
        /// </summary>
        Observable<bool> OnSprintChanged { get; }

        /// <summary>
        /// Updates sprint state based on current input and mode.
        /// Call once per frame from the player controller's Update loop.
        /// </summary>
        /// <param name="sprintInputHeld">Whether the sprint key (Shift) is currently held.</param>
        /// <param name="mode">Current player input mode.</param>
        void Tick(bool sprintInputHeld, PlayerInputMode mode);

        /// <summary>
        /// Forces sprint to end immediately.
        /// Call when <c>OnStaminaDepleted</c> is received from the Health/Stamina system.
        /// </summary>
        void NotifyStaminaDepleted();

        /// <summary>
        /// Marks stamina as available again (e.g. stamina recovered between nights).
        /// Allows sprint to be re-activated on next Tick.
        /// </summary>
        void NotifyStaminaRestored();
    }
}
