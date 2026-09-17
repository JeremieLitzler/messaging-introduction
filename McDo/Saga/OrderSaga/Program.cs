using System.Reflection;
using Microsoft.Extensions.Configuration;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Persistence.FileSystem;
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
		"dotnet user-secrets set \"ConnectionStrings:RabbitMq\" \"rabbitmq://<user>:<password>@<host>/<virtual_host>\"");

var sagaDbConnectionString = config.GetConnectionString("OrderSagaDb")
	?? throw new InvalidOperationException(
		"Missing connection string 'OrderSagaDb'. Set it with: " +
		"dotnet user-secrets set \"ConnectionStrings:OrderSagaDb\" \"<postgres connection string>\"");

using var activator = new BuiltinHandlerActivator();
activator.Register((bus, _) => new OrderSaga.Process(bus));

var bus = Configure.With(activator)
	.Transport(t => t.UseRabbitMq(rabbitMqConnectionString, "orders-saga"))
	.Routing(r => r.TypeBased()
		.Map<PrepareMealCommand>("restaurant")
		.Map<DistributeOrderCommand>("restaurant")
		.Map<CancelOrderCommand>("orders-saga"))
	.Timeouts( t => t.UseFileSystem("D:\\Git\\GitHub\\messaging-introduction\\McDo\\Saga\\OrderSaga\\.timeouts-db"))
	//.Sagas(s => s.StoreInSqlServer(
	//	sagaDbConnectionString,
	//	"OrderSaga",
	//	"OrderSagaIndex")
	//)
	.Sagas(s => s.UseFilesystem("D:\\Git\\GitHub\\messaging-introduction\\McDo\\Saga\\OrderSaga\\.saga-db"))
	//.Sagas(s => s.StoreInMemory())
	.Start();

await bus.Subscribe<MealReadyEvent>();

Console.WriteLine("Done? Press any key.");
Console.ReadLine();
