using UnityEngine;

namespace CIG
{
	public class HUDController : MonoBehaviour
	{
		[SerializeField]
		private HUDView _hudView;

		[SerializeField]
		private VisitingHUDView _visitingHUDView;

		private IslandsManager _islandsManager;

		private void OnDestroy()
		{
			if (_islandsManager != null)
			{
				_islandsManager.VisitingStartedEvent -= OnVisitingStarted;
				_islandsManager.VisitingStoppedEvent -= OnVisitingStopped;
				_islandsManager = null;
			}
		}

		public void Initialize(IslandsManager islandsManager)
		{
			_islandsManager = islandsManager;
			_islandsManager.VisitingStartedEvent += OnVisitingStarted;
			_islandsManager.VisitingStoppedEvent += OnVisitingStopped;
			ToggleHUD(_islandsManager.IsVisiting);
		}

		private void ToggleHUD(bool isVisiting)
		{
			_hudView.gameObject.SetActive(!isVisiting);
			_visitingHUDView.gameObject.SetActive(isVisiting);
			if (isVisiting)
			{
				_visitingHUDView.SetData();
			}
			else
			{
				_visitingHUDView.Deactivate();
			}
		}

		private void OnVisitingStarted()
		{
			ToggleHUD(isVisiting: true);
		}

		private void OnVisitingStopped()
		{
			ToggleHUD(isVisiting: false);
		}
	}
}
