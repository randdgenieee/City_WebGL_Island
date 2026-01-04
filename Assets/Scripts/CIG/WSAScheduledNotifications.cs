using System;

namespace CIG
{
	public static class WSAScheduledNotifications
	{
		private const string PeekLogoWide = "ms-appx:///Assets/Wide310x150Logo.png";

		private const string PeekLogoSquare = "ms-appx:///Assets/Square150x150Logo.png";

		private const string SmallLogo = "ms-appx:///Assets/Square44x44Logo.png";

		public static void CancelAllNotifications()
		{
			CancelAllToastNotifications();
			ClearAllTileNotifications();
			CancelAllTileNotifications();
		}

		public static void ScheduleToastNotification(string text, DateTime deliveryTime, Uri image = null)
		{
		}

		public static void CancelAllToastNotifications()
		{
		}

		public static void ScheduleTileNotification(string text, DateTime deliveryTime)
		{
		}

		private static void CancelAllTileNotifications()
		{
		}

		private static void ClearAllTileNotifications()
		{
		}
	}
}
