using CIG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CIGLandmarkBuilding : CIGBuilding
{
	public static readonly Color ToColor = new Color(0.306f, 0.024f, 0.451f, 0f);

	public const float ParticleMinWaitTime = 20f;

	public const float ParticleMaxWaitTime = 30f;

	public const float BuildingParticlesDelay = 0.5f;

	public const float PulseDuration = 2f;

	[SerializeField]
	private BoostUnderlay _boostUnderlay;

	[SerializeField]
	private BuildingParticles _landmarkParticles;

	private HashSet<string> _boostedBuildingsIDs = new HashSet<string>();

	private const string CurrentBoostPercentageKey = "CurrentBoostPercentage";

	public const string CurrentBoostTilesKey = "CurrentBoostTiles";

	private const string BoostedBuildingsIDsKey = "BoostedBuildingsIDs";

	public LandmarkBuildingProperties LandmarkProperties
	{
		get;
		private set;
	}

	public BuildingParticles LandmarkParticles => _landmarkParticles;

	public int BoostPercentage
	{
		get;
		private set;
	}

	public int BoostTiles
	{
		get;
		private set;
	}

	public int ExtraBoostPercentageNextLevel => GetBoostPercentageForLevel(base.CurrentLevel + 1) - GetBoostPercentageForLevel(base.CurrentLevel);

	public int ExtraBoostTilesNextLevel => GetBoostTilesForLevel(base.CurrentLevel + 1) - GetBoostTilesForLevel(base.CurrentLevel);

	private void Awake()
	{
		_boostUnderlay.gameObject.SetActive(value: false);
	}

	public override void Initialize(StorageDictionary storage, IsometricGrid isometricGrid, IslandsManagerView islandsManagerView, WorldMap worldMap, BuildingWarehouseManager buildingWarehouseManager, CraneManager craneManager, GameStats gameStats, GameState gameState, PopupManager popupManager, Multipliers multipliers, Timing timing, RoutineRunner routineRunner, GridTileProperties properties, OverlayManager overlayManager, CIGIslandState islandState, GridIndex? index = default(GridIndex?))
	{
		LandmarkProperties = (LandmarkBuildingProperties)properties;
		base.Initialize(storage, isometricGrid, islandsManagerView, worldMap, buildingWarehouseManager, craneManager, gameStats, gameState, popupManager, multipliers, timing, routineRunner, properties, overlayManager, islandState, index);
		_landmarkParticles.Initialize(this);
		_landmarkParticles.PlayedEvent += OnParticlesPlayed;
		if (base.State == BuildingState.Normal)
		{
			Subscribe();
			if (HasRoad)
			{
				SetBuildingsInRangeBoosted();
				_landmarkParticles.Play(20f, 30f);
			}
		}
		_boostUnderlay.SetData(base.Properties.Size.u + BoostTiles * 2, base.Properties.Size.v);
	}

	protected override void OnDestroy()
	{
		Unsubscribe();
		if (_landmarkParticles != null)
		{
			_landmarkParticles.PlayedEvent -= OnParticlesPlayed;
		}
		base.OnDestroy();
	}

	public override void OnAdjacentRoadsChanged(Road road)
	{
		base.OnAdjacentRoadsChanged(road);
		if (base.State == BuildingState.Normal)
		{
			if (HasRoad)
			{
				SetBuildingsInRangeBoosted();
				_landmarkParticles.Play(20f, 30f);
			}
			else
			{
				SetBoostedBuildingsNotBoosted();
				_landmarkParticles.Stop();
			}
		}
	}

	public override void UpdateSortingOrder()
	{
		base.UpdateSortingOrder();
		_landmarkParticles.SetSortingOrder(base.SpriteRenderer.sortingOrder);
	}

	protected override void OnConstructionFinished()
	{
		Subscribe();
		BoostPercentage = GetBoostPercentageForLevel(base.CurrentLevel);
		BoostTiles = GetBoostTilesForLevel(base.CurrentLevel);
		Serialize();
		if (HasRoad)
		{
			SetBuildingsInRangeBoosted();
			_landmarkParticles.Play(20f, 30f);
		}
		_boostUnderlay.SetData(base.Properties.Size.u + BoostTiles * 2, base.Properties.Size.v);
		base.OnConstructionFinished();
	}

	protected override void OnDemolishStarted()
	{
		base.OnDemolishStarted();
		Unsubscribe();
		_landmarkParticles.Stop();
		SetBoostedBuildingsNotBoosted();
	}

	protected override void OnDemolishCancelled()
	{
		Subscribe();
		if (HasRoad)
		{
			SetBuildingsInRangeBoosted();
			_landmarkParticles.Play(20f, 30f);
		}
		base.OnDemolishCancelled();
	}

	protected override void OnUpgradeCompleted(double completionTime)
	{
		base.OnUpgradeCompleted(completionTime);
		if (base.CurrentLevel >= 0)
		{
			if (base.CurrentLevel < LandmarkProperties.BoostTilesPerLevel.Count)
			{
				BoostTiles = LandmarkProperties.BoostTilesPerLevel[base.CurrentLevel];
				_boostUnderlay.SetData(base.Properties.Size.u + BoostTiles * 2, base.Properties.Size.v);
				SetBuildingsInRangeBoosted();
			}
			if (base.CurrentLevel < LandmarkProperties.BoostPercentagePerLevel.Count)
			{
				int num = LandmarkProperties.BoostPercentagePerLevel[base.CurrentLevel];
				int amount = num - BoostPercentage;
				foreach (string boostedBuildingsID in _boostedBuildingsIDs)
				{
					Building building = _isometricGrid.GetGridTileByUniqueIdentifier(boostedBuildingsID) as Building;
					if (building != null)
					{
						building.AddBoostPercentage(amount);
					}
					else
					{
						UnityEngine.Debug.LogWarning("Invalid building id: " + boostedBuildingsID);
					}
				}
				BoostPercentage = num;
			}
		}
	}

	protected override void OnHiddenChanged(bool hidden)
	{
		base.OnHiddenChanged(hidden);
		_boostUnderlay.gameObject.SetActive(hidden);
		if (hidden)
		{
			_landmarkParticles.Stop();
		}
		else if (base.State == BuildingState.Normal && HasRoad)
		{
			_landmarkParticles.Play(20f, 30f);
		}
	}

	private void OnGridTileAdded(GridTile tile)
	{
		if (HasRoad)
		{
			Building building = tile as Building;
			if (building != null && IsInRange(building.Index, building.Properties.Size))
			{
				SetBuildingBoosted(building);
			}
		}
	}

	private void OnGridTileRemoved(GridTile tile)
	{
		if (!HasRoad)
		{
			return;
		}
		Building building = tile as Building;
		if (building != null)
		{
			if (building == this)
			{
				SetBoostedBuildingsNotBoosted();
			}
			else
			{
				SetBuildingNotBoosted(building);
			}
		}
	}

	private void Subscribe()
	{
		_isometricGrid.GridTileAddedEvent += OnGridTileAdded;
		_isometricGrid.GridTileRemovedEvent += OnGridTileRemoved;
	}

	private void Unsubscribe()
	{
		_isometricGrid.GridTileAddedEvent -= OnGridTileAdded;
		_isometricGrid.GridTileRemovedEvent -= OnGridTileRemoved;
	}

	private bool IsInRange(GridIndex index, GridSize size)
	{
		int num = base.Index.u + BoostTiles + size.u - 1;
		int num2 = base.Index.v + BoostTiles + size.v - 1;
		int num3 = base.Index.u - base.Properties.Size.u - BoostTiles + 1;
		int num4 = base.Index.v - base.Properties.Size.v - BoostTiles + 1;
		if (index.u >= num3 && index.u <= num && index.v >= num4)
		{
			return index.v <= num2;
		}
		return false;
	}

	private void SetBuildingsInRangeBoosted()
	{
		foreach (Building item in _isometricGrid.FindInRange<Building>(this, BoostTiles))
		{
			SetBuildingBoosted(item);
		}
	}

	private void SetBuildingBoosted(Building building)
	{
		if (!(building == null) && !(building == this) && !(building is CIGLandmarkBuilding) && _boostedBuildingsIDs.Add(building.UniqueIdentifier))
		{
			building.AddBoostPercentage(BoostPercentage);
			Serialize();
		}
	}

	private void SetBoostedBuildingsNotBoosted()
	{
		foreach (string item in new HashSet<string>(_boostedBuildingsIDs))
		{
			Building building = _isometricGrid.GetGridTileByUniqueIdentifier(item) as Building;
			if (building != null)
			{
				SetBuildingNotBoosted(building);
			}
			else
			{
				UnityEngine.Debug.LogWarning("Invalid building id: " + item);
			}
		}
	}

	private void SetBuildingNotBoosted(Building building)
	{
		if (!(building == null) && !(building == this) && _boostedBuildingsIDs.Remove(building.UniqueIdentifier))
		{
			building.AddBoostPercentage(-BoostPercentage);
			Serialize();
		}
	}

	private IEnumerator PlayOtherBuildingsParticlesRoutine()
	{
		yield return new WaitForSeconds(0.5f);
		foreach (string boostedBuildingsID in _boostedBuildingsIDs)
		{
			GridTile gridTileByUniqueIdentifier = _isometricGrid.GetGridTileByUniqueIdentifier(boostedBuildingsID);
			if (gridTileByUniqueIdentifier != null)
			{
				gridTileByUniqueIdentifier.PulseColor(ToColor, 2f);
			}
			else
			{
				UnityEngine.Debug.LogWarning("Invalid building id: " + boostedBuildingsID);
			}
		}
	}

	private int GetBoostPercentageForLevel(int level)
	{
		int count = LandmarkProperties.BoostPercentagePerLevel.Count;
		if (level >= 0 && level < count)
		{
			return LandmarkProperties.BoostPercentagePerLevel[level];
		}
		if (level >= count)
		{
			return LandmarkProperties.BoostPercentagePerLevel[count - 1];
		}
		return LandmarkProperties.BoostPercentagePerLevel[0];
	}

	private int GetBoostTilesForLevel(int level)
	{
		int count = LandmarkProperties.BoostTilesPerLevel.Count;
		if (level >= 0 && level < count)
		{
			return LandmarkProperties.BoostTilesPerLevel[level];
		}
		if (level >= count)
		{
			return LandmarkProperties.BoostTilesPerLevel[count - 1];
		}
		return LandmarkProperties.BoostTilesPerLevel[0];
	}

	private void OnParticlesPlayed()
	{
		PulseColor(ToColor, 2f);
		StartCoroutine(PlayOtherBuildingsParticlesRoutine());
	}

	public override StorageDictionary Serialize()
	{
		StorageDictionary storageDictionary = base.Serialize();
		storageDictionary.Set("CurrentBoostPercentage", BoostPercentage);
		storageDictionary.Set("CurrentBoostTiles", BoostTiles);
		storageDictionary.Set("BoostedBuildingsIDs", _boostedBuildingsIDs);
		return storageDictionary;
	}

	protected override void Deserialize(StorageDictionary storage)
	{
		base.Deserialize(storage);
		BoostPercentage = storage.Get("CurrentBoostPercentage", GetBoostPercentageForLevel(base.CurrentLevel));
		BoostTiles = storage.Get("CurrentBoostTiles", GetBoostTilesForLevel(base.CurrentLevel));
		_boostedBuildingsIDs = storage.GetHashSet<string>("BoostedBuildingsIDs");
	}
}
