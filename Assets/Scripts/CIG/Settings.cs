using CIG.Translation;
using UnityEngine;

namespace CIG
{
	public class Settings
	{
		public delegate void SettingChangedEventHandler();

		private const string MusicEnabledKey = "MusicEnabled";

		private const string SoundEffectsEnabledKey = "SoundEffectsEnabled";

		private const string NotificationsEnabledKey = "NotificationsEnabled";

		private const string CultureIdentifierKey = "CultureIdentifier";

		private const string AuthenticationAllowedKey = "AuthenticationAllowed";

		private const string SocialAuthenticationAllowedKey = "SocialAuthenticationAllowed";

		private readonly StorageDictionary _storage;

		public string DefaultLanguage => Localization.GetSystemDefaultCulture().Identifier;

		public bool MusicEnabled
		{
			get;
			private set;
		}

		public bool SoundEffectsEnabled
		{
			get;
			private set;
		}

		public bool NotificationsEnabled
		{
			get;
			private set;
		}

		public string CultureIdentifier
		{
			get;
			private set;
		}

		public bool AuthenticationAllowed
		{
			get;
			private set;
		}

		public bool SocialAuthenticationAllowed
		{
			get;
			private set;
		}

		public event SettingChangedEventHandler SettingChangedEvent;

		private void FireSettingChangedEvent()
		{
			this.SettingChangedEvent?.Invoke();
		}

		public Settings(StorageDictionary storage)
		{
			_storage = storage;
			MusicEnabled = _storage.Get("MusicEnabled", defaultValue: true);
			SoundEffectsEnabled = _storage.Get("SoundEffectsEnabled", defaultValue: true);
			NotificationsEnabled = _storage.Get("NotificationsEnabled", defaultValue: true);
			AuthenticationAllowed = _storage.Get("AuthenticationAllowed", defaultValue: false);
			SocialAuthenticationAllowed = _storage.Get("SocialAuthenticationAllowed", defaultValue: false);
			CultureIdentifier = _storage.Get("CultureIdentifier", DefaultLanguage);
			if (Localization.ContainsCulture(CultureIdentifier))
			{
				SwitchLanguage(CultureIdentifier, logAnalytics: false);
				return;
			}
			UnityEngine.Debug.LogError("Deserialized invalid CultureIdentifier: " + CultureIdentifier + ". The identifier '" + DefaultLanguage + "' will be used instead.");
			SwitchLanguage(DefaultLanguage, logAnalytics: false);
		}

		public void SwitchLanguage(Localization.Culture newCulture, bool logAnalytics = true)
		{
			if (Localization.CurrentCulture != newCulture && Localization.ContainsCulture(newCulture))
			{
				Localization.CurrentCulture = newCulture;
				CultureIdentifier = newCulture.Identifier;
				FireSettingChangedEvent();
				if (logAnalytics)
				{
					Analytics.SettingsLanguageChanged(CultureIdentifier);
				}
			}
		}

		public void SwitchLanguage(string cultureIdentifier, bool logAnalytics = true)
		{
			int num = 0;
			int count = Localization.AvailableCultures.Count;
			Localization.Culture culture;
			while (true)
			{
				if (num < count)
				{
					culture = Localization.AvailableCultures[num];
					if (culture.Identifier == cultureIdentifier)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			SwitchLanguage(culture, logAnalytics);
		}

		public void ToggleMusic(bool on)
		{
			if (MusicEnabled != on)
			{
				MusicEnabled = on;
				FireSettingChangedEvent();
				Analytics.SettingsMusicChanged(on);
			}
		}

		public void ToggleSoundEffects(bool on)
		{
			if (SoundEffectsEnabled != on)
			{
				SoundEffectsEnabled = on;
				FireSettingChangedEvent();
				Analytics.SettingsSFXChanged(on);
			}
		}

		public void ToggleNotifications(bool on)
		{
			if (NotificationsEnabled != on)
			{
				NotificationsEnabled = on;
				FireSettingChangedEvent();
				Analytics.SetNotificationsEnabled(NotificationsEnabled);
			}
		}

		public void ToggleAuthenticationAllowed(bool on)
		{
			if (AuthenticationAllowed != on)
			{
				AuthenticationAllowed = on;
				FireSettingChangedEvent();
			}
		}

		public void ToggleSocialAuthenticationAllowed(bool on)
		{
			if (SocialAuthenticationAllowed != on)
			{
				ToggleAuthenticationAllowed(on: true);
				SocialAuthenticationAllowed = on;
				Analytics.SocialAuthenticationChanged(on);
				FireSettingChangedEvent();
			}
		}

		public void Serialize()
		{
			_storage.Set("MusicEnabled", MusicEnabled);
			_storage.Set("SoundEffectsEnabled", SoundEffectsEnabled);
			_storage.Set("NotificationsEnabled", NotificationsEnabled);
			_storage.Set("CultureIdentifier", CultureIdentifier);
			_storage.Set("AuthenticationAllowed", AuthenticationAllowed);
			_storage.Set("SocialAuthenticationAllowed", SocialAuthenticationAllowed);
		}
	}
}
