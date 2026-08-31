using Rebus.Bus;
using Rebus.Handlers;
using Restaurant.Messages;

namespace Customer.Handlers
{
	internal class OrderReadyCommandHandler : IHandleMessages<OrderReadyCommand>
	{
		private readonly IBus _bus;

		public OrderReadyCommandHandler(IBus bus)
		{
			_bus = bus;
		}

		public Task Handle(OrderReadyCommand message)
		{
			Console.WriteLine(message.Summary);
			Console.WriteLine("leave");
			return Task.CompletedTask;
		}
	}
}
