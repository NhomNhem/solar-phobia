using SolarPhobia.Application.Messages;
using SolarPhobia.Domain;

namespace SolarPhobia.Application.UseCases
{
    public abstract class TransitionToNightUseCase
    {
        public void Execute(TransitionToNightCommand command)
        {
            if (command == null || command.State == null)
            {
                return;
            }

            command.State.CurrentPhase = GamePhase.NIGHT_TRAVEL;
        }
    }
}

