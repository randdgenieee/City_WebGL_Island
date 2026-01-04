using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class InteractableButtonMaterial : InteractableButtonComponent
	{
		[SerializeField]
		private Graphic _graphic;

		[SerializeField]
		private Material _enabledMaterial;

		[SerializeField]
		private Material _disabledMaterial;

		protected override void OnInteractable()
		{
			_graphic.material = _enabledMaterial;
		}

		protected override void OnNonInteractable()
		{
			_graphic.material = _disabledMaterial;
		}
	}
}
