using System;
using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	public class Multipliers : IStorable
	{
		public class Multiplier
		{
			public decimal Value
			{
				get;
				set;
			}

			public string ServerKey
			{
				get;
			}

			public Multiplier(string serverKey)
			{
				Value = decimal.One;
				ServerKey = serverKey;
			}
		}

		private const decimal DefaultModifierValue = decimal.One;

		private const decimal MinumumModifierValue = 0.1m;

		private readonly Dictionary<MultiplierType, Multiplier> MultipliersDict = new Dictionary<MultiplierType, Multiplier>
		{
			{
				MultiplierType.ExpansionCashCost,
				new Multiplier("expansionCashCostMultiplier")
			},
			{
				MultiplierType.ResidentialCashCost,
				new Multiplier("residentialBuildingCostCashMultiplier")
			},
			{
				MultiplierType.ResidentialGoldCost,
				new Multiplier("residentialBuildingCostGoldMultiplier")
			},
			{
				MultiplierType.CommercialCashCost,
				new Multiplier("commercialBuildingCostCashMultiplier")
			},
			{
				MultiplierType.CommercialGoldCost,
				new Multiplier("commercialBuildingCostGoldMultiplier")
			},
			{
				MultiplierType.DecorationCashCost,
				new Multiplier("decorationCashCostMultiplier")
			},
			{
				MultiplierType.DecorationsGoldCost,
				new Multiplier("decorationGoldCostMultiplier")
			},
			{
				MultiplierType.CommunityCashCost,
				new Multiplier("communityBuildingCostCashMultiplier")
			},
			{
				MultiplierType.CommunityGoldCost,
				new Multiplier("communityBuildingCostGoldMultiplier")
			},
			{
				MultiplierType.ResidentialInstantGoldCostBuilding,
				new Multiplier("residentialInstantGoldCostBuildingMultiplier")
			},
			{
				MultiplierType.CommercialInstantGoldCostBuilding,
				new Multiplier("commercialInstantGoldCostBuildingMultiplier")
			},
			{
				MultiplierType.CommunityInstantGoldCostBuilding,
				new Multiplier("communityInstantGoldCostBuildingMultiplier")
			},
			{
				MultiplierType.ResidentialInstantGoldCostUpgrade,
				new Multiplier("residentialInstantGoldCostUpgradeMultiplier")
			},
			{
				MultiplierType.CommercialInstantGoldCostUpgrade,
				new Multiplier("commercialInstantGoldCostUpgradeMultiplier")
			},
			{
				MultiplierType.CommunityInstantGoldCostUpgrade,
				new Multiplier("communityInstantGoldCostUpgradeMultiplier")
			},
			{
				MultiplierType.ResidentialCashCostUpgrade,
				new Multiplier("residentialCashCostUpgradeMultiplier")
			},
			{
				MultiplierType.CommercialCashCostUpgrade,
				new Multiplier("commercialCashCostUpgradeMultiplier")
			},
			{
				MultiplierType.CommunityCashCostUpgrade,
				new Multiplier("communityCashCostUpgradeMultiplier")
			},
			{
				MultiplierType.KeyDealGoldKeyCost,
				new Multiplier("keyDealGoldKeyCostMultiplier")
			},
			{
				MultiplierType.KeyDealSilverKeyCost,
				new Multiplier("keyDealSilverKeyCostMultiplier")
			},
			{
				MultiplierType.LandmarksGoldKeyCost,
				new Multiplier("landmarksGoldKeyCostMultiplier")
			},
			{
				MultiplierType.LandmarksSilverKeyCost,
				new Multiplier("landmarksSilverKeyCostMultiplier")
			},
			{
				MultiplierType.UpgradeGoldKeyReward,
				new Multiplier("upgradeGoldKeyRewardMultiplier")
			},
			{
				MultiplierType.UpgradeSilverKeyReward,
				new Multiplier("upgradeSilverKeyRewardMultiplier")
			},
			{
				MultiplierType.LevelUpCashReward,
				new Multiplier("levelUpCashRewardMultiplier")
			},
			{
				MultiplierType.UpspeedGoldCost,
				new Multiplier("upspeedGoldCostMultiplier")
			},
			{
				MultiplierType.CommercialBuildingProfit,
				new Multiplier("commercialBuildingProfitMultiplier")
			},
			{
				MultiplierType.XP,
				new Multiplier("xpMultiplier")
			}
		};

		public Multipliers()
		{
		}

		public Multipliers(StorageDictionary storage)
		{
			foreach (KeyValuePair<MultiplierType, Multiplier> item in MultipliersDict)
			{
				item.Value.Value = ClampMultiplier(storage.Get(item.Key.ToString(), decimal.One));
			}
		}

		public decimal GetMultiplier(MultiplierType type)
		{
			if (MultipliersDict.TryGetValue(type, out Multiplier value))
			{
				return value.Value;
			}
			UnityEngine.Debug.LogErrorFormat("Multiplier of type '{0}' could not be found", type);
			return decimal.One;
		}

		public void HandlePullResponse(Dictionary<string, string> properties)
		{
			foreach (KeyValuePair<MultiplierType, Multiplier> item in MultipliersDict)
			{
				item.Value.Value = ClampMultiplier(properties.Get(item.Value.ServerKey, item.Value.Value));
			}
		}

		private decimal ClampMultiplier(decimal value)
		{
			return Math.Max(value, 0.1m);
		}

		StorageDictionary IStorable.Serialize()
		{
			StorageDictionary storageDictionary = new StorageDictionary();
			foreach (KeyValuePair<MultiplierType, Multiplier> item in MultipliersDict)
			{
				storageDictionary.Set(item.Key.ToString(), item.Value.Value);
			}
			return storageDictionary;
		}
	}
}
