using SolarPhobia.Application.Services.Interfaces;
using SolarPhobia.Domain.ValueObjects;
using System.Collections.Generic;
using System.Linq;
using System;

namespace SolarPhobia.Application.Services.Objective
{
    public class RitualAssignmentService : IRitualAssignmentService, IDisposable
    {
        private readonly Dictionary<string, RitualType> _assignments = new();
        private readonly IResourceEffectApplier _effectApplier;
        private readonly bool _hasEffectApplier;

        private static readonly IReadOnlyDictionary<string, RitualType> PreferredRituals =
        new Dictionary<string, RitualType>
        {
            ["linh"] = RitualType.Tea,
            ["van"] = RitualType.Incense,
            ["minh"] = RitualType.Offering
        };

        private static readonly IReadOnlyList<string> ValidSoulIds = new[] { "linh", "van", "minh" };

        public IReadOnlyDictionary<string, RitualType> Assignments => _assignments;

        public RitualAssignmentService()
        {
            _effectApplier = null;
            _hasEffectApplier = false;
        }

        public RitualAssignmentService(IResourceEffectApplier effectApplier)
        {
            _effectApplier = effectApplier ?? throw new ArgumentNullException(nameof(effectApplier));
            _hasEffectApplier = true;
        }

        public bool TryAssignRitual(string soulId, RitualType ritual)
        {
            if (!IsValidSoulId(soulId))
            {
                return false;
            }

            _assignments[soulId] = ritual;

            if (_hasEffectApplier && _effectApplier != null)
            {
                float scalingRatio = 1f;
                bool preferred = IsPreferredRitual(soulId, ritual);

                if (_effectApplier.TrySpendHuongHoa(15))
                {
                    scalingRatio = Math.Clamp(
                    (float)_effectApplier.GetCurrentHuongHoa() / 15f, 0.5f, 1f);
                }

                switch (ritual)
                {
                    case RitualType.Tea:
                    _effectApplier.ApplyTeaEffect(soulId, scalingRatio, preferred);
                    break;
                    case RitualType.Incense:
                    _effectApplier.ApplyIncenseEffect(soulId, scalingRatio, preferred);
                    break;
                    case RitualType.Offering:
                    _effectApplier.ApplyOfferingEffect(soulId, scalingRatio, preferred);
                    break;
                }
            }
            {
                UnityEngine.Debug.LogWarning(
                "[RitualAssignmentService] IResourceEffectApplier not registered — " +
                $"ritual {ritual} assigned to {soulId} without gameplay effect");
            }

            return true;
        }

        public bool TryRemoveRitual(string soulId)
        {
            if (!IsValidSoulId(soulId))
            {
                return false;
            }

            return _assignments.Remove(soulId);
        }

        public void Clear()
        {
            _assignments.Clear();
        }

        public bool IsPreferredRitual(string soulId, RitualType ritual)
        {
            return PreferredRituals.TryGetValue(soulId, out var preferred)
            && preferred == ritual;
        }

        public void Dispose()
        {
            _assignments.Clear();
        }

        private static bool IsValidSoulId(string soulId)
        {
            return !string.IsNullOrEmpty(soulId) && ValidSoulIds.Contains(soulId);
        }
    }
}
