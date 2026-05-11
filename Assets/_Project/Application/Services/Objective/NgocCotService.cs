using SolarPhobia.Domain.Services;

namespace SolarPhobia.Application.Services.Objective
{
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
