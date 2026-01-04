using UnityEngine;

namespace CIG
{
	public class ConstructionFloorTile : BaseConstructionTile
	{
		public void Initialize(Sprite sprite, int sortingOrder)
		{
			Initialize(sortingOrder);
			_spriteRenderer.sprite = sprite;
		}
	}
}
