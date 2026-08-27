using System.Reflection;
using Microsoft.Extensions.Configuration;
using Rebus.Activation;
using Rebus.Config;

// https://github.com/rebus-org/Rebus/wiki/Pub-sub-messaging
// RabbitMQ support native pub/sub.

var config = new ConfigurationBuilder()
	.AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true)
	.AddEnvironmentVariables()
	.Build();

var rabbitMqConnectionString = config.GetConnectionString("RabbitMq")
	?? throw new InvalidOperationException(
		"Missing connection string 'RabbitMq'. Set it with: " +
		"dotnet user-secrets set \"ConnectionStrings:RabbitMq\" \"rabbitmq://guest:guest@localhost/croissant\"");

using var activator = new BuiltinHandlerActivator();

var bus = Configure.With(activator)
	.Transport(t => t.UseRabbitMqAsOneWayClient(rabbitMqConnectionString))
	.Start();

string command = string.Empty;
while (command != "quit")
{
	Console.WriteLine("What do you want to announce to your team? (or 'quit' to exit):");
	command = Console.ReadLine();

	if (command != "quit")
	{
		var announcementMessage= new Manager.Messages.AnnouncementMessage { Content = command };
		await bus.Publish(announcementMessage);
		Console.WriteLine($"Sent announcement message: {announcementMessage.Content}");
	}

}

Console.WriteLine("That's all folks!");
Console.ReadLine();
