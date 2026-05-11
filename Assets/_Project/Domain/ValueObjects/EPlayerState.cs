// Assets/_Project/Domain/ValueObjects/EPlayerState.cs
namespace SolarPhobia.Domain.ValueObjects
{
    /// <summary>
    /// All possible states for the player Finite State Machine during night survival.
    /// Covers movement, skills, and interaction states.
    /// TR-player-009.
    /// </summary>
    public enum EPlayerState
    {
        Idle,
        Moving,
        Sprinting,
        Jumping,
        Falling,
        Dashing,
        Swinging,
        Gliding,
        Interacting,
        Exhausted,
        Crouching
    }
}
