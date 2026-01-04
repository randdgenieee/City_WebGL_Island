using UnityEngine;

namespace CIG
{
	public class RatingRequesterView : MonoBehaviour
	{
		private PopupManager _popupManager;

		private RatingRequester _ratingRequester;

		private WorldMapView _worldMapView;

		public void Initialize(RatingRequester ratingRequester, PopupManager popupManager, WorldMapView worldMapView)
		{
			_popupManager = popupManager;
			_ratingRequester = ratingRequester;
			_worldMapView = worldMapView;
			_popupManager.PopupClosedEvent += OnPopupClosed;
			_ratingRequester.QualifiedForRatingChangedEvent += OnQualifiedForRatingChanged;
			_worldMapView.VisibilityChangedEvent += OnWorldMapViewVisibilityChanged;
			_worldMapView.ShowNewIslandsAnimationPlayingChangedEvent += OnWorldMapShowNewIslandAnimationPlayingChanged;
			TryRequestRating();
		}

		private void OnDestroy()
		{
			if (_ratingRequester != null)
			{
				_ratingRequester.QualifiedForRatingChangedEvent -= OnQualifiedForRatingChanged;
				_ratingRequester = null;
			}
			if (_popupManager != null)
			{
				_popupManager.PopupClosedEvent -= OnPopupClosed;
				_popupManager = null;
			}
			if (_worldMapView != null)
			{
				_worldMapView.VisibilityChangedEvent -= OnWorldMapViewVisibilityChanged;
				_worldMapView.ShowNewIslandsAnimationPlayingChangedEvent -= OnWorldMapShowNewIslandAnimationPlayingChanged;
				_worldMapView = null;
			}
		}

		private void TryRequestRating()
		{
			if (!_popupManager.IsShowingPopup && !_worldMapView.IsVisible && !_worldMapView.IsShowingNewIslands)
			{
				_ratingRequester.TryRequestRating();
			}
		}

		private void OnQualifiedForRatingChanged()
		{
			TryRequestRating();
		}

		private void OnPopupClosed(PopupRequest request, bool instant)
		{
			TryRequestRating();
		}

		private void OnWorldMapViewVisibilityChanged(bool visible)
		{
			TryRequestRating();
		}

		private void OnWorldMapShowNewIslandAnimationPlayingChanged(bool isplaying)
		{
			TryRequestRating();
		}
	}
}
