using CIG.Translation;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class KeyDealItem : MonoBehaviour
	{
		[SerializeField]
		private GameObject _purchasedBanner;

		[SerializeField]
		private Graphic[] _greyscaleGraphics;

		[SerializeField]
		private BuildingImage _buildingImage;

		[SerializeField]
		private LocalizedText _categoryText;

		[SerializeField]
		private Button _frameButton;

		[SerializeField]
		private Image _oldPriceIcon;

		[SerializeField]
		private LocalizedText _oldPriceText;

		[SerializeField]
		private InteractableButton _buyButton;

		[SerializeField]
		private ButtonStyleView _buttonStyleView;

		[SerializeField]
		private CurrencyView _currencyView;

		private GameState _gameState;

		private KeyDeal _keyDeal;

		private Action<KeyDeal> _onBuyPressed;

		public void Initialize(GameState gameState, Action<KeyDeal> onBuyPressed)
		{
			_gameState = gameState;
			_onBuyPressed = onBuyPressed;
		}

		public void RefreshKeyDeal(KeyDeal keyDeal)
		{
			if (keyDeal == null || keyDeal.Type == KeyDeal.KeyDealType.Invalid)
			{
				base.gameObject.SetActive(value: false);
				_keyDeal = null;
				return;
			}
			base.gameObject.SetActive(value: true);
			_keyDeal = keyDeal;
			MaterialType materialType = (!_keyDeal.Purchased) ? MaterialType.UIClip : MaterialType.UIClipGreyscale;
			_buildingImage.Initialize(keyDeal.BuildingProperties, materialType);
			SwitchGraphicsMaterial(materialType);
			_purchasedBanner.SetActive(keyDeal.Purchased);
			_categoryText.LocalizedString = keyDeal.BuildingProperties.CategoryName;
			_currencyView.Initialize(keyDeal.Price);
			if (keyDeal.BuildingProperties.IsUnlocked(_gameState.Level))
			{
				Currency buildingConstructionCost = keyDeal.BuildingConstructionCost;
				_oldPriceIcon.sprite = SingletonMonobehaviour<CurrencySpriteAssetCollection>.Instance.GetCurrencySprite(buildingConstructionCost);
				_oldPriceText.LocalizedString = Localization.Integer(buildingConstructionCost.Value);
			}
			else
			{
				_oldPriceIcon.sprite = SingletonMonobehaviour<CurrencySpriteAssetCollection>.Instance.GetCurrencySprite(CurrencyType.XP);
				_oldPriceText.LocalizedString = Localization.Format(Localization.Key("level_x"), Localization.Integer(keyDeal.BuildingProperties.GetNextUnlockLevel(_gameState.Level)));
			}
			RefreshPrice();
		}

		public void RefreshPrice()
		{
			if (_keyDeal != null)
			{
				bool flag = _gameState.CanAfford(_keyDeal.Price);
				if (_keyDeal.Purchased || !flag)
				{
					_buttonStyleView.ApplyStyle(ButtonStyleType.ShopGrey);
				}
				else if (_keyDeal.BuildingProperties.IsGoldBuilding)
				{
					_buttonStyleView.ApplyStyle(ButtonStyleType.ShopGold);
				}
				else
				{
					_buttonStyleView.ApplyStyle(ButtonStyleType.ShopGreen);
				}
				InteractableButton buyButton = _buyButton;
				bool interactable = _frameButton.interactable = (!_keyDeal.Purchased & flag);
				buyButton.interactable = interactable;
			}
		}

		public void OnBuyPressed()
		{
			EventTools.Fire(_onBuyPressed, _keyDeal);
		}

		private void SwitchGraphicsMaterial(MaterialType materialType)
		{
			Material asset = SingletonMonobehaviour<MaterialAssetCollection>.Instance.GetAsset(materialType);
			int i = 0;
			for (int num = _greyscaleGraphics.Length; i < num; i++)
			{
				_greyscaleGraphics[i].material = asset;
			}
		}
	}
}
