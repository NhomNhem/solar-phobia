// Assets/_Project/Domain/Events/PlayerStateChangedEvent.cs
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Domain.Events
{
    /// <summary>
    /// Event published whenever the player FSM transitions to a new state.
    /// Other systems subscribe to react to state changes.
    /// TR-player-009.
    /// </summary>
    public readonly struct PlayerStateChangedEvent
    {
        public readonly EPlayerState PreviousState;
        public readonly EPlayerState NewState;

        public PlayerStateChangedEvent(EPlayerState previousState, EPlayerState newState)
        {
            PreviousState = previousState;
            NewState = newState;
        }
    }
}
