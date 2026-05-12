using System;
using System.Collections.Generic;
using System.Linq;
using SolarPhobia.Application.Messages;
using SolarPhobia.Domain.ValueObjects;

namespace SolarPhobia.Application.Day
{
    public class DaySelectionValidator : IDaySelectionValidator
    {
        private const int RequiredSaved = 2;
        private const int TotalSouls = 3;

        /// <summary>
        /// Validates that exactly 2 souls are Saved and 1 is Abandoned.
        /// </summary>
        public SelectionValidationResult Validate(IReadOnlyList<SoulSelectionState> selections)
        {
            if (selections == null)
                throw new ArgumentNullException(nameof(selections));

            int saved = selections.Count(s => s.State == DaySelectionState.Saved);
            int abandoned = selections.Count(s => s.State == DaySelectionState.Abandoned);

            bool isValid = saved == RequiredSaved && abandoned == TotalSouls - RequiredSaved;

            return new SelectionValidationResult
            {
                IsValid = isValid,
                SavedCount = saved,
                AbandonedCount = abandoned,
                TotalSouls = TotalSouls,
                ErrorMessage = isValid
                    ? null
                    : $"Need exactly {RequiredSaved} Saved, {TotalSouls - RequiredSaved} Abandoned"
            };
        }

        /// <summary>
        /// Auto-completes the selection using the given priority order.
        /// Marks the first N slots as Saved, last as Abandoned.
        /// </summary>
        public IReadOnlyList<SoulSelectionState> AutoComplete(
            IReadOnlyList<SoulSelectionState> currentSelections,
            IReadOnlyList<string> priorityOrder)
        {
            if (currentSelections == null)
                throw new ArgumentNullException(nameof(currentSelections));
            if (priorityOrder == null)
                throw new ArgumentNullException(nameof(priorityOrder));

            var result = new List<SoulSelectionState>();

            for (int i = 0; i < priorityOrder.Count; i++)
            {
                var soulId = priorityOrder[i];

                var existing = currentSelections.FirstOrDefault(s => s.SoulId == soulId);

                DaySelectionState state;
                if (i < RequiredSaved)
                {
                    state = DaySelectionState.Saved;
                }
                else
                {
                    state = DaySelectionState.Abandoned;
                }

                result.Add(new SoulSelectionState(soulId, state));
            }

            return result;
        }
    }
}

