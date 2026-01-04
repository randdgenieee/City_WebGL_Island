using UnityEngine;

namespace CIG
{
	public abstract class MovingSpriteAgent : MovingAgent
	{
		[SerializeField]
		protected SpriteRenderer _spriteRenderer;

		[SerializeField]
		[Tooltip("Optional")]
		private AnimatedSprite _animatedSprite;

		[SerializeField]
		private ChildSpriteRenderer[] _childSpriteRenderers;

		protected override void SetSprites(MovingAgentSprite sprites)
		{
			if (_animatedSprite != null)
			{
				_animatedSprite.ReplaceSprites(sprites.Sprites);
			}
			else
			{
				_spriteRenderer.sprite = sprites.Sprites[0];
			}
			Vector3 localScale = _spriteRenderer.transform.localScale;
			localScale.x = (sprites.IsFlipped ? (0f - Mathf.Abs(localScale.x)) : Mathf.Abs(localScale.x));
			_spriteRenderer.transform.localScale = localScale;
			int i = 0;
			for (int num = _childSpriteRenderers.Length; i < num; i++)
			{
				_childSpriteRenderers[i].SetSprites(sprites.ChildSprites[i].Sprites);
			}
		}

		protected override void SetAlpha(float alpha)
		{
			Color color = _spriteRenderer.color;
			color.a = alpha;
			_spriteRenderer.color = color;
		}
	}
}
