using CIG;
using UnityEngine;

public class ReadOnlyWrapper : GridTile
{
	public enum BuildingType
	{
		Unknown = -1,
		Residential,
		Community,
		Commercial,
		Decoration,
		Landmark,
		Scenery
	}

	[SerializeField]
	private GridTileIconManager _gridTileIconManager;

	[SerializeField]
	private CurrencyAnimationSource _currencyAnimationSource;

	private VisitingUpgradableBuildingView _visitingUpgradableBuildingView;

	public GridTileIconManager GridTileIconManager => _gridTileIconManager;

	public BuildingType Type
	{
		get;
		private set;
	}

	public void Initialize(StorageDictionary storage, GridTile original, IsometricGrid isometricGrid, IslandsManagerView islandsManagerView, WorldMap worldMap, BuildingWarehouseManager buildingWarehouseManager, CraneManager craneManager, GameStats gameStats, GameState gameState, PopupManager popupManager, Multipliers multipliers, Timing timing, RoutineRunner routineRunner, OverlayManager overlayManager, GridTileProperties properties, CIGIslandState islandState)
	{
		_gridTileIconManager.Initialize(overlayManager);
		Initialize(storage, isometricGrid, islandsManagerView, worldMap, buildingWarehouseManager, craneManager, gameStats, gameState, popupManager, multipliers, timing, routineRunner, properties, overlayManager, islandState);
		CopyGridTilePrefab(original);
		CopyBuildingBehaviour(storage, original);
		_currencyAnimationSource.Initialize(this);
	}

	public override void UpdateSortingOrder()
	{
		base.UpdateSortingOrder();
		if (_visitingUpgradableBuildingView != null)
		{
			_visitingUpgradableBuildingView.UpdateSortingOrder(base.SpriteRenderer.sortingOrder);
		}
	}

	public bool IsOfType(params BuildingType[] buildingTypes)
	{
		if (buildingTypes != null)
		{
			int i = 0;
			for (int num = buildingTypes.Length; i < num; i++)
			{
				if (buildingTypes[i] == Type)
				{
					return true;
				}
			}
		}
		return false;
	}

	private void CopyBuildingBehaviour(StorageDictionary storage, GridTile gridTile)
	{
		if ((object)gridTile == null)
		{
			goto IL_007e;
		}
		CIGLandmarkBuilding cIGLandmarkBuilding;
		if ((object)(cIGLandmarkBuilding = (gridTile as CIGLandmarkBuilding)) == null)
		{
			if (!(gridTile is CIGCommercialBuilding))
			{
				if (!(gridTile is CIGCommunityBuilding))
				{
					if (!(gridTile is CIGResidentialBuilding))
					{
						if (!(gridTile is CIGDecoration))
						{
							goto IL_007e;
						}
						Type = BuildingType.Decoration;
					}
					else
					{
						Type = BuildingType.Residential;
					}
				}
				else
				{
					Type = BuildingType.Community;
				}
			}
			else
			{
				Type = BuildingType.Commercial;
			}
		}
		else
		{
			CIGLandmarkBuilding cIGLandmarkBuilding2 = cIGLandmarkBuilding;
			Type = BuildingType.Landmark;
			base.gameObject.AddComponent<VisitingLandmarkView>().Initialize(storage, this, _isometricGrid, cIGLandmarkBuilding2.LandmarkParticles);
		}
		goto IL_0085;
		IL_0085:
		UpgradableBuilding upgradableBuilding;
		if ((object)(upgradableBuilding = (gridTile as UpgradableBuilding)) != null && IsOfType(BuildingType.Commercial, BuildingType.Community, BuildingType.Residential, BuildingType.Landmark))
		{
			_visitingUpgradableBuildingView = base.gameObject.AddComponent<VisitingUpgradableBuildingView>();
			_visitingUpgradableBuildingView.Initialize(storage, this, upgradableBuilding.UpgradeSignPrefab);
		}
		return;
		IL_007e:
		Type = BuildingType.Unknown;
		goto IL_0085;
	}
}
