using System;
using UnityEngine;

namespace CIG
{
	public class BuildingButtonsOverlay : Overlay
	{
		[SerializeField]
		private OverlayButton _infoButton;

		[SerializeField]
		private OverlayButton _moveButton;

		[SerializeField]
		private OverlayButton _upgradeButton;

		[SerializeField]
		private OverlayButton _demolishButton;

		public void Initialize(bool infoActive, Action infoAction, bool moveActive, Action moveAction, bool upgradeActive, Action upgradeAction, bool demolishActive, Action demolishAction)
		{
			_infoButton.Initialize(infoActive, infoAction);
			_moveButton.Initialize(moveActive, moveAction);
			_upgradeButton.Initialize(upgradeActive, upgradeAction);
			_demolishButton.Initialize(demolishActive, demolishAction);
		}
	}
}
