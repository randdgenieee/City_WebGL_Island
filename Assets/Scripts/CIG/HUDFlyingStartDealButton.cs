using Tweening;
using UnityEngine;

namespace CIG
{
	public class HUDFlyingStartDealButton : HUDRegionElement
	{
		[SerializeField]
		private DateTimeTimerView _timer;

		[SerializeField]
		private Tweener _activateTweener;

		private FlyingStartDealManager _flyingStartDealManager;

		private PopupManager _popupManager;

		public void Initialize(FlyingStartDealManager flyingStartDealManager, PopupManager popupManager)
		{
			_flyingStartDealManager = flyingStartDealManager;
			_popupManager = popupManager;
			_flyingStartDealManager.ActiveChangedEvent += OnActiveChanged;
			OnActiveChanged(_flyingStartDealManager.IsActive);
		}

		private void OnDestroy()
		{
			if (_flyingStartDealManager != null)
			{
				_flyingStartDealManager.ActiveChangedEvent -= OnActiveChanged;
				_flyingStartDealManager = null;
			}
		}

		public void OnClicked()
		{
			_popupManager.RequestPopup(new FlyingStartDealPopupRequest(_flyingStartDealManager));
		}

		protected override void UpdateVisibility()
		{
			base.gameObject.SetActive(_regionShowing && _flyingStartDealManager.IsActive && _flyingStartDealManager.StoreProduct != null);
		}

		private void OnActiveChanged(bool isActive)
		{
			UpdateVisibility();
			if (isActive)
			{
				_activateTweener.StopAndReset();
				_activateTweener.Play();
				_timer.StartTimer(_flyingStartDealManager.ExpireTime);
			}
			else
			{
				_timer.StopTimer();
			}
		}
	}
}
