using CIG.Translation;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	public class IslandSelectedOverlay : Overlay
	{
		[SerializeField]
		private OverlayButton _openButton;

		[SerializeField]
		private OverlayButton _unlockButton;

		[SerializeField]
		private OverlayButton _moveButton;

		[SerializeField]
		private SurfaceTypeInfoView _surfaceTypeInfoViewPrefab;

		[SerializeField]
		private RectTransform _surfaceTypeInfoParent;

		[SerializeField]
		private LocalizedText _title;

		[SerializeField]
		private LocalizedText _airshipAndIslandStatusText;

		[SerializeField]
		private LocalizedText _airshipToFarAwayText;

		public OverlayButton UnlockButton => _unlockButton;

		public OverlayButton OpenButton => _openButton;

		public void Initialize(bool openActive, Action openAction, bool unlockEnabled, bool unlockActive, Action unlockAction, bool moveEnabled, bool moveActive, Action moveAction, List<SurfaceTypeInfo> surfaceTypeInfo, bool airshipOnIsland)
		{
			_openButton.Initialize(openActive, openAction);
			_openButton.gameObject.SetActive(openActive);
			UnlockButton.Initialize(unlockEnabled, unlockAction);
			UnlockButton.gameObject.SetActive(unlockActive);
			_moveButton.Initialize(moveEnabled, moveAction);
			_moveButton.gameObject.SetActive(moveActive);
			_title.LocalizedString = (unlockActive ? Localization.Key("undiscovered_island") : Localization.Key("discovered_island"));
			bool flag = (unlockActive | moveActive) && !airshipOnIsland;
			_airshipAndIslandStatusText.gameObject.SetActive(flag && unlockActive);
			if (flag && unlockActive)
			{
				_airshipAndIslandStatusText.LocalizedString = Localization.Key("unlock_this_island");
			}
			bool flag2 = (!unlockEnabled || !moveEnabled) & flag;
			_airshipToFarAwayText.gameObject.SetActive(flag2);
			if (flag2)
			{
				_airshipToFarAwayText.LocalizedString = (unlockActive ? Localization.Key("tutorial_move_airship") : Localization.Key("move_airship_to_send"));
			}
			FillInIslandInformation(surfaceTypeInfo);
		}

		public void FillInIslandInformation(List<SurfaceTypeInfo> surfaceTypeInfo)
		{
			int i = 0;
			for (int count = surfaceTypeInfo.Count; i < count; i++)
			{
				SurfaceTypeInfo surfaceTypeInfo2 = surfaceTypeInfo[i];
				UnityEngine.Object.Instantiate(_surfaceTypeInfoViewPrefab, _surfaceTypeInfoParent).Initialize(surfaceTypeInfo2.SurfaceType, surfaceTypeInfo2.Percentage);
			}
		}
	}
}
