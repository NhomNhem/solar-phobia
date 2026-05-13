
namespace SolarPhobia.Domain
{
    public static class EndingEvaluator
    {
        public static EndingType Evaluate(KindnessScore kindnessScore)
        {
            var kindness = kindnessScore.Value;
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

