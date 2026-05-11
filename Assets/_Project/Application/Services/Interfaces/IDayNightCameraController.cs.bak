// Assets/_Project/Application/Services/Interfaces/IDayNightCameraController.cs
using SolarPhobia.Domain.Events;

namespace SolarPhobia.Application.Services
{
    /// <summary>
    /// Controls camera behavior based on game phase transitions.
    /// Manages day (fixed 2.5D top-down), night (side-scroll follow),
    /// transitions (zoom, color grade, vignette), and resolve cinematics.
    /// </summary>
    public interface IDayNightCameraController
    {
        // ── Tuning Knobs ────────────────────────────────────────────

        /// <summary>Camera distance during day phases. Range: 3-8.</summary>
        float DayCameraDistance { get; set; }

        /// <summary>Camera distance during night phases. Range: 8-15.</summary>
        float NightCameraDistance { get; set; }

        /// <summary>Duration of day-to-night transition in seconds. Range: 0.3-1.0.</summary>
        float TransitionDurationDayToNight { get; set; }

        /// <summary>Duration of night-to-day transition in seconds. Range: 0.2-0.8.</summary>
        float TransitionDurationNightToDay { get; set; }

        /// <summary>Maximum vignette intensity during night. Range: 0.3-0.8.</summary>
        float VignetteMaxAlpha { get; set; }

        /// <summary>Smooth follow lerp factor for player tracking. Range: 0.1-0.5.</summary>
        float CameraFollowSmooth { get; set; }

        // ── State Queries ──────────────────────────────────────────

        /// <summary>True when the camera is in night mode (follow + effects active).</summary>
        bool IsNight { get; }

        /// <summary>True when ChoiceLock phase is active (camera frozen at day position).</summary>
        bool IsInChoiceLock { get; }

        // ── Input (Testable) ───────────────────────────────────────

        /// <summary>
        /// Apply mouse look rotation for night phases.
        /// Called from Tick() with Input.GetAxis("Mouse Y"), or directly from tests.
        /// Clamped to ±30° on the Y-axis.
        /// </summary>
        void ApplyMouseLook(float mouseDeltaY);

        // ── Cinematic Events ───────────────────────────────────────

        /// <summary>
        /// Trigger the lose cinematic: rapid darken + camera shake + fade to black.
        /// Called automatically when <see cref="ISensoryTierService.OnNightFailed"/> fires,
        /// or can be invoked directly by game flow.
        /// </summary>
        void HandleNightFailed(NightFailedEvent evt);
    }
}
