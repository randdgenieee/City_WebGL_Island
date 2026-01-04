using CIG.Translation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace CIG
{
	public class UpgradableBuilding : Building
	{
		public delegate void ToggleLevelIconEventHandler(bool active);

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass42_0
		{
			public Currencies cost;

			public UpgradableBuilding _003C_003E4__this;

			internal void _003CStartUpgrade_003Eb__1(bool success, Currencies spent)
			{
				if (success)
				{
					_003C_003E4__this.StartUpgrading();
					_003C_003E4__this._gameStats.AddBuildingUpgradeStarted(_003C_003E4__this.BuildingProperties, _003C_003E4__this.CurrentLevel + 1, cost);
				}
			}
		}

		[CompilerGenerated]
		private sealed class _003CUpgradeBehaviour_003Ed__62 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public UpgradableBuilding _003C_003E4__this;

			object IEnumerator<object>.Current
			{
				[DebuggerHidden]
				get
				{
					return _003C_003E2__current;
				}
			}

			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return _003C_003E2__current;
				}
			}

			[DebuggerHidden]
			public _003CUpgradeBehaviour_003Ed__62(int _003C_003E1__state)
			{
				this._003C_003E1__state = _003C_003E1__state;
			}

			[DebuggerHidden]
			void IDisposable.Dispose()
			{
			}

			private bool MoveNext()
			{
				int num = _003C_003E1__state;
				UpgradableBuilding upgradableBuilding = _003C_003E4__this;
				switch (num)
				{
				default:
					return false;
				case 0:
					_003C_003E1__state = -1;
					if (upgradableBuilding._upgradeUpspeedableProcess == null)
					{
						upgradableBuilding._upgradeUpspeedableProcess = new UpspeedableProcess(upgradableBuilding._timing, upgradableBuilding._multipliers, upgradableBuilding._gameState, upgradableBuilding.UpgradeDuration, CurrenciesSpentReason.SpeedupUpgrade);
					}
					upgradableBuilding.ShowUpgradeProgressBar(upgradableBuilding._upgradeUpspeedableProcess);
					_003C_003E2__current = upgradableBuilding._upgradeUpspeedableProcess;
					_003C_003E1__state = 1;
					return true;
				case 1:
					_003C_003E1__state = -1;
					if (upgradableBuilding._upgradeUpspeedableProcess.Cancelled)
					{
						upgradableBuilding._craneManager.FinishTracking(upgradableBuilding);
					}
					else
					{
						upgradableBuilding.PerformUpgrade(upgradableBuilding._upgradeUpspeedableProcess.EndTime);
					}
					upgradableBuilding._upgradeBehaviourRoutine = null;
					upgradableBuilding._upgradeUpspeedableProcess = null;
					return false;
				}
			}

			bool IEnumerator.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				return this.MoveNext();
			}

			[DebuggerHidden]
			void IEnumerator.Reset()
			{
				throw new NotSupportedException();
			}
		}

		private const int MinUpgradeSignLevel = 10;

		[SerializeField]
		private UpgradeSign _upgradeSign;

		private UpspeedableProcess _upgradeUpspeedableProcess;

		private IEnumerator _upgradeBehaviourRoutine;

		private ProgressOverlay _upgradeProgressBar;

		private const string UpgradeUpspeedableProcessKey = "UpgradeUpspeedableProcess";

		public const string LevelKey = "level";

		public UpgradeSign UpgradeSignPrefab => _upgradeSign;

		public static bool LevelIconsActive
		{
			get;
			private set;
		}

		public int CurrentLevel
		{
			get;
			private set;
		}

		public virtual int DisplayLevel => CurrentLevel;

		public virtual bool CanUpgrade
		{
			get
			{
				if (!IsUpgrading)
				{
					return CurrentLevel < base.BuildingProperties.MaximumLevel;
				}
				return false;
			}
		}

		public bool IsUpgrading => _upgradeUpspeedableProcess != null;

		public virtual Currency UpgradeCost
		{
			get
			{
				int index = Mathf.Clamp(CurrentLevel, 0, base.BuildingProperties.UpgradeCashCosts.Count - 1);
				return base.BuildingProperties.UpgradeCashCosts[index];
			}
		}

		public virtual Currency UpgradeInstantCost
		{
			get
			{
				int index = Mathf.Clamp(CurrentLevel, 0, base.BuildingProperties.UpgradeGoldCosts.Count - 1);
				return base.BuildingProperties.UpgradeGoldCosts[index];
			}
		}

		public Currency UpgradeCashReward
		{
			get
			{
				int index = Mathf.Clamp(CurrentLevel, 0, base.BuildingProperties.UpgradeCashRewards.Count - 1);
				decimal value = Math.Round(base.BuildingProperties.UpgradeCashRewards[index].Value * _multipliers.GetMultiplier(MultiplierType.UpgradeSilverKeyReward));
				return new Currency(base.BuildingProperties.UpgradeCashRewards[index].Name, value);
			}
		}

		public Currency UpgradeGoldReward
		{
			get
			{
				int index = Mathf.Clamp(CurrentLevel, 0, base.BuildingProperties.UpgradeGoldRewards.Count - 1);
				decimal value = Math.Round(base.BuildingProperties.UpgradeGoldRewards[index].Value * _multipliers.GetMultiplier(MultiplierType.UpgradeGoldKeyReward));
				return new Currency(base.BuildingProperties.UpgradeGoldRewards[index].Name, value);
			}
		}

		public int UpgradeDuration
		{
			get
			{
				int index = Mathf.Clamp(CurrentLevel, 0, base.BuildingProperties.UpgradeDurationsSeconds.Count - 1);
				return base.BuildingProperties.UpgradeDurationsSeconds[index];
			}
		}

		protected virtual bool CanShowUpgradeSign
		{
			get
			{
				if (base.State == BuildingState.Normal && CurrentLevel <= base.BuildingProperties.MaximumLevel && !IsUpgrading && (CurrentLevel == base.BuildingProperties.MaximumLevel || CanUpgrade))
				{
					return !base.Hidden;
				}
				return false;
			}
		}

		public static event ToggleLevelIconEventHandler ToggleLevelIconEvent;

		private static void FireToggleLevelIconEvent(bool active)
		{
			UpgradableBuilding.ToggleLevelIconEvent?.Invoke(active);
		}

		public override void Initialize(StorageDictionary storage, IsometricGrid isometricGrid, IslandsManagerView islandsManagerView, WorldMap worldMap, BuildingWarehouseManager buildingWarehouseManager, CraneManager craneManager, GameStats gameStats, GameState gameState, PopupManager popupManager, Multipliers multipliers, Timing timing, RoutineRunner routineRunner, GridTileProperties properties, OverlayManager overlayManager, CIGIslandState islandState, GridIndex? index = default(GridIndex?))
		{
			base.Initialize(storage, isometricGrid, islandsManagerView, worldMap, buildingWarehouseManager, craneManager, gameStats, gameState, popupManager, multipliers, timing, routineRunner, properties, overlayManager, islandState, index);
			_upgradeSign.Initialize(this, visiting: false);
			UpdateLevelIcon(CurrentLevel);
			UpdateUpgradeSign();
			if (IsUpgrading)
			{
				StartUpgradeRoutine();
			}
			ToggleLevelIconEvent += OnToggleLevelIcon;
			_gameState.ValueChangedEvent += OnGameStateValueChanged;
		}

		protected override void OnDestroy()
		{
			StopUpgradeRoutine();
			ToggleLevelIconEvent -= OnToggleLevelIcon;
			if (_gameState != null)
			{
				_gameState.ValueChangedEvent -= OnGameStateValueChanged;
			}
			base.OnDestroy();
		}

		public static void ToggleLevelIcons(bool active)
		{
			if (active != LevelIconsActive)
			{
				LevelIconsActive = active;
				FireToggleLevelIconEvent(LevelIconsActive);
			}
		}

		public override void BuildFromWarehouse(bool newBuilding, int level)
		{
			CurrentLevel = level;
			UpdateLevelIcon(CurrentLevel);
			base.BuildFromWarehouse(newBuilding, level);
		}

		public void StartUpgrade()
		{
			if (CanUpgrade)
			{
				_craneManager.CheckCraneAvailable(_003CStartUpgrade_003Eg__UpgradeAction_007C42_0, null, base.BuildingProperties.UpgradeCranes);
			}
		}

		public void UpgradeImmediately()
		{
			if (CanUpgrade)
			{
				_gameState.SpendCurrencies(UpgradeInstantCost, CurrenciesSpentReason.BuildingUpgradeInstant, delegate(bool success, Currencies spent)
				{
					if (success)
					{
						SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.UpgradeBuilding);
						Currency upgradeGoldReward = UpgradeGoldReward;
						_gameState.EarnCurrencies(upgradeGoldReward, CurrenciesEarnedReason.BuildingUpgrade, new FlyingCurrenciesData(this, (int)upgradeGoldReward.Value));
						if (upgradeGoldReward.Value > decimal.Zero)
						{
							_plingManager.ShowCurrencyPlings(_timing, upgradeGoldReward);
						}
						PerformUpgrade(_timing.GameTime);
					}
				});
			}
		}

		public override void UpdateSortingOrder()
		{
			base.UpdateSortingOrder();
			UpdateUpgradeSign();
			_upgradeSign.UpdateSortingOrder(base.SpriteRenderer.sortingOrder + 1);
		}

		public virtual ILocalizedString ReasonWhyCantUpgrade()
		{
			if (CanUpgrade)
			{
				return null;
			}
			return Localization.Key("object_upgraded_to_maximum_level");
		}

		protected override void OnBuildingPressed()
		{
			if (base.State == BuildingState.Normal && IsUpgrading)
			{
				Sprite asset = SingletonMonobehaviour<UISpriteAssetCollection>.Instance.GetAsset(UISpriteType.UpgradeIcon);
				Sprite asset2 = SingletonMonobehaviour<UISpriteAssetCollection>.Instance.GetAsset(UISpriteType.WaitIcon);
				SpeedupPopupRequest request = new SpeedupPopupRequest(_upgradeUpspeedableProcess, base.BuildingProperties.LocalizedName, Localization.Key("upgrading_verb"), Localization.Key("wait"), null, asset2, base.BuildingProperties, asset, "building_upgrade");
				_popupManager.RequestPopup(request);
			}
			else
			{
				base.OnBuildingPressed();
			}
		}

		protected virtual void UpdateLevelIcon(int level)
		{
			if (LevelIconsActive && !IsUpgrading)
			{
				if (base.BuildingProperties.MaximumLevel != 0 && base.State != BuildingState.Constructing)
				{
					_gridTileIconManager.SetIcon<LevelGridTileIcon>(GridTileIconType.Level).SetLevel(level);
				}
			}
			else
			{
				_gridTileIconManager.RemoveIcon(GridTileIconType.Level);
			}
		}

		protected override void OnConstructionFinished()
		{
			UpdateLevelIcon(CurrentLevel);
			UpdateUpgradeSign();
			base.OnConstructionFinished();
		}

		protected override void OnDemolishStarted()
		{
			base.OnDemolishStarted();
			_upgradeUpspeedableProcess?.Cancel();
			_gridTileIconManager.RemoveIcon(GridTileIconType.Level);
		}

		protected virtual void OnUpgradeStarted()
		{
			Currency upgradeCashReward = UpgradeCashReward;
			_gameState.EarnCurrencies(upgradeCashReward, CurrenciesEarnedReason.BuildingUpgrade, new FlyingCurrenciesData(this, (int)upgradeCashReward.Value));
			if (upgradeCashReward.Value > decimal.Zero)
			{
				_plingManager.ShowCurrencyPlings(_timing, upgradeCashReward);
			}
			_craneManager.StartTracking(this, _upgradeUpspeedableProcess.EndTime, base.BuildingProperties.UpgradeCranes);
		}

		protected virtual void OnUpgradeCompleted(double completionTime)
		{
			RemoveUpgradeProgressBar();
			UpdateLevelIcon(CurrentLevel);
			UpdateUpgradeSign();
		}

		protected override void OnHiddenChanged(bool hidden)
		{
			base.OnHiddenChanged(hidden);
			UpdateUpgradeSign();
			_upgradeSign.SetHidden(hidden);
		}

		private void UpdateUpgradeSign()
		{
			if (CanShowUpgradeSign)
			{
				if (CurrentLevel <= 0)
				{
					decimal value = _gameState.Balance.GetValue("Cash");
					decimal value2 = UpgradeCost.Value;
					bool flag = _gameState.Level >= 10;
					bool flag2 = value2 <= value;
					_upgradeSign.gameObject.SetActive(flag && flag2);
				}
				else
				{
					_upgradeSign.UpdateSign(base.Mirrored, CurrentLevel, base.BuildingProperties.MaximumLevel);
					_upgradeSign.gameObject.SetActive(value: true);
				}
			}
			else
			{
				_upgradeSign.gameObject.SetActive(value: false);
			}
		}

		private void StartUpgrading()
		{
			SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.UpgradeBuilding);
			StartUpgradeRoutine();
			OnUpgradeStarted();
			Serialize();
		}

		private void PerformUpgrade(double completionTime)
		{
			CurrentLevel++;
			_upgradeUpspeedableProcess = null;
			UpdateLevelIcon(CurrentLevel);
			OnUpgradeCompleted(completionTime);
			_craneManager.FinishTracking(this);
			Serialize();
		}

		private void ShowUpgradeProgressBar(UpspeedableProcess process)
		{
			if (_upgradeProgressBar == null)
			{
				_upgradeProgressBar = _overlayManager.CreateOverlay<ProgressOverlay>(base.gameObject, OverlayType.Progress);
			}
			_upgradeProgressBar.Initialize(process);
			_gridTileIconManager.SetIcon<GridTileIcon>(GridTileIconType.UpgradeArrow);
		}

		private void RemoveUpgradeProgressBar()
		{
			if (_upgradeProgressBar != null)
			{
				_upgradeProgressBar.Remove();
				_upgradeProgressBar = null;
				_gridTileIconManager.RemoveIcon(GridTileIconType.UpgradeArrow);
			}
		}

		private void StartUpgradeRoutine()
		{
			StopUpgradeRoutine();
			_routineRunner.StartCoroutine(_upgradeBehaviourRoutine = UpgradeBehaviour());
		}

		private void StopUpgradeRoutine()
		{
			if (_routineRunner != null && _upgradeBehaviourRoutine != null)
			{
				_routineRunner.StopCoroutine(_upgradeBehaviourRoutine);
				_upgradeBehaviourRoutine = null;
			}
		}

		private void OnToggleLevelIcon(bool active)
		{
			UpdateLevelIcon(CurrentLevel);
		}

		private void OnGameStateValueChanged(string key, object oldValue, object newValue)
		{
			if (key == "EarnedBalance" || key == "GiftedBalance")
			{
				UpdateUpgradeSign();
			}
		}

		private IEnumerator UpgradeBehaviour()
		{
			if (_upgradeUpspeedableProcess == null)
			{
				_upgradeUpspeedableProcess = new UpspeedableProcess(_timing, _multipliers, _gameState, UpgradeDuration, CurrenciesSpentReason.SpeedupUpgrade);
			}
			ShowUpgradeProgressBar(_upgradeUpspeedableProcess);
			yield return _upgradeUpspeedableProcess;
			if (_upgradeUpspeedableProcess.Cancelled)
			{
				_craneManager.FinishTracking(this);
			}
			else
			{
				PerformUpgrade(_upgradeUpspeedableProcess.EndTime);
			}
			_upgradeBehaviourRoutine = null;
			_upgradeUpspeedableProcess = null;
		}

		public override StorageDictionary Serialize()
		{
			StorageDictionary storageDictionary = base.Serialize();
			storageDictionary.Set("level", CurrentLevel);
			storageDictionary.SetOrRemoveStorable("UpgradeUpspeedableProcess", _upgradeUpspeedableProcess, _upgradeUpspeedableProcess == null);
			return storageDictionary;
		}

		protected override void Deserialize(StorageDictionary storage)
		{
			base.Deserialize(storage);
			CurrentLevel = storage.Get("level", 0);
			if (storage.Contains("UpgradeUpspeedableProcess"))
			{
				_upgradeUpspeedableProcess = new UpspeedableProcess(storage.GetStorageDict("UpgradeUpspeedableProcess"), _timing, _multipliers, _gameState);
			}
		}

		[CompilerGenerated]
		private void _003CStartUpgrade_003Eg__UpgradeAction_007C42_0(Currency extraCost)
		{
			Currencies cost = new Currencies(UpgradeCost);
			if (extraCost.IsValid)
			{
				cost += extraCost;
			}
			_gameState.SpendCurrencies(cost, base.BuildingProperties.GoldCostAtMax, allowShopPopup: true, CurrenciesSpentReason.BuildingUpgrade, delegate(bool success, Currencies spent)
			{
				if (success)
				{
					StartUpgrading();
					_gameStats.AddBuildingUpgradeStarted(base.BuildingProperties, CurrentLevel + 1, cost);
				}
			});
		}
	}
}
