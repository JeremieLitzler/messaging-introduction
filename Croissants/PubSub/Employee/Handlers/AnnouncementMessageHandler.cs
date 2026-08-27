using System;
using System.Collections.Generic;
using System.Text;

namespace Employee.Handlers
{
	internal class AnnouncementMessageHandler : Rebus.Handlers.IHandleMessages<Manager.Messages.AnnouncementMessage>
	{
		public Task Handle(Manager.Messages.AnnouncementMessage message)
		{
			Console.WriteLine($"Received announcement message: {message.Content}");
			return Task.CompletedTask;
		}
	}
}
