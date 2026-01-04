using UnityEngine;

namespace CIG
{
	public class DailyQuestTarget : QuestTarget
	{
		private readonly long _targetAmount;

		private readonly Currencies _reward;

		private bool _collected;

		private const string TargetAmountKey = "TargetAmount";

		private const string RewardKey = "Reward";

		private const string CollectedKey = "CollectedKey";

		public override long TargetAmount => _targetAmount;

		public override Currencies Reward => _reward;

		public override bool Completed => _collected;

		public DailyQuestTarget(StorageDictionary storage, DailyQuestProperties properties)
		{
			if (storage.Contains("TargetAmount") && storage.Contains("Reward"))
			{
				_targetAmount = storage.Get("TargetAmount", 0L);
				_reward = storage.GetModel("Reward", (StorageDictionary sd) => new Currencies(sd), new Currencies());
			}
			else
			{
				int index = Random.Range(0, properties.RandomTargets.Count);
				_targetAmount = properties.RandomTargets[index];
				_reward = properties.RandomRewards[index];
			}
			_collected = storage.Get("CollectedKey", defaultValue: false);
		}

		public override Currencies Collect()
		{
			_collected = true;
			return Reward;
		}

		public override StorageDictionary Serialize()
		{
			StorageDictionary storageDictionary = base.Serialize();
			storageDictionary.Set("TargetAmount", _targetAmount);
			storageDictionary.Set("Reward", _reward);
			storageDictionary.Set("CollectedKey", _collected);
			return storageDictionary;
		}
	}
}
