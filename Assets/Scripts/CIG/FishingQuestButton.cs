using Tweening;
using UnityEngine;

namespace CIG
{
	public class FishingQuestButton : QuestButton
	{
		private const int TotalTweenerPlays = 3;

		[SerializeField]
		private Tweener _tweener;

		[SerializeField]
		private DateTimeTimerView _timer;

		private FishingEvent _fishingEvent;

		private int _tweenerPlayCount;

		private void Awake()
		{
			_tweener.FinishedPlaying += OnTweenerFinishedPlaying;
		}

		public void Initialize(FishingEvent fishingEvent)
		{
			Initialize(fishingEvent.FishingQuest.Quest);
			_fishingEvent = fishingEvent;
			_fishingEvent.QuestAchievedEvent += OnQuestAchieved;
			if (AntiCheatDateTime.UtcNow < _fishingEvent.FishingQuest.EndTime)
			{
				_timer.StartTimer(_fishingEvent.FishingQuest.EndTime);
			}
			_tweenerPlayCount = 0;
			_tweener.StopAndReset();
			_tweener.Play();
		}

		public override void Deinitialize()
		{
			if (_fishingEvent != null)
			{
				_fishingEvent.QuestAchievedEvent -= OnQuestAchieved;
				_fishingEvent = null;
			}
			base.Deinitialize();
		}

		protected override void OnDestroy()
		{
			if (_tweener != null)
			{
				_tweener.FinishedPlaying -= OnTweenerFinishedPlaying;
			}
			base.OnDestroy();
		}

		public override void OnClicked()
		{
			_fishingEvent.OpenQuestPopup();
		}

		private void OnQuestAchieved()
		{
			_timer.StopTimer();
		}

		private void OnTweenerFinishedPlaying(Tweener tweener)
		{
			_tweenerPlayCount++;
			if (_tweenerPlayCount < 3)
			{
				tweener.Play();
			}
		}
	}
}
