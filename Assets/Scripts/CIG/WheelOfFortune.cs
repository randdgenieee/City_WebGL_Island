using SparkLinq;

namespace CIG
{
	public class WheelOfFortune : IStorable
	{
		public delegate void WheelSpunEventHandler(Currencies spent, Currencies earned, int index);

		private const string CurrentRewardsKey = "CurrentRewards";

		private const string PendingRewardKey = "PendingReward";

		private readonly StorageDictionary _storage;

		private readonly GameState _gameState;

		private readonly WebService _webService;

		private readonly WheelOfFortuneProperties _properties;

		public WheelOfFortuneRewards CurrentRewards
		{
			get;
			private set;
		}

		public WheelOfFortuneReward PendingReward
		{
			get;
			private set;
		}

		public Currency NormalCost => _properties.NormalPrice;

		public bool CanAffordNormalCost => _gameState.CanAfford(NormalCost);

		public Currency PremiumCost => _properties.PremiumPrice;

		public event WheelSpunEventHandler WheelSpunEvent;

		private void FireWheelSpunEvent(Currencies spent, Currencies earned, int index)
		{
			this.WheelSpunEvent?.Invoke(spent, earned, index);
		}

		public WheelOfFortune(StorageDictionary storage, WebService webService, GameState gameState, WheelOfFortuneProperties properties)
		{
			_storage = storage;
			_gameState = gameState;
			_webService = webService;
			_properties = properties;
			CurrentRewards = (_storage.Contains("CurrentRewards") ? new WheelOfFortuneRewards(_storage.GetStorageDict("CurrentRewards")) : GetRandomRewards());
			if (_storage.Contains("PendingReward"))
			{
				PendingReward = new WheelOfFortuneReward(_storage.GetStorageDict("PendingReward"));
			}
		}

		public void NormalSpin()
		{
			if (PendingReward == null)
			{
				_gameState.SpendCurrencies(_properties.NormalPrice, CurrenciesSpentReason.WheelOfFortune, delegate(bool success, Currencies spent)
				{
					if (success)
					{
						PendingReward = CurrentRewards.GetReward();
						FireWheelSpunEvent(spent, PendingReward.Reward, PendingReward.Index);
					}
				});
			}
		}

		public void PremiumSpin()
		{
			if (PendingReward == null)
			{
				_gameState.SpendCurrencies(_properties.PremiumPrice, CurrenciesSpentReason.WheelOfFortune, delegate(bool success, Currencies spent)
				{
					if (success)
					{
						PendingReward = CurrentRewards.GetReward();
						FireWheelSpunEvent(spent, PendingReward.Reward, PendingReward.Index);
					}
				});
			}
		}

		public void CollectReward(object earnSource = null)
		{
			if (PendingReward != null)
			{
				_gameState.EarnCurrencies(PendingReward.Reward, CurrenciesEarnedReason.WheelOfFortune, new FlyingCurrenciesData(earnSource));
				PendingReward = null;
				CurrentRewards = GetRandomRewards();
			}
		}

		private WheelOfFortuneRewards GetRandomRewards()
		{
			return new WheelOfFortuneRewards(_properties.Streaks.PickRandom().Rewards.Shuffle(), _webService);
		}

		StorageDictionary IStorable.Serialize()
		{
			_storage.SetOrRemoveStorable("CurrentRewards", CurrentRewards, CurrentRewards == null);
			_storage.SetOrRemoveStorable("PendingReward", PendingReward, PendingReward == null);
			return _storage;
		}
	}
}
