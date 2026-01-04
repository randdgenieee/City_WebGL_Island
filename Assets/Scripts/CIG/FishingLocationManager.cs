using SparkLinq;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	public class FishingLocationManager : MonoBehaviour
	{
		[SerializeField]
		private FishingLocation _fishingLocationPrefab;

		private readonly List<FishingLocation> _inactiveLocations = new List<FishingLocation>();

		private readonly List<FishingLocation> _activeLocations = new List<FishingLocation>();

		private FishingEvent _fishingEvent;

		public bool IsAnimating
		{
			get;
			private set;
		}

		public void Initialize(IslandSetup setup, FishingEvent fishingEvent, PopupManager popupManager, OverlayManager overlayManager)
		{
			_fishingEvent = fishingEvent;
			int i = 0;
			for (int count = setup.FishingLocations.Count; i < count; i++)
			{
				FishingLocation fishingLocation = UnityEngine.Object.Instantiate(_fishingLocationPrefab, base.transform);
				fishingLocation.Initialize(_fishingEvent, popupManager, overlayManager, setup.FishingLocations[i]);
				_inactiveLocations.Add(fishingLocation);
			}
			_fishingEvent.IsActiveChangedEvent += OnEventIsActiveChanged;
			_fishingEvent.QuestStartedEvent += OnQuestStarted;
			_fishingEvent.QuestAchievedEvent += OnQuestAchieved;
			_fishingEvent.ActiveLocationsChangedEvent += OnActiveLocationsChanged;
			UpdateVisibleLocations();
		}

		private void OnDestroy()
		{
			if (_fishingEvent != null)
			{
				_fishingEvent.IsActiveChangedEvent -= OnEventIsActiveChanged;
				_fishingEvent.QuestStartedEvent -= OnQuestStarted;
				_fishingEvent.QuestAchievedEvent -= OnQuestAchieved;
				_fishingEvent.ActiveLocationsChangedEvent -= OnActiveLocationsChanged;
				_fishingEvent = null;
			}
			if (IsometricIsland.Current != null)
			{
				IsometricIsland.Current.CameraOperator.PopDisableInputRequest(this);
			}
		}

		public void FocusOnRandomLocation(Action<FishingLocation> animationFinishedCallback)
		{
			FishingLocation fishingLocation = _activeLocations.PickRandom();
			if (fishingLocation != null)
			{
				IsAnimating = true;
				fishingLocation.ZoomTo(delegate(FishingLocation location)
				{
					OnAnimationFinished(animationFinishedCallback, location);
				});
			}
		}

		private void UpdateVisibleLocations()
		{
			if (_fishingEvent.IsActive && _fishingEvent.FishingQuest.Quest.State == QuestState.InProgress)
			{
				int num = _fishingEvent.ActiveLocations - _activeLocations.Count;
				if (num > 0)
				{
					for (int i = 0; i < num; i++)
					{
						if (_inactiveLocations.Count <= 0)
						{
							break;
						}
						int index = UnityEngine.Random.Range(0, _inactiveLocations.Count);
						FishingLocation fishingLocation = _inactiveLocations[index];
						fishingLocation.SetActive(active: true);
						fishingLocation.SetIcon(_fishingEvent.FishingQuest.IsActive);
						_inactiveLocations.RemoveAt(index);
						_activeLocations.Add(fishingLocation);
					}
				}
				else
				{
					if (num >= 0)
					{
						return;
					}
					_activeLocations.Sort((FishingLocation lhs, FishingLocation rhs) => lhs.HasPlayed.CompareTo(rhs.HasPlayed));
					int num2 = -num;
					for (int j = 0; j < num2; j++)
					{
						if (_activeLocations.Count <= 0)
						{
							break;
						}
						int index2 = _activeLocations.Count - 1;
						FishingLocation fishingLocation2 = _activeLocations[index2];
						_activeLocations.RemoveAt(index2);
						fishingLocation2.SetActive(active: false);
					}
				}
			}
			else
			{
				int k = 0;
				for (int count = _activeLocations.Count; k < count; k++)
				{
					FishingLocation fishingLocation3 = _activeLocations[k];
					fishingLocation3.SetActive(active: false);
					_inactiveLocations.Add(fishingLocation3);
				}
				_activeLocations.Clear();
			}
		}

		private void OnActiveLocationsChanged(int count)
		{
			UpdateVisibleLocations();
		}

		private void OnQuestStarted()
		{
			int i = 0;
			for (int count = _activeLocations.Count; i < count; i++)
			{
				_activeLocations[i].SetIcon(show: true);
			}
		}

		private void OnQuestAchieved()
		{
			int i = 0;
			for (int count = _activeLocations.Count; i < count; i++)
			{
				_activeLocations[i].SetIcon(show: false);
			}
		}

		private void OnAnimationFinished(Action<FishingLocation> callback, FishingLocation fishingLocation)
		{
			IsAnimating = false;
			EventTools.Fire(callback, fishingLocation);
		}

		private void OnEventIsActiveChanged(bool isActive)
		{
			UpdateVisibleLocations();
		}
	}
}
