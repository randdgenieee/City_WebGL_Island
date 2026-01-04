using CIG;
using CIG.Translation;
using System;
using UnityEngine;
using UnityEngine.UI;

public class BuildingShopItem : ShopItem
{
	[SerializeField]
	private LocalizedText _buildingNameLabel;

	[SerializeField]
	private BuildingImage _buildingImage;

	[SerializeField]
	private Button _backgroundButton;

	[SerializeField]
	private Image _currencyIcon;

	[SerializeField]
	private GameObject _buyButtonContainer;

	[SerializeField]
	private GameObject _unlockLevelContainer;

	[SerializeField]
	private LocalizedText _unlockLevelLabel;

	[SerializeField]
	private GameObject _newLabel;

	[SerializeField]
	private ButtonStyleView _buyButtonStyle;

	public void Init(BuildingProperties buildingProperties, Currency price, int currentLevel, bool isLocked, bool showNewItem, Action<BuildingProperties> onClick)
	{
		_newLabel.SetActive(showNewItem);
		bool flag = price.Value == decimal.Zero;
		int i = (buildingProperties.UnlockLevels.Count != 0) ? buildingProperties.UnlockLevels[0] : 0;
		isLocked |= !buildingProperties.IsUnlocked(currentLevel);
		if (isLocked && buildingProperties.IsUnlocked(currentLevel))
		{
			i = buildingProperties.GetNextUnlockLevel(currentLevel);
		}
		Initialize(delegate
		{
			onClick(buildingProperties);
		}, flag ? Localization.Key("free") : Localization.Integer(price.Value));
		_buildingNameLabel.LocalizedString = buildingProperties.LocalizedName;
		_unlockLevelLabel.LocalizedString = Localization.Integer(i);
		_buildingImage.Initialize(buildingProperties, (!isLocked) ? MaterialType.UIClip : MaterialType.UIClipGreyscale);
		_currencyIcon.gameObject.SetActive(!flag);
		_currencyIcon.sprite = SingletonMonobehaviour<CurrencySpriteAssetCollection>.Instance.GetCurrencySprite(price);
		_backgroundButton.interactable = !isLocked;
		_buyButtonContainer.SetActive(!isLocked);
		_unlockLevelContainer.SetActive(isLocked);
		_buildingNameLabel.gameObject.SetActive(!isLocked);
		_buyButtonStyle.ApplyStyle(buildingProperties.IsGoldBuilding ? ButtonStyleType.ShopGold : ButtonStyleType.ShopGreen);
	}
}
