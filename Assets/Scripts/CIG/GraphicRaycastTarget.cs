using UnityEngine.UI;

namespace CIG
{
	public class GraphicRaycastTarget : Graphic
	{
		protected override void Awake()
		{
			base.Awake();
			raycastTarget = true;
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
		}
	}
}
