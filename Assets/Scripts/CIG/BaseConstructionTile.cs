using UnityEngine;

namespace CIG
{
	public abstract class BaseConstructionTile : MonoBehaviour
	{
		[SerializeField]
		protected SpriteRenderer _spriteRenderer;

		protected void Initialize(int sortingOrder)
		{
			_spriteRenderer.sortingOrder = sortingOrder;
		}
	}
}
