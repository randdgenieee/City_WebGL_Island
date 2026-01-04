namespace CIG
{
	public sealed class AudioManager : SingletonMonobehaviour<AudioManager>
	{
		private AudioPlaybackEngine _audioPlaybackEngine;

		private Settings _settings;

		public void Initialize(Settings settings)
		{
			_audioPlaybackEngine = SingletonMonobehaviour<AudioPlaybackEngine>.Instance;
			_settings = settings;
			_settings.SettingChangedEvent += OnSettingsChanged;
			OnSettingsChanged();
		}

		public void Deinitialize()
		{
			if (_settings != null)
			{
				_settings.SettingChangedEvent -= OnSettingsChanged;
				_settings = null;
			}
			_audioPlaybackEngine = null;
		}

		protected override void OnDestroy()
		{
			Deinitialize();
			base.OnDestroy();
		}

		public void ToggleMusic()
		{
			if (_settings.MusicEnabled)
			{
				_audioPlaybackEngine.PlayMusic();
			}
			else
			{
				_audioPlaybackEngine.StopMusic();
			}
		}

		public void SetMusicVolume(float volume)
		{
			_audioPlaybackEngine.SetMusicVolume(volume);
		}

		public void SetMusicVolume(float volume, float duration)
		{
			_audioPlaybackEngine.SetMusicVolume(volume, duration);
		}

		public void PlayClip(Clip clip, bool playOneAtATime = false, float volumeScale = 1f, float delay = 0f)
		{
			if (_settings.SoundEffectsEnabled)
			{
				_audioPlaybackEngine.PlayClip(clip, playOneAtATime, volumeScale, delay);
			}
		}

		public float GetClipLength(Clip clip)
		{
			return _audioPlaybackEngine.GetClipLength(clip);
		}

		private void OnSettingsChanged()
		{
			ToggleMusic();
		}
	}
}
