using UnityEngine;

namespace CIG
{
	public class SpriteRendererPlingManager : PlingManager
	{
		[SerializeField]
		private Renderer _renderer;

		protected override Vector3 PlingPosition
		{
			get
			{
				Vector3 center = _renderer.bounds.center;
				center.y = _renderer.bounds.max.y;
				return center;
			}
		}
	}
}
