using System;
using UnityEngine;

namespace CIG
{
    public class Builder : MonoBehaviour
    {
        public bool IsBuilding { get; private set; }

        public PreviewBuilding PreviewTile { get; private set; }

        public CIGBuilding CurrentBuilding
        {
            get
            {
                return this.PreviewTile.Building;
            }
        }

        public static bool TilesHidden
        {
            get
            {
                return Builder._tilesHidden;
            }
            set
            {
                if (value != Builder._tilesHidden)
                {
                    Builder._tilesHidden = value;
                    Builder.FireTilesHiddenChangedEvent();
                }
            }
        }

        public event Builder.StartBuildingEventHandler StartBuildingEvent;

        private void FireStartBuildingEvent(SurfaceType surfaceType)
        {
            Builder.StartBuildingEventHandler startBuildingEvent = this.StartBuildingEvent;
            if (startBuildingEvent == null)
            {
                return;
            }
            startBuildingEvent(surfaceType);
        }

        public event Builder.StopBuildingEventHandler StopBuildingEvent;

        private void FireStopBuildingEvent()
        {
            Builder.StopBuildingEventHandler stopBuildingEvent = this.StopBuildingEvent;
            if (stopBuildingEvent == null)
            {
                return;
            }
            stopBuildingEvent();
        }

        public event Builder.BuildingBuiltEventHandler BuildingBuiltEvent;

        private void FireBuildingBuiltEvent(GridTile building, bool isNewBuilding)
        {
            Builder.BuildingBuiltEventHandler buildingBuiltEvent = this.BuildingBuiltEvent;
            if (buildingBuiltEvent == null)
            {
                return;
            }
            buildingBuiltEvent(building, isNewBuilding);
        }

        public static event Builder.TilesHiddenChangedEventHandler TilesHiddenChangedEvent;

        private static void FireTilesHiddenChangedEvent()
        {
            Builder.TilesHiddenChangedEventHandler tilesHiddenChangedEvent = Builder.TilesHiddenChangedEvent;
            if (tilesHiddenChangedEvent == null)
            {
                return;
            }
            tilesHiddenChangedEvent(Builder.TilesHidden);
        }

        public void Initialize(IslandCameraOperator cameraOperator, IsometricGrid isometricGrid, IslandInput islandInput, IslandsManagerView islandsManagerView, WorldMap worldMap, BuildingWarehouseManager buildingWarehouseManager, CraneManager craneManager, GameStats gameStats, GameState gameState, PopupManager popupManager, Multipliers multipliers, Timing timing, RoutineRunner routineRunner, OverlayManager overlayManager, CIGIslandState islandState)
        {
            this._cameraOperator = cameraOperator;
            this._isometricGrid = isometricGrid;
            this._islandInput = islandInput;
            this._islandsManagerView = islandsManagerView;
            this._worldMap = worldMap;
            this._buildingWarehouseManager = buildingWarehouseManager;
            this._craneManager = craneManager;
            this._gameStats = gameStats;
            this._gameState = gameState;
            this._popupManager = popupManager;
            this._multipliers = multipliers;
            this._timing = timing;
            this._routineRunner = routineRunner;
            this._overlayManager = overlayManager;
            this._islandState = islandState;
        }

        private void OnDestroy()
        {
            if (this._overlayManager != null)
            {
                this._overlayManager.EnableInteractionRequest(this);
                this._overlayManager = null;
            }
            if (this._islandInput != null)
            {
                this._islandInput.PopDisableIslandInteractionRequest(this);
            }
            this._cameraOperator = null;
            this._isometricGrid = null;
        }

