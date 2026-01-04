using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class AnimatedSpriteImage : AnimatedSpriteBase
	{
		[SerializeField]
		private Image _image;

		public Image Image => _image;

		private void Awake()
		{
			if (_image == null)
			{
				UnityEngine.Debug.LogWarningFormat("Did you forget to link the Image for '{0}'?", base.name);
			}
		}

		protected override void UpdateSprite(Sprite sprite)
		{
			_image.sprite = sprite;
			_image.enabled = (sprite != null);
		}
	}
}
