namespace CIG
{
	public class GameSparksPushNotifications
	{
		private readonly CIGGameSparksInstance _gameSparksInstance;

		private readonly GameSparksAuthenticator _authenticator;

		private readonly Settings _settings;

		private bool _notificationsEnabled;

		public GameSparksPushNotifications(CIGGameSparksInstance gameSparksInstance, GameSparksAuthenticator authenticator, Settings settings)
		{
			_gameSparksInstance = gameSparksInstance;
			_authenticator = authenticator;
			_settings = settings;
			_notificationsEnabled = _settings.NotificationsEnabled;
			_settings.SettingChangedEvent += OnSettingsChanged;
		}

		private void OnSettingsChanged()
		{
			if (_authenticator.CurrentAuthentication.IsAuthenticated && _notificationsEnabled != _settings.NotificationsEnabled)
			{
				_notificationsEnabled = _settings.NotificationsEnabled;
				new PushNotificationsToggle(_gameSparksInstance, FirebaseManager.MessageToken, _notificationsEnabled).Send(null);
			}
		}
	}
}
