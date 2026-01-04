using UnityEngine;

namespace CIG
{
	public class HUDRegionUpdater : MonoBehaviour
	{
		[SerializeField]
		private HUDRegion[] _hudRegions;

		public void RequestHide(object requester, HUDRegionType regions)
		{
			int i = 0;
			for (int num = _hudRegions.Length; i < num; i++)
			{
				HUDRegion hUDRegion = _hudRegions[i];
				if (Contains(regions, hUDRegion.HudRegion))
				{
					hUDRegion.RequestHide(requester);
				}
			}
		}

		public void RequestShow(object requester)
		{
			int i = 0;
			for (int num = _hudRegions.Length; i < num; i++)
			{
				HUDRegion hUDRegion = _hudRegions[i];
				if (Contains(HUDRegionType.All, hUDRegion.HudRegion))
				{
					hUDRegion.RequestShow(requester);
				}
			}
		}

		private bool Contains(HUDRegionType regions, HUDRegionType region)
		{
			return (regions & region) != HUDRegionType.None;
		}
	}
}
