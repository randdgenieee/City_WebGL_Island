using CIG;
using CIG.Translation;
using System;
using Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TutorialDialog : MonoBehaviour
{
	public enum AdvisorPositionType
	{
		Left,
		Right
	}

	[Serializable]
	private class AdvisorPosition
	{
		[SerializeField]
		private AdvisorPositionType _positionType;

		[SerializeField]
		private RectTransform _advisorImage;

		[SerializeField]
		private Tweener _advisorTweener;

		[SerializeField]
		private LocalizedText _dialogBodyText;

		[SerializeField]
		private Tweener _dialogTweener;

		[SerializeField]
		private GameObject _continueButton;

		public AdvisorPositionType PositionType => _positionType;

		public RectTransform AdvisorImage => _advisorImage;

		public Tweener AdvisorTweener => _advisorTweener;

		public Tweener DialogTweener => _dialogTweener;

		public LocalizedText DialogBodyText => _dialogBodyText;

		public GameObject ContinueButton => _continueButton;
	}

	[SerializeField]
	private AdvisorPosition[] _advisorPositions;

	[SerializeField]
	private Image _overlayImage;

	[SerializeField]
	private Tweener _overlayTweener;

	private AdvisorPosition _currentAdvisorPosition;

	private AdvisorPositionType? _newPositionType;

	private ILocalizedString _dialogText;

	private Action _continueCallback;

	private bool _useOverlay;

	private bool _useContinueButton;

	private bool _isAdvisorShowing;

	private bool _isDialogShowing;

	private bool _isOverlayShowing;

	public void Initialize()
	{
		_overlayTweener.FinishedPlaying += OnOverlayTweenerFinishedPlaying;
		HideTutorialDialog(animated: false);
	}

	private void OnDestroy()
	{
		if (_overlayTweener != null)
		{
			_overlayTweener.FinishedPlaying -= OnOverlayTweenerFinishedPlaying;
		}
		if (_currentAdvisorPosition != null)
		{
			if (_currentAdvisorPosition.DialogTweener != null)
			{
				_currentAdvisorPosition.DialogTweener.FinishedPlaying -= OnDialogTweenerFinishedPlaying;
			}
			if (_currentAdvisorPosition.AdvisorTweener != null)
			{
				_currentAdvisorPosition.AdvisorTweener.FinishedPlaying -= OnAdvisorTweenerFinishedPlaying;
			}
		}
		ScreenView.PopScreenView("tutorial_dialog");
	}

	public void Show(ILocalizedString bodyText, AdvisorPositionType advisorPosition, bool useOverlay, bool useContinueButton, Action continueCallback)
	{
		_dialogText = bodyText;
		_useOverlay = useOverlay;
		_useContinueButton = useContinueButton;
		_continueCallback = continueCallback;
		ShowTutorialDialog(advisorPosition);
		ScreenView.PushScreenView("tutorial_dialog");
	}

	public void Hide()
	{
		HideTutorialDialog(animated: true);
		ScreenView.PopScreenView("tutorial_dialog");
	}

	public void OnContinueClicked()
	{
		if (_currentAdvisorPosition != null && ((_currentAdvisorPosition.AdvisorTweener.IsPlaying && !_isAdvisorShowing) || (_currentAdvisorPosition.DialogTweener.IsPlaying && !_isDialogShowing)))
		{
			ShowNow();
			return;
		}
		Action continueCallback = _continueCallback;
		Hide();
		EventTools.Fire(continueCallback);
	}

	private void ShowNow()
	{
		if (_currentAdvisorPosition != null)
		{
			_currentAdvisorPosition.AdvisorTweener.StopAndReset(resetToEnd: true);
			_isAdvisorShowing = true;
			_currentAdvisorPosition.DialogTweener.StopAndReset(resetToEnd: true);
			_isDialogShowing = true;
			if (_dialogText != null)
			{
				_currentAdvisorPosition.DialogBodyText.LocalizedString = _dialogText;
				_dialogText = null;
			}
			if (_useOverlay)
			{
				_overlayTweener.StopAndReset(resetToEnd: true);
				_isOverlayShowing = true;
			}
		}
	}

	private void ShowTutorialDialog(AdvisorPositionType advisorPositionType)
	{
		base.gameObject.SetActive(value: true);
		if (_isAdvisorShowing)
		{
			if (_currentAdvisorPosition != null && _currentAdvisorPosition.PositionType != advisorPositionType)
			{
				_newPositionType = advisorPositionType;
				HideAdvisor();
				return;
			}
			ShowAdvisor(advisorPositionType);
			if (_isDialogShowing)
			{
				HideDialog();
			}
			else
			{
				ShowDialog();
			}
		}
		else
		{
			ShowAdvisor(advisorPositionType);
		}
	}

	private void HideTutorialDialog(bool animated)
	{
		_continueCallback = null;
		if (animated)
		{
			HideAdvisor();
			return;
		}
		_overlayTweener.StopAndReset();
		_newPositionType = null;
		_dialogText = null;
		if (_currentAdvisorPosition != null)
		{
			_currentAdvisorPosition.AdvisorTweener.FinishedPlaying -= OnAdvisorTweenerFinishedPlaying;
			_currentAdvisorPosition.AdvisorTweener.StopAndReset();
			_currentAdvisorPosition.DialogTweener.FinishedPlaying -= OnDialogTweenerFinishedPlaying;
			_currentAdvisorPosition.DialogTweener.StopAndReset();
		}
		_currentAdvisorPosition = null;
		base.gameObject.SetActive(value: false);
	}

	private void ShowAdvisor(AdvisorPositionType advisorPositionType)
	{
		SetAdvisorPosition(advisorPositionType);
		Tweener advisorTweener = _currentAdvisorPosition.AdvisorTweener;
		if (advisorTweener.IsPlaying)
		{
			if (advisorTweener.IsPlaybackReversed)
			{
				advisorTweener.StopAndReset(resetToEnd: true);
			}
		}
		else if (!_isAdvisorShowing)
		{
			advisorTweener.StopAndReset();
			advisorTweener.Play();
		}
		ShowOverlay();
	}

	private void HideAdvisor()
	{
		if (_currentAdvisorPosition != null)
		{
			Tweener advisorTweener = _currentAdvisorPosition.AdvisorTweener;
			if (advisorTweener.IsPlaying)
			{
				if (!advisorTweener.IsPlaybackReversed)
				{
					HideTutorialDialog(animated: false);
				}
			}
			else if (_isAdvisorShowing)
			{
				advisorTweener.StopAndReset(resetToEnd: true);
				advisorTweener.PlayReverse();
			}
		}
		HideDialog();
		HideOverlay();
	}

	private void ShowDialog()
	{
		if (_currentAdvisorPosition == null || _dialogText == null)
		{
			return;
		}
		_currentAdvisorPosition.DialogBodyText.LocalizedString = _dialogText;
		_dialogText = null;
		if (_currentAdvisorPosition.DialogTweener.IsPlaying)
		{
			if (_currentAdvisorPosition.DialogTweener.IsPlaybackReversed)
			{
				_currentAdvisorPosition.DialogTweener.StopAndReset(resetToEnd: true);
			}
		}
		else if (!_isDialogShowing)
		{
			_currentAdvisorPosition.DialogTweener.StopAndReset();
			_currentAdvisorPosition.DialogTweener.Play();
		}
	}

	private void HideDialog()
	{
		if (_currentAdvisorPosition == null)
		{
			return;
		}
		if (_currentAdvisorPosition.DialogTweener.IsPlaying)
		{
			if (!_currentAdvisorPosition.DialogTweener.IsPlaybackReversed)
			{
				_currentAdvisorPosition.DialogTweener.StopAndReset();
			}
		}
		else if (_isDialogShowing)
		{
			_currentAdvisorPosition.DialogTweener.StopAndReset(resetToEnd: true);
			_currentAdvisorPosition.DialogTweener.PlayReverse();
		}
	}

	private void ShowOverlay()
	{
		if (_useOverlay)
		{
			_overlayImage.raycastTarget = true;
			if (_overlayTweener.IsPlaying)
			{
				if (_overlayTweener.IsPlaybackReversed)
				{
					_overlayTweener.StopAndReset(resetToEnd: true);
				}
			}
			else if (!_isOverlayShowing)
			{
				_overlayTweener.StopAndReset();
				_overlayTweener.Play();
			}
		}
		else
		{
			HideOverlay();
		}
	}

	private void HideOverlay()
	{
		_overlayImage.raycastTarget = false;
		if (_overlayTweener.IsPlaying)
		{
			if (!_overlayTweener.IsPlaybackReversed)
			{
				_overlayTweener.StopAndReset();
			}
		}
		else if (_isOverlayShowing)
		{
			_overlayTweener.StopAndReset(resetToEnd: true);
			_overlayTweener.PlayReverse();
		}
	}

	private void SetAdvisorPosition(AdvisorPositionType advisorPositionType)
	{
		int i = 0;
		for (int num = _advisorPositions.Length; i < num; i++)
		{
			AdvisorPosition advisorPosition = _advisorPositions[i];
			if (advisorPosition.PositionType == advisorPositionType)
			{
				advisorPosition.AdvisorImage.gameObject.SetActive(value: true);
				if (_currentAdvisorPosition != null)
				{
					_currentAdvisorPosition.AdvisorTweener.FinishedPlaying -= OnAdvisorTweenerFinishedPlaying;
					_currentAdvisorPosition.DialogTweener.FinishedPlaying -= OnDialogTweenerFinishedPlaying;
				}
				_currentAdvisorPosition = advisorPosition;
				_currentAdvisorPosition.AdvisorTweener.FinishedPlaying += OnAdvisorTweenerFinishedPlaying;
				_currentAdvisorPosition.DialogTweener.FinishedPlaying += OnDialogTweenerFinishedPlaying;
				advisorPosition.ContinueButton.SetActive(_useContinueButton);
			}
			else
			{
				advisorPosition.AdvisorImage.gameObject.SetActive(value: false);
			}
		}
	}

	private void OnAdvisorTweenerFinishedPlaying(Tweener tweener)
	{
		if (tweener.IsPlaybackReversed)
		{
			_isAdvisorShowing = false;
			if (_newPositionType.HasValue)
			{
				ShowAdvisor(_newPositionType.Value);
				_newPositionType = null;
			}
			else
			{
				HideTutorialDialog(animated: false);
			}
		}
		else
		{
			_isAdvisorShowing = true;
			ShowDialog();
		}
	}

	private void OnDialogTweenerFinishedPlaying(Tweener tweener)
	{
		_isDialogShowing = !tweener.IsPlaybackReversed;
		if (tweener.IsPlaybackReversed && _currentAdvisorPosition != null && !_currentAdvisorPosition.AdvisorTweener.IsPlaying)
		{
			ShowDialog();
		}
	}

	private void OnOverlayTweenerFinishedPlaying(Tweener tweener)
	{
		_isOverlayShowing = !tweener.IsPlaybackReversed;
	}
}
