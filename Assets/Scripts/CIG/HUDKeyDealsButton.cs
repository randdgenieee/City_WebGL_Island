using Tweening;
using UnityEngine;

namespace CIG
{
	public class HUDKeyDealsButton : HUDCurrencyTweenHelper
	{
		[SerializeField]
		private Tweener _iconTweener;

		private PopupManager _popupManager;

		public void Initialize(PopupManager popupManager)
		{
			_popupManager = popupManager;
			Initialize();
			SetActiveTweener(_iconTweener);
		}

		protected override void OnDestroy()
		{
			_popupManager = null;
			base.OnDestroy();
		}

		public void OnClicked()
		{
			_popupManager.RequestPopup(new KeyDealsPopupRequest());
		}

		protected override void UpdateValue(decimal value)
		{
		}
	}
}
