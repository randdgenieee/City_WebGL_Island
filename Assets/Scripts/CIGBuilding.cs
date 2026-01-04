using CIG;

public abstract class CIGBuilding : HappinessBuilding
{
	private const string WasBuiltWithCashKey = "WasBuiltWithCash";

	public bool WasBuiltWithCash
	{
		get;
		private set;
	}

	public override bool InfoRequiresFrequentRefresh
	{
		get
		{
			if (base.IsUpgrading || base.State == BuildingState.Constructing || base.State == BuildingState.Demolishing)
			{
				return true;
			}
			if (CanActivate || base.State == BuildingState.Preview)
			{
				return false;
			}
			return base.InfoRequiresFrequentRefresh;
		}
	}

	public override int Happiness
	{
		get
		{
			return base.Happiness;
		}
		protected set
		{
			int num = value - base.Happiness;
			base.Happiness = value;
			if (num != 0)
			{
				_islandState.AddHappiness(num);
			}
		}
	}

	protected virtual bool UseGreyShader
	{
		get
		{
			if (base.BuildingProperties.Activatable)
			{
				return base.CurrentLevel == 0;
			}
			return false;
		}
	}

	public void Initialize(StorageDictionary storage, IsometricGrid isometricGrid, IslandsManagerView islandsManagerView, WorldMap worldMap, BuildingWarehouseManager buildingWarehouseManager, CraneManager craneManager, GameStats gameStats, GameState gameState, PopupManager popupManager, Multipliers multipliers, Timing timing, RoutineRunner routineRunner, OverlayManager overlayManager, GridTileProperties properties, CIGIslandState islandState, bool isCashBuilding)
	{
		Initialize(storage, isometricGrid, islandsManagerView, worldMap, buildingWarehouseManager, craneManager, gameStats, gameState, popupManager, multipliers, timing, routineRunner, properties, overlayManager, islandState);
		WasBuiltWithCash = isCashBuilding;
		if (UseGreyShader)
		{
			SetGreyShader(enabled: true);
		}
	}

	protected override void OnBuildingLongPressed()
	{
		if (base.State == BuildingState.Normal && !base.IsUpgrading && base.BuildingProperties.Movable)
		{
			_popupManager.RequestPopup(new BuildConfirmMovePopupRequest(this, moveCameraToTarget: false));
		}
		else
		{
			base.OnBuildingLongPressed();
		}
	}

	protected override void OnBuildingPressed()
	{
		if (base.State == BuildingState.Normal && !base.IsUpgrading)
		{
			_popupManager.RequestPopup(new BuildingPopupRequest(this, CanActivate ? BuildingPopupContent.Activate : BuildingPopupContent.Upgrade));
		}
		else
		{
			base.OnBuildingPressed();
		}
	}

	protected override void OnUpgradeStarted()
	{
		if (CanActivate && base.BuildingProperties.ConstructionReward.IsMatchingName("XP") && !_gameState.ReachedMaxLevel)
		{
			_gameState.EarnCurrencies(base.BuildingProperties.ConstructionReward, CurrenciesEarnedReason.BuildingConstruction, new FlyingCurrenciesData(this));
			_plingManager.ShowCurrencyPlings(_timing, base.BuildingProperties.ConstructionReward, Clip.CollectXP);
		}
		base.OnUpgradeStarted();
	}

	protected override void OnUpgradeCompleted(double completionTime)
	{
		base.OnUpgradeCompleted(completionTime);
		if (base.CurrentLevel == 10)
		{
			_gameStats.AddNumberOfLevel10Upgrades(1);
		}
		if (base.CurrentLevel == base.BuildingProperties.MaximumLevel && base.BuildingProperties.MaximumLevel > 1)
		{
			_gameStats.AddNumberOfMaxLevelBuildings(1);
		}
		if (!base.BuildingProperties.Activatable || (base.BuildingProperties.Activatable && base.CurrentLevel > 1))
		{
			_gameStats.AddNumberOfUpgrades(1);
		}
		if (base.BuildingProperties.Activatable && base.CurrentLevel == 1)
		{
			SetGreyShader(UseGreyShader);
		}
	}

	private void SetGreyShader(bool enabled)
	{
		SetMaterial(SingletonMonobehaviour<MaterialAssetCollection>.Instance.GetAsset(enabled ? MaterialType.SpriteGreyScale : MaterialType.SpriteTransparent));
	}

	public override StorageDictionary Serialize()
	{
		StorageDictionary storageDictionary = base.Serialize();
		storageDictionary.Set("WasBuiltWithCash", WasBuiltWithCash);
		return storageDictionary;
	}

	protected override void Deserialize(StorageDictionary storage)
	{
		base.Deserialize(storage);
		WasBuiltWithCash = storage.Get("WasBuiltWithCash", defaultValue: false);
	}
}
