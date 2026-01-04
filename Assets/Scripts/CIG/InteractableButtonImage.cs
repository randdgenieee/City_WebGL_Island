using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class InteractableButtonImage : InteractableButtonComponent
	{
		[SerializeField]
		private Image _image;

		[SerializeField]
		private Sprite _enabledImage;

		[SerializeField]
		private Sprite _disabledImage;

		protected override void OnInteractable()
		{
			_image.sprite = _enabledImage;
		}

		protected override void OnNonInteractable()
		{
			_image.sprite = _disabledImage;
		}
	}
}
