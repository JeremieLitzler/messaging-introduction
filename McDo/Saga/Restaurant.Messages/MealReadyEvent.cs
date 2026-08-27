namespace Restaurant.Messages
{
	public class MealReadyEvent
	{
		public string OrderId { get; set; }
		public string MealName { get; set; }
	}
}
