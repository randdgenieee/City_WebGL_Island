using CIG;
using UnityEngine;
using UnityEngine.EventSystems;

public class AirshipPlatform : GridTile, IPointerClickHandler, IEventSystemHandler
{
	[SerializeField]
	private AirshipCinematic _cinematic;

	public override void Initialize(StorageDictionary storage, IsometricGrid isometricGrid, IslandsManagerView islandsManagerView, WorldMap worldMap, BuildingWarehouseManager buildingWarehouseManager, CraneManager craneManager, GameStats gameStats, GameState gameState, PopupManager popupManager, Multipliers multipliers, Timing timing, RoutineRunner routineRunner, GridTileProperties properties, OverlayManager overlayManager, CIGIslandState islandState, GridIndex? index = default(GridIndex?))
	{
		base.Initialize(storage, isometricGrid, islandsManagerView, worldMap, buildingWarehouseManager, craneManager, gameStats, gameState, popupManager, multipliers, timing, routineRunner, properties, overlayManager, islandState, index);
		_cinematic.Initialize(_islandsManagerView, _worldMap);
	}

	void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
	{
		_islandsManagerView.IslandsManager.CloseCurrentIsland();
	}

	public void StartCinematic()
	{
		Builder.TilesHidden = false;
		_cinematic.StartCinematic(IsometricIsland.Current);
	}
}
