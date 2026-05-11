using SolarPhobia.Application.Messages;
using SolarPhobia.Domain;

namespace SolarPhobia.Application.UseCases
{
    public class TravelToNextShrineUseCase
    {
        public void Execute(TravelToNextShrineCommand command)
        {
            if (command == null || command.State == null)
            {
                return;
            }

            command.State.CurrentShrineId = command.NextShrineId;
            command.State.CurrentPhase = GamePhase.SHRINE_ARRIVAL;
        }
    }
}

