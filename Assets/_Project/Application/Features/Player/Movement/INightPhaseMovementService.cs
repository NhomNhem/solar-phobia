using R3;
using UnityEngine;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Features.Player.Movement
{
    public interface INightPhaseMovementService
    {
        bool TryMove(Vector3 inputDirection, PhaseState currentPhase);
        bool TrySprint(PhaseState currentPhase);
        bool TryDash(Vector3 direction, PhaseState currentPhase);
        bool TrySwing(Vector3 targetPoint, PhaseState currentPhase);
        bool TryGlide(bool isActive, PhaseState currentPhase);
        float GetCurrentWard();
        Observable<float> OnWardChanged { get; }
    }
}
