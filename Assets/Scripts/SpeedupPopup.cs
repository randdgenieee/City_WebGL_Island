using CIG;
using CIG.Translation;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SpeedupPopup : Popup
{
	[SerializeField]
	private LocalizedText _titleLabel;

	[SerializeField]
	private LocalizedText _subtitleLabel;

	[SerializeField]
	private LocalizedText _bodyLabel;

	[SerializeField]
	private LocalizedText _speedupButtonLabel;

	[SerializeField]
	private LocalizedText _cancelButtonLabel;

	[SerializeField]
	private Image _cancelButtonIcon;

	[SerializeField]
	private LocalizedText _timerLabel;

	[SerializeField]
	private BuildingImage _buildingImage;

	[SerializeField]
	private Image _smallIcon;

	private Timing _timing;

	private string _analyticsScreenName;

	private UpspeedableProcess _upspeedableProcess;

	private Action _cancelButtonAction;

	private IEnumerator _timerRoutine;

	public override string AnalyticsScreenName => _analyticsScreenName;

	public override void Initialize(Model model, CIGCanvasScaler canvasScaler)
	{
		base.Initialize(model, canvasScaler);
		_timing = model.Game.Timing;
	}

	public override void Open(PopupRequest request)
	{
		base.Open(request);
		SpeedupPopupRequest request2 = GetRequest<SpeedupPopupRequest>();
		_analyticsScreenName = $"speedup_{request2.AnalyticsScreenName}";
		_upspeedableProcess = request2.UpspeedableProcess;
		_titleLabel.LocalizedString = request2.Title;
		_subtitleLabel.LocalizedString = request2.Subtitle;
		request2.CancelButton.ApplyTo(_cancelButtonLabel, _timerLabel, _cancelButtonIcon, out _cancelButtonAction);
		request2.IconSettings.ApplyTo(_buildingImage, _smallIcon);
		if (_timerRoutine != null)
		{
			StopCoroutine(_timerRoutine);
		}
		StartCoroutine(_timerRoutine = TimerRoutine());
	}

	public override void Close(bool instant)
	{
		if (_timerRoutine != null)
		{
			StopCoroutine(_timerRoutine);
			_timerRoutine = null;
		}
		base.Close(instant);
	}

	public void OnSpeedupClicked()
	{
		if (_upspeedableProcess.PaidSpeedup())
		{
			OnCloseClicked();
		}
	}

	public void OnCancelClicked()
	{
		OnCloseClicked();
		if (_cancelButtonAction != null)
		{
			_cancelButtonAction();
			_cancelButtonAction = null;
		}
	}

	private IEnumerator TimerRoutine()
	{
		while (_upspeedableProcess.keepWaiting)
		{
			_timerLabel.LocalizedString = Localization.TimeSpan(TimeSpan.FromSeconds(Mathf.RoundToInt((float)_upspeedableProcess.TimeLeft)), hideSecondPartWhenZero: true);
			ILocalizedString localizedString = Localization.Integer(_upspeedableProcess.UpspeedCost.Value);
			_speedupButtonLabel.LocalizedString = localizedString;
			_bodyLabel.LocalizedString = Localization.Format(Localization.Key("building_speedup"), localizedString);
			yield return new WaitForGameTimeSeconds(_timing, 1.0);
		}
		OnCloseClicked();
	}
}
