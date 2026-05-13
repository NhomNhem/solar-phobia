using SolarPhobia.Domain.Events;

namespace SolarPhobia.Application.Features.Phase
{
    public interface IDayNightCameraController
    {
        float DayCameraDistance { get; set; }
        float NightCameraDistance { get; set; }
        float TransitionDurationDayToNight { get; set; }
        float TransitionDurationNightToDay { get; set; }
        float VignetteMaxAlpha { get; set; }
        float CameraFollowSmooth { get; set; }
        bool IsNight { get; }
        bool IsInChoiceLock { get; }
        void ApplyMouseLook(float mouseDeltaY);
        void HandleNightFailed(NightFailedEvent evt);
    }
}
