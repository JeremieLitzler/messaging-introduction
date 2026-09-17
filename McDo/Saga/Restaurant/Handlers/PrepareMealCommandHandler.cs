using Rebus.Bus;
using Rebus.Handlers;
using Restaurant.Messages;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Text;

namespace Restaurant.Handlers
{
	internal class PrepareMealCommandHandler : IHandleMessages<PrepareMealCommand>
	{
		private readonly IBus _bus;

		public PrepareMealCommandHandler(IBus bus)
		{
			_bus = bus;
		}

		public async Task Handle(PrepareMealCommand message)
		{
			// Set a random wait period to prepare meal
			Random random = new Random();

			Console.WriteLine($"Type enter to server {message.MealName}");
			var _ = Console.ReadLine();

			await _bus.Publish(new MealReadyEvent
			{
				OrderId = message.OrderId,
				MealName = message.MealName,
			});
		}
	}
}
