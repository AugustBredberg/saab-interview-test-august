using System;

namespace TicketManagementSystem.Helpers
{
    static class TicketHelpers
    {
		public static Priority AdjustTicketPriority(string title, Priority priority, DateTime timestamp)
		{
			var titleContainsKeywords =
					title.Contains("Crash") ||
					title.Contains("Important") ||
					title.Contains("Failure");

			var isOlderThanOneHour = timestamp < DateTime.UtcNow - TimeSpan.FromHours(1);
			if (isOlderThanOneHour || titleContainsKeywords)
			{
				switch (priority)
				{
					case Priority.Low:
						priority = Priority.Medium;
						break;
					case Priority.Medium:
						priority = Priority.High;
						break;
				}
			}

			return priority;
		}

		public static double GetTicketPrice(Priority priority)
		{
			switch (priority)
			{
				case Priority.High:
					return 100;
				default:
					return 50;
			}
		}
	}
}
