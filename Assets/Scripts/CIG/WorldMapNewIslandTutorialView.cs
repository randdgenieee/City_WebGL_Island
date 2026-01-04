using UnityEngine;

namespace CIG
{
	public class WorldMapNewIslandTutorialView : MonoBehaviour
	{
		private WorldMapView _worldMapView;

		private AirshipView _airshipView;

		private TutorialPointer _tutorialPointer;

		private PopupManagerView _popupManagerView;

		private WorldMapIsland _worldMapIsland;

		public void Initialize(WorldMapView worldMapView, PopupManagerView popupManagerView, TutorialPointer tutorialPointer)
		{
			_worldMapView = worldMapView;
			_airshipView = _worldMapView.AirshipView;
			_tutorialPointer = tutorialPointer;
			_popupManagerView = popupManagerView;
			_worldMapView.VisibilityChangedEvent += OnWorldMapVisibilityChanged;
			_worldMapView.NewIslandUnlockedEvent += OnNewIslandUnlocked;
			_popupManagerView.PopupShownEvent += OnPopupShown;
			_popupManagerView.PopupHiddenEvent += OnPopupHidden;
			_airshipView.AirshipAnimationFinishedEvent += OnAirshipAnimationFinished;
			if (_worldMapView.NewlyUnlockedIsland != null)
			{
				OnNewIslandUnlocked(_worldMapView.NewlyUnlockedIsland);
			}
		}

		private void OnDestroy()
		{
			if (_worldMapView != null)
			{
				_worldMapView.VisibilityChangedEvent -= OnWorldMapVisibilityChanged;
				_worldMapView.NewIslandUnlockedEvent -= OnNewIslandUnlocked;
				_worldMapView = null;
			}
			if (_worldMapIsland != null)
			{
				_worldMapIsland.IslandOverlayToggledEvent -= OnIslandOverlayToggled;
				_worldMapIsland = null;
			}
			if (_popupManagerView != null)
			{
				_popupManagerView.PopupShownEvent -= OnPopupShown;
				_popupManagerView.PopupHiddenEvent -= OnPopupHidden;
				_popupManagerView = null;
			}
			if (_airshipView != null)
			{
				_airshipView.AirshipAnimationFinishedEvent -= OnAirshipAnimationFinished;
				_airshipView = null;
			}
		}

		private void ToggleMask(bool overlayActive)
		{
			if (!_airshipView.IsFlying && _worldMapView.IsVisible && !_popupManagerView.IsShowingPopup && _worldMapIsland != null)
			{
				if (overlayActive)
				{
					Transform transform = _worldMapIsland.IslandButtons.OpenButton.transform;
					_tutorialPointer.ShowOnWorldMap(this, (RectTransform)transform);
				}
				else
				{
					_tutorialPointer.ShowOnWorldMap(this, _worldMapIsland.MaskTransform);
				}
			}
			else
			{
				_tutorialPointer.Hide(this);
			}
		}

		private void ReleaseWorldMapIsland()
		{
			if (_worldMapIsland != null)
			{
				_worldMapIsland.IslandOverlayToggledEvent -= OnIslandOverlayToggled;
				_worldMapIsland = null;
			}
		}

		private void OnNewIslandUnlocked(WorldMapIsland worldMapIsland)
		{
			ReleaseWorldMapIsland();
			if (worldMapIsland != null)
			{
				_worldMapIsland = worldMapIsland;
				_worldMapIsland.IslandOverlayToggledEvent += OnIslandOverlayToggled;
				_worldMapView.CameraOperator.ScrollAndZoom(_worldMapIsland.gameObject, 1400f);
			}
			ToggleMask(_worldMapIsland != null && _worldMapIsland.IslandButtons != null);
		}

		private void OnWorldMapVisibilityChanged(bool visible)
		{
			ReleaseWorldMapIsland();
			ToggleMask(overlayActive: false);
		}

		private void OnIslandOverlayToggled(Overlay overlay, bool active)
		{
			ToggleMask(active);
		}

		private void OnPopupShown(Popup popup)
		{
			ToggleMask(overlayActive: false);
		}

		private void OnPopupHidden(Popup popup)
		{
			ToggleMask(_worldMapIsland != null && _worldMapIsland.IslandButtons != null);
		}

		private void OnAirshipAnimationFinished()
		{
			ToggleMask(_worldMapIsland != null && _worldMapIsland.IslandButtons != null);
		}
	}
}
