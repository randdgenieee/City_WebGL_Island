using CIG.Translation;

namespace CIG
{
	public class WarehouseTutorialView : TutorialView<WarehouseTutorial>
	{
		private HUDWarehouseButton _warehouseButton;

		protected override bool CanShow
		{
			get
			{
				if (base.CanShow && !_popupManagerView.IsShowingPopup)
				{
					return !_worldMapView.IsVisible;
				}
				return false;
			}
		}

		public void Initialize(WarehouseTutorial tutorial, TutorialDialog tutorialDialog, TutorialPointer tutorialPointer, PopupManagerView popupManagerView, WorldMapView worldMapView, IslandsManagerView islandsManagerView, HUDWarehouseButton warehouseButton)
		{
			_warehouseButton = warehouseButton;
			Initialize(tutorial, tutorialDialog, tutorialPointer, popupManagerView, worldMapView, islandsManagerView);
		}

		protected override void Show()
		{
			WarehouseTutorial.TutorialState state = _tutorial.State;
			if (state == WarehouseTutorial.TutorialState.OpenDialog)
			{
				_tutorialDialog.Show(Localization.Key("tutorial.buildingwarehouse_new_building"), TutorialDialog.AdvisorPositionType.Left, useOverlay: false, useContinueButton: false, null);
				_tutorialPointer.Show(this, _warehouseButton.MaskTransform);
				_warehouseButton.SetFirstTimeBadgeShown(shown: true);
			}
		}

		protected override void OnPopupShown(Popup popup)
		{
			if (popup as BuildingWarehousePopup != null)
			{
				_warehouseButton.SetFirstTimeBadgeShown(shown: false);
			}
			base.OnPopupShown(popup);
		}

		protected override void OnPopupHidden(Popup popup)
		{
			if (popup as BuildingWarehousePopup != null)
			{
				_warehouseButton.SetFirstTimeBadgeShown(shown: false);
				_tutorial.Finish();
			}
			base.OnPopupHidden(popup);
		}
	}
}
