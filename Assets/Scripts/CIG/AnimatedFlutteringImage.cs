using UnityEngine;

namespace CIG
{
	public class AnimatedFlutteringImage : FlutteringImage
	{
		[SerializeField]
		private AnimatedSpriteImage _animatedImage;

		public override Vector3 Extents
		{
			get
			{
				if (_animatedImage.Sprites.Length != 0)
				{
					Sprite sprite = _animatedImage.Sprites[0];
					return sprite.bounds.extents * sprite.pixelsPerUnit;
				}
				return Vector3.zero;
			}
		}

		public override void Initialize()
		{
			_animatedImage.Play();
		}
	}
}
