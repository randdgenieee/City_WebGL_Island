using UnityEngine;

namespace CIG
{
	public class WorldMapUnlockIslandTutorialView : TutorialView<WorldMapUnlockIslandTutorial>
	{
		private WorldMapIsland _worldMapIsland;

		private HUDMapButton _hudMapButton;

		protected override bool CanShow
		{
			get
			{
				if (!base.CanShow || _popupManagerView.IsShowingPopup)
				{
					return false;
				}
				switch (_tutorial.State)
				{
				case WorldMapUnlockIslandTutorial.TutorialState.None:
					return false;
				case WorldMapUnlockIslandTutorial.TutorialState.ReturnToWorldMap:
					return base.CanShowOnIsland;
				default:
					return base.CanShowOnWorldMap;
				}
			}
		}

		public void Initialize(WorldMapUnlockIslandTutorial tutorial, TutorialDialog tutorialDialog, TutorialPointer tutorialPointer, PopupManagerView popupManagerView, WorldMapView worldMapView, IslandsManagerView islandsManagerView, HUDMapButton mapButton)
		{
			_hudMapButton = mapButton;
			_worldMapIsland = worldMapView.GetWorldMapIsland(IslandId.Island03);
			_worldMapIsland.IslandOverlayToggledEvent += OnIslandOverlayToggled;
			Initialize(tutorial, tutorialDialog, tutorialPointer, popupManagerView, worldMapView, islandsManagerView);
		}

		public override void Deinitialize()
		{
			if (_worldMapIsland != null)
			{
				_worldMapIsland.IslandOverlayToggledEvent -= OnIslandOverlayToggled;
				_worldMapIsland = null;
			}
			base.Deinitialize();
		}

		protected override void Show()
		{
			if (!(_worldMapIsland == null))
			{
				switch (_tutorial.State)
				{
				case WorldMapUnlockIslandTutorial.TutorialState.WaitForUnlock:
				case WorldMapUnlockIslandTutorial.TutorialState.WaitForVisit:
					break;
				case WorldMapUnlockIslandTutorial.TutorialState.ClickIsland:
					_tutorialPointer.ShowOnWorldMap(this, (RectTransform)_worldMapIsland.transform);
					_worldMapView.SetTutorialFocusIsland(_worldMapIsland.IslandId);
					break;
				case WorldMapUnlockIslandTutorial.TutorialState.PressUnlockIsland:
				{
					Transform transform = _worldMapIsland.IslandButtons.UnlockButton.transform;
					_tutorialPointer.ShowOnWorldMap(this, (RectTransform)transform);
					break;
				}
				case WorldMapUnlockIslandTutorial.TutorialState.ReturnToWorldMap:
					_tutorialPointer.Show(this, (RectTransform)_hudMapButton.transform);
					break;
				}
			}
		}

		private void OnIslandOverlayToggled(Overlay overlay, bool active)
		{
			IslandUnlockOverlay islandUnlockOverlay;
			if (overlay is IslandSelectedOverlay)
			{
				_tutorial.OverlayToggled(active);
			}
			else if ((object)(islandUnlockOverlay = (overlay as IslandUnlockOverlay)) != null)
			{
				if (active)
				{
					islandUnlockOverlay.OverrideData(Currency.CashCurrency(decimal.Zero), Currency.GoldCurrency(decimal.Zero), 5);
					Hide();
				}
				else
				{
					UpdateTutorialVisibility();
				}
			}
		}
	}
}
