using System.Reflection;
using Microsoft.Extensions.Configuration;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Persistence.InMem;
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

var sagaDbConnectionString = config.GetConnectionString("OrderSagaDb")
	?? throw new InvalidOperationException(
		"Missing connection string 'OrderSagaDb'. Set it with: " +
		"dotnet user-secrets set \"ConnectionStrings:OrderSagaDb\" \"<postgres connection string>\"");

using var activator = new BuiltinHandlerActivator();
activator.Register((bus, _) => new OrderSaga.OrderSaga(bus));

var bus = Configure.With(activator)
	.Transport(t => t.UseRabbitMq(rabbitMqConnectionString, "orders-saga"))
	.Routing(r => r.TypeBased().Map<PrepareMealCommand>("restaurant"))
	.Sagas(s => s.StoreInPostgres(
		sagaDbConnectionString,
		"order-saga",
		"order-saga-index")
	)
	//.Sagas(s => s.StoreInMemory())
	.Start();

await bus.Subscribe<MealReadyEvent>();

Console.WriteLine("Done? Press any key.");
Console.ReadLine();
