using CIG.Translation;
using System;
using System.Collections;
using Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class DailyRewardsCollectPopup : ToggleableInteractionPopup
	{
		[Serializable]
		private class DailyRewardCollectStyle
		{
			[SerializeField]
			private FlutteringImageSystem _flutteringImageSystem;

			[SerializeField]
			private Sprite _iconSprite;

			public FlutteringImageSystem FlutteringImageSystem => _flutteringImageSystem;

			public Sprite IconSprite => _iconSprite;
		}

		[SerializeField]
		private GameObject _subtitle;

		[SerializeField]
		private Image[] _currencyIcons;

		[SerializeField]
		private Image _currencyCollectIcon;

		[SerializeField]
		private LocalizedText _currencyAmount;

		[SerializeField]
		private NumberTweenHelper _discountTextTweener;

		[SerializeField]
		private ParticleSystem _collectParticles;

		[SerializeField]
		private Tweener _collectTweener;

		[SerializeField]
		private GameObject _collectWindowPlingParticle;

		[SerializeField]
		private GameObject _collectWindowRays;

		[SerializeField]
		private GameObject _rewardDoubledBadge;

		[SerializeField]
		private Tweener _rewardDoubledBadgeTweener;

		[SerializeField]
		private LocalizedText _rewardDoubledBadgeText;

		[SerializeField]
		private GameObject _doubleRewardsWindow;

		[SerializeField]
		private LocalizedText _badgeText;

		[SerializeField]
		private DailyRewardCollectStyle _cashItemStyle;

		[SerializeField]
		private DailyRewardCollectStyle _silverKeyItemStyle;

		[SerializeField]
		private DailyRewardCollectStyle _goldKeyItemStyle;

		[SerializeField]
		private DailyRewardCollectStyle _tokenItemStyle;

		private DailyRewardsManager _dailyRewardsManager;

		private IEnumerator _doubleRewardAnimationRoutine;

		private FlutteringImageSystem _activeFlutteringImageSystem;

		private Currency _todaysReward;

		public override string AnalyticsScreenName => "daily_reward_collect";

		public override void Initialize(Model model, CIGCanvasScaler canvasScaler)
		{
			base.Initialize(model, canvasScaler);
			_dailyRewardsManager = model.Game.DailyRewardsManager;
			_cashItemStyle.FlutteringImageSystem.Initialize(model.Game.Timing);
			_silverKeyItemStyle.FlutteringImageSystem.Initialize(model.Game.Timing);
			_goldKeyItemStyle.FlutteringImageSystem.Initialize(model.Game.Timing);
			_tokenItemStyle.FlutteringImageSystem.Initialize(model.Game.Timing);
		}

		public override void Open(PopupRequest request)
		{
			base.Open(request);
			DailyRewardCollectPopupRequest request2 = GetRequest<DailyRewardCollectPopupRequest>();
			_todaysReward = request2.TodaysReward;
			bool flag = _dailyRewardsManager.CanDoubleReward && !_dailyRewardsManager.RewardIsDoubled;
			_subtitle.SetActive(flag);
			_doubleRewardsWindow.SetActive(flag);
			_collectWindowRays.SetActive(!flag);
			_collectWindowPlingParticle.SetActive(!flag);
			_rewardDoubledBadge.SetActive(value: false);
			_badgeText.LocalizedString = Localization.Format(Localization.Key("times_x"), Localization.Integer(2));
			SetStyle(flag);
		}

		public void OnCollectClicked()
		{
			base.Interactable = false;
			StartCoroutine(CollectAnimationRoutine());
		}

		public void OnWatchVideoClicked()
		{
			base.Interactable = false;
			_dailyRewardsManager.WatchVideo(ShowDoubledReward, VideoCanceled);
		}

		private void ShowDoubledReward(Currency doubledReward)
		{
			base.Interactable = true;
			_doubleRewardsWindow.SetActive(value: false);
			_subtitle.SetActive(value: false);
			_collectWindowRays.SetActive(value: true);
			_collectWindowPlingParticle.SetActive(value: true);
			if (_doubleRewardAnimationRoutine == null)
			{
				StartCoroutine(_doubleRewardAnimationRoutine = DoubleRewardAnimationRoutine(_todaysReward.Value, doubledReward.Value));
			}
		}

		private void SetStyle(bool videoAvailable)
		{
			_currencyAmount.LocalizedString = Localization.Integer(_todaysReward.Value);
			switch (_todaysReward.Name)
			{
			case "Cash":
				SetCurrencyIcons(_cashItemStyle.IconSprite);
				_activeFlutteringImageSystem = _cashItemStyle.FlutteringImageSystem;
				break;
			case "SilverKey":
				SetCurrencyIcons(_silverKeyItemStyle.IconSprite);
				_activeFlutteringImageSystem = _silverKeyItemStyle.FlutteringImageSystem;
				break;
			case "GoldKey":
				SetCurrencyIcons(_goldKeyItemStyle.IconSprite);
				_activeFlutteringImageSystem = _goldKeyItemStyle.FlutteringImageSystem;
				break;
			case "Token":
				SetCurrencyIcons(_tokenItemStyle.IconSprite);
				_activeFlutteringImageSystem = _tokenItemStyle.FlutteringImageSystem;
				break;
			}
			if (videoAvailable && _activeFlutteringImageSystem != null)
			{
				_activeFlutteringImageSystem.Play();
			}
		}

		private void SetCurrencyIcons(Sprite icon)
		{
			int i = 0;
			for (int num = _currencyIcons.Length; i < num; i++)
			{
				_currencyIcons[i].sprite = icon;
			}
		}

		private void OnCollect()
		{
			_dailyRewardsManager.Collect();
			_activeFlutteringImageSystem.Stop();
			ForceClose();
		}

		private void VideoCanceled()
		{
			base.Interactable = true;
			_doubleRewardsWindow.SetActive(value: false);
			_subtitle.SetActive(value: false);
			_collectWindowRays.SetActive(value: true);
			_collectWindowPlingParticle.SetActive(value: true);
		}

		private IEnumerator CollectAnimationRoutine()
		{
			SpawnedParticles currencyRewardParticles = SingletonMonobehaviour<ParticlesAssetCollection>.Instance.GetCurrencyRewardParticles(_todaysReward);
			if (currencyRewardParticles != null)
			{
				UnityEngine.Object.Instantiate(currencyRewardParticles, _currencyCollectIcon.transform).Play();
			}
			SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.QuestComplete);
			_collectParticles.Play();
			_collectTweener.StopAndReset();
			_collectTweener.Play();
			yield return new WaitWhile(() => _collectTweener.IsPlaying);
			yield return new WaitForSeconds(0.5f);
			OnCollect();
		}

		private IEnumerator DoubleRewardAnimationRoutine(decimal startingValue, decimal doubledValue)
		{
			yield return new WaitForSeconds(0.2f);
			_discountTextTweener.TweenTo(startingValue, doubledValue);
			yield return new WaitWhile(() => _discountTextTweener.Running);
			_collectTweener.StopAndReset();
			_collectTweener.Play();
			yield return new WaitForSeconds(0.2f);
			_rewardDoubledBadge.SetActive(value: true);
			_rewardDoubledBadgeTweener.StopAndReset();
			_rewardDoubledBadgeTweener.Play();
			_rewardDoubledBadgeText.LocalizedString = Localization.Format(Localization.Key("times_x"), Localization.Integer(2));
			_doubleRewardAnimationRoutine = null;
		}
	}
}
