using CIG;
using CIG.Translation;
using System;
using UnityEngine;
using UnityEngine.UI;

public class RoadSelectionPopup : Popup
{
	[Serializable]
	private class RoadButton
	{
		[SerializeField]
		private RoadType _roadType;

		[SerializeField]
		private Image _buttonImage;

		public RoadType RoadType => _roadType;

		public Image ButtonImage => _buttonImage;
	}

	[SerializeField]
	private RoadButton[] _roadButtons;

	[SerializeField]
	private Sprite _selectedSprite;

	[SerializeField]
	private Sprite _deselectedSprite;

	private RoadBuilder _roadBuilder;

	public override string AnalyticsScreenName => "road_building";

	public override void Open(PopupRequest request)
	{
		base.Open(request);
		_roadBuilder = IsometricIsland.Current.RoadBuilder;
		GameEvents.Invoke(new UnemiShouldCloseEvent(this));
		SelectRoad();
	}

	public void OnRoadClicked()
	{
		SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.ButtonClick);
		SelectRoad();
	}

	public void OnWalkwayClicked()
	{
		SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.ButtonClick);
		_roadBuilder.BeginRoad(RoadType.Path);
		SetButtonSelected(RoadType.Path);
	}

	public void OnRiverClicked()
	{
		SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.ButtonClick);
		_roadBuilder.BeginRoad(RoadType.River);
		SetButtonSelected(RoadType.River);
	}

	public void OnDemolishClicked()
	{
		SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.ButtonClick);
		_roadBuilder.BeginRoad(RoadType.None);
		SetButtonSelected(RoadType.None);
	}

	public void OnConfirmRoadBuildClicked()
	{
		_roadBuilder.ApplyRoads();
		OnCloseClicked();
	}

	public void OnCancelRoadBuildClicked()
	{
		if (_roadBuilder.HasChanges)
		{
			GenericPopupRequest request = new GenericPopupRequest("road_building_confirm").SetTexts(Localization.Key("conflict_title"), Localization.Key("confirm_undo")).SetGreenButton(Localization.Key("timewarp_confirm"), null, CancelRoads).SetRedButton(Localization.Key("timewarp_cancel"));
			_popupManager.RequestPopup(request);
		}
		else
		{
			CancelRoads();
		}
	}

	protected override void Closed()
	{
		_roadBuilder = null;
		base.Closed();
	}

	private void CancelRoads()
	{
		if (_roadBuilder != null)
		{
			_roadBuilder.CancelRoads();
		}
		OnCloseClicked();
	}

	private void SelectRoad()
	{
		IsometricIsland.Current.RoadBuilder.BeginRoad(RoadType.Road);
		SetButtonSelected(RoadType.Road);
	}

	private void SetButtonSelected(RoadType roadType)
	{
		int i = 0;
		for (int num = _roadButtons.Length; i < num; i++)
		{
			RoadButton roadButton = _roadButtons[i];
			if (roadButton.RoadType == roadType)
			{
				roadButton.ButtonImage.sprite = _selectedSprite;
			}
			else
			{
				roadButton.ButtonImage.sprite = _deselectedSprite;
			}
		}
	}
}
