using Tweening;
using UnityEngine;

namespace CIG
{
	public class HUDMinigamesButton : HUDCurrencyTweenHelper
	{
		[SerializeField]
		private Tweener _iconTweener;

		[SerializeField]
		private GameObject _badge;

		private GameState _gameState;

		private PopupManager _popupManager;

		private ArcadeManager _arcadeManager;

		public void Initialize(GameState gameState, PopupManager popupManager, ArcadeManager arcadeManager)
		{
			_gameState = gameState;
			_popupManager = popupManager;
			_arcadeManager = arcadeManager;
			Initialize();
			_gameState.BalanceChangedEvent += OnBalanceChanged;
			SetActiveTweener(_iconTweener);
			UpdateBadge();
		}

		protected override void OnDestroy()
		{
			_popupManager = null;
			_arcadeManager = null;
			if (_gameState != null)
			{
				_gameState.BalanceChangedEvent -= OnBalanceChanged;
				_gameState = null;
			}
			base.OnDestroy();
		}

		public void OnClicked()
		{
			_popupManager.RequestPopup(new WheelOfFortunePopupRequest());
		}

		protected override void UpdateValue(decimal value)
		{
		}

		private void UpdateBadge()
		{
			_badge.SetActive(_arcadeManager.CanPlay);
		}

		private void OnBalanceChanged(Currencies oldBalance, Currencies newBalance, FlyingCurrenciesData flyingCurrenciesData)
		{
			UpdateBadge();
		}
	}
}
