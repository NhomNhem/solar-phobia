// Assets/_Project/Application/Services/NgocCotService.cs
using SolarPhobia.Domain.Services;

namespace SolarPhobia.Application.Services
{
    /// <summary>
    /// Tracks Ngọc Cốt relic pickups and provides the ward drain multiplier.
    /// Maximum 3 pickups per night phase. Bone count resets at start of each night.
    /// Formula: drainMultiplier = 1 + (boneCount × 0.25)
    /// </summary>
    public class NgocCotService : INgocCotService
    {
        private const int MaxBonesPerNight = 3;
        private const float BoneMultiplierIncrement = 0.25f;

        private int _boneCount;

        public int BoneCount => _boneCount;

        public float BoneMultiplier => 1f + (_boneCount * BoneMultiplierIncrement);

        public bool TryCollectRelic()
        {
            if (_boneCount >= MaxBonesPerNight)
            {
                return false;
            }

            _boneCount++;
            return true;
        }

        public void ResetForNight()
        {
            _boneCount = 0;
        }
    }
}
