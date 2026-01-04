using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class ChildImage : ChildRenderer
	{
		[SerializeField]
		private Image _image;

		[SerializeField]
		[Tooltip("Optional")]
		private AnimatedSpriteImage _animatedSpriteImage;

		public override void SetSprites(Sprite[] sprites)
		{
			if (_animatedSpriteImage == null)
			{
				_image.sprite = sprites[0];
			}
			else
			{
				_animatedSpriteImage.ReplaceSprites(sprites);
			}
		}

		public override void SetMaterial(Material material)
		{
			_image.material = material;
		}

		public override void SetColor(Color color)
		{
			_image.material.color = color;
		}

		public override void SetHidden(bool hidden)
		{
			_image.enabled = !hidden;
		}

		public override void SetPaused(bool paused)
		{
			if (_animatedSpriteImage != null)
			{
				if (paused)
				{
					_animatedSpriteImage.Stop();
				}
				else if (_animatedSpriteImage.Sprites.Length != 0)
				{
					_animatedSpriteImage.Play();
				}
			}
		}
	}
}