        public void BeginBuild(BuildingProperties properties, bool moveCameraToTarget, bool isNewBuilding, bool isCashBuilding)
        {
            if (this.IsBuilding)
            {
                UnityEngine.Debug.LogWarning("Can't begin building because this Builder is already building.");
                return;
            }
            this._isNewBuilding = isNewBuilding;
            this._overlayManager.DisableInteractionRequest(this);
            this._islandInput.PushDisableIslandInteractionRequest(this);
            CIGBuilding cigbuilding = (CIGBuilding)UnityEngine.Object.Instantiate<GridTile>(SingletonMonobehaviour<BuildingsAssetCollection>.Instance.GetAsset(properties.BaseKey), this._isometricGrid.transform);
            cigbuilding.Initialize(new StorageDictionary(), this._isometricGrid, this._islandsManagerView, this._worldMap, this._buildingWarehouseManager, this._craneManager, this._gameStats, this._gameState, this._popupManager, this._multipliers, this._timing, this._routineRunner, this._overlayManager, properties, this._islandState, isCashBuilding);
            cigbuilding.name = properties.BaseKey;
            GridIndex gridIndex = this._isometricGrid.ConvertIslandCoordinateToGridIndex(this._cameraOperator.CameraToOperate.transform.position, false);
            if (gridIndex.isInvalid)
            {
                gridIndex.u = this._isometricGrid.Size.u / 2;
                gridIndex.v = this._isometricGrid.Size.v / 2;
            }
            GridIndex startIndex;
            if (!this.TryFindSpaceOnGrid(cigbuilding, gridIndex, false, false, out startIndex) && !this.TryFindSpaceOnGrid(cigbuilding, gridIndex, true, false, out startIndex) && !this.TryFindSpaceOnGrid(cigbuilding, gridIndex, true, true, out startIndex))
            {
                startIndex = gridIndex;
            }
            this.IsBuilding = true;
            this.PreviewTile = UnityEngine.Object.Instantiate<PreviewBuilding>(this._previewBuildingPrefab, this._isometricGrid.transform);
            this.PreviewTile.Initialize(this._isometricGrid, this._islandsManagerView, this._worldMap, this._buildingWarehouseManager, this._craneManager, this._gameStats, this._gameState, this._popupManager, this._multipliers, this._timing, this._routineRunner, this._overlayManager, cigbuilding, true, startIndex, this, this._islandState);
            if (moveCameraToTarget)
            {
                this._cameraOperator.ScrollTo(this.PreviewTile.transform.position, 0.6f);
            }
            this.FireStartBuildingEvent(this.PreviewTile.Properties.SurfaceType);
        }

        public void BeginMove(CIGBuilding building, bool moveCameraToBuilding)
        {
            if (this.IsBuilding)
            {
                UnityEngine.Debug.LogWarning("Can't moving building because this Builder is already building.");
                return;
            }
            building.TryStopPulseRoutine();
            this._isNewBuilding = false;
            this._overlayManager.DisableInteractionRequest(this);
            this._islandInput.PushDisableIslandInteractionRequest(this);
            this._originalIndex = building.Index;
            this._isometricGrid.RemoveAtIndex(this._originalIndex, building.Properties.Size, false);
            this.PreviewTile = UnityEngine.Object.Instantiate<PreviewBuilding>(this._previewBuildingPrefab, this._isometricGrid.transform);
            this.PreviewTile.Initialize(this._isometricGrid, this._islandsManagerView, this._worldMap, this._buildingWarehouseManager, this._craneManager, this._gameStats, this._gameState, this._popupManager, this._multipliers, this._timing, this._routineRunner, this._overlayManager, building, false, this._originalIndex, this, this._islandState);
            this.IsBuilding = true;
            if (moveCameraToBuilding)
            {
                this._cameraOperator.ScrollTo(this.PreviewTile.gameObject, 0.6f);
            }
            this.FireStartBuildingEvent(this.PreviewTile.Properties.SurfaceType);
        }

        public void UpdateMirrored(bool mirrored)
        {
            if (!this.IsBuilding)
            {
                return;
            }
            this.PreviewTile.UpdateMirrored(mirrored);
        }

        public void CancelBuild()
        {
            if (!this.IsBuilding)
            {
                return;
            }
            this._overlayManager.EnableInteractionRequest(this);
            this._islandInput.PopDisableIslandInteractionRequest(this);
            if (this.PreviewTile.Status == GridTile.GridTileStatus.Moving)
            {
                GridTile gridTile = this.PreviewTile.CancelMove();
                if (!this.BuildAtInternal(gridTile, this._originalIndex, gridTile.Mirrored, false, true))
                {
                    UnityEngine.Debug.LogWarning("[CancelBuild] Can't place preview building back at its original index!");
                    gridTile.DestroyTile();
                }
            }
            this.PreviewTile.CancelBuild();
            this.PreviewTile = null;
            this.IsBuilding = false;
            this._originalIndex = GridIndex.invalid;
            this.FireStopBuildingEvent();
        }

