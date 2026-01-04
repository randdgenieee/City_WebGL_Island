using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class ButtonStyleView : MonoBehaviour
	{
		[SerializeField]
		private ButtonStyleType _initialButtonStyle = ButtonStyleType.None;

		[SerializeField]
		private Image _buttonImage;

		[SerializeField]
		private TextStyleView _text;

		private void Start()
		{
			ApplyStyle(_initialButtonStyle);
		}

		public void ApplyStyle(ButtonStyleType style)
		{
			if (style != ButtonStyleType.None)
			{
				ButtonStyle asset = SingletonMonobehaviour<ButtonStyleAssetCollection>.Instance.GetAsset(style);
				ApplyStyle(asset);
			}
		}

		private void ApplyStyle(ButtonStyle buttonStyle)
		{
			_buttonImage.sprite = buttonStyle.ButtonImage;
			_text.ApplyStyle(buttonStyle.TextStyleType);
		}
	}
}
