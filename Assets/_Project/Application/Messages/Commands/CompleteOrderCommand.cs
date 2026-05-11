using SolarPhobia.Application.Messages;
using SolarPhobia.Domain;

namespace SolarPhobia.Application.Messages.Commands
{
    public class CompleteOrderCommand
    {
        public GameSessionState State { get; }
        public Order Order { get; }

        public CompleteOrderCommand(GameSessionState state, Order order)
        {
            State = state;
            Order = order;
        }
    }
}
