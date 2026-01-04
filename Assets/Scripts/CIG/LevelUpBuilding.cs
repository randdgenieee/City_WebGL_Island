using UnityEngine;

namespace CIG
{
	public class LevelUpBuilding : RewardScrollRectItem
	{
		[SerializeField]
		private BuildingImage _buildingImage;

		[SerializeField]
		private GameObject _newLabel;

		[SerializeField]
		private LocalizedText _name;

		[SerializeField]
		private ButtonStyleView _shopButtonStyle;

		private PopupManager _popupManager;

		private BuildingProperties _buildingProperties;

		public void Initialize(PopupManager popupManager, int level, BuildingProperties buildingProperties)
		{
			_popupManager = popupManager;
			_buildingProperties = buildingProperties;
			base.name = GetType().Name + ":" + _buildingProperties.BaseKey;
			_buildingImage.Initialize(_buildingProperties);
			_newLabel.SetActive(_buildingProperties.UnlockLevels[0] == level);
			_name.LocalizedString = _buildingProperties.LocalizedName;
			_shopButtonStyle.ApplyStyle(_buildingProperties.IsGoldBuilding ? ButtonStyleType.ShopGold : ButtonStyleType.ShopGreen);
		}

		public void OnClick()
		{
			BuildingPopupRequest request = new BuildingPopupRequest(_buildingProperties, BuildingPopupContent.Preview);
			_popupManager.RequestPopup(request);
		}
	}
}
