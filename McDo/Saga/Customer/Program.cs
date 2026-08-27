using System.Reflection;
using Microsoft.Extensions.Configuration;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using Restaurant.Messages;

var config = new ConfigurationBuilder()
	.AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true)
	.AddEnvironmentVariables()
	.Build();

var rabbitMqConnectionString = config.GetConnectionString("RabbitMq")
	?? throw new InvalidOperationException(
		"Missing connection string 'RabbitMq'. Set it with: " +
		"dotnet user-secrets set \"ConnectionStrings:RabbitMq\" \"rabbitmq://guest:guest@localhost/macdo\"");

using var activator = new BuiltinHandlerActivator();

var bus = Configure.With(activator)
	.Transport(t => t.UseRabbitMqAsOneWayClient(rabbitMqConnectionString))
	.Routing(r => r.TypeBased().Map<Restaurant.Messages.PlaceOrderCommand>("orders-saga"))
	.Start();

string command = string.Empty;
while (command != "quit")
{
	Console.WriteLine("What do you want to order ? (or 'quit' to exit):");
	command = Console.ReadLine();

	if (command != "quit")
	{
		var order = new Restaurant.Messages.PlaceOrderCommand
		{
			OrderId = Guid.NewGuid().ToString(),
			Items = new List<OrderItem>()
			{
				new OrderItem() { Quantity = 6, Name = "Burger"},
				new OrderItem() { Quantity = 2, Name = "Frites"},
				new OrderItem() { Quantity = 2, Name = "Coca"}
			}
		};
		await bus.Send(order);
		Console.WriteLine($"Waiting for my order {command} ");
	}

}

Console.WriteLine("Thank you for visiting Mac Donald today!");
Console.ReadLine();
