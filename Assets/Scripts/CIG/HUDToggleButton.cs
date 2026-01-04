using UnityEngine;

namespace CIG
{
	public class HUDToggleButton : HUDRegionElement
	{
		[SerializeField]
		private GameObject _activeObject;

		public void SetActive(bool active)
		{
			_activeObject.SetActive(active);
		}
	}
}
