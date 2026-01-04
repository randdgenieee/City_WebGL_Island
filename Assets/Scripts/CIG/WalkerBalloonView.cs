using System;
using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class WalkerBalloonView : GridTileIcon
	{
		[SerializeField]
		private Image _icon;

		[SerializeField]
		private Button _button;

		[SerializeField]
		private CurrencyAnimationSource _currencyAnimationSource;

		[SerializeField]
		private PlingManager _plingManager;

		private WalkerBalloon _walkerBalloon;

		private Timing _timing;

		private Action _expireCallback;

		public void Initialize(WalkerBalloon walkerBalloon, Timing timing, OverlayManager overlayManager, Action expireCallback)
		{
			_walkerBalloon = walkerBalloon;
			_timing = timing;
			_expireCallback = expireCallback;
			_walkerBalloon.ExpiredEvent += OnWalkerBalloonExpired;
			_walkerBalloon.CurrenciesCollectedEvent += OnCurrencyCollected;
			_currencyAnimationSource.Initialize(_walkerBalloon);
			_plingManager.Initialize(overlayManager);
			_icon.sprite = SingletonMonobehaviour<UISpriteAssetCollection>.Instance.GetWalkerBalloonSprite(_walkerBalloon.WalkerBalloonType);
			_walkerBalloon.Shown();
		}

		private void OnDestroy()
		{
			if (_walkerBalloon != null)
			{
				_walkerBalloon.ExpiredEvent -= OnWalkerBalloonExpired;
				_walkerBalloon.CurrenciesCollectedEvent -= OnCurrencyCollected;
				_walkerBalloon = null;
			}
		}

		public void OnIconClicked()
		{
			_button.interactable = false;
			_walkerBalloon.Collect();
		}

		private void OnWalkerBalloonExpired(WalkerBalloon walkerBalloon)
		{
			if (_expireCallback == null)
			{
				Remove();
			}
			else
			{
				_expireCallback();
			}
		}

		private void OnCurrencyCollected(Currency currency, Clip sound)
		{
			_plingManager.ShowCurrencyPlings(_timing, currency, sound);
		}
	}
}
