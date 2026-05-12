using System.Collections.Generic;
using SolarPhobia.Application.Messages;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Day
{
    public interface IDaySelectionValidator
    {
        SelectionValidationResult Validate(IReadOnlyList<SoulSelectionState> selections);

        IReadOnlyList<SoulSelectionState> AutoComplete(
            IReadOnlyList<SoulSelectionState> currentSelections,
            IReadOnlyList<string> priorityOrder);
    }
}

