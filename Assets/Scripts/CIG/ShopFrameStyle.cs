using System;
using UnityEngine;

namespace CIG
{
	[Serializable]
	public class ShopFrameStyle
	{
		[SerializeField]
		private Sprite _frameSprite;

		[SerializeField]
		private Color _headerColor;

		[SerializeField]
		private TextStyleType _headerTextStyle;

		public Sprite FrameSprite => _frameSprite;

		public Color HeaderColor => _headerColor;

		public TextStyleType HeaderTextStyle => _headerTextStyle;
	}
}
