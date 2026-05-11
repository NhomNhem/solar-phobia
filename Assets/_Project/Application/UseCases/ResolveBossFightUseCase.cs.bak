using SolarPhobia.Application.Messages;
using SolarPhobia.Domain;

namespace SolarPhobia.Application.UseCases
{
    public class ResolveBossFightUseCase
    {
        public EndingType Execute(GameSessionState state)
        {
            if (state == null)
            {
                return EndingType.Neutral;
            }

            return state.KindnessScore.Value >= 6 ? EndingType.Good : EndingType.Bad;
        }
    }
}

