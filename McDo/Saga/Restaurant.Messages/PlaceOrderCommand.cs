namespace Restaurant.Messages
{
	public class PlaceOrderCommand
	{
		public string OrderId { get; set; }

		public List<OrderItem> Items { get; set; }
	}

	public class OrderItem
	{
		public string Name { get; set; }
		public int Quantity { get; set; }
	}
}
