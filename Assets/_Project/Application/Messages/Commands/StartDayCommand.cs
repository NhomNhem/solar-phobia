using SolarPhobia.Application.Messages;

namespace SolarPhobia.Application.Messages.Commands
{
    public class StartDayCommand
    {
        public GameSessionState State { get; }

        public StartDayCommand(GameSessionState state)
        {
            State = state;
        }
    }
}
