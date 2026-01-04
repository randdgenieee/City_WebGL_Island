using UnityEngine;

namespace CIG
{
	public abstract class ChildRenderer : MonoBehaviour
	{
		[SerializeField]
		private bool _canCopy = true;

		public bool CanCopy => _canCopy;

		public abstract void SetSprites(Sprite[] sprites);

		public abstract void SetMaterial(Material material);

		public abstract void SetColor(Color color);

		public abstract void SetHidden(bool hidden);

		public abstract void SetPaused(bool paused);
	}
}
