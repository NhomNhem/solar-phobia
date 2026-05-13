using UnityEngine;

namespace SolarPhobia.Application.Features.Player.Movement
{
    public interface ICharacterController
    {
        void Move(Vector3 motion);
        bool IsGrounded { get; }
    }
}
