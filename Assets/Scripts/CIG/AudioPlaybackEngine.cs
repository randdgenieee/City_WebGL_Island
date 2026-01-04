using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	[RequireComponent(typeof(AudioSource))]
	public sealed class AudioPlaybackEngine : SingletonMonobehaviour<AudioPlaybackEngine>
	{
		[Serializable]
		private class AudioClipSettings
		{
			[SerializeField]
			private Clip _clip;

			[SerializeField]
			private AudioClip _audioClip;

			public Clip Clip => _clip;

			public AudioClip AudioClip => _audioClip;
		}

		[SerializeField]
		private AudioSource _musicAudioSource;

		[SerializeField]
		private AudioSource _sfxAudioSource;

		[SerializeField]
		private List<AudioClipSettings> _audioClips;

		private readonly Dictionary<string, float> _clipLastPlayTime = new Dictionary<string, float>();

		private IEnumerator _musicVolumeRoutine;

		protected override void Awake()
		{
			base.Awake();
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}

		public void PlayMusic()
		{
			if (!_musicAudioSource.isPlaying)
			{
				_musicAudioSource.Play();
			}
		}

		public void StopMusic()
		{
			if (_musicAudioSource.isPlaying)
			{
				_musicAudioSource.Stop();
			}
		}

		public void SetMusicVolume(float volume)
		{
			if (_musicVolumeRoutine != null)
			{
				StopCoroutine(_musicVolumeRoutine);
			}
			_musicAudioSource.volume = volume;
		}

		public void SetMusicVolume(float volume, float duration)
		{
			if (duration > 0f)
			{
				SetMusicVolume(volume);
				StartCoroutine(_musicVolumeRoutine = MusicVolumeResetRoutine(duration));
			}
		}

		public void PlayClip(Clip clip, bool playOneAtATime = false, float volumeScale = 1f, float delay = 0f)
		{
			AudioClipSettings audioClipSettings = _audioClips.Find((AudioClipSettings x) => x.Clip == clip);
			if (audioClipSettings == null)
			{
				UnityEngine.Debug.LogWarning($"Missing AudioClipSettings for {clip}");
			}
			else if (audioClipSettings.AudioClip == null)
			{
				UnityEngine.Debug.LogWarning($"This audio clip for {clip} has not been set yet");
			}
			else
			{
				StartCoroutine(PlayAudioClip(audioClipSettings.AudioClip, volumeScale, playOneAtATime, delay));
			}
		}

		public float GetClipLength(Clip clip)
		{
			AudioClipSettings audioClipSettings = _audioClips.Find((AudioClipSettings x) => x.Clip == clip);
			if (audioClipSettings == null)
			{
				UnityEngine.Debug.LogWarningFormat("Missing AudioClipSettings for {0}", clip);
				return 0f;
			}
			if (audioClipSettings.AudioClip == null)
			{
				UnityEngine.Debug.LogWarningFormat("This audio clip for {0} has not been set yet", clip);
				return 0f;
			}
			return audioClipSettings.AudioClip.length;
		}

		private IEnumerator PlayAudioClip(AudioClip clip, float volumeScale, bool playOneAtATime, float delay)
		{
			if (delay > 0f)
			{
				yield return new WaitForSeconds(delay);
			}
			float value;
			if (!playOneAtATime || !_clipLastPlayTime.TryGetValue(clip.name, out value) || Time.realtimeSinceStartup - value > clip.length)
			{
				_sfxAudioSource.PlayOneShot(clip, volumeScale);
				if (playOneAtATime)
				{
					_clipLastPlayTime[clip.name] = Time.realtimeSinceStartup;
				}
			}
		}

		private IEnumerator MusicVolumeResetRoutine(float duration)
		{
			yield return new WaitForSecondsRealtime(duration);
			_musicAudioSource.volume = 1f;
		}
	}
}
