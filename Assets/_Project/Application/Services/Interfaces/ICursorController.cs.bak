// Assets/_Project/Application/Services/Interfaces/ICursorController.cs
using SolarPhobia.Domain.ValueObjects;
using UnityEngine;

namespace SolarPhobia.Application.Services
{
    /// <summary>
    /// Determines the desired cursor state for each game phase.
    /// Implements TR-player-007: Cursor Visibility — Phase-Driven Show/Hide.
    ///
    /// This service owns the phase → cursor state mapping.
    /// The MonoBehaviour layer applies the state to Unity's <c>Cursor</c> API.
    ///
    /// Mapping:
    ///   DayService                             → visible=true,  lockState=None
    ///   NightSurvival                          → visible=false, lockState=Locked
    ///   ChoiceLock / EndingEvaluation / others → visible=true,  lockState=None
    /// </summary>
    public interface ICursorController
    {
        /// <summary>
        /// Returns the desired cursor visibility for the given phase.
        /// </summary>
        bool GetCursorVisible(PhaseState phase);

        /// <summary>
        /// Returns the desired cursor lock state for the given phase.
        /// </summary>
        CursorLockMode GetCursorLockMode(PhaseState phase);
    }
}
