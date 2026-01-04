using UnityEngine;

namespace CIG
{
	public class ConstructionScaffolding : BaseConstructionTile
	{
		private Sprite[] _sprites;

		public void Initialize(Sprite[] sprites, int sortingOrder)
		{
			Initialize(sortingOrder);
			_sprites = sprites;
			SetStage(0);
		}

		public void SetStage(int stage)
		{
			_spriteRenderer.sprite = _sprites[Mathf.Clamp(stage, 0, _sprites.Length - 1)];
		}
	}
}
