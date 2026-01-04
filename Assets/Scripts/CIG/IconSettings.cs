using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class IconSettings
	{
		private readonly BuildingProperties _buildingProperties;

		private readonly Sprite _largeIcon;

		private readonly float _largeIconMinWidth;

		private readonly float _largeIconMaxWidth;

		private readonly float _largeIconMinHeight;

		private readonly float _largeIconMaxHeight;

		private readonly Sprite _smallIcon;

		private readonly float _smallIconMinWidth;

		private readonly float _smallIconMaxWidth;

		private readonly float _smallIconMinHeight;

		private readonly float _smallIconMaxHeight;

		private readonly Sprite _backgroundSprite;

		public IconSettings(Sprite backgroundSprite, Sprite largeIcon, float largeIconMinWidth, float largeIconMaxWidth, float largeIconMinHeight, float largeIconMaxHeight, Sprite smallIcon, float smallIconMinWidth, float smallIconMaxWidth, float smallIconMinHeight, float smallIconMaxHeight)
		{
			_backgroundSprite = ((backgroundSprite == null) ? SingletonMonobehaviour<UISpriteAssetCollection>.Instance.GetAsset(UISpriteType.GenericPopupDefaultIconBackground) : backgroundSprite);
			_largeIcon = largeIcon;
			_largeIconMinWidth = largeIconMinWidth;
			_largeIconMaxWidth = largeIconMaxWidth;
			_largeIconMinHeight = largeIconMinHeight;
			_largeIconMaxHeight = largeIconMaxHeight;
			_smallIcon = smallIcon;
			_smallIconMinWidth = smallIconMinWidth;
			_smallIconMaxWidth = smallIconMaxWidth;
			_smallIconMinHeight = smallIconMinHeight;
			_smallIconMaxHeight = smallIconMaxHeight;
		}

		public IconSettings(BuildingProperties buildingProperties, float largeIconMinWidth, float largeIconMaxWidth, float largeIconMinHeight, float largeIconMaxHeight, Sprite smallIcon, float smallIconMinWidth, float smallIconMaxWidth, float smallIconMinHeight, float smallIconMaxHeight)
		{
			_buildingProperties = buildingProperties;
			_largeIconMinWidth = largeIconMinWidth;
			_largeIconMaxWidth = largeIconMaxWidth;
			_largeIconMinHeight = largeIconMinHeight;
			_largeIconMaxHeight = largeIconMaxHeight;
			_smallIcon = smallIcon;
			_smallIconMinWidth = smallIconMinWidth;
			_smallIconMaxWidth = smallIconMaxWidth;
			_smallIconMinHeight = smallIconMinHeight;
			_smallIconMaxHeight = smallIconMaxHeight;
		}

		public bool ApplyTo(BuildingImage buildingImage, Image smallImage)
		{
			if (_buildingProperties != null || _largeIcon != null)
			{
				if (_buildingProperties != null)
				{
					buildingImage.Initialize(_buildingProperties, MaterialType.UIClip, _largeIconMaxWidth);
				}
				else
				{
					buildingImage.Initialize(_largeIcon, _backgroundSprite, null);
				}
				ClampIconSize(buildingImage.MainImage, _largeIconMinWidth, _largeIconMaxWidth, _largeIconMinHeight, _largeIconMaxHeight);
				smallImage.sprite = _smallIcon;
				smallImage.gameObject.SetActive(_smallIcon != null);
				ClampIconSize(smallImage, _smallIconMinWidth, _smallIconMaxWidth, _smallIconMinHeight, _smallIconMaxHeight);
				return true;
			}
			return false;
		}

		private void ClampIconSize(Image image, float minWidth, float maxWidth, float minHeight, float maxHeight)
		{
			if (!(image.sprite == null))
			{
				float x = Mathf.Clamp(image.sprite.rect.width, minWidth, maxWidth);
				float y = Mathf.Clamp(image.sprite.rect.height, minHeight, maxHeight);
				image.rectTransform.sizeDelta = new Vector2(x, y);
			}
		}
	}
}
