using SolarPhobia.Application.Messages;

namespace SolarPhobia.Application.Messages.Commands
{
    public class TransitionToNightCommand
    {
        public GameSessionState State { get; }

        public TransitionToNightCommand(GameSessionState state)
        {
            State = state;
        }
    }
}
