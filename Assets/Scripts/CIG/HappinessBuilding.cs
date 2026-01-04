using UnityEngine;

namespace CIG
{
	public class HappinessBuilding : ActivatableBuilding
	{
		private int _currentHappiness;

		private const string HappinessKey = "happiness";

		public virtual int Happiness
		{
			get
			{
				return _currentHappiness;
			}
			protected set
			{
				_currentHappiness = value;
			}
		}

		public int ExtraHappinessNextLevel
		{
			get
			{
				if (base.CurrentLevel >= 0 && base.CurrentLevel < base.BuildingProperties.HappinessPerLevel.Count - 1)
				{
					return base.BuildingProperties.HappinessPerLevel[base.CurrentLevel + 1] - base.BuildingProperties.HappinessPerLevel[base.CurrentLevel];
				}
				return 0;
			}
		}

		public int UnboostedHappiness
		{
			get
			{
				if (!HasRoad)
				{
					return 0;
				}
				return base.BuildingProperties.HappinessPerLevel[base.CurrentLevel];
			}
		}

		public override bool HasRoad
		{
			get
			{
				return base.HasRoad;
			}
			protected set
			{
				base.HasRoad = value;
				SetHappiness();
			}
		}

		protected override void OnConstructionFinished()
		{
			SetHappiness();
			base.OnConstructionFinished();
		}

		protected override void OnDemolishStarted()
		{
			base.OnDemolishStarted();
			Happiness = 0;
		}

		protected override void OnDemolishCancelled()
		{
			SetHappiness();
			base.OnDemolishCancelled();
		}

		protected override void OnUpgradeCompleted(double completionTime)
		{
			base.OnUpgradeCompleted(completionTime);
			SetHappiness();
		}

		protected override void OnBoostedPercentageChanged()
		{
			base.OnBoostedPercentageChanged();
			SetHappiness();
			Serialize();
		}

		private void SetHappiness()
		{
			if (base.CurrentLevel >= 0 && base.CurrentLevel < base.BuildingProperties.HappinessPerLevel.Count)
			{
				int num = base.BuildingProperties.HappinessPerLevel[base.CurrentLevel];
				int num2 = num + Mathf.CeilToInt((float)(base.ClampedBoostPercentage * num) / 100f);
				Happiness = (HasRoad ? num2 : 0);
			}
		}

		public override StorageDictionary Serialize()
		{
			StorageDictionary storageDictionary = base.Serialize();
			storageDictionary.Set("happiness", _currentHappiness);
			return storageDictionary;
		}

		protected override void Deserialize(StorageDictionary storage)
		{
			base.Deserialize(storage);
			_currentHappiness = storage.Get("happiness", 0);
		}
	}
}
