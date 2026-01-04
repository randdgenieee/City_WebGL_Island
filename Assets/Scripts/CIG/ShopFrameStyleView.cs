using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class ShopFrameStyleView : MonoBehaviour
	{
		[SerializeField]
		private ShopFrameStyleType _initialShopFrameStyle = ShopFrameStyleType.None;

		[SerializeField]
		private TextStyleView _textStyleView;

		[SerializeField]
		private Image _frameImage;

		[SerializeField]
		private Image _headerImage;

		private void Start()
		{
			ApplyStyle(_initialShopFrameStyle);
		}

		public void ApplyStyle(ShopFrameStyleType styleType)
		{
			if (styleType != ShopFrameStyleType.None)
			{
				ShopFrameStyle asset = SingletonMonobehaviour<ShopFrameStyleAssetCollection>.Instance.GetAsset(styleType);
				ApplyStyle(asset);
			}
		}

		private void ApplyStyle(ShopFrameStyle style)
		{
			_frameImage.sprite = style.FrameSprite;
			_headerImage.color = style.HeaderColor;
			_textStyleView.ApplyStyle(style.HeaderTextStyle);
		}
	}
}
