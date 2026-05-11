using SolarPhobia.Application.Messages;
using SolarPhobia.Domain;

namespace SolarPhobia.Application.UseCases
{
    public class EvaluateEndingUseCase
    {
        public EndingType Execute(EvaluateEndingQuery query)
        {
            if (query == null || query.State == null)
            {
                return EndingType.Neutral;
            }

            var kindness = query.State.KindnessScore.Value;
            if (kindness >= 6)
            {
                return EndingType.Good;
            }

            if (kindness <= 1)
            {
                return EndingType.Bad;
            }

            return EndingType.Neutral;
        }
    }
}

