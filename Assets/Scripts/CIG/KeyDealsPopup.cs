using CIG.Translation;
using System.Collections;
using UnityEngine;

namespace CIG
{
	public class KeyDealsPopup : Popup
	{
		[SerializeField]
		private LocalizedText _timerText;

		[SerializeField]
		private LocalizedText _goldKeysText;

		[SerializeField]
		private LocalizedText _silverKeysText;

		[SerializeField]
		private LocalizedText _refreshPriceText;

		[SerializeField]
		private Transform _itemsContainer;

		[SerializeField]
		private KeyDealItem _keyDealItemPrefab;

		private GameState _gameState;

		private KeyDealsManager _keyDealsManager;

		private Coroutine _timerUpdateRoutine;

		private KeyDealItem[] _keyDealItems;

		public override string AnalyticsScreenName => "key_deals";

		public override void Initialize(Model model, CIGCanvasScaler canvasScaler)
		{
			base.Initialize(model, canvasScaler);
			_gameState = model.Game.GameState;
			_keyDealsManager = model.Game.KeyDealsManager;
			int amount = _keyDealsManager.Amount;
			_keyDealItems = new KeyDealItem[amount];
			for (int i = 0; i < amount; i++)
			{
				KeyDealItem keyDealItem = Object.Instantiate(_keyDealItemPrefab, _itemsContainer);
				keyDealItem.Initialize(_gameState, OnBuyPressed);
				_keyDealItems[i] = keyDealItem;
			}
		}

		protected override void OnDestroy()
		{
			if (_gameState != null)
			{
				_gameState.ValueChangedEvent -= OnValueChanged;
				_gameState = null;
			}
			if (_keyDealsManager != null)
			{
				_keyDealsManager.OnKeyDealsChangedEvent -= OnKeyDealsChanged;
				_keyDealsManager = null;
			}
			base.OnDestroy();
		}

		public override void Open(PopupRequest request)
		{
			base.Open(request);
			_timerUpdateRoutine = StartCoroutine(TimerUpdateRoutine());
			_refreshPriceText.LocalizedString = Localization.Integer(1);
			UpdateCurrencyAmountTexts();
			_gameState.ValueChangedEvent += OnValueChanged;
			_keyDealsManager.OnKeyDealsChangedEvent += OnKeyDealsChanged;
			OnKeyDealsChanged(_keyDealsManager.KeyDeals);
		}

		public void OnRefreshPressed()
		{
			_keyDealsManager.BuyKeyDealsRefresh();
		}

		protected override void Closed()
		{
			StopCoroutine(_timerUpdateRoutine);
			_gameState.ValueChangedEvent -= OnValueChanged;
			_keyDealsManager.OnKeyDealsChangedEvent -= OnKeyDealsChanged;
			base.Closed();
		}

		private void OnBuyPressed(KeyDeal keyDeal)
		{
			_keyDealsManager.BuyKeyDeal(keyDeal);
		}

		private void OnValueChanged(string key, object oldValue, object newValue)
		{
			if (key == "EarnedBalance" || key == "GiftedBalance")
			{
				UpdateCurrencyAmountTexts();
				int i = 0;
				for (int num = _keyDealItems.Length; i < num; i++)
				{
					_keyDealItems[i].RefreshPrice();
				}
			}
		}

		private void OnKeyDealsChanged(KeyDeal[] keyDeals)
		{
			int i = 0;
			for (int num = _keyDealItems.Length; i < num; i++)
			{
				KeyDeal keyDeal = null;
				if (i < _keyDealsManager.KeyDeals.Length)
				{
					keyDeal = _keyDealsManager.KeyDeals[i];
				}
				_keyDealItems[i].RefreshKeyDeal(keyDeal);
			}
		}

		private void UpdateCurrencyAmountTexts()
		{
			_goldKeysText.LocalizedString = Localization.Integer(_gameState.Balance.GetValue("GoldKey"));
			_silverKeysText.LocalizedString = Localization.Integer(_gameState.Balance.GetValue("SilverKey"));
		}

		private IEnumerator TimerUpdateRoutine()
		{
			while (true)
			{
				_timerText.LocalizedString = Localization.FullTimeSpan(_keyDealsManager.DealTimeRemaining, hidePartWhenZero: false);
				yield return new WaitForSecondsRealtime(1f);
			}
		}
	}
}
