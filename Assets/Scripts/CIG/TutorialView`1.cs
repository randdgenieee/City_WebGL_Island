using UnityEngine;

namespace CIG
{
	public abstract class TutorialView<T> : MonoBehaviour where T : Tutorial
	{
		protected T _tutorial;

		protected TutorialDialog _tutorialDialog;

		protected TutorialPointer _tutorialPointer;

		protected PopupManagerView _popupManagerView;

		protected WorldMapView _worldMapView;

		protected IslandsManagerView _islandsManagerView;

		protected IsometricIsland _subscribedIsland;

		protected virtual bool CanShow
		{
			get
			{
				if (_tutorial != null && !_tutorial.IsCompleted)
				{
					return !_islandsManagerView.IslandsManager.IsVisiting;
				}
				return false;
			}
		}

		protected bool CanShowOnWorldMap
		{
			get
			{
				if (_worldMapView.IsVisible)
				{
					return !_worldMapView.IsShowingNewIslands;
				}
				return false;
			}
		}

		protected bool CanShowOnIsland
		{
			get
			{
				if (!_worldMapView.IsVisible && _subscribedIsland != null)
				{
					return !_subscribedIsland.CinematicPlaying;
				}
				return false;
			}
		}

		protected bool CanShowOnIslandAndWorldMap
		{
			get
			{
				if (!_worldMapView.IsVisible)
				{
					if (_subscribedIsland != null)
					{
						return !_subscribedIsland.CinematicPlaying;
					}
					return false;
				}
				return true;
			}
		}

		public void Initialize(T tutorial, TutorialDialog tutorialDialog, TutorialPointer tutorialPointer, PopupManagerView popupManagerView, WorldMapView worldMapView, IslandsManagerView islandsManagerView)
		{
			_tutorial = tutorial;
			_tutorialDialog = tutorialDialog;
			_tutorialPointer = tutorialPointer;
			_popupManagerView = popupManagerView;
			_worldMapView = worldMapView;
			_islandsManagerView = islandsManagerView;
			_tutorial.StateChangedEvent += OnStateChanged;
			_popupManagerView.PopupShownEvent += OnPopupShown;
			_popupManagerView.PopupHiddenEvent += OnPopupHidden;
			_worldMapView.VisibilityChangedEvent += OnWorldMapVisibilityChanged;
			_worldMapView.ShowNewIslandsAnimationPlayingChangedEvent += OnShowNewIslandsAnimationFinished;
			_islandsManagerView.IslandChangedEvent += OnIslandChanged;
			OnIslandChanged(IsometricIsland.Current);
			UpdateTutorialVisibility();
		}

		public virtual void Deinitialize()
		{
			Hide();
			if (_popupManagerView != null)
			{
				_popupManagerView.PopupShownEvent -= OnPopupShown;
				_popupManagerView.PopupHiddenEvent -= OnPopupHidden;
				_popupManagerView = null;
			}
			if (_tutorial != null)
			{
				_tutorial.StateChangedEvent -= OnStateChanged;
				_tutorial = null;
			}
			if (_worldMapView != null)
			{
				_worldMapView.VisibilityChangedEvent -= OnWorldMapVisibilityChanged;
				_worldMapView.ShowNewIslandsAnimationPlayingChangedEvent -= OnShowNewIslandsAnimationFinished;
				_worldMapView = null;
			}
			if (_islandsManagerView != null)
			{
				_islandsManagerView.IslandChangedEvent -= OnIslandChanged;
				_islandsManagerView = null;
			}
			_tutorialDialog = null;
			_tutorialPointer = null;
		}

		protected virtual void OnDestroy()
		{
			Deinitialize();
		}

		protected abstract void Show();

		protected virtual void Hide()
		{
			if (_tutorialDialog != null)
			{
				_tutorialDialog.Hide();
			}
			if (_tutorialPointer != null)
			{
				_tutorialPointer.Hide(this);
			}
		}

		protected void UpdateTutorialVisibility()
		{
			Hide();
			TryShow();
		}

		protected virtual void OnPopupShown(Popup popup)
		{
			UpdateTutorialVisibility();
		}

		protected virtual void OnPopupHidden(Popup popup)
		{
			UpdateTutorialVisibility();
		}

		protected virtual void OnWorldMapVisibilityChanged(bool visible)
		{
			UpdateTutorialVisibility();
		}

		protected virtual void OnIslandChanged(IsometricIsland isometricIsland)
		{
			if (_subscribedIsland != null)
			{
				_subscribedIsland.CinematicPlayingChangedEvent -= OnIslandCinematicPlayingChanged;
			}
			_subscribedIsland = isometricIsland;
			if (_subscribedIsland != null)
			{
				_subscribedIsland.CinematicPlayingChangedEvent += OnIslandCinematicPlayingChanged;
				OnIslandCinematicPlayingChanged(_subscribedIsland.CinematicPlaying);
			}
		}

		private void TryShow()
		{
			if (CanShow)
			{
				Show();
			}
		}

		private void OnStateChanged()
		{
			TryShow();
		}

		private void OnIslandCinematicPlayingChanged(bool islandCinematicPlaying)
		{
			UpdateTutorialVisibility();
		}

		private void OnShowNewIslandsAnimationFinished(bool isPlaying)
		{
			UpdateTutorialVisibility();
		}
	}
}
