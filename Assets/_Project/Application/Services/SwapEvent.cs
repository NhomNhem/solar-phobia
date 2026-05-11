using System;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Services
{
    /// <summary>Event emitted when player initiates a swap with a soul.</summary>
    public readonly struct SwapEvent
    {
        public readonly string PlayerId;
        public readonly string SoulId;
        public readonly float AnimationDuration; // 0.5s per GDD

        public SwapEvent(string playerId, string soulId)
        {
            PlayerId = playerId;
            SoulId = soulId;
            AnimationDuration = 0.5f;
        }
    }
}
