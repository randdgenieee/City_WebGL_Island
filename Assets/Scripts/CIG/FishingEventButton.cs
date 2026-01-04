using Tweening;
using UnityEngine;

namespace CIG
{
	public class FishingEventButton : HUDCurrencyTweenHelper
	{
		[SerializeField]
		private Tweener _inactiveGroupTweener;

		[SerializeField]
		private Tweener _activeGroupTweener;

		[SerializeField]
		private GameObject _questActiveRoot;

		[SerializeField]
		private FishingQuestButton _questButton;

		[SerializeField]
		private GameObject _questInactiveRoot;

		private FishingEvent _fishingEvent;

		public void Initialize(FishingEvent fishingEvent)
		{
			_fishingEvent = fishingEvent;
			Initialize();
			_fishingEvent.IsActiveChangedEvent += OnEventActiveChanged;
			_fishingEvent.QuestStartedEvent += OnQuestStarted;
			_fishingEvent.QuestStoppedEvent += OnQuestStopped;
			UpdateContent();
		}

		protected override void OnDestroy()
		{
			if (_fishingEvent != null)
			{
				_fishingEvent.IsActiveChangedEvent -= OnEventActiveChanged;
				_fishingEvent.QuestStartedEvent -= OnQuestStarted;
				_fishingEvent.QuestStoppedEvent -= OnQuestStopped;
				_fishingEvent = null;
			}
			base.OnDestroy();
		}

		public void OnClicked()
		{
			_fishingEvent.OpenQuestPopup();
		}

		protected override void UpdateValue(decimal value)
		{
		}

		private void UpdateContent()
		{
			base.gameObject.SetActive(_fishingEvent.IsActive);
			bool flag = _fishingEvent.FishingQuest != null && _fishingEvent.FishingQuest.IsActive;
			_questInactiveRoot.SetActive(!flag);
			_questActiveRoot.SetActive(flag);
			SetActiveTweener(flag ? _activeGroupTweener : _inactiveGroupTweener);
			if (_fishingEvent.FishingQuest != null && _fishingEvent.FishingQuest.IsActive)
			{
				_questButton.Initialize(_fishingEvent);
			}
		}

		private void OnEventActiveChanged(bool isActive)
		{
			UpdateContent();
		}

		private void OnQuestStarted()
		{
			UpdateContent();
		}

		private void OnQuestStopped()
		{
			UpdateContent();
		}
	}
}
