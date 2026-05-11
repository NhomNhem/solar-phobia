using SolarPhobia.Application.Messages;
using SolarPhobia.Domain;

namespace SolarPhobia.Application.UseCases
{
    public class SubmitDialogueChoiceUseCase
    {
        public void Execute(SubmitDialogueChoiceCommand command)
        {
            if (command == null || command.State == null || command.Choice == null)
            {
                return;
            }

            command.State.KindnessScore = command.State.KindnessScore.Add(command.Choice.KindnessValue);
            command.State.CurrentDialogueId = command.Choice.Target;
        }
    }
}

