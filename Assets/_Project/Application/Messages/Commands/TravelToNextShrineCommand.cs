using SolarPhobia.Application.Messages;

namespace SolarPhobia.Application.Messages.Commands
{
    public class TravelToNextShrineCommand
    {
        public GameSessionState State { get; }
        public string NextShrineId { get; }

        public TravelToNextShrineCommand(GameSessionState state, string nextShrineId)
        {
            State = state;
            NextShrineId = nextShrineId;
        }
    }
}
