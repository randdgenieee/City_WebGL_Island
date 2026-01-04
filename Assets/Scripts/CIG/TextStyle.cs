using System;
using UnityEngine;

namespace CIG
{
	[Serializable]
	public class TextStyle
	{
		[SerializeField]
		private Color _textColor;

		[SerializeField]
		private int _fontSize;

		[SerializeField]
		private bool _hasShadow;

		[SerializeField]
		[ShowWhen("_hasShadow", ComparisonType.Equals, new object[]
		{
			true
		})]
		private Color _shadowColor;

		[SerializeField]
		[ShowWhen("_hasShadow", ComparisonType.Equals, new object[]
		{
			true
		})]
		private Vector2 _shadowSize;

		[SerializeField]
		private bool _hasOutline;

		[SerializeField]
		[ShowWhen("_hasOutline", ComparisonType.Equals, new object[]
		{
			true
		})]
		private Color _outlineColor;

		[SerializeField]
		[ShowWhen("_hasOutline", ComparisonType.Equals, new object[]
		{
			true
		})]
		private Vector2 _outlineSize;

		public Color TextColor => _textColor;

		public int FontSize => _fontSize;

		public bool HasShadow => _hasShadow;

		public Color ShadowColor => _shadowColor;

		public Vector2 ShadowSize => _shadowSize;

		public bool HasOutline => _hasOutline;

		public Color OutlineColor => _outlineColor;

		public Vector2 OutlineSize => _outlineSize;
	}
}
