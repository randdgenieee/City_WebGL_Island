using CIG;
using CIG.Translation;
using System;
using System.Collections;
using UnityEngine;

public class CIGResidentialBuilding : CIGBuilding
{
	private int _currentMaxPeople;

	private int _currentPeople;

	private IEnumerator _residentialBehaviourRoutine;

	private const string PeopleKey = "people";

	private const string MaxPeopleKey = "maxPeople";

	public ResidentialBuildingProperties ResidentialProperties
	{
		get;
		private set;
	}

	public int MaxPeople
	{
		get
		{
			return _currentMaxPeople;
		}
		protected set
		{
			int num = value - _currentMaxPeople;
			_currentMaxPeople = value;
			if (num != 0)
			{
				_islandState.AddHousing(num);
			}
		}
	}

	public int UnboostedMaxPeople => ResidentialProperties.PeoplePerLevel[base.CurrentLevel];

	public int People
	{
		get
		{
			return _currentPeople;
		}
		protected set
		{
			int num = value - _currentPeople;
			_currentPeople = value;
			if (num != 0)
			{
				_islandState.AddPopulation(num);
				if (base.gameObject.activeInHierarchy)
				{
					TextStyleType textStyle = (num > 0) ? TextStyleType.GreenOutlined22 : TextStyleType.RedOutlined22;
					_plingManager.ShowPling(PlingType.Citizen, Localization.Format((num > 0) ? Localization.Key("plus") : Localization.Key("minus"), Localization.Integer(Math.Abs(num))), textStyle);
				}
			}
		}
	}

	public int ExtraPeopleNextLevel
	{
		get
		{
			if (base.CurrentLevel >= 0 && base.CurrentLevel < ResidentialProperties.PeoplePerLevel.Count - 1)
			{
				return ResidentialProperties.PeoplePerLevel[base.CurrentLevel + 1] - ResidentialProperties.PeoplePerLevel[base.CurrentLevel];
			}
			return 0;
		}
	}

	protected virtual double UnchangedWaitTime => UnityEngine.Random.Range(2f, 30f);

	protected virtual double ChangedWaitTime => UnityEngine.Random.Range(1f, 3f);

	public override bool InfoRequiresFrequentRefresh
	{
		get
		{
			if (base.State == BuildingState.Preview || base.State != BuildingState.Normal || base.IsUpgrading)
			{
				return base.InfoRequiresFrequentRefresh;
			}
			return true;
		}
	}

	public override Currency UpgradeCost => base.UpgradeCost.Multiply(_multipliers.GetMultiplier(MultiplierType.ResidentialCashCostUpgrade), RoundingMethod.Nearest);

	public override Currency UpgradeInstantCost => base.UpgradeInstantCost.Multiply(_multipliers.GetMultiplier(MultiplierType.ResidentialInstantGoldCostUpgrade), RoundingMethod.Nearest);

	public override void Initialize(StorageDictionary storage, IsometricGrid isometricGrid, IslandsManagerView islandsManagerView, WorldMap worldMap, BuildingWarehouseManager buildingWarehouseManager, CraneManager craneManager, GameStats gameStats, GameState gameState, PopupManager popupManager, Multipliers multipliers, Timing timing, RoutineRunner routineRunner, GridTileProperties properties, OverlayManager overlayManager, CIGIslandState islandState, GridIndex? index = default(GridIndex?))
	{
		ResidentialProperties = (ResidentialBuildingProperties)properties;
		base.Initialize(storage, isometricGrid, islandsManagerView, worldMap, buildingWarehouseManager, craneManager, gameStats, gameState, popupManager, multipliers, timing, routineRunner, properties, overlayManager, islandState, index);
		if (base.State == BuildingState.Normal)
		{
			StartResidentialBehavior();
		}
	}

	protected override void OnDestroy()
	{
		StopResidentialBehavior();
		base.OnDestroy();
	}

	protected bool UpdatePeople()
	{
		int num = People;
		if (base.BuildingProperties.CheckForRoad && !HasRoad)
		{
			num--;
		}
		else if (_islandState.Population > _islandState.AvailableHappiness)
		{
			num--;
		}
		else if (_islandState.Population < _islandState.AvailableHappiness)
		{
			num++;
		}
		num = Math.Max(Math.Min(num, MaxPeople), 0);
		if (num != People)
		{
			People = num;
			Serialize();
			return true;
		}
		return false;
	}

	protected override void OnConstructionFinished()
	{
		SetMaxPeople();
		base.OnConstructionFinished();
		StartResidentialBehavior();
	}

	protected override void OnDemolishStarted()
	{
		base.OnDemolishStarted();
		StopResidentialBehavior();
		int num3 = MaxPeople = (People = 0);
	}

	protected override void OnDemolishCancelled()
	{
		SetMaxPeople();
		base.OnDemolishCancelled();
		StartResidentialBehavior();
	}

	protected override void OnUpgradeCompleted(double completionTime)
	{
		base.OnUpgradeCompleted(completionTime);
		SetMaxPeople();
	}

	protected override void OnBoostedPercentageChanged()
	{
		base.OnBoostedPercentageChanged();
		SetMaxPeople();
		Serialize();
	}

	private void SetMaxPeople()
	{
		int num = ResidentialProperties.PeoplePerLevel[base.CurrentLevel];
		int num3 = MaxPeople = num + Mathf.CeilToInt((float)(base.ClampedBoostPercentage * num) / 100f);
		if (People > num3)
		{
			People = num3;
		}
	}

	private IEnumerator ResidentialBehaviour()
	{
		bool flag = true;
		while (true)
		{
			yield return new WaitForGameTimeSeconds(_timing, flag ? ChangedWaitTime : UnchangedWaitTime);
			flag = UpdatePeople();
		}
	}

	private void StartResidentialBehavior()
	{
		if (_residentialBehaviourRoutine != null)
		{
			_routineRunner.StopCoroutine(_residentialBehaviourRoutine);
		}
		_routineRunner.StartCoroutine(_residentialBehaviourRoutine = ResidentialBehaviour());
	}

	private void StopResidentialBehavior()
	{
		if (_routineRunner != null && _residentialBehaviourRoutine != null)
		{
			_routineRunner.StopCoroutine(_residentialBehaviourRoutine);
			_residentialBehaviourRoutine = null;
		}
	}

	public override StorageDictionary Serialize()
	{
		StorageDictionary storageDictionary = base.Serialize();
		storageDictionary.Set("people", People);
		storageDictionary.Set("maxPeople", MaxPeople);
		return storageDictionary;
	}

	protected override void Deserialize(StorageDictionary storage)
	{
		base.Deserialize(storage);
		_currentPeople = storage.Get("people", 0);
		_currentMaxPeople = storage.Get("maxPeople", 0);
	}
}
