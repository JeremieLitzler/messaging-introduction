using Bogus.DataSets;
using Restaurant.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderSaga.Tests.Builders
{
	public class PlaceOrderCommandBuilder
	{

		private string _orderId = Guid.NewGuid().ToString();
		private List<OrderItem> _items = new List<OrderItem>();
		public static PlaceOrderCommandBuilder ACommand() => new PlaceOrderCommandBuilder();

		public PlaceOrderCommandBuilder Do(Action<PlaceOrderCommandBuilder> action)
		{
			action(this);
			return this;
		}
		public PlaceOrderCommandBuilder WithOrderId(string orderId) =>
			Do(builder => builder._orderId = orderId);

		public PlaceOrderCommandBuilder WithItem(string name, int quantity) =>
			Do(builder =>
			{
				builder._items.Add(new OrderItem()
				{
					Name = name,
					Quantity = quantity
				});
			});

		public PlaceOrderCommand Build()
		{
			return new PlaceOrderCommand
			{
				OrderId = _orderId,
				Items = _items
			};
		}
	}
}