        public CIGBuilding FinishBuild(Action<CIGBuilding> onAnimationFinished = null)
        {
            if (!this.IsBuilding)
            {
                return null;
            }
            this._overlayManager.EnableInteractionRequest(this);
            this._islandInput.PopDisableIslandInteractionRequest(this);
            return this.PreviewTile.FinishBuild(delegate (CIGBuilding tile)
            {
                if (!this.BuildAtInternal(tile, tile.Index, tile.Mirrored, false, true))
                {
                    UnityEngine.Debug.LogWarning("[FinishBuild] Can't place preview building back at the given index!");
                    tile.DestroyTile();
                }
                this.IsBuilding = false;
                this._originalIndex = GridIndex.invalid;
                this.FireStopBuildingEvent();
                Action<CIGBuilding> onAnimationFinished2 = onAnimationFinished;
                if (onAnimationFinished2 == null)
                {
                    return;
                }
                onAnimationFinished2(tile);
            });
        }

        public bool BuildAt(GridTile prefab, GridTileProperties properties, GridIndex index, bool mirrored, bool forced, bool serialize)
        {
            GridTile gridTile = UnityEngine.Object.Instantiate<GridTile>(prefab);
            gridTile.Initialize(new StorageDictionary(), this._isometricGrid, this._islandsManagerView, this._worldMap, this._buildingWarehouseManager, this._craneManager, this._gameStats, this._gameState, this._popupManager, this._multipliers, this._timing, this._routineRunner, properties, this._overlayManager, this._islandState, null);
            gridTile.name = prefab.name;
            bool flag = this.BuildAtInternal(gridTile, index, mirrored, forced, serialize);
            if (!flag && gridTile.Status == GridTile.GridTileStatus.Moving)
            {
                flag = this.BuildAtInternal(gridTile, this._originalIndex, mirrored, forced, serialize);
            }
            if (!flag)
            {
                gridTile.DestroyTile();
            }
            return flag;
        }

        public bool CanBuild()
        {
            return this.CanBuildTile(this.PreviewTile, this.PreviewTile.Index, false, false, false, false);
        }

        private bool BuildAtInternal(GridTile tile, GridIndex index, bool mirrored, bool forced, bool serialize)
        {
            if (!this.CanBuildTile(tile, index, mirrored, forced, false, false))
            {
                return false;
            }
            if (tile.Status == GridTile.GridTileStatus.Moving)
            {
                Analytics.BuildingMoved(tile.name);
            }
            tile.Mirrored = mirrored;
            tile.SpriteRenderer.color = Color.white;
            this._isometricGrid.AddTileAt(tile, index, serialize);
            this.FireBuildingBuiltEvent(tile, this._isNewBuilding);
            return true;
        }

        private bool CanBuildTile(GridTile tile, GridIndex index, bool mirrored, bool forced, bool ignoreLocked = false, bool ignoreOccupied = false)
        {
            if (!this._isometricGrid.IsWithinBounds(index, tile.Properties.Size))
            {
                return false;
            }
            if (mirrored && !tile.Properties.CanMirror)
            {
                return false;
            }
            int num = 0;
            for (int i = index.v; i > index.v - tile.Properties.Size.v; i--)
            {
                for (int j = index.u; j > index.u - tile.Properties.Size.u; j--)
                {
                    GridElement gridElement = this._isometricGrid[j, i];
                    if (gridElement.Tile != null && !ignoreOccupied)
                    {
                        if (!forced || tile is Scenery)
                        {
                            return false;
                        }
                        if (!(gridElement.Tile is Scenery))
                        {
                            UnityEngine.Debug.LogWarning(string.Format("In order to build {0} at {1}, we need to remove {2} at ({3},{4})", new object[]
                            {
                                tile.name,
                                index,
                                gridElement.Tile.name,
                                j,
                                i
                            }));
                        }
                        gridElement.Tile.DestroyTile();
                    }
                    if (!forced)
                    {
                        if (this.IsElementTypeCompatible(gridElement.Type, tile.Properties.SurfaceType))
                        {
                            num++;
                        }
                        if (!this.CanBuildTileOnElement(tile, gridElement, ignoreLocked))
                        {
                            return false;
                        }
                    }
                }
            }
            return forced || num == tile.Properties.Size.u * tile.Properties.Size.v;
        }

