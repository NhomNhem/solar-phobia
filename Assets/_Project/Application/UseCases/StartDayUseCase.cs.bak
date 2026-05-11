using SolarPhobia.Domain;
using SolarPhobia.Application.Messages;

namespace SolarPhobia.Application.UseCases
{
    public class StartDayUseCase
    {
        public void Execute(StartDayCommand command)
        {
            if (command == null || command.State == null)
            {
                return;
            }

            command.State.CurrentPhase = GamePhase.DAY_HUB;
        }
    }
}

