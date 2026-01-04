using UnityEngine;

namespace CIG
{
	public class ChildSpriteRenderer : ChildRenderer
	{
		[SerializeField]
		private SpriteRenderer _spriteRenderer;

		[SerializeField]
		[Tooltip("Optional")]
		private AnimatedSprite _animatedSprite;

		[SerializeField]
		private int _sortingOrderOffset;

		public AnimatedSprite AnimatedSprite => _animatedSprite;

		public SpriteRenderer SpriteRenderer => _spriteRenderer;

		public void UpdateSortingOrder(int sortingOrder, string sortingLayer)
		{
			_spriteRenderer.sortingOrder = sortingOrder + _sortingOrderOffset;
			_spriteRenderer.sortingLayerName = sortingLayer;
		}

		public override void SetSprites(Sprite[] sprites)
		{
			if (_animatedSprite == null)
			{
				_spriteRenderer.sprite = sprites[0];
			}
			else
			{
				_animatedSprite.ReplaceSprites(sprites);
			}
		}

		public override void SetMaterial(Material material)
		{
			_spriteRenderer.material = material;
		}

		public override void SetColor(Color color)
		{
			_spriteRenderer.color = color;
		}

		public override void SetHidden(bool hidden)
		{
			_spriteRenderer.enabled = !hidden;
		}

		public override void SetPaused(bool paused)
		{
			if (_animatedSprite != null)
			{
				if (paused)
				{
					_animatedSprite.Stop();
				}
				else if (_animatedSprite.Sprites.Length != 0)
				{
					_animatedSprite.Play();
				}
			}
		}
	}
}
