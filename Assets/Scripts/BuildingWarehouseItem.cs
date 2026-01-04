using CIG;
using CIG.Translation;
using System;
using Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BuildingWarehouseItem : MonoBehaviour
{
	[SerializeField]
	private BuildingImage _buildingImage;

	[SerializeField]
	private Tweener _frontImageTweener;

	[SerializeField]
	private Image _rays;

	[SerializeField]
	private LocalizedText _description;

	[SerializeField]
	private GameObject _levelContainer;

	[SerializeField]
	private LocalizedText _level;

	[SerializeField]
	private GameObject _priceContainer;

	[SerializeField]
	private LocalizedText _price;

	[SerializeField]
	private Button _backgroundButton;

	[SerializeField]
	private GameObject _purchasedContainer;

	[SerializeField]
	private GameObject _buttonContainer;

	[SerializeField]
	private Button _bottomButton;

	[SerializeField]
	private ButtonStyleView _bottomButtonStyle;

	[SerializeField]
	private LocalizedText _buttonText;

	[SerializeField]
	private GameObject _lockedContainer;

	[SerializeField]
	private RectTransform _maskTransform;

	private Action _onClick;

	public RectTransform MaskTransform => _maskTransform;

	public void InitPopulatedSlot(BuildingProperties buildingProperties, int level, Action onClick)
	{
		_description.LocalizedString = buildingProperties.LocalizedName;
		_buildingImage.Initialize(buildingProperties);
		_bottomButtonStyle.ApplyStyle(buildingProperties.IsGoldBuilding ? ButtonStyleType.ShopGold : ((buildingProperties is LandmarkBuildingProperties) ? ButtonStyleType.ShopPurple : ButtonStyleType.ShopGreen));
		_levelContainer.SetActive(value: true);
		_level.LocalizedString = Localization.Integer(level);
		_backgroundButton.interactable = true;
		_rays.gameObject.SetActive(value: false);
		_buttonContainer.SetActive(value: true);
		_purchasedContainer.SetActive(value: false);
		_lockedContainer.SetActive(value: false);
		_bottomButton.interactable = true;
		_onClick = onClick;
		_description.gameObject.SetActive(value: true);
		_priceContainer.SetActive(value: false);
		_buttonText.LocalizedString = Localization.Key("warehouse_place_object");
	}

	public void InitUnlockedSlot()
	{
		_buildingImage.Initialize(SingletonMonobehaviour<UISpriteAssetCollection>.Instance.GetAsset(UISpriteType.WarehouseSlot), null, SingletonMonobehaviour<ShopFrameStyleAssetCollection>.Instance.GetAsset(ShopFrameStyleType.Cranes).FrameSprite);
		_bottomButtonStyle.ApplyStyle(ButtonStyleType.ShopGreen);
		_levelContainer.SetActive(value: false);
		_backgroundButton.interactable = false;
		_rays.gameObject.SetActive(value: true);
		_buttonContainer.SetActive(value: false);
		_purchasedContainer.SetActive(value: true);
		_lockedContainer.SetActive(value: false);
		_description.gameObject.SetActive(value: true);
		_priceContainer.SetActive(value: false);
		_description.LocalizedString = Localization.Key("slot_empty");
	}

	public void InitLockedSlot(int price, Action onClick)
	{
		_buildingImage.Initialize(SingletonMonobehaviour<UISpriteAssetCollection>.Instance.GetAsset(UISpriteType.WarehouseSlot), null, SingletonMonobehaviour<ShopFrameStyleAssetCollection>.Instance.GetAsset(ShopFrameStyleType.Cranes).FrameSprite, MaterialType.UIClipGreyscale);
		_levelContainer.SetActive(value: false);
		_onClick = onClick;
		if (onClick != null)
		{
			_bottomButton.interactable = true;
			_backgroundButton.interactable = true;
			_bottomButtonStyle.ApplyStyle(ButtonStyleType.ShopGreen);
			_description.gameObject.SetActive(value: false);
			_priceContainer.SetActive(value: true);
			_price.LocalizedString = Localization.Integer(price);
		}
		else
		{
			_bottomButton.interactable = false;
			_backgroundButton.interactable = false;
			_bottomButtonStyle.ApplyStyle(ButtonStyleType.ShopGrey);
			_description.gameObject.SetActive(value: true);
			_priceContainer.SetActive(value: false);
			_description.LocalizedString = Localization.Key("unlock_previous_slot");
		}
		_buttonContainer.SetActive(value: true);
		_rays.gameObject.SetActive(value: true);
		_purchasedContainer.SetActive(value: false);
		_lockedContainer.SetActive(value: true);
		_buttonText.LocalizedString = Localization.Key("buy");
	}

	public void OnClick()
	{
		_onClick?.Invoke();
	}

	public void SetFrontImageAnimated(bool animated)
	{
		if (animated && !_frontImageTweener.IsPlaying)
		{
			_frontImageTweener.Reset();
			_frontImageTweener.Play();
		}
		else if (!animated && _frontImageTweener.IsPlaying)
		{
			_frontImageTweener.StopAndReset();
		}
	}
}
