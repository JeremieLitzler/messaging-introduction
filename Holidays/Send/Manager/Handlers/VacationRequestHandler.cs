using Manager.Messages;
using Rebus.Handlers;

namespace Manager.Handlers
{
	internal class VacationRequestHandler : IHandleMessages<VacationRequest>
	{
		public Task Handle(VacationRequest message)
		{
			Console.WriteLine($"Received vacation request: {message.Request}");
			return Task.CompletedTask;
		}
	}
}
