using CIG;
using System;
using UnityEngine;
using UnityEngine.UI;

public class GenericPopup : Popup
{
	[SerializeField]
	private Button _closeButton;

	[SerializeField]
	private LocalizedText _titleLabel;

	[SerializeField]
	private LocalizedText _subtitleLabel;

	[SerializeField]
	private LocalizedText _bodyLabel;

	[SerializeField]
	private GameObject _greenButtonRoot;

	[SerializeField]
	private LocalizedText _greenLabel;

	[SerializeField]
	private LocalizedText _greenUpperLabel;

	[SerializeField]
	private Image _greenIcon;

	[SerializeField]
	private GameObject _redButtonRoot;

	[SerializeField]
	private LocalizedText _redLabel;

	[SerializeField]
	private LocalizedText _redUpperLabel;

	[SerializeField]
	private Image _redIcon;

	[SerializeField]
	private BuildingImage _buildingImage;

	[SerializeField]
	private Image _smallIcon;

	[SerializeField]
	private GameObject _iconRoot;

	private string _analyticsScreenName;

	private Action _greenButtonAction;

	private Action _redButtonAction;

	private Action _closeAction;

	public override string AnalyticsScreenName => _analyticsScreenName;

	public override void Open(PopupRequest request)
	{
		base.Open(request);
		GenericPopupRequest request2 = GetRequest<GenericPopupRequest>();
		_analyticsScreenName = request2.AnalyticsScreenName;
		_closeAction = request2.CloseAction;
		_titleLabel.LocalizedString = request2.Title;
		_subtitleLabel.gameObject.SetActive(request2.Subtitle != null);
		_subtitleLabel.LocalizedString = request2.Subtitle;
		_bodyLabel.gameObject.SetActive(request2.Body != null);
		_bodyLabel.LocalizedString = request2.Body;
		SetButton(request2.GreenButton, _greenButtonRoot, _greenLabel, _greenUpperLabel, _greenIcon, out _greenButtonAction);
		SetButton(request2.RedButton, _redButtonRoot, _redLabel, _redUpperLabel, _redIcon, out _redButtonAction);
		SetIcons(request2.IconSettings);
		_closeButton.gameObject.SetActive(request2.Dismissable);
	}

	public override void OnCloseClicked()
	{
		Action closeAction = _closeAction;
		_closeAction = null;
		ClosePopup();
		EventTools.Fire(closeAction);
	}

	public void OnGreenButtonClicked()
	{
		Action greenButtonAction = _greenButtonAction;
		_greenButtonAction = null;
		ClosePopup();
		EventTools.Fire(greenButtonAction);
	}

	public void OnRedButtonClicked()
	{
		Action redButtonAction = _redButtonAction;
		_redButtonAction = null;
		ClosePopup();
		EventTools.Fire(redButtonAction);
	}

	private void SetButton(ButtonSettings settings, GameObject button, LocalizedText text, LocalizedText upperText, Image icon, out Action action)
	{
		bool flag = settings != null;
		button.SetActive(flag);
		if (flag)
		{
			settings.ApplyTo(text, upperText, icon, out action);
		}
		else
		{
			action = null;
		}
	}

	private void SetIcons(IconSettings settings)
	{
		bool flag = settings?.ApplyTo(_buildingImage, _smallIcon) ?? false;
		_iconRoot.gameObject.SetActive(flag);
		if (flag)
		{
			_titleLabel.TextField.alignment = TextAnchor.MiddleLeft;
			_bodyLabel.TextField.alignment = TextAnchor.MiddleLeft;
		}
		else
		{
			_titleLabel.TextField.alignment = TextAnchor.MiddleCenter;
			_bodyLabel.TextField.alignment = TextAnchor.MiddleCenter;
		}
	}

	private void ClosePopup()
	{
		base.OnCloseClicked();
	}
}
