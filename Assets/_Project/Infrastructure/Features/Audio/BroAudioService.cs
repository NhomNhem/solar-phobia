using System;
using NhemDangFugBixs.NhemLogging;
using SolarPhobia.Application.Features.Audio;
using VContainer;

namespace SolarPhobia.Infrastructure.Features.Audio
{
    public class BroAudioService : IAudioCueService
    {
        private readonly INhemLogger _logger;

        public BroAudioService(INhemLogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void PlaySound(string soundName)
        {
            _logger.Log($"[BroAudio] Playing sound: {soundName}");
        }

        public void StopAll()
        {
            _logger.Log("[BroAudio] Stopping all audio");
        }

        public void PlayStrikeSFX()
        {
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
