using System.Reflection;
using Microsoft.Extensions.Configuration;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Persistence.FileSystem;

// https://github.com/rebus-org/Rebus/wiki/Pub-sub-messaging
// RabbitMQ support native pub/sub.

var config = new ConfigurationBuilder()
	.AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true)
	.AddEnvironmentVariables()
	.Build();

var rabbitMqConnectionString = config.GetConnectionString("RabbitMq")
	?? throw new InvalidOperationException(
		"Missing connection string 'RabbitMq'. Set it with: " +
		"dotnet user-secrets set \"ConnectionStrings:RabbitMq\" \"rabbitmq://<user>:<password>@<host>/<virtual_host>\"");

using var activator = new BuiltinHandlerActivator();

activator.Register(() => new Employee.Handlers.AnnouncementMessageHandler());

var bus = Configure.With(activator)
	.Transport(t => t.UseRabbitMq(rabbitMqConnectionString, "employee"))
	.Start();


await bus.Subscribe<Manager.Messages.AnnouncementMessage>();

Console.WriteLine("That's all folks!");
Console.ReadLine();
