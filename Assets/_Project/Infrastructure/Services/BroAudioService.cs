using NhemDangFugBixs.NhemLogging;
using SolarPhobia.Application.Repositories;
using SolarPhobia.Application.Services;

using VContainer;

namespace SolarPhobia.Infrastructure.Services
{
    public class BroAudioService : IAudioService
    {
        [Inject] internal INhemLogger _logger = new NhemUnityLogger();

        // NOTE: In a real implementation, these strings should correspond to
        // SoundIDs or names configured in the BroAudio Library Manager.

        public void PlaySound(string soundName)
        {
            // BroAudio doesn't primarily use strings, but we can search or use a default if needed.
            _logger.Log($"[BroAudio] Playing sound: {soundName}");
        }

        public void StopAll()
        {
            _logger.Log("[BroAudio] Stopping all audio");
        }

        public void PlayStrikeSFX()
        {
            // Implementation note: Replace with actual SoundID from LibraryManager
            // BroAudio.Play(AudioID.StrikeSFX);
            _logger.Log("[BroAudio] Playing Strike SFX");
        }

        public void PlaySwapSound()
        {
            _logger.Log("[BroAudio] Playing Swap Sound");
        }

        public void PlayShoveImpact()
        {
            _logger.Log("[BroAudio] Playing Shove Impact");
        }

        public void PlaySoulBurn()
        {
            _logger.Log("[BroAudio] Playing Soul Burn");
        }

        public void PlaySprintSound()
        {
            _logger.Log("[BroAudio] Playing Sprint Sound");
        }

        public void PlayDashSound()
        {
            _logger.Log("[BroAudio] Playing Dash Sound");
        }

        public void PlaySwingSound()
        {
            _logger.Log("[BroAudio] Playing Swing Sound");
        }
    }
}