        private bool IsElementTypeCompatible(SurfaceType type, SurfaceType required)
        {
            return type == required || (required == SurfaceType.AnyTypeOfLand && type.IsLand());
        }

        private bool CanBuildTileOnElement(GridTile tile, GridElement element, bool ignoreLocked = false)
        {
            return (ignoreLocked || element.Unlocked) && this.IsElementTypeCompatible(element.Type, tile.Properties.SurfaceType);
        }

        private bool TryFindSpaceOnGrid(CIGBuilding clone, GridIndex indexHint, bool ignoreLocked, bool ignoreOccupied, out GridIndex index)
        {
            bool flag = false;
            index = indexHint;
            if (!this.CanBuildTile(clone, index, false, false, ignoreLocked, ignoreOccupied))
            {
                float num = (float)((this._isometricGrid.Size.u + this._isometricGrid.Size.v) * (this._isometricGrid.Size.u + this._isometricGrid.Size.v));
                for (int i = 1; i < this._isometricGrid.Size.u + this._isometricGrid.Size.v; i++)
                {
                    GridIndex gridIndex;
                    for (int j = -i; j <= i; j++)
                    {
                        gridIndex.u = indexHint.u + j;
                        gridIndex.v = indexHint.v - i;
                        float num2 = (float)((indexHint.u - gridIndex.u) * (indexHint.u - gridIndex.u) + (indexHint.v - gridIndex.v) * (indexHint.v - gridIndex.v));
                        if (num2 < num && this.CanBuildTile(clone, gridIndex, false, false, ignoreLocked, ignoreOccupied))
                        {
                            num = num2;
                            index = gridIndex;
                            flag = true;
                        }
                        gridIndex.v = indexHint.v + i;
                        num2 = (float)((indexHint.u - gridIndex.u) * (indexHint.u - gridIndex.u) + (indexHint.v - gridIndex.v) * (indexHint.v - gridIndex.v));
                        if (num2 < num && this.CanBuildTile(clone, gridIndex, false, false, ignoreLocked, ignoreOccupied))
                        {
                            num = num2;
                            index = gridIndex;
                            flag = true;
                        }
                    }
                    for (int k = -i + 1; k <= i - 1; k++)
                    {
                        gridIndex.v = indexHint.v + k;
                        gridIndex.u = indexHint.u - i;
                        float num2 = (float)((indexHint.u - gridIndex.u) * (indexHint.u - gridIndex.u) + (indexHint.v - gridIndex.v) * (indexHint.v - gridIndex.v));
                        if (num2 < num && this.CanBuildTile(clone, gridIndex, false, false, ignoreLocked, ignoreOccupied))
                        {
                            num = num2;
                            index = gridIndex;
                            flag = true;
                        }
                        gridIndex.u = indexHint.u + i;
                        num2 = (float)((indexHint.u - gridIndex.u) * (indexHint.u - gridIndex.u) + (indexHint.v - gridIndex.v) * (indexHint.v - gridIndex.v));
                        if (num2 < num && this.CanBuildTile(clone, gridIndex, false, false, ignoreLocked, ignoreOccupied))
                        {
                            num = num2;
                            index = gridIndex;
                            flag = true;
                        }
                    }
                    if (flag)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        [SerializeField]
        private PreviewBuilding _previewBuildingPrefab;

        private IslandCameraOperator _cameraOperator;

        private IsometricGrid _isometricGrid;

        private IslandInput _islandInput;

        private IslandsManagerView _islandsManagerView;

        private WorldMap _worldMap;

        private BuildingWarehouseManager _buildingWarehouseManager;

        private CraneManager _craneManager;

        private GameStats _gameStats;

        private GameState _gameState;

        private PopupManager _popupManager;

        private Multipliers _multipliers;

        private Timing _timing;

        private RoutineRunner _routineRunner;

        private CIGIslandState _islandState;

        private OverlayManager _overlayManager;

        private GridIndex _originalIndex = GridIndex.invalid;

        private bool _isNewBuilding;

        private static bool _tilesHidden;

        public delegate void StartBuildingEventHandler(SurfaceType surfaceType);

        public delegate void StopBuildingEventHandler();

        public delegate void BuildingBuiltEventHandler(GridTile building, bool isNewBuilding);

        public delegate void TilesHiddenChangedEventHandler(bool hidden);
    }
}
