using SparkLinq;
using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	public class WheelOfFortuneRewards : IStorable
	{
		private const string RewardsKey = "Rewards";

		public List<Currencies> Rewards
		{
			get;
			private set;
		}

		public WheelOfFortuneRewards(List<Currencies> rewards, WebService webService)
		{
			List<Currencies> list2 = Rewards = rewards.Select(delegate(Currencies reward)
			{
				Currencies currencies = (!webService.WheelOfFortuneTokenRewardEnabled && reward.Contains("Token")) ? new Currencies() : new Currencies(reward);
				currencies.SetValue("XP", currencies.GetValue("XP") * webService.Multipliers.GetMultiplier(MultiplierType.XP));
				return currencies.WithoutEmpty();
			});
		}

		public WheelOfFortuneRewards(StorageDictionary storage)
		{
			Rewards = storage.GetModels("Rewards", (StorageDictionary sd) => new Currencies(sd));
		}

		public WheelOfFortuneReward GetReward()
		{
			int index = Random.Range(0, Rewards.Count);
			return new WheelOfFortuneReward(index, Rewards[index]);
		}

		public StorageDictionary Serialize()
		{
			StorageDictionary storageDictionary = new StorageDictionary();
			storageDictionary.Set("Rewards", Rewards);
			return storageDictionary;
		}
	}
}
