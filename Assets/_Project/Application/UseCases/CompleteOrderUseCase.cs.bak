using SolarPhobia.Application.Messages;

namespace SolarPhobia.Application.UseCases
{
    public class CompleteOrderUseCase
    {
        public void Execute(CompleteOrderCommand command)
        {
            if (command == null || command.State == null || command.Order == null)
            {
                return;
            }

            if (!command.Order.IsComplete)
            {
                return;
            }

            command.State.SpiritEssence += 1;
            command.State.CurrentOrderId = null;
        }
    }
}

