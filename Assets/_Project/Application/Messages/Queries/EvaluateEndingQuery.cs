using SolarPhobia.Application.Messages;

namespace SolarPhobia.Application.Messages.Queries
{
    public class EvaluateEndingQuery
    {
        public GameSessionState State { get; }

        public EvaluateEndingQuery(GameSessionState state)
        {
            State = state;
        }
    }
}
