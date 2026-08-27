using System.Reflection;
using Microsoft.Extensions.Configuration;
using Rebus.Activation;
using Rebus.Config;

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

var bus = Configure.With(activator)
	.Transport(t => t.UseRabbitMq(rabbitMqConnectionString, "restaurant"))
	.Start();

Console.WriteLine("Press enter to quit");
Console.ReadLine();
