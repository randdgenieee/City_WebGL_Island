using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class InteractableButtonText : InteractableButtonComponent
	{
		[SerializeField]
		private Text _text;

		[SerializeField]
		private Color _enabledColor;

		[SerializeField]
		private Color _disabledColor;

		protected override void OnInteractable()
		{
			_text.color = _enabledColor;
		}

		protected override void OnNonInteractable()
		{
			_text.color = _disabledColor;
		}
	}
}
