using R3;
using UnityEngine;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Services
{
    /// <summary>
    /// Service handling Night Phase movement and skills.
    /// </summary>
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