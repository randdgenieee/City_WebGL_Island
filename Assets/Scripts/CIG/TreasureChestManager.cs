using CIG.Translation;
using SparkLinq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	public class TreasureChestManager : IHasNotification, IProductConsumer
	{
		public delegate void ChestOpenableChangedHandler();

		public delegate void ChestOpenIntentEventHandler();

		public static readonly TreasureChestType[] PaidChestTypes = new TreasureChestType[3]
		{
			TreasureChestType.Silver,
			TreasureChestType.Gold,
			TreasureChestType.Platinum
		};

		private const double WoodenChestRefreshTimeHours = 8.0;

		private static readonly CIGGameVersion WoodenChestWithVideoGameVersion = new CIGGameVersion(1, 8, 0);

		public const string ContainsLandmarkKey = "Landmark";

		public const string ContainsGoldBuildingKey = "BuildingGold";

		public const string ContainsCashBuildingKey = "BuildingCash";

		private readonly StorageDictionary _storage;

		private readonly GameState _gameState;

		private readonly GameStats _gameStats;

		private readonly CraneManager _craneManager;

		private readonly BuildingWarehouseManager _buildingWarehouseManager;

		private readonly RoutineRunner _routineRunner;

		private readonly PopupManager _popupManager;

		private readonly Multipliers _multipliers;

		private readonly Properties _properties;

		private readonly VideoAds3Manager _videoAdsManager;

		private readonly TutorialIntermediary _tutorialManager;

		private readonly TreasureChestManagerProperties _treasureChestManagerProperties;

		private readonly Dictionary<TreasureChestType, bool> _canOpenChest = new Dictionary<TreasureChestType, bool>();

		private readonly List<TreasureChest> _treasureChests = new List<TreasureChest>();

		private readonly List<TreasureChestRewards> _unconsumedRewards;

		private DateTime _lastWoodenChestCollectTime;

		private IEnumerator _waitForWoodenChestRoutine;

		private const string LastWoodenChestCollectTimeKey = "LastWoodenChestCollectTime";

		private const string UnconsumedRewardsKey = "UnconsumedRewards";

		private DateTime WoodenChestRefreshTime => _lastWoodenChestCollectTime.AddHours(8.0);

		public TimeSpan WoodenChestTimeRemaining => WoodenChestRefreshTime - AntiCheatDateTime.UtcNow;

		public bool WoodenChestTimeExpired => WoodenChestTimeRemaining <= TimeSpan.Zero;

		public bool HasOpenableChest
		{
			get
			{
				bool flag = CanOpenChest(TreasureChestType.Wooden);
				int i = 0;
				for (int num = PaidChestTypes.Length; i < num; i++)
				{
					flag |= CanOpenChest(PaidChestTypes[i]);
				}
				return flag;
			}
		}

		public bool ShouldWatchVideoToOpenWoodenChest
		{
			get;
		}

		public event ChestOpenableChangedHandler ChestOpenableChangedEvent;

		public event ChestOpenIntentEventHandler ChestOpenIntentEvent;

		private void FireChestOpenableChangedEvent()
		{
			this.ChestOpenableChangedEvent?.Invoke();
		}

		private void FireChestOpenIntentEvent()
		{
			this.ChestOpenIntentEvent?.Invoke();
		}

		public TreasureChestManager(StorageDictionary storage, GameState gameState, LocalNotificationManager localNotificationManager, CraneManager craneManager, BuildingWarehouseManager buildingWarehouseManager, RoutineRunner routineRunner, PopupManager popupManager, GameStats gameStats, Properties properties, Multipliers multipliers, VideoAds3Manager videoAdsManager, TutorialIntermediary tutorialManager, CIGGameVersion firstVersion)
		{
			_storage = storage;
			_gameState = gameState;
			_craneManager = craneManager;
			_buildingWarehouseManager = buildingWarehouseManager;
			_routineRunner = routineRunner;
			_popupManager = popupManager;
			_gameStats = gameStats;
			_videoAdsManager = videoAdsManager;
			_tutorialManager = tutorialManager;
			_properties = properties;
			_multipliers = multipliers;
			_treasureChestManagerProperties = _properties.TreasureChestManagerProperties;
			ShouldWatchVideoToOpenWoodenChest = (firstVersion >= WoodenChestWithVideoGameVersion);
			if (!_storage.Contains("LastWoodenChestCollectTime"))
			{
				_storage.Set("LastWoodenChestCollectTime", AntiCheatDateTime.UtcNow);
			}
			_lastWoodenChestCollectTime = _storage.GetDateTime("LastWoodenChestCollectTime", AntiCheatDateTime.UtcNow);
			_unconsumedRewards = _storage.GetModels("UnconsumedRewards", (StorageDictionary sd) => new TreasureChestRewards(sd, _properties));
			int i = 0;
			for (int count = _treasureChestManagerProperties.TreasureChestProperties.Count; i < count; i++)
			{
				TreasureChest item = new TreasureChest(_gameState, _treasureChestManagerProperties.TreasureChestProperties[i]);
				_treasureChests.Add(item);
			}
			UpdatePaidOpenableChests();
			UpdateOpenableWoodenChest();
			if (!CanOpenChest(TreasureChestType.Wooden))
			{
				_routineRunner.StartCoroutine(_waitForWoodenChestRoutine = WaitForWoodenChestRoutine());
			}
			localNotificationManager.HasNotification(this);
			_gameState.BalanceChangedEvent += OnBalanceChanged;
			_gameState.LevelUpEvent += OnLevelUp;
			_videoAdsManager.AvailabilityChangedEvent += OnVideoAvailableChanged;
			_tutorialManager.TutorialFinishedEvent += OnTutorialFinished;
			int j = 0;
			for (int count2 = _unconsumedRewards.Count; j < count2; j++)
			{
				ShowPopup(_unconsumedRewards[j]);
			}
		}

		public TreasureChest GetTreasureChest(TreasureChestType type)
		{
			return _treasureChests.Find((TreasureChest t) => t.Properties.TreasureChestType == type);
		}

		public bool CanOpenChest(TreasureChestType type)
		{
			bool value;
			return _canOpenChest.TryGetValue(type, out value) && value;
		}

		public void BuyChestWithKeys(TreasureChest chest)
		{
			FireChestOpenIntentEvent();
			TreasureChestRewards rewards = GetChestRewards(chest.Properties);
			_gameState.SpendCurrencies(chest.Properties.Cost, CurrenciesSpentReason.TreasureChest, delegate(bool success, Currencies spent)
			{
				if (success)
				{
					_gameStats.AddTreasureChestPurchased(chest.Properties.TreasureChestType, chest.Properties.Cost);
					AddToRewards(rewards);
					ShowPopup(rewards);
				}
				else
				{
					UnityEngine.Debug.LogError("Trying to open a metallic chest while the player does not have enough keys.");
				}
			});
		}

		public void OpenWoodenChest()
		{
			FireChestOpenIntentEvent();
			if (ShouldWatchVideoToOpenWoodenChest)
			{
				_videoAdsManager.ShowAd(delegate(bool success, bool clicked)
				{
					if (success)
					{
						OpenWoodenChest(watchedVideo: true);
					}
				}, VideoSource.OpenWoodenChest);
			}
			else
			{
				OpenWoodenChest(watchedVideo: false);
			}
		}

		public void ConsumeRewards(TreasureChestRewards rewards)
		{
			rewards.Reward.Give(_gameState, _craneManager, _buildingWarehouseManager, WarehouseSource.Chest);
			_unconsumedRewards.Remove(rewards);
		}

		public string GetIAPGameProductNameFromChestType(TreasureChestType type)
		{
			return GetChest(type).Properties.IAPGameProductName;
		}

		public TreasureChest GetChest(TreasureChestType type)
		{
			return _treasureChests.Find((TreasureChest c) => c.Properties.TreasureChestType == type);
		}

		public TreasureChestType? GetOpenableChestType()
		{
			if (CanOpenChest(TreasureChestType.Wooden))
			{
				return TreasureChestType.Wooden;
			}
			int i = 0;
			for (int num = PaidChestTypes.Length; i < num; i++)
			{
				TreasureChestType treasureChestType = PaidChestTypes[i];
				if (CanOpenChest(treasureChestType))
				{
					return treasureChestType;
				}
			}
			return null;
		}

		private void OpenWoodenChest(bool watchedVideo)
		{
			_lastWoodenChestCollectTime = AntiCheatDateTime.UtcNow;
			TreasureChest treasureChest = GetTreasureChest(TreasureChestType.Wooden);
			TreasureChestRewards chestRewards = GetChestRewards(treasureChest.Properties);
			AddToRewards(chestRewards);
			ShowPopup(chestRewards);
			_gameStats.AddWoodenTreasureChestOpened(watchedVideo);
			UpdateOpenableWoodenChest();
			if (_waitForWoodenChestRoutine != null)
			{
				_routineRunner.StopCoroutine(_waitForWoodenChestRoutine);
			}
			_routineRunner.StartCoroutine(_waitForWoodenChestRoutine = WaitForWoodenChestRoutine());
		}

		private TreasureChestRewards GetChestRewards(TreasureChestProperties chestProperties)
		{
			TreasureChestRewards treasureChestRewards = new TreasureChestRewards(chestProperties.TreasureChestType);
			int silverKeysToSpend = (int)chestProperties.Value.GetValue("SilverKey");
			int goldKeysToSpend = (int)chestProperties.Value.GetValue("GoldKey");
			int num = 0;
			int maxBuildings = chestProperties.MaxBuildings;
			int containsCurrenciesCount = chestProperties.ContainsCurrenciesCount;
			List<BuildingProperties> list = from b in _properties.AllLandmarkBuildings.ToList()
				where _gameStats.NumberOf(b.BaseKey) == 0 && _buildingWarehouseManager.GetBuildingCount(b.BaseKey) == 0
				select b;
			if (list.Count == 0)
			{
				list = _properties.AllLandmarkBuildings.ToList();
			}
			list.Shuffle();
			List<BuildingProperties> list2 = (from b in _properties.AllBuildingProperties.ToList()
				where !(b is LandmarkBuildingProperties) && b.SilverKeysConstructionCost > decimal.Zero
				select b).Shuffle();
			List<BuildingProperties> list3 = (from b in _properties.AllBuildingProperties.ToList()
				where !(b is LandmarkBuildingProperties) && b.GoldKeysConstructionCost > decimal.Zero
				select b).Shuffle();
			treasureChestRewards.Reward.Currencies += chestProperties.GuaranteedCurrencies;
			int i = 0;
			for (int num2 = Mathf.Min(chestProperties.GuaranteedLandmarks, list.Count); i < num2; i++)
			{
				treasureChestRewards.Reward.Buildings.Add(list[i]);
				list.RemoveAt(0);
				num++;
			}
			while (num < chestProperties.GuaranteedLandmarks)
			{
				list = _properties.AllLandmarkBuildings.ToList().Shuffle();
				int j = num;
				for (int num3 = Mathf.Min(chestProperties.GuaranteedLandmarks, num + list.Count); j < num3; j++)
				{
					treasureChestRewards.Reward.Buildings.Add(list[j]);
					list.RemoveAt(0);
					num++;
				}
			}
			int k = 0;
			for (int num4 = Mathf.Min(chestProperties.GuaranteedGoldBuildings, list3.Count); k < num4; k++)
			{
				treasureChestRewards.Reward.Buildings.Add(list3[0]);
				list3.RemoveAt(0);
				num++;
			}
			int l = 0;
			for (int num5 = Mathf.Min(chestProperties.GuaranteedCashBuildings, list2.Count); l < num5; l++)
			{
				treasureChestRewards.Reward.Buildings.Add(list2[0]);
				list2.RemoveAt(0);
				num++;
			}
			if ((decimal)silverKeysToSpend > _treasureChestManagerProperties.SilverKeyValueLevelUp && CanReceiveXP(treasureChestRewards) && ChanceAddItem(chestProperties.ChanceLevelUp))
			{
				treasureChestRewards.Reward.Currencies += Currency.LevelUpCurrency(decimal.One);
				silverKeysToSpend -= (int)_treasureChestManagerProperties.SilverKeyValueLevelUp;
			}
			if (silverKeysToSpend > 0 && chestProperties.ContainsBuildingsCash)
			{
				List<BuildingProperties> list4 = from b in list2
					where b.SilverKeysConstructionCost <= (decimal)silverKeysToSpend
					select b;
				int m = 0;
				for (int count = list4.Count; m < count; m++)
				{
					if (num >= maxBuildings)
					{
						break;
					}
					BuildingProperties buildingProperties = list4[m];
					decimal silverKeysConstructionCost = buildingProperties.SilverKeysConstructionCost;
					if (silverKeysConstructionCost <= (decimal)silverKeysToSpend)
					{
						treasureChestRewards.Reward.Buildings.Add(buildingProperties);
						silverKeysToSpend -= (int)silverKeysConstructionCost;
						num++;
					}
				}
			}
			if (silverKeysToSpend > 0 && chestProperties.ContainsBuildingsGold)
			{
				int tempGoldKeysToSpend = (int)((decimal)silverKeysToSpend * _treasureChestManagerProperties.SilverKeyValueGoldKeys);
				List<BuildingProperties> list5 = from b in list3
					where b.GoldKeysConstructionCost <= (decimal)tempGoldKeysToSpend
					select b;
				int n = 0;
				for (int count2 = list5.Count; n < count2; n++)
				{
					if (num >= maxBuildings)
					{
						break;
					}
					BuildingProperties buildingProperties2 = list5[n];
					decimal goldKeysConstructionCost = buildingProperties2.GoldKeysConstructionCost;
					if (goldKeysConstructionCost <= (decimal)tempGoldKeysToSpend)
					{
						treasureChestRewards.Reward.Buildings.Add(buildingProperties2);
						tempGoldKeysToSpend -= (int)goldKeysConstructionCost;
						num++;
					}
					silverKeysToSpend = (int)((decimal)tempGoldKeysToSpend / _treasureChestManagerProperties.SilverKeyValueGoldKeys);
				}
			}
			int num6 = containsCurrenciesCount;
			if (silverKeysToSpend > 0 && chestProperties.ContainsGoldKeys)
			{
				int num7 = (num6 == 1) ? silverKeysToSpend : ((int)((float)silverKeysToSpend * UnityEngine.Random.value));
				int num8 = (int)((decimal)num7 * _treasureChestManagerProperties.SilverKeyValueGoldKeys);
				if (num8 > 0)
				{
					treasureChestRewards.Reward.Currencies += Currency.GoldKeyCurrency(num8);
					silverKeysToSpend -= num7;
				}
				num6--;
			}
			if (silverKeysToSpend > 0 && chestProperties.ContainsSilverKeys)
			{
				int num9 = (num6 == 1) ? silverKeysToSpend : ((int)((float)silverKeysToSpend * UnityEngine.Random.value));
				if (num9 > 0)
				{
					treasureChestRewards.Reward.Currencies += Currency.SilverKeyCurrency(num9);
					silverKeysToSpend -= num9;
				}
				num6--;
			}
			if (silverKeysToSpend > 0 && chestProperties.ContainsGold)
			{
				int num10 = (num6 == 1) ? silverKeysToSpend : ((int)((float)silverKeysToSpend * UnityEngine.Random.value));
				int num11 = (int)((decimal)num10 * _treasureChestManagerProperties.SilverKeyValueGold);
				if (num11 > 0)
				{
					treasureChestRewards.Reward.Currencies += Currency.GoldCurrency(num11);
					silverKeysToSpend -= num10;
				}
				num6--;
			}
			if (silverKeysToSpend > 0 && chestProperties.ContainsCash)
			{
				int num12 = (num6 == 1) ? silverKeysToSpend : ((int)((float)silverKeysToSpend * UnityEngine.Random.value));
				int num13 = (int)((decimal)num12 * _treasureChestManagerProperties.SilverKeyValueCash);
				if (num13 > 0)
				{
					treasureChestRewards.Reward.Currencies += Currency.CashCurrency(num13);
					silverKeysToSpend -= num12;
				}
				num6--;
			}
			if (silverKeysToSpend > 0 && chestProperties.ContainsTokens)
			{
				int num14 = (num6 == 1) ? silverKeysToSpend : ((int)((float)silverKeysToSpend * UnityEngine.Random.value));
				int num15 = (int)((decimal)num14 * _treasureChestManagerProperties.SilverKeyValueTokens);
				if (num15 > 0)
				{
					treasureChestRewards.Reward.Currencies += Currency.TokenCurrency(num15);
					silverKeysToSpend -= num14;
				}
				num6--;
			}
			if (num6 == 1 && chestProperties.ContainsXP && CanReceiveXP(treasureChestRewards))
			{
				int num16 = (int)((decimal)silverKeysToSpend / _treasureChestManagerProperties.SilverKeyValueLevelUp * _gameState.GetXpForLevelsUp(decimal.One));
				if (num16 > 0)
				{
					treasureChestRewards.Reward.Currencies += Currency.XPCurrency(num16);
				}
			}
			if ((decimal)goldKeysToSpend > _treasureChestManagerProperties.GoldKeyValueLevelUp && CanReceiveXP(treasureChestRewards) && ChanceAddItem(chestProperties.ChanceLevelUp))
			{
				treasureChestRewards.Reward.Currencies += Currency.LevelUpCurrency(decimal.One);
				goldKeysToSpend -= (int)_treasureChestManagerProperties.GoldKeyValueLevelUp;
			}
			if ((decimal)goldKeysToSpend > _treasureChestManagerProperties.GoldKeyValueCrane && ChanceAddItem(chestProperties.ChanceCrane))
			{
				treasureChestRewards.Reward.Currencies += Currency.CraneCurrency(decimal.One);
				goldKeysToSpend -= (int)_treasureChestManagerProperties.GoldKeyValueCrane;
			}
			if (goldKeysToSpend > 0 && num < maxBuildings && ChanceAddItem(chestProperties.ChanceLandmark))
			{
				BuildingProperties buildingProperties3 = list.First((BuildingProperties b) => b.GoldKeysConstructionCost > decimal.Zero && b.GoldKeysConstructionCost <= (decimal)goldKeysToSpend);
				if (buildingProperties3 != null)
				{
					treasureChestRewards.Reward.Buildings.Add(buildingProperties3);
					goldKeysToSpend -= (int)buildingProperties3.GoldKeysConstructionCost;
					num++;
				}
			}
			if (goldKeysToSpend > 0 && chestProperties.ContainsBuildingsCash)
			{
				int tempSilverKeysToSpend = (int)((decimal)goldKeysToSpend / _treasureChestManagerProperties.SilverKeyValueGoldKeys);
				List<BuildingProperties> list6 = from b in list2
					where b.SilverKeysConstructionCost <= (decimal)tempSilverKeysToSpend
					select b;
				int num17 = 0;
				for (int count3 = list6.Count; num17 < count3; num17++)
				{
					if (num >= maxBuildings)
					{
						break;
					}
					BuildingProperties buildingProperties4 = list6[num17];
					decimal silverKeysConstructionCost2 = buildingProperties4.SilverKeysConstructionCost;
					if (silverKeysConstructionCost2 <= (decimal)tempSilverKeysToSpend)
					{
						treasureChestRewards.Reward.Buildings.Add(buildingProperties4);
						tempSilverKeysToSpend -= (int)silverKeysConstructionCost2;
						num++;
					}
					goldKeysToSpend = (int)((decimal)tempSilverKeysToSpend * _treasureChestManagerProperties.SilverKeyValueGoldKeys);
				}
			}
			if (goldKeysToSpend > 0 && chestProperties.ContainsBuildingsGold)
			{
				List<BuildingProperties> list7 = from b in list3
					where b.GoldKeysConstructionCost <= (decimal)goldKeysToSpend
					select b;
				int num18 = 0;
				for (int count4 = list7.Count; num18 < count4; num18++)
				{
					if (num >= maxBuildings)
					{
						break;
					}
					BuildingProperties buildingProperties5 = list7[num18];
					decimal goldKeysConstructionCost2 = buildingProperties5.GoldKeysConstructionCost;
					if (goldKeysConstructionCost2 <= (decimal)goldKeysToSpend)
					{
						treasureChestRewards.Reward.Buildings.Add(buildingProperties5);
						goldKeysToSpend -= (int)goldKeysConstructionCost2;
						num++;
					}
				}
			}
			int num19 = containsCurrenciesCount;
			if (goldKeysToSpend > 0 && chestProperties.ContainsGoldKeys)
			{
				int num20 = (num19 == 1) ? goldKeysToSpend : ((int)((float)goldKeysToSpend * UnityEngine.Random.value));
				if (num20 > 0)
				{
					treasureChestRewards.Reward.Currencies += Currency.GoldKeyCurrency(num20);
					goldKeysToSpend -= num20;
				}
				num19--;
			}
			if (goldKeysToSpend > 0 && chestProperties.ContainsSilverKeys)
			{
				int num21 = (num19 == 1) ? goldKeysToSpend : ((int)((float)goldKeysToSpend * UnityEngine.Random.value));
				int num22 = (int)((decimal)num21 / _treasureChestManagerProperties.SilverKeyValueGoldKeys);
				if (num22 > 0)
				{
					treasureChestRewards.Reward.Currencies += Currency.SilverKeyCurrency(num22);
					goldKeysToSpend -= num21;
				}
				num19--;
			}
			if (goldKeysToSpend > 0 && chestProperties.ContainsGold)
			{
				int num23 = (num19 == 1) ? goldKeysToSpend : ((int)((float)goldKeysToSpend * UnityEngine.Random.value));
				int num24 = (int)((decimal)num23 * _treasureChestManagerProperties.GoldKeyValueGold);
				if (num24 > 0)
				{
					treasureChestRewards.Reward.Currencies += Currency.GoldCurrency(num24);
					goldKeysToSpend -= num23;
				}
				num19--;
			}
			if (goldKeysToSpend > 0 && chestProperties.ContainsCash)
			{
				int num25 = (num19 == 1) ? goldKeysToSpend : ((int)((float)goldKeysToSpend * UnityEngine.Random.value));
				int num26 = (int)((decimal)num25 * _treasureChestManagerProperties.GoldKeyValueCash);
				if (num26 > 0)
				{
					treasureChestRewards.Reward.Currencies += Currency.CashCurrency(num26);
					goldKeysToSpend -= num25;
				}
				num19--;
			}
			if (goldKeysToSpend > 0 && chestProperties.ContainsTokens)
			{
				int num27 = (num19 == 1) ? goldKeysToSpend : ((int)((float)goldKeysToSpend * UnityEngine.Random.value));
				int num28 = (int)((decimal)num27 * _treasureChestManagerProperties.GoldKeyValueTokens);
				if (num28 > 0)
				{
					treasureChestRewards.Reward.Currencies += Currency.TokenCurrency(num28);
					goldKeysToSpend -= num27;
				}
				num19--;
			}
			if (num19 == 1 && chestProperties.ContainsXP && CanReceiveXP(treasureChestRewards))
			{
				int num29 = (int)((decimal)goldKeysToSpend / _treasureChestManagerProperties.GoldKeyValueLevelUp * _gameState.GetXpForLevelsUp(decimal.One) * _multipliers.GetMultiplier(MultiplierType.XP));
				if (num29 > 0)
				{
					treasureChestRewards.Reward.Currencies += Currency.XPCurrency(num29);
				}
			}
			return treasureChestRewards;
		}

		private bool ChanceAddItem(float chance)
		{
			float value = UnityEngine.Random.value;
			if (!Mathf.Approximately(chance, 0f))
			{
				if (!(value <= chance))
				{
					return Mathf.Approximately(value, chance);
				}
				return true;
			}
			return false;
		}

		private bool CanReceiveXP(TreasureChestRewards rewards)
		{
			return (decimal)_gameState.Level + rewards.Reward.Currencies.GetValue("LevelUp") < (decimal)_gameState.MaxLevel;
		}

		private void ShowPopup(TreasureChestRewards rewards)
		{
			TreasureChestPopupRequest request = new TreasureChestPopupRequest(rewards);
			bool num = _popupManager.IsShowingPopup && !(_popupManager.TopPopup is TreasureChestPopupRequest);
			_popupManager.RequestPopup(request);
			if (num)
			{
				_popupManager.CloseAllOpenPopups(instant: false);
			}
		}

		private void UpdatePaidOpenableChests()
		{
			int i = 0;
			for (int num = PaidChestTypes.Length; i < num; i++)
			{
				TreasureChestType treasureChestType = PaidChestTypes[i];
				TreasureChest treasureChest = GetTreasureChest(treasureChestType);
				UpdateOpenableChest(treasureChestType, _gameState.CanAfford(treasureChest.Properties.Cost));
			}
		}

		private void UpdateOpenableWoodenChest()
		{
			UpdateOpenableChest(TreasureChestType.Wooden, WoodenChestTimeExpired && (!ShouldWatchVideoToOpenWoodenChest || _videoAdsManager.IsReady));
		}

		private void UpdateOpenableChest(TreasureChestType chestType, bool canOpen)
		{
			if (_canOpenChest.TryGetValue(chestType, out bool value))
			{
				_canOpenChest[chestType] = canOpen;
				if (canOpen != value)
				{
					FireChestOpenableChangedEvent();
				}
			}
			else
			{
				_canOpenChest[chestType] = canOpen;
				FireChestOpenableChangedEvent();
			}
		}

		private void OnBalanceChanged(Currencies oldBalance, Currencies newBalance, FlyingCurrenciesData flyingCurrenciesData)
		{
			UpdatePaidOpenableChests();
		}

		private void OnVideoAvailableChanged()
		{
			UpdateOpenableWoodenChest();
		}

		private IEnumerator WaitForWoodenChestRoutine()
		{
			yield return new WaitForSecondsRealtime((float)WoodenChestTimeRemaining.TotalSeconds);
			UpdateOpenableWoodenChest();
		}

		private void AddToRewards(TreasureChestRewards rewards)
		{
			_unconsumedRewards.Add(rewards);
		}

		private void TryGiveFreeWoodenChest()
		{
			if (_gameStats.NumberOfWoodenChestsOpened == 0 && _tutorialManager.InitialTutorialFinished && _gameState.Level >= 2)
			{
				_lastWoodenChestCollectTime = AntiCheatDateTime.UtcNow.AddHours(-8.0);
				UpdateOpenableWoodenChest();
			}
		}

		private void OnLevelUp(int previousLevel, int newLevel)
		{
			if (newLevel == 2)
			{
				TryGiveFreeWoodenChest();
			}
		}

		private void OnTutorialFinished(Tutorial tutorial)
		{
			if (tutorial.TutorialType == TutorialType.BuildCommunity)
			{
				TryGiveFreeWoodenChest();
			}
		}

		bool IProductConsumer.ConsumeProduct(TOCIStoreProduct product)
		{
			TreasureChest treasureChest = _treasureChests.Find((TreasureChest c) => c.Properties.IAPGameProductName == product.GameProductName);
			if (treasureChest == null)
			{
				UnityEngine.Debug.LogError("Can't consume product '" + product.Title + "' because game product name '" + product.GameProductName + "' can't be matched to a chest.");
				return false;
			}
			TreasureChestRewards chestRewards = GetChestRewards(treasureChest.Properties);
			StoreProductCategory category = product.Category;
			if ((uint)(category - 6) <= 1u)
			{
				AddToRewards(chestRewards);
				ShowPopup(chestRewards);
				_gameStats.AddTreasureChestPurchased(treasureChest.Properties.TreasureChestType, new Currencies());
				return true;
			}
			UnityEngine.Debug.LogErrorFormat("Missing product consumer for product category '{0}'", product.Category);
			return false;
		}

		PlannedNotification[] IHasNotification.GetNotifications()
		{
			List<PlannedNotification> list = new List<PlannedNotification>();
			if (WoodenChestTimeRemaining > TimeSpan.Zero)
			{
				list.Add(new PlannedNotification((int)WoodenChestTimeRemaining.TotalSeconds, Localization.Key("notification_wooden_chest"), sound: false, 4));
			}
			return list.ToArray();
		}

		public void Serialize()
		{
			_storage.Set("LastWoodenChestCollectTime", _lastWoodenChestCollectTime);
			_storage.Set("UnconsumedRewards", _unconsumedRewards);
		}
	}
}
