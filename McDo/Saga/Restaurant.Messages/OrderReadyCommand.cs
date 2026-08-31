using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurant.Messages
{
	public class OrderReadyCommand
	{
		public string OrderId { get; private set; }
		public string Summary => $"Order {OrderId} is ready!";

		public OrderReadyCommand(string orderId)
		{
			OrderId = orderId;
		}
	}
}
