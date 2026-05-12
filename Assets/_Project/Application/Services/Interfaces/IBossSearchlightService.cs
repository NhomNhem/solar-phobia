using R3;
using UnityEngine;
using SolarPhobia.Domain.ValueObjects;

using SolarPhobia.Application.Resources;

using SolarPhobia.Application.Strike;

using SolarPhobia.Application.Consequences.WaterTrap;

using SolarPhobia.Application.Rituals;

using SolarPhobia.Application.Shrines;

using SolarPhobia.Application.Day;

using SolarPhobia.Application.Flow;

using SolarPhobia.Application.Player.State;

using SolarPhobia.Application.Player.Input;

using SolarPhobia.Application.Player.Interactions;

using SolarPhobia.Application.Player.Cursor;

using SolarPhobia.Application.Player.Events;

using SolarPhobia.Application.Combat;

using SolarPhobia.Application.Phase.Reset;

namespace SolarPhobia.Application.Services
{
    /// <summary>
    /// Interface for boss searchlight sweep and strike logic.
    /// </summary>
    public interface IBossSearchlightService
    {
        void ActivateSearchlight();
        void DeactivateSearchlight();
        bool IsPlayerExposed(Vector3 playerPosition, bool isInCover);
        void ExecuteStrike();
        Observable<bool> OnTelegraphActive { get; }
        Observable<float> OnWardPenalty { get; }
    }
}
