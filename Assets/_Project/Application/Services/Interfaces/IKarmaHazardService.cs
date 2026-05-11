using System;
using R3;
using UnityEngine;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Services
{
    /// <summary>
    /// Interface for karma hazard spawning based on sacrificed ghost type.
    /// </summary>
    public interface IKarmaHazardService
    {
        /// <summary>
        /// Spawn karma hazard based on sacrificed ghost type at the specified position.
        /// </summary>
        void SpawnHazardForGhost(string ghostType, Vector3 position);

        /// <summary>
        /// Clear all active karma hazards.
        /// </summary>
        void ClearHazards();

        /// <summary>
        /// Observable that triggers when a hazard is spawned.
        /// </summary>
        Observable<KarmaHazardData> OnHazardSpawned { get; }
    }
}
