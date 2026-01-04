using System.Collections.Generic;

namespace CIG
{
	public class DailyRewardPopupRequest : PopupRequest
	{
		public List<Currency> Streak
		{
			get;
		}

		public int DayIndex
		{
			get;
		}

		public Currency TodaysReward
		{
			get;
		}

		public DailyRewardPopupRequest(List<Currency> streak, int dayIndex, Currency todaysReward)
			: base(typeof(DailyRewardPopup), enqueue: true, dismissable: false)
		{
			Streak = streak;
			DayIndex = dayIndex;
			TodaysReward = todaysReward;
		}
	}
}
