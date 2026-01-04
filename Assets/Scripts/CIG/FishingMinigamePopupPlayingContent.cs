using CIG.Translation;
using System;
using System.Collections;
using Tweening;
using UnityEngine;

namespace CIG
{
	public class FishingMinigamePopupPlayingContent : MonoBehaviour
	{
		public delegate void MinigameFinishedEventHandler();

		private const float ScoredTextShowDuration = 0.8f;

		private const float TimeUpBannerShowDuration = 1.5f;

		private const int CountdownSeconds = 3;

		private const float CountdownInterval = 0.6f;

		private const int TimerSoundSeconds = 3;

		[SerializeField]
		private Tweener _closeTweener;

		[SerializeField]
		private Tweener _missAreaTweener;

		[SerializeField]
		private RectTransform _greenArea;

		[SerializeField]
		private Tweener _greenAreaTweener;

		[SerializeField]
		private RectTransform _yellowArea;

		[SerializeField]
		private Tweener _yellowAreaTweener;

		[SerializeField]
		private RectTransform _marker;

		[SerializeField]
		private Tweener _markerTweener;

		[SerializeField]
		private FishingMinigamePopupReelButton _reelButton;

		[SerializeField]
		private LocalizedText _scoredText;

		[SerializeField]
		private Tweener _scoredTextTweener;

		[SerializeField]
		private TextMeshProText _timerText;

		[SerializeField]
		private GameObject _timerTextRoot;

		[SerializeField]
		private Tweener _timerTextTweener;

		[SerializeField]
		private GameObject _bannerRoot;

		[SerializeField]
		private Tweener _bannerTweener;

		[SerializeField]
		private GameObject _bannerTextRoot;

		[SerializeField]
		private TextMeshProText _bannerText;

		[SerializeField]
		private Tweener _bannerTextTweener;

		private FishingMinigame _fishingMinigame;

		private IEnumerator _countdownRoutine;

		private IEnumerator _timerRoutine;

		private IEnumerator _scoredRoutine;

		public event MinigameFinishedEventHandler MinigameFinishedEvent;

		private void FireMinigameFinishedEvent()
		{
			this.MinigameFinishedEvent?.Invoke();
		}

		private void Awake()
		{
			_bannerTweener.FinishedPlaying += OnBannerTweenerFinishedPlaying;
			_timerTextTweener.FinishedPlaying += OnTimerTweenerFinishedPlaying;
		}

		public void Initialize(FishingMinigame fishingMinigame)
		{
			_fishingMinigame = fishingMinigame;
			_fishingMinigame.StartedEvent += OnMinigameStarted;
			_fishingMinigame.FishCaughtEvent += OnFishCaught;
			_fishingMinigame.MarkerMovedEvent += OnMarkerMoved;
			_fishingMinigame.FinishedEvent += OnMinigameFinished;
			_fishingMinigame.Area.AreaMovedEvent += OnAreaMoved;
			_reelButton.Initialize(_fishingMinigame);
			_timerTextRoot.SetActive(value: false);
			_scoredText.gameObject.SetActive(value: false);
			_bannerRoot.SetActive(value: false);
			_marker.gameObject.SetActive(value: false);
			OnAreaMoved();
			StartCountdown();
		}

		public void Deinitialize()
		{
			if (_fishingMinigame != null)
			{
				_fishingMinigame.StartedEvent -= OnMinigameStarted;
				_fishingMinigame.FishCaughtEvent -= OnFishCaught;
				_fishingMinigame.MarkerMovedEvent -= OnMarkerMoved;
				_fishingMinigame.FinishedEvent -= OnMinigameFinished;
				_fishingMinigame.Area.AreaMovedEvent -= OnAreaMoved;
				_fishingMinigame = null;
			}
			_reelButton.Deinitialize();
		}

		private void OnDestroy()
		{
			if (_bannerTweener != null)
			{
				_bannerTweener.FinishedPlaying -= OnBannerTweenerFinishedPlaying;
			}
			if (_timerTextTweener != null)
			{
				_timerTextTweener.FinishedPlaying -= OnTimerTweenerFinishedPlaying;
			}
			Deinitialize();
		}

		public void SetActive(bool active)
		{
			base.gameObject.SetActive(active);
		}

		private void StartCountdown()
		{
			if (_countdownRoutine != null)
			{
				StopCoroutine(_countdownRoutine);
			}
			StartCoroutine(_countdownRoutine = CountdownRoutine());
		}

		private void SetBannerText(string text)
		{
			_bannerTextTweener.StopAndReset();
			_bannerTextTweener.Play();
			_bannerText.Text = text;
		}

		private void OnMinigameStarted()
		{
			StartCoroutine(_timerRoutine = TimerRoutine());
		}

