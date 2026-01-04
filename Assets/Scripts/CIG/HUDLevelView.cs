using CIG.Translation;
using Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class HUDLevelView : HUDCurrencyTweenHelper
	{
		[SerializeField]
		private Tweener _iconTweener;

		[SerializeField]
		private LocalizedText _levelLabel;

		[SerializeField]
		private Image _levelProgressBar;

		[SerializeField]
		private Tweener _levelUpTweener;

		private GameState _gameState;

		private PopupManager _popupManager;

		private int _shouldShowLevel;

		private int _currentlyShowingLevel;

		private decimal _currentProgress;

		private decimal _endProgress;

		private bool _waitingForPopup;

		public void Initialize(GameState gameState, PopupManager popupManager)
		{
			_gameState = gameState;
			_popupManager = popupManager;
			Initialize();
			_currentlyShowingLevel = _gameState.Level;
			_shouldShowLevel = _currentlyShowingLevel;
			_currentProgress = (decimal)_gameState.LevelProgress;
			base.FinishedTweeningEvent += OnFinishedTweening;
			_popupManager.PopupClosedEvent += OnPopupClosed;
			_levelUpTweener.FinishedPlaying += OnLevelUpTweenerFinished;
			SetActiveTweener(_iconTweener);
			TweenTo(_currentProgress, _currentProgress, 0f);
		}

		protected override void OnDestroy()
		{
			base.FinishedTweeningEvent -= OnFinishedTweening;
			_levelUpTweener.FinishedPlaying -= OnLevelUpTweenerFinished;
			if (_popupManager != null)
			{
				_popupManager.PopupClosedEvent -= OnPopupClosed;
				_popupManager = null;
			}
			_gameState = null;
			base.OnDestroy();
		}

		public override void FlyingCurrencyFinishedPlaying(Currency earnedCurrency, bool animateHudElement = true)
		{
			base.FlyingCurrencyFinishedPlaying(earnedCurrency, animateHudElement);
			if (animateHudElement)
			{
				UpdateTweening();
			}
			else
			{
				UpdateValue((decimal)_gameState.LevelProgress);
			}
		}

		protected override void UpdateValue(decimal value)
		{
			_currentProgress = value;
			_levelProgressBar.fillAmount = (float)_currentProgress;
		}

		private void UpdateTweening()
		{
			if (!base.Running && !_waitingForPopup)
			{
				if (_currentlyShowingLevel >= _gameState.Level)
				{
					TweenTo(_currentProgress, (decimal)_gameState.LevelProgress);
					return;
				}
				_shouldShowLevel = _currentlyShowingLevel + 1;
				TweenTo(_currentProgress, decimal.One);
			}
		}

		private void OnFinishedTweening()
		{
			if (_shouldShowLevel > _currentlyShowingLevel)
			{
				_levelUpTweener.StopAndReset();
				_levelUpTweener.Play();
				_currentlyShowingLevel = _shouldShowLevel;
			}
			_levelLabel.LocalizedString = Localization.Integer(_currentlyShowingLevel);
		}

		private void OnLevelUpTweenerFinished(Tweener tweener)
		{
			Currencies levelReward = _gameState.GetLevelReward(_currentlyShowingLevel);
			_popupManager.RequestPopup(new LevelUpPopupRequest(_currentlyShowingLevel, levelReward));
			_waitingForPopup = true;
		}

		private void OnPopupClosed(PopupRequest popup, bool instant)
		{
			LevelUpPopupRequest levelUpPopupRequest;
			if ((levelUpPopupRequest = (popup as LevelUpPopupRequest)) != null && levelUpPopupRequest.Level == _currentlyShowingLevel)
			{
				_waitingForPopup = false;
				_gameState.VisuallyLevelledUp(_currentlyShowingLevel);
				_currentProgress = default(decimal);
				UpdateTweening();
			}
		}
	}
}
