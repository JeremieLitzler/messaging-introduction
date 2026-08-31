namespace Restaurant.Messages
{
	public class DistributeOrderCommand
	{
		public string OrderId { get; private set; } = null!;

		public DistributeOrderCommand(string orderId)
		{
			OrderId = orderId;
		}
	}
}
