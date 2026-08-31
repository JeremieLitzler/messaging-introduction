using Rebus.Bus;
using Rebus.Handlers;
using Restaurant.Messages;

namespace Restaurant.Handlers
{
	internal class DistributeOrderCommandHandler : IHandleMessages<DistributeOrderCommand>
	{
		private readonly IBus _bus;

		public DistributeOrderCommandHandler(IBus bus)
		{
			_bus = bus;
		}

		public Task Handle(DistributeOrderCommand message)
		{
			Console.WriteLine($"Order {message.OrderId} is ready to distribute!");
			_bus.Send(new OrderReadyCommand(message.OrderId));
			return Task.CompletedTask;
		}
	}
}
