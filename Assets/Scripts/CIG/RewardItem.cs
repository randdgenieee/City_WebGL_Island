using UnityEngine;

namespace CIG
{
	public class RewardItem : RewardScrollRectItem
	{
		[SerializeField]
		private BuildingImage _image;

		[SerializeField]
		private LocalizedText _text;

		protected virtual float MaxImageWidth => 175f;

		public virtual void Initialize(RewardItemData data)
		{
			if (data.BuildingProperties == null)
			{
				_image.Initialize(data.Sprite, null, null, MaterialType.UITransparent, MaxImageWidth);
				_text.LocalizedString = data.Text;
			}
			else
			{
				_image.Initialize(data.BuildingProperties, MaterialType.UITransparent, MaxImageWidth);
				_text.LocalizedString = data.BuildingProperties.LocalizedName;
			}
		}
	}
}