		private void OnFishCaught(int amount)
		{
			if (_scoredRoutine != null)
			{
				StopCoroutine(_scoredRoutine);
			}
			StartCoroutine(_scoredRoutine = ScoredRoutine(amount));
			_markerTweener.StopAndReset();
			_markerTweener.Play();
			switch (amount)
			{
			case 0:
				SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.FishingCatchNone);
				_missAreaTweener.StopAndReset();
				_missAreaTweener.Play();
				break;
			case 1:
				SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.FishingCatchOne);
				_greenAreaTweener.StopAndReset();
				_greenAreaTweener.Play();
				break;
			default:
				SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.FishingCatchTwo);
				_yellowAreaTweener.StopAndReset();
				_yellowAreaTweener.Play();
				break;
			}
		}

		private void OnMarkerMoved(float position)
		{
			_marker.anchorMin = new Vector2(position / 100f, 0f);
			_marker.anchorMax = new Vector2(position / 100f, 1f);
			_marker.sizeDelta = Vector2.zero;
		}

		private void OnAreaMoved()
		{
			_greenArea.anchorMin = new Vector2(_fishingMinigame.Area.GreenAreaStartPercentage / 100f, 0f);
			_greenArea.anchorMax = new Vector2(_fishingMinigame.Area.GreenAreaEndPercentage / 100f, 1f);
			_greenArea.sizeDelta = Vector2.zero;
			_yellowArea.anchorMin = new Vector2(_fishingMinigame.Area.YellowAreaStartPercentage / 100f, 0f);
			_yellowArea.anchorMax = new Vector2(_fishingMinigame.Area.YellowAreaEndPercentage / 100f, 1f);
			_yellowArea.sizeDelta = Vector2.zero;
		}

		private void OnMinigameFinished(int score)
		{
			_fishingMinigame.FinishedEvent -= OnMinigameFinished;
			if (_timerRoutine != null)
			{
				StopCoroutine(_timerRoutine);
				_timerRoutine = null;
			}
			StartCoroutine(EndAnimationRoutine());
		}

		private void OnBannerTweenerFinishedPlaying(Tweener tweener)
		{
			if (tweener.IsPlaybackReversed)
			{
				_bannerRoot.SetActive(value: false);
				_bannerTextRoot.SetActive(value: false);
			}
		}

		private void OnTimerTweenerFinishedPlaying(Tweener tweener)
		{
			if (tweener.IsPlaybackReversed)
			{
				_timerTextRoot.SetActive(value: false);
			}
		}

		private IEnumerator CountdownRoutine()
		{
			yield return ShowBannerRoutine();
			for (int i = 0; i < 3; i++)
			{
				SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.FishingCountdown);
				SetBannerText((3 - i).ToString());
				yield return new WaitForSecondsRealtime(0.6f);
			}
			SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.FishingCountdownFinished);
			SetBannerText("Start!");
			yield return new WaitForSecondsRealtime(0.6f);
			_bannerTweener.StopAndReset(resetToEnd: true);
			_bannerTweener.PlayReverse();
			_countdownRoutine = null;
			_fishingMinigame.StartPlaying();
		}

		private IEnumerator ShowBannerRoutine()
		{
			_bannerRoot.SetActive(value: true);
			_bannerTextRoot.SetActive(value: false);
			_bannerTweener.StopAndReset();
			_bannerTweener.Play();
			yield return new WaitWhile(() => _bannerTweener.IsPlaying);
			_bannerTextRoot.SetActive(value: true);
		}

		private IEnumerator TimerRoutine()
		{
			_timerTextRoot.SetActive(value: true);
			_timerTextTweener.StopAndReset();
			_timerTextTweener.Play();
			_marker.gameObject.SetActive(value: true);
			while (_fishingMinigame.IsPlaying)
			{
				int num = (int)Math.Ceiling(_fishingMinigame.TimeLeft);
				_timerText.Text = num.ToString("00");
				if (num < 3)
				{
					SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.FishingTimer);
				}
				yield return new WaitForSecondsRealtime(1f);
			}
			_marker.gameObject.SetActive(value: false);
			_timerRoutine = null;
		}

		private IEnumerator ScoredRoutine(int amount)
		{
			_scoredText.gameObject.SetActive(value: true);
			LocalizedText scoredText = _scoredText;
			object key;
			switch (amount)
			{
			default:
				key = "great";
				break;
			case 1:
				key = "good_exclamation";
				break;
			case 0:
				key = "miss";
				break;
			}
			scoredText.LocalizedString = Localization.Key((string)key);
			_scoredTextTweener.StopAndReset();
			_scoredTextTweener.Play();
			yield return new WaitForSecondsRealtime(0.8f);
			_scoredText.gameObject.SetActive(value: false);
		}

		private IEnumerator EndAnimationRoutine()
		{
			_marker.gameObject.SetActive(value: false);
			_timerText.Text = 0.ToString("00");
			SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.FishingTimesUp);
			yield return ShowBannerRoutine();
			SetBannerText("Time's up!");
			yield return new WaitForSecondsRealtime(1.5f);
			_bannerTweener.StopAndReset();
			_bannerTweener.PlayReverse();
			_timerTextTweener.StopAndReset(resetToEnd: true);
			_timerTextTweener.PlayReverse();
			_closeTweener.StopAndReset();
			_closeTweener.Play();
			yield return new WaitWhile(() => (!_bannerTweener.IsPlaying && !_timerTextTweener.IsPlaying) ? _closeTweener.IsPlaying : true);
			FireMinigameFinishedEvent();
		}
	}
}
