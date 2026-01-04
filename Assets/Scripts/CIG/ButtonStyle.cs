using System;
using UnityEngine;

namespace CIG
{
	[Serializable]
	public class ButtonStyle
	{
		[SerializeField]
		private Sprite _buttonImage;

		[SerializeField]
		private TextStyleType _textStyleType;

		public Sprite ButtonImage => _buttonImage;

		public TextStyleType TextStyleType => _textStyleType;
	}
}
