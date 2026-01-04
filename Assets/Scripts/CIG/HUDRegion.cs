using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	public class HUDRegion : MonoBehaviour
	{
		[SerializeField]
		private HUDRegionType _hudRegion;

		[SerializeField]
		private HUDRegionElement[] _hudRegionElements;

		private readonly List<object> _requesters = new List<object>();

		public HUDRegionType HudRegion => _hudRegion;

		public void RequestHide(object requester)
		{
			_requesters.Add(requester);
			SetShowing(isShowing: false);
		}

		public void RequestShow(object requester)
		{
			_requesters.Remove(requester);
			if (_requesters.Count == 0)
			{
				SetShowing(isShowing: true);
			}
		}

		private void SetShowing(bool isShowing)
		{
			int i = 0;
			for (int num = _hudRegionElements.Length; i < num; i++)
			{
				_hudRegionElements[i].SetRegionShowing(isShowing);
			}
		}
	}
}
