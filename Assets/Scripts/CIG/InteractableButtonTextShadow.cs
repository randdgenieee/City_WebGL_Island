using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class InteractableButtonTextShadow : InteractableButtonComponent
	{
		[SerializeField]
		private Shadow _shadow;

		[SerializeField]
		private Color _enabledColor;

		[SerializeField]
		private Color _disabledColor;

		protected override void OnInteractable()
		{
			_shadow.effectColor = _enabledColor;
		}

		protected override void OnNonInteractable()
		{
			_shadow.effectColor = _disabledColor;
		}
	}
}
