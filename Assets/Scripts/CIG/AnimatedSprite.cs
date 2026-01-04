using UnityEngine;

namespace CIG
{
	public class AnimatedSprite : AnimatedSpriteBase
	{
		[SerializeField]
		private SpriteRenderer _spriteRenderer;

		private void Awake()
		{
			if (_spriteRenderer == null)
			{
				UnityEngine.Debug.LogWarningFormat("Did you forget to link the SpriteRenderer for '{0}'?", base.name);
			}
		}

		protected override void UpdateSprite(Sprite sprite)
		{
			_spriteRenderer.sprite = sprite;
		}
	}
}
