using Microsoft.Extensions.Configuration;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using Restaurant.Messages;
using System.Reflection;

var config = new ConfigurationBuilder()
	.AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true)
	.AddEnvironmentVariables()
	.Build();

var rabbitMqConnectionString = config.GetConnectionString("RabbitMq")
	?? throw new InvalidOperationException(
		"Missing connection string 'RabbitMq'. Set it with: " +
		"dotnet user-secrets set \"ConnectionStrings:RabbitMq\" \"rabbitmq://guest:guest@localhost/macdo\"");

using var activator = new BuiltinHandlerActivator();

activator.Register((bus, _) => new Restaurant.Handlers.PrepareMealCommandHandler(bus));
activator.Register((bus, _) => new Restaurant.Handlers.DistributeOrderCommandHandler(bus));

var bus = Configure.With(activator)
	.Transport(t => t.UseRabbitMq(rabbitMqConnectionString, "restaurant"))
	.Routing(r => r.TypeBased().Map<OrderReadyCommand>("customer"))
	.Start();

Console.WriteLine("Press enter to quit");
Console.ReadLine();
