namespace SolarPhobia.Application.Features.Audio
{
    /// <summary>
    /// Application port for playing gameplay audio cues.
    /// </summary>
    public interface IAudioCueService
    {
        void PlaySwapSound();

        void PlayShoveImpact();

        void PlaySoulBurn();

        void PlaySprintSound();

        void PlayDashSound();

        void PlaySwingSound();
    }
}
