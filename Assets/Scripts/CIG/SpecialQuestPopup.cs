using CIG.Translation;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class SpecialQuestPopup : Popup
	{
		[SerializeField]
		private TextMeshProText _tmpTitleText;

		[SerializeField]
		private Image _icon;

		[SerializeField]
		private QuestItem _questItem;

		[SerializeField]
		private LocalizedText _titleText;

		[SerializeField]
		private LocalizedText _timerText;

		[SerializeField]
		private LocalizedText _fishingRodsAmountText;

		[SerializeField]
		private LocalizedText _costText;

		[SerializeField]
		private Button _startButton;

		[SerializeField]
		private GameObject _inactiveGroup;

		private GameState _gameState;

		private SpecialQuest _specialQuest;

		private string _analyticsScreenName;

		private IEnumerator _timerRoutine;

		public override string AnalyticsScreenName => _analyticsScreenName;

		public override void Initialize(Model model, CIGCanvasScaler canvasScaler)
		{
			base.Initialize(model, canvasScaler);
			_gameState = model.Game.GameState;
		}

		protected override void OnDestroy()
		{
			if (_specialQuest != null)
			{
				_specialQuest.StartedEvent -= OnQuestStarted;
				_specialQuest = null;
			}
			base.OnDestroy();
		}

		public override void Open(PopupRequest request)
		{
			base.Open(request);
			SpecialQuestPopupRequest request2 = GetRequest<SpecialQuestPopupRequest>();
			_analyticsScreenName = $"special_quest_{request2.AnalyticsScreenName}";
			_tmpTitleText.Text = request2.Title;
			_icon.sprite = request2.Icon;
			_specialQuest = request2.SpecialQuest;
			_titleText.LocalizedString = request2.LocalizedTitle;
			_questItem.Initialize(_specialQuest.Quest);
			_fishingRodsAmountText.LocalizedString = Localization.Integer(_gameState.Balance.GetValue("FishingRod"));
			_costText.LocalizedString = Localization.Integer(_specialQuest.StartCost.GetCurrency(0).Value);
			_startButton.interactable = _gameState.CanAfford(_specialQuest.StartCost);
			_inactiveGroup.SetActive(!_specialQuest.IsActive);
			if (_specialQuest.IsActive)
			{
				StartTimer();
			}
			else
			{
				_timerText.LocalizedString = Localization.TimeSpan(TimeSpan.FromSeconds(_specialQuest.DurationSeconds), hideSecondPartWhenZero: true);
			}
			_specialQuest.StartedEvent += OnQuestStarted;
			_specialQuest.ExpiredEvent += OnQuestExpired;
		}

		public void OnStartClicked()
		{
			if (_specialQuest.StartQuest())
			{
				OnCloseClicked();
			}
		}

		protected override void Closed()
		{
			if (_timerRoutine != null)
			{
				StopCoroutine(_timerRoutine);
				_timerRoutine = null;
			}
			if (_specialQuest != null)
			{
				_specialQuest.StartedEvent -= OnQuestStarted;
				_specialQuest.ExpiredEvent -= OnQuestExpired;
				_specialQuest = null;
			}
			base.Closed();
		}

		private void StartTimer()
		{
			if (_timerRoutine != null)
			{
				StopCoroutine(_timerRoutine);
			}
			StartCoroutine(_timerRoutine = TimerRoutine());
		}

		private void OnQuestStarted()
		{
			_inactiveGroup.SetActive(value: false);
			StartTimer();
		}

		private void OnQuestExpired()
		{
			OnCloseClicked();
		}

		private IEnumerator TimerRoutine()
		{
			while (true)
			{
				TimeSpan timeLeft;
				TimeSpan timeSpan = timeLeft = _specialQuest.TimeLeft;
				if (!(timeSpan.TotalSeconds > 0.0))
				{
					break;
				}
				_timerText.LocalizedString = Localization.TimeSpan(timeLeft, hideSecondPartWhenZero: true);
				yield return new WaitForSecondsRealtime(1f);
			}
			_timerText.LocalizedString = Localization.TimeSpan(TimeSpan.Zero, hideSecondPartWhenZero: true);
			_timerRoutine = null;
		}
	}
}
