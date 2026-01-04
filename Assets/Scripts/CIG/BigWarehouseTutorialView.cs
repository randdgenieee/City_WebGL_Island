using CIG.Translation;

namespace CIG
{
	public class BigWarehouseTutorialView : TutorialView<BigWarehouseTutorial>
	{
		private HUDWarehouseButton _warehouseButton;

		private IsometricGrid _subscribedGrid;

		private BuildingPopup _buildingPopup;

		private BuildingWarehousePopup _buildingWarehousePopup;

		protected override bool CanShow
		{
			get
			{
				if (!base.CanShow || !base.CanShowOnIsland)
				{
					return false;
				}
				switch (_tutorial.State)
				{
				case BigWarehouseTutorial.TutorialState.BuildingOpened:
					return _popupManagerView.TopPopup is BuildingPopup;
				case BigWarehouseTutorial.TutorialState.WarehouseOpened1:
				case BigWarehouseTutorial.TutorialState.WarehouseOpened2:
				case BigWarehouseTutorial.TutorialState.FinishText:
					return _popupManagerView.TopPopup is BuildingWarehousePopup;
				default:
					return !_popupManagerView.IsShowingPopup;
				}
			}
		}

		public void Initialize(BigWarehouseTutorial tutorial, TutorialDialog tutorialDialog, TutorialPointer tutorialPointer, PopupManagerView popupManagerView, WorldMapView worldMapView, IslandsManagerView islandsManagerView, HUDWarehouseButton warehouseButton, BuildingPopup buildingPopup, BuildingWarehousePopup buildingWarehousePopup)
		{
			_warehouseButton = warehouseButton;
			_buildingPopup = buildingPopup;
			_buildingWarehousePopup = buildingWarehousePopup;
			Initialize(tutorial, tutorialDialog, tutorialPointer, popupManagerView, worldMapView, islandsManagerView);
		}

		public override void Deinitialize()
		{
			if (_subscribedGrid != null)
			{
				_subscribedGrid.GridTileRemovedEvent -= OnTileRemoved;
				_subscribedGrid = null;
			}
			base.Deinitialize();
		}

		protected override void Show()
		{
			switch (_tutorial.State)
			{
			case (BigWarehouseTutorial.TutorialState)5:
				break;
			case BigWarehouseTutorial.TutorialState.IntroText1:
				_tutorialDialog.Show(Localization.Key("tutorial.warehouse_1"), TutorialDialog.AdvisorPositionType.Right, useOverlay: true, useContinueButton: true, OnContinueClicked);
				break;
			case BigWarehouseTutorial.TutorialState.IntroText2:
				_tutorialDialog.Show(Localization.Key("tutorial.warehouse_2"), TutorialDialog.AdvisorPositionType.Right, useOverlay: true, useContinueButton: true, OnContinueClicked);
				break;
			case BigWarehouseTutorial.TutorialState.OpenBuilding:
				Builder.TilesHidden = false;
				_tutorialPointer.ShowOnIsland(this, _tutorial.Building.transform, _tutorial.Building.SpriteRenderer.sprite);
				IsometricIsland.Current.CameraOperator.PushDisableInputRequest(this);
				IsometricIsland.Current.CameraOperator.ScrollTo(_tutorial.Building.gameObject);
				break;
			case BigWarehouseTutorial.TutorialState.BuildingOpened:
				_tutorialPointer.Show(this, _buildingPopup.WarehouseButtonMaskTransform);
				_tutorialDialog.Show(Localization.Key("tutorial.warehouse_3"), TutorialDialog.AdvisorPositionType.Right, useOverlay: false, useContinueButton: true, OnContinueClicked);
				break;
			case BigWarehouseTutorial.TutorialState.OpenWarehouse:
				_tutorialPointer.Show(this, _warehouseButton.MaskTransform);
				break;
			case BigWarehouseTutorial.TutorialState.WarehouseOpened1:
				_tutorialDialog.Show(Localization.Key("tutorial.warehouse_4"), TutorialDialog.AdvisorPositionType.Right, useOverlay: true, useContinueButton: true, OnContinueClicked);
				break;
			case BigWarehouseTutorial.TutorialState.WarehouseOpened2:
			{
				BuildingWarehouseItem buildingWarehouseItem = _buildingWarehousePopup.ScrollToAndGetEmptySlot();
				if (buildingWarehouseItem != null)
				{
					_tutorialPointer.Show(this, buildingWarehouseItem.MaskTransform);
				}
				_tutorialDialog.Show(Localization.Key("tutorial.warehouse_5"), TutorialDialog.AdvisorPositionType.Right, useOverlay: false, useContinueButton: true, OnContinueClicked);
				break;
			}
			case BigWarehouseTutorial.TutorialState.FinishText:
				_tutorialPointer.Hide(this);
				_tutorialDialog.Show(Localization.Key("tutorial.warehouse_6"), TutorialDialog.AdvisorPositionType.Right, useOverlay: false, useContinueButton: true, OnContinueClicked);
				break;
			}
		}

		protected override void Hide()
		{
			base.Hide();
			if (IsometricIsland.Current != null)
			{
				IsometricIsland.Current.CameraOperator.PopDisableInputRequest(this);
			}
		}

		protected override void OnIslandChanged(IsometricIsland isometricIsland)
		{
			if (_subscribedGrid != null)
			{
				_subscribedGrid.GridTileRemovedEvent -= OnTileRemoved;
				_subscribedGrid = null;
			}
			base.OnIslandChanged(isometricIsland);
			if (_subscribedIsland != null)
			{
				_subscribedGrid = _subscribedIsland.IsometricGrid;
				_subscribedGrid.GridTileRemovedEvent += OnTileRemoved;
			}
			_tutorial.OnIslandChanged(isometricIsland);
		}

		private void OnContinueClicked()
		{
			_tutorialPointer.Hide(this);
			_tutorial.TextDismissed();
		}

		private void OnTileRemoved(GridTile tile)
		{
			_tutorial.TileRemoved();
		}
	}
}
