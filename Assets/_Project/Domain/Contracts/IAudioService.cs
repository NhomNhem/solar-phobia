// Assets/_Project/Domain/Services/IAudioService.cs
namespace SolarPhobia.Domain
{
    /// <summary>
    /// Interface for audio service.
    /// </summary>
    public interface IAudioService
    {
        /// <summary>
        /// Play the strike sound effect.
        /// </summary>
        void PlayStrikeSFX();

        /// <summary>
        /// Play a sound by name.
        /// </summary>
        void PlaySound(string soundName);
    }
}
