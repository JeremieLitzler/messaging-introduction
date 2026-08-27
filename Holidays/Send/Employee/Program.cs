// we have the container adapter in a variable here, but you should stash it
// in a static field somewhere, and then dispose it when your app shuts down
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Routing.TypeBased;

var config = new ConfigurationBuilder()
	.AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true)
	.AddEnvironmentVariables()
	.Build();

var rabbitMqConnectionString = config.GetConnectionString("RabbitMq")
	?? throw new InvalidOperationException(
		"Missing connection string 'RabbitMq'. Set it with: " +
		"dotnet user-secrets set \"ConnectionStrings:RabbitMq\" \"rabbitmq://guest:guest@localhost/holidays\"");

using var activator = new BuiltinHandlerActivator();

var bus = Configure.With(activator)
	.Transport(t => t.UseRabbitMqAsOneWayClient(rabbitMqConnectionString))
	.Routing(r => r.TypeBased().Map<Manager.Messages.VacationRequest>("manager"))
	.Start();

string command = string.Empty;
while (command != "quit")
{
	Console.WriteLine("What do you want to say to your manager ? (or 'quit' to exit):");
	command = Console.ReadLine();

	if (command != "quit")
	{
		var vacationRequest = new Manager.Messages.VacationRequest { Request = command };
		await bus.Send(vacationRequest);
		Console.WriteLine($"Sent vacation request: { vacationRequest.Request }");
	}

}

Console.WriteLine("Thank you and goodbye");
Console.ReadLine();
