using UnityEngine;

namespace CIG
{
	public class InteractableButtonStyle : InteractableButtonComponent
	{
		[SerializeField]
		private ButtonStyleView _buttonStyle;

		[SerializeField]
		private ButtonStyleType _enabledStyle;

		[SerializeField]
		private ButtonStyleType _disabledStyle;

		protected override void OnInteractable()
		{
			_buttonStyle.ApplyStyle(_enabledStyle);
		}

		protected override void OnNonInteractable()
		{
			_buttonStyle.ApplyStyle(_disabledStyle);
		}
	}
}
