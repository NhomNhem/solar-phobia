using SolarPhobia.Application.Services.Interfaces;
using SolarPhobia.Domain.ValueObjects;
using UnityEngine;
using VContainer;

namespace SolarPhobia.Application.Services.Input
{
    public class CursorController : ICursorController
    {
        // ── Constructor ────────────────────────────────────────────
        [Inject]
        public CursorController() { }

        // ── ICursorController ──────────────────────────────────────
        /// <inheritdoc/>
        public bool GetCursorVisible(PhaseState phase)
        {
            return phase != PhaseState.NightSurvival;
        }

        /// <inheritdoc/>
        public CursorLockMode GetCursorLockMode(PhaseState phase)
        {
            return phase == PhaseState.NightSurvival
            ? CursorLockMode.Locked
            : CursorLockMode.None;
        }
    }
}
