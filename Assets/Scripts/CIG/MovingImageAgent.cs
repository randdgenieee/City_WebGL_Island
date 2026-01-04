using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public abstract class MovingImageAgent : MovingAgent
	{
		[SerializeField]
		private Image _image;

		[SerializeField]
		[Tooltip("Optional")]
		private AnimatedSpriteImage _animatedSpriteImage;

		[SerializeField]
		private ChildImage[] _childImages;

		protected override void SetSprites(MovingAgentSprite sprites)
		{
			if (_animatedSpriteImage != null)
			{
				_animatedSpriteImage.ReplaceSprites(sprites.Sprites);
			}
			else
			{
				_image.sprite = sprites.Sprites[0];
			}
			Vector3 localScale = _image.transform.localScale;
			localScale.x = (sprites.IsFlipped ? (0f - Mathf.Abs(localScale.x)) : Mathf.Abs(localScale.x));
			_image.transform.localScale = localScale;
			int i = 0;
			for (int num = _childImages.Length; i < num; i++)
			{
				_childImages[i].SetSprites(sprites.ChildSprites[i].Sprites);
			}
		}

		protected override void SetAlpha(float alpha)
		{
			Color color = _image.color;
			color.a = alpha;
			_image.color = color;
		}
	}
}
