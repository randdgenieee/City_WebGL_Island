namespace CIG
{
	public class TreasureChestRewards : IStorable
	{
		public delegate void VideoWatchedForDoubleRewardEventHandler(bool success);

		private const string TreasureChestTypeKey = "TreasureChestType";

		private const string RewardKey = "Reward";

		private const string RewardIsDoubledKey = "IsDoubled";

		public TreasureChestType TreasureChestType
		{
			get;
		}

		public Reward Reward
		{
			get;
		}

		public bool IsDoubled
		{
			get;
			private set;
		}

		public event VideoWatchedForDoubleRewardEventHandler VideoWatchedForDoubleRewardEvent;

		private void FireVideoWatchedForDoubleRewardEvent(bool success)
		{
			this.VideoWatchedForDoubleRewardEvent?.Invoke(success);
		}

		public TreasureChestRewards(TreasureChestType treasureChestType)
		{
			TreasureChestType = treasureChestType;
			Reward = new Reward();
		}

		public void WatchVideoForDoubleReward(VideoAds3Manager videoManager)
		{
			if (videoManager.IsReady)
			{
				videoManager.ShowAd(delegate(bool success, bool clicked)
				{
					if (success)
					{
						Reward.Currencies *= 2m;
						IsDoubled = true;
					}
					FireVideoWatchedForDoubleRewardEvent(success);
				}, VideoSource.DoubleWoodenChestReward);
			}
			else
			{
				FireVideoWatchedForDoubleRewardEvent(success: false);
			}
		}

		public TreasureChestRewards(StorageDictionary storage, Properties properties)
		{
			TreasureChestType = (TreasureChestType)storage.Get("TreasureChestType", -1);
			Reward = storage.GetModel("Reward", (StorageDictionary sd) => new Reward(sd, properties), new Reward());
			IsDoubled = storage.Get("IsDoubled", defaultValue: false);
		}

		StorageDictionary IStorable.Serialize()
		{
			StorageDictionary storageDictionary = new StorageDictionary();
			storageDictionary.Set("TreasureChestType", (int)TreasureChestType);
			storageDictionary.Set("Reward", Reward);
			storageDictionary.Set("IsDoubled", IsDoubled);
			return storageDictionary;
		}
	}
}
