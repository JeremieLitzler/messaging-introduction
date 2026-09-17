using NSubstitute;
using OrderSaga.Tests.Builders;
using Rebus.Bus;
using Restaurant.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using static OrderSaga.Tests.Builders.PlaceOrderCommandBuilder;

namespace OrderSaga.Tests
{
	public class OrderSagaShould
	{

		private readonly IBus _bus = Substitute.For<IBus>();

		private Process ASaga()
		{
			return new Process(_bus)
			{
				Data = new OrderSagaData()
				{
				}
			};
		}

		private static async Task SagaHandleMealReadyEvent(string orderId, Process saga)
		{
			await saga.Handle(new MealReadyEvent
			{
				OrderId = orderId,
				MealName = "Pizza"
			});
		}

		[Test]
		public async Task BeInAPendingStateAterReceivingAnOrder()
		{
			var mealName = "Pizza";
			var command = ACommand()
				.WithItem(mealName, 1)
				.Build();

			Process saga = ASaga();

			await saga.Handle(command);

			await Assert.That(saga.Data.BatchesToPrepare.Count).IsEqualTo(1);
			await Assert.That(saga.Data.BatchesToPrepare[mealName]).IsEqualTo(1);
			await Assert.That(saga.Data.IsOrderComplete).IsFalse();
		}

		[Test]
		public async Task BeInAPendingStateWhenMultipleItemsAreOrderedAndNotAllAreReady()
		{
			var mealName = "Pizza";
			var command = ACommand()
				.WithItem(mealName, 2)
				.Build();

			Process saga = ASaga();

			await saga.Handle(command);
			await SagaHandleMealReadyEvent(command.OrderId, saga);
			await Assert.That(saga.Data.BatchesToPrepare.Count).IsEqualTo(1);
			await Assert.That(saga.Data.BatchesToPrepare[mealName]).IsEqualTo(1);
			await Assert.That(saga.Data.IsOrderComplete).IsFalse();
		}


		[Test]
		public async Task BeInACompletedStateAfterReceivingAllMeals()
		{
			var command = PlaceOrderCommandBuilder.ACommand()
				.WithItem("Pizza", 1)
				.Build();
			var saga = ASaga();
			await saga.Handle(command);
			await SagaHandleMealReadyEvent(command.OrderId, saga);
			await Assert.That(saga.Data.BatchesToPrepare.Count).IsEqualTo(0);
			await Assert.That(saga.Data.IsOrderComplete).IsTrue();
		}

		[Test]
		public async Task AskRestaurantToPrepareMeal()
		{
			var command = PlaceOrderCommandBuilder.ACommand()
				.WithItem("Pizza", 1)
				.Build(); 
			var saga =  ASaga();

			await saga.Handle(command);
			
			await _bus.Received().Send(Arg.Is<PrepareMealCommand>(c => c.OrderId == command.OrderId && c.MealName == "Pizza"));
		}

		[Test]
		public async Task AskRestaurantToDistributeOrder()
		{
			var command = ACommand()
				.WithItem("Pizza", 1)
				.Build();
			var saga = ASaga();
			await saga.Handle(command);
			await SagaHandleMealReadyEvent(command.OrderId, saga);
			await _bus.Received().Send(Arg.Is<DistributeOrderCommand>(c => c.OrderId == command.OrderId));
		}
	}
}
