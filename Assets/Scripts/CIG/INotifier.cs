namespace CIG
{
	public interface INotifier
	{
		void RegisterForNotifications();

		void ScheduleNotification(NativeNotification notification);

		void CancelAllNotifications();
	}
}
