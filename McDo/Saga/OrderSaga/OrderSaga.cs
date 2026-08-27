using Rebus.Bus;
using Rebus.Handlers;
using Rebus.Sagas;
using Restaurant.Messages;

namespace OrderSaga
{
	internal class OrderSaga :
		Saga<OrderSagaData>,
		IAmInitiatedBy<PlaceOrderCommand>,
		IHandleMessages<MealReadyEvent>
	{
		private IBus _bus;

		public OrderSaga(IBus bus)
		{
			_bus = bus;
		}

		protected override void CorrelateMessages(ICorrelationConfig<OrderSagaData> config)
		{
			config.Correlate<PlaceOrderCommand>(m => m.OrderId, d => d.OrderId);
			config.Correlate<MealReadyEvent>(m => m.OrderId, d => d.OrderId);
		}

		public async Task Handle(Restaurant.Messages.PlaceOrderCommand message)
		{
			Console.WriteLine($"Order {message.OrderId}. Let's prepare it...");

			Data.Meals = message
				.Items
				.ToDictionary(item => item.Name, item => item.Quantity);
			foreach (var command in message
				.Items
				.SelectMany(item => Enumerable
									.Range(0, item.Quantity)
									.Select(_ => new PrepareMealCommand
									{
										OrderId = message.OrderId,
										MealName = item.Name
									})))
			{
				await _bus.Send(command);
			}
		}

		public Task Handle(MealReadyEvent message)
		{
			// Set the meal as ready
			Data.Meals[message.MealName]--;
			// Check if order is complete
			if (Data.IsOrderComplete)
			{
				Console.WriteLine($"Order {message.OrderId} is complete!");
				return Task.CompletedTask;
			}
			// Otherwise, continue waiting...
			Console.WriteLine($"Order still has {Data.MealsToComplete} meals to prepare...");
			return Task.CompletedTask;
		}
	}

	internal class OrderSagaData : SagaData
	{
		public string OrderId { get; set; }
		// State is a dico tracking the meals readyness
		public Dictionary<string, int> Meals { get; set; } = new Dictionary<string, int>();

		// Order is complete when all meals are ready
		public bool IsOrderComplete => Meals.Values.All(count => count == 0);
		public int MealsToComplete => Meals.Count(m => m.Value > 0);
	}
}
