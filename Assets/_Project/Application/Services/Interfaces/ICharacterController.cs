using UnityEngine;

namespace SolarPhobia.Application.Services
{
    /// <summary>
    /// Interface for character movement operations.
    /// Allows faking in tests without Unity dependencies.
    /// </summary>
    public interface ICharacterController
    {
        /// <summary>Move the character by the given vector.</summary>
        void Move(Vector3 motion);

        /// <summary>Whether the character is grounded.</summary>
        bool IsGrounded { get; }
    }
}

