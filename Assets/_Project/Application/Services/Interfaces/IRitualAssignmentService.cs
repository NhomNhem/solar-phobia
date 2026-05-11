using System.Collections.Generic;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Services
{
    public interface IRitualAssignmentService
    {
        IReadOnlyDictionary<string, RitualType> Assignments { get; }
        bool TryAssignRitual(string soulId, RitualType ritual);
        bool TryRemoveRitual(string soulId);
        void Clear();
        bool IsPreferredRitual(string soulId, RitualType ritual);
    }
}
