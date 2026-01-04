namespace CIG
{
    public static class Notifier
    {
        public static bool IsAvailableOnPlatform => true;

        static Notifier()
        {
        }

        public static void CancelAllNotifications()
        {
            if (IsAvailableOnPlatform)
            {
            }
        }

        public static void RequestPermissions()
        {
            if (IsAvailableOnPlatform)
            {
            }
        }

        public static void ScheduleNotification(NativeNotification notification)
        {
            if (IsAvailableOnPlatform)
            {
            }
        }
    }
}
