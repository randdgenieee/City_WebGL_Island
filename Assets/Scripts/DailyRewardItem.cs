using CIG.Translation;
using System;
using System.Collections;
using Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewardItem : MonoBehaviour
{
	public enum ItemState
	{
		Collected,
		Active,
		Inactive
	}

	[SerializeField]
	private Image _panel;

	[SerializeField]
	private Image _icon;

	[SerializeField]
	private LocalizedText _dayLabel;

	[SerializeField]
	private Image _currencyIcon;

	[SerializeField]
	private LocalizedText _amountLabel;

	[SerializeField]
	private Tweener _raysTweener;

	[SerializeField]
	private Tweener _pulseTweener;

	[SerializeField]
	private GameObject _todayBanner;

	[SerializeField]
	private GameObject _checkmark;

	[SerializeField]
	private Button _collectButton;

	[SerializeField]
	private Button _backgroundButton;

	private Action _collectCallback;

	private IEnumerator _collectAnimationRoutine;

	public void Show(DailyRewardPopup.DailyRewardItemStyle style, int dayIndex, ILocalizedString amount, ItemState state, Action collectCallback)
	{
		_collectCallback = collectCallback;
		_panel.sprite = style.PanelSprite;
		_icon.sprite = style.IconSprite;
		_dayLabel.LocalizedString = Localization.Format(Localization.Key("day_x"), Localization.Integer(dayIndex + 1));
		_currencyIcon.sprite = style.CurrencySprite;
		_amountLabel.LocalizedString = amount;
		_todayBanner.SetActive(state == ItemState.Active);
		_checkmark.SetActive(state == ItemState.Collected);
		_collectButton.gameObject.SetActive(state == ItemState.Active || state == ItemState.Inactive);
		_collectButton.interactable = (state == ItemState.Active);
		_backgroundButton.interactable = (state == ItemState.Active);
		if (state == ItemState.Active)
		{
			_raysTweener.Play();
			_pulseTweener.Play();
		}
		else
		{
			_raysTweener.StopAndReset();
			_pulseTweener.StopAndReset();
		}
	}

	public void OnCollectClicked()
	{
		_collectCallback();
	}
}
