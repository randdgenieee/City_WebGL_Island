using UnityEngine;

namespace CIG
{
	public class HUDRegionElement : MonoBehaviour
	{
		protected bool _regionShowing;

		public void SetRegionShowing(bool isShowing)
		{
			_regionShowing = isShowing;
			UpdateVisibility();
		}

		protected virtual void UpdateVisibility()
		{
			base.gameObject.SetActive(_regionShowing);
		}
	}
}
