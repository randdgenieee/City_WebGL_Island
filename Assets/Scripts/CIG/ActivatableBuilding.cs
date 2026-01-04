using System;

namespace CIG
{
    public class ActivatableBuilding : UpgradableBuilding
    {
        public virtual bool CanActivate
        {
            get
            {
                return base.BuildingProperties.Activatable && !this.Activated;
            }
        }

        public override bool CanUpgrade
        {
            get
            {
                if (!base.BuildingProperties.Activatable)
                {
                    return base.CanUpgrade;
                }
                return !this.Activated || base.CanUpgrade;
            }
        }

        public virtual bool Activated
        {
            get
            {
                return !base.BuildingProperties.Activatable || base.CurrentLevel > 0;
            }
        }

        public override int DisplayLevel
        {
            get
            {
                return base.CurrentLevel - (base.BuildingProperties.Activatable ? 1 : 0);
            }
        }

        protected override bool CanShowUpgradeSign
        {
            get
            {
                return base.CanShowUpgradeSign && (!base.BuildingProperties.Activatable || this.Activated);
            }
        }

        public override void Initialize(StorageDictionary storage, IsometricGrid isometricGrid, IslandsManagerView islandsManagerView, WorldMap worldMap, BuildingWarehouseManager buildingWarehouseManager, CraneManager craneManager, GameStats gameStats, GameState gameState, PopupManager popupManager, Multipliers multipliers, Timing timing, RoutineRunner routineRunner, GridTileProperties properties, OverlayManager overlayManager, CIGIslandState islandState, GridIndex? index = null)
        {
            base.Initialize(storage, isometricGrid, islandsManagerView, worldMap, buildingWarehouseManager, craneManager, gameStats, gameState, popupManager, multipliers, timing, routineRunner, properties, overlayManager, islandState, index);
            if (!base.BuildingProperties.Activatable)
            {
                this.UpdateActivateHintIcon();
            }
        }

        protected void UpdateActivateHintIcon()
        {
            if (!this.Activated && !base.IsUpgrading)
            {
                this.ShowActivateHintIcon();
                return;
            }
            this.RemoveActivateHintIcon();
        }

        protected override void UpdateLevelIcon(int level)
        {
            if (!base.BuildingProperties.Activatable || this.Activated)
            {
                base.UpdateLevelIcon(this.DisplayLevel);
            }
        }

        protected override void OnUpgradeCompleted(double completionTime)
        {
            base.OnUpgradeCompleted(completionTime);
            if (!base.BuildingProperties.Activatable)
            {
                return;
            }
            this.UpdateActivateHintIcon();
        }

        private void ShowActivateHintIcon()
        {
            this._gridTileIconManager.SetIcon<ButtonGridTileIcon>(GridTileIconType.Activate).Init(new Action(this.OnBuildingPressed));
        }

        private void RemoveActivateHintIcon()
        {
            this._gridTileIconManager.RemoveIcon(GridTileIconType.Activate);
        }
    }
}
