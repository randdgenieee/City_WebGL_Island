using CIG;
using System.Collections.Generic;
using UnityEngine;

public class LocalNotificationManager
{
	private const int RegisterForNotificationsLevel = 5;

	private readonly StorageDictionary _storage;

	private readonly Settings _settings;

	private readonly GameState _gameState;

	private readonly WebService _webService;

	private bool _hasRequestedPermissions;

	private readonly List<IHasNotification> _hasNotifications = new List<IHasNotification>();

	private const string HasRequestedPermissionsKey = "HasRequestedPermissions";

	public LocalNotificationManager(StorageDictionary storage, Settings settings, GameState gameState, WebService webService, SessionManager sessionManager)
	{
		_storage = storage;
		_settings = settings;
		_gameState = gameState;
		_webService = webService;
		_hasRequestedPermissions = _storage.Get("HasRequestedPermissions", defaultValue: false);
		CancelAllNotifications();
		if (_gameState.Level < 5)
		{
			_gameState.VisuallyLevelledUpEvent += OnPlayerVisuallyLevelledUp;
		}
		else
		{
			RequestPermissions();
		}
		sessionManager.LeaveGame += OnLeaveGame;
		sessionManager.ReturnToGame += OnReturnToGame;
	}

	public void HasNotification(IHasNotification hasNotification)
	{
		_hasNotifications.Add(hasNotification);
	}

	public void NoLongerHasNotification(IHasNotification hasNoNotifiaction)
	{
		if (_hasNotifications.Contains(hasNoNotifiaction))
		{
			_hasNotifications.Remove(hasNoNotifiaction);
		}
		else
		{
			UnityEngine.Debug.LogWarning("HasNotifications doesn't contain this IHasNotification!");
		}
	}

	private void ScheduleNotifications()
	{
		if (!_settings.NotificationsEnabled || !_hasRequestedPermissions)
		{
			return;
		}
		CancelAllNotifications();
		List<PlannedNotification> list = new List<PlannedNotification>();
		int count = _hasNotifications.Count;
		for (int i = 0; i < count; i++)
		{
			PlannedNotification[] notifications = _hasNotifications[i].GetNotifications();
			int num = notifications.Length;
			for (int j = 0; j < num; j++)
			{
				PlannedNotification plannedNotification = notifications[j];
				if (plannedNotification.IsValid)
				{
					list.Add(plannedNotification);
				}
			}
		}
		count = list.Count;
		if (_webService.UseBadge)
		{
			list.Sort((PlannedNotification a, PlannedNotification b) => a.Seconds.CompareTo(b.Seconds));
			for (int k = 0; k < count; k++)
			{
				PlannedNotification plannedNotification2 = list[k];
				int badgeCount = k + 1;
				Notifier.ScheduleNotification(new NativeNotification(plannedNotification2.Seconds, "City Island 5", plannedNotification2.Description.Translate(), plannedNotification2.Sound, badgeCount, plannedNotification2.Id));
			}
			UnityEngine.Debug.LogFormat("Succesfully scheduled {0} notifications!", list.Count);
		}
		else
		{
			for (int l = 0; l < count; l++)
			{
				PlannedNotification plannedNotification3 = list[l];
				Notifier.ScheduleNotification(new NativeNotification(plannedNotification3.Seconds, "City Island 5", plannedNotification3.Description.Translate(), plannedNotification3.Sound, 0, plannedNotification3.Id));
			}
		}
	}

	private void CancelAllNotifications()
	{
		Notifier.CancelAllNotifications();
	}

	private void RequestPermissions()
	{
		Notifier.RequestPermissions();
		_hasRequestedPermissions = true;
	}

	private void OnPlayerVisuallyLevelledUp(int level)
	{
		if (level >= 5)
		{
			_gameState.VisuallyLevelledUpEvent -= OnPlayerVisuallyLevelledUp;
			RequestPermissions();
		}
	}

	private void OnLeaveGame()
	{
		ScheduleNotifications();
	}

	private void OnReturnToGame()
	{
		CancelAllNotifications();
	}

	public void Serialize()
	{
		_storage.Set("HasRequestedPermissions", _hasRequestedPermissions);
	}
}
