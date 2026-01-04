using CIG.Translation;
using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	public class ComeBackNotificationsManager : IHasNotification
	{
		private readonly ILocalizedString[] _possibleNotifications = new ILocalizedString[7];

		public ComeBackNotificationsManager(LocalNotificationManager localNotificationManager)
		{
			localNotificationManager.HasNotification(this);
		}

		public PlannedNotification[] GetNotifications()
		{
			LoadLocalisations();
			ILocalizedString[] array = RandomizedNotificationTexts();
			List<PlannedNotification> list = new List<PlannedNotification>();
			list.Add(new PlannedNotification(GetDaysInSeconds(7), array[0], sound: false, 2));
			int num = 1;
			for (int num2 = 1; num2 <= 16; num2 *= 2)
			{
				if (num >= array.Length)
				{
					array = RandomizedNotificationTexts();
					num = 0;
				}
				list.Add(new PlannedNotification(GetDaysInSeconds(15 * num2), array[num], sound: false, 2));
				num++;
			}
			return list.ToArray();
		}

		private int GetDaysInSeconds(int day)
		{
			return 86400 * day;
		}

		private void LoadLocalisations()
		{
			for (int i = 0; i < _possibleNotifications.Length; i++)
			{
				_possibleNotifications[i] = Localization.Key("come_back_notification" + (i + 1));
			}
		}

		private ILocalizedString[] RandomizedNotificationTexts()
		{
			ILocalizedString[] possibleNotifications = _possibleNotifications;
			int i = 0;
			for (int num = possibleNotifications.Length; i < num; i++)
			{
				int num2 = i + (int)(Random.value * (float)(possibleNotifications.Length - i));
				ILocalizedString localizedString = possibleNotifications[num2];
				possibleNotifications[num2] = possibleNotifications[i];
				possibleNotifications[i] = localizedString;
			}
			return possibleNotifications;
		}
	}
}
