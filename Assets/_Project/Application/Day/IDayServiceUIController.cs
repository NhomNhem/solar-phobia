using System.Collections.Generic;
using R3;
using SolarPhobia.Application.Messages;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Day
{
    public interface IDayServiceUIController
    {
        ReadOnlyReactiveProperty<bool> IsUIVisible { get; }
        ReadOnlyReactiveProperty<SelectionConfirmedPayload> LastConfirmedPayload { get; }
        ReadOnlyReactiveProperty<bool> IsConfirmEnabled { get; }
        SelectionValidationResult LastValidation { get; }
        IReadOnlyDictionary<string, RitualType> RitualAssignments { get; }
        bool IsUIVisibleValue { get; }
        SelectionConfirmedPayload LastConfirmedPayloadValue { get; }
        bool IsConfirmEnabledValue { get; }

        void ToggleSoulSelection(string soulId);
        bool AssignRitual(string soulId, RitualType ritual);
        bool RemoveRitual(string soulId);
        bool IsPreferredRitual(string soulId, RitualType ritual);
        bool TryConfirmSelection();
        void Reset();
    }
}

