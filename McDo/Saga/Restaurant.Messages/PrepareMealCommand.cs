using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurant.Messages
{
	public class PrepareMealCommand
	{
		public string MealName {  get; set; }
		public string OrderId { get; set; }
	}
}
