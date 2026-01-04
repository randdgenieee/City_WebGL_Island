using CIG.Translation;
using System;
using Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class RegularChestShopItem : ChestShopItem
	{
		[SerializeField]
		private LocalizedText _chestNameLabel;

		[SerializeField]
		private GameObject _popularChoiceBanner;

		[SerializeField]
		private GameObject _particles;

		[SerializeField]
		private CurrencyView _keyPrice;

		[SerializeField]
		private Tweener _iconInTweener;

		[SerializeField]
		private Tweener _iconOutTweener;

		[SerializeField]
		private Image _chestImage;

		[SerializeField]
		private Image _buildingImage;

		[SerializeField]
		private InteractableButton _keyButton;

		private GameState _gameState;

		private TreasureChest _treasureChest;

		private TOCIStoreProduct _storeProduct;

		private Action<TOCIStoreProduct> _onIAPClicked;

		private Action<TreasureChest> _onKeysClicked;

		private Action<TreasureChest, TOCIStoreProduct> _onContentsClicked;

		public override TreasureChestType TreasureChestType => _treasureChest.Properties.TreasureChestType;

		private void OnDestroy()
		{
			if (_iconInTweener != null)
			{
				_iconInTweener.FinishedPlaying -= OnIconInTweenerFinishedPlaying;
			}
			if (_iconOutTweener != null)
			{
				_iconOutTweener.FinishedPlaying -= OnIconOutTweenerFinishedPlaying;
			}
		}

		public void Initialize(GameState gameState, TreasureChest treasureChest, Action<TOCIStoreProduct> onIAPClicked, Action<TreasureChest> onKeysClicked, Action<TreasureChest, TOCIStoreProduct> onContentsClicked, TOCIStoreProduct storeProduct)
		{
			_gameState = gameState;
			_treasureChest = treasureChest;
			_storeProduct = storeProduct;
			_onIAPClicked = onIAPClicked;
			_onKeysClicked = onKeysClicked;
			_onContentsClicked = onContentsClicked;
			Initialize(ItemClicked, Localization.Literal(_storeProduct.FormattedPrice));
			_chestNameLabel.LocalizedString = _treasureChest.Name;
			_chestImage.sprite = SingletonMonobehaviour<UISpriteAssetCollection>.Instance.GetChestSprite(_treasureChest.Properties.TreasureChestType);
			_keyPrice.Initialize(_treasureChest.Properties.Cost.WithoutEmpty().GetCurrency(0));
			_popularChoiceBanner.SetActive(_treasureChest.Properties.TreasureChestType == TreasureChestType.Gold);
			_particles.SetActive(_treasureChest.Properties.TreasureChestType == TreasureChestType.Platinum);
			_iconInTweener.FinishedPlaying += OnIconInTweenerFinishedPlaying;
			_iconOutTweener.FinishedPlaying += OnIconOutTweenerFinishedPlaying;
		}

		public override void SetVisible(bool visible)
		{
			base.SetVisible(visible);
			if (visible)
			{
				if (_treasureChest.Properties.TreasureChestType == TreasureChestType.Platinum)
				{
					_iconInTweener.Play();
				}
				_keyButton.interactable = _treasureChest.CanAfford;
				Analytics.IAPViewed(_storeProduct.Identifier);
			}
			else if (_treasureChest.Properties.TreasureChestType == TreasureChestType.Platinum)
			{
				if (_iconInTweener.IsPlaying)
				{
					_iconInTweener.Stop();
				}
				if (_iconOutTweener.IsPlaying)
				{
					_iconOutTweener.Stop();
				}
			}
		}

		public void OnKeysClicked()
		{
			EventTools.Fire(_onKeysClicked, _treasureChest);
		}

		public void OnIAPClicked()
		{
			EventTools.Fire(_onIAPClicked, _storeProduct);
		}

		public void OnContentsClicked()
		{
			EventTools.Fire(_onContentsClicked, _treasureChest, _storeProduct);
		}

		private void ItemClicked()
		{
			if (_gameState.CanAfford(_treasureChest.Properties.Cost))
			{
				OnKeysClicked();
			}
			else
			{
				OnIAPClicked();
			}
		}

		private void OnIconInTweenerFinishedPlaying(Tweener tweener)
		{
			_chestImage.gameObject.SetActive(!_chestImage.gameObject.activeSelf);
			_buildingImage.gameObject.SetActive(!_buildingImage.gameObject.activeSelf);
			_iconOutTweener.Play();
		}

		private void OnIconOutTweenerFinishedPlaying(Tweener tweener)
		{
			_iconInTweener.Play();
		}
	}
}
