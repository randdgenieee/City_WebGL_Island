namespace CIG
{
	public class DailyRewardCollectPopupRequest : PopupRequest
	{
		public Currency TodaysReward
		{
			get;
		}

		public DailyRewardCollectPopupRequest(Currency todaysReward)
			: base(typeof(DailyRewardsCollectPopup), enqueue: true, dismissable: false)
		{
			TodaysReward = todaysReward;
		}
	}
}
