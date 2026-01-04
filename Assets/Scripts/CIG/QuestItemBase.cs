using CIG.Translation;
using System.Collections;
using Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public abstract class QuestItemBase : MonoBehaviour
	{
		public delegate void CollectAnimationFinishedEventHandler(Vector3 from, QuestSpriteType questSpriteType);

		[SerializeField]
		private LocalizedText _description;

		[SerializeField]
		private LocalizedText _rewardLabel;

		[SerializeField]
		private CompositeSpriteImage _questTypeIcon;

		[SerializeField]
		private Image _rewardIcon;

		[SerializeField]
		private Button _collectButton;

		[SerializeField]
		private Image _completedBanner;

		[SerializeField]
		private LocalizedText _completedBannerText;

		[SerializeField]
		private CurrencyAnimationSource _currencyAnimationSource;

		[SerializeField]
		private Tweener _bannerTweener;

		[SerializeField]
		private Tweener _textTweener;

		[SerializeField]
		private Tweener _raysTweener;

		protected Quest _quest;

		private IEnumerator _collectRoutine;

		public event CollectAnimationFinishedEventHandler CollectAnimationFinishedEvent;

		private void FireCollectAnimationFinishedEvent(Vector3 from, QuestSpriteType questSpriteType)
		{
			this.CollectAnimationFinishedEvent?.Invoke(from, questSpriteType);
		}

		public virtual void Initialize(Quest quest)
		{
			Deinitialize();
			_currencyAnimationSource.Initialize(this);
			_quest = quest;
			_questTypeIcon.Initialize(SingletonMonobehaviour<QuestSpriteAssetCollection>.Instance.GetAsset(_quest.QuestSpriteType).LargeSpriteData);
			UpdateLook();
		}

		public virtual void Deinitialize()
		{
			_quest = null;
			_currencyAnimationSource.Deinitialize();
			_textTweener.StopAndReset();
			_bannerTweener.StopAndReset();
			_raysTweener.StopIfPlaying();
		}

		private void OnDestroy()
		{
			Deinitialize();
		}

		private void OnDisable()
		{
			if (_collectRoutine != null)
			{
				StopCoroutine(_collectRoutine);
				_collectRoutine = null;
			}
		}

		public void OnClaimRewardClicked()
		{
			if (_quest.State == QuestState.Achieved)
			{
				StartCoroutine(_collectRoutine = CollectRoutine());
			}
		}

		protected void UpdateLook()
		{
			if (_collectRoutine == null)
			{
				_description.LocalizedString = _quest.Description;
				UpdateProgress();
				switch (_quest.State)
				{
				case QuestState.InProgress:
					SetLookInProgress();
					break;
				case QuestState.Achieved:
					SetLookAchieved();
					break;
				case QuestState.Completed:
					SetLookCompleted();
					break;
				}
			}
		}

		protected abstract void UpdateProgress();

		private void SetReward(Currencies reward, bool collected)
		{
			Currency currency = reward.GetCurrency(0);
			if (currency.IsValid)
			{
				_rewardIcon.sprite = SingletonMonobehaviour<CurrencySpriteAssetCollection>.Instance.GetCurrencySprite(currency);
				_rewardLabel.LocalizedString = (collected ? Localization.Key("collected") : Localization.Integer(currency.Value));
			}
			else
			{
				UnityEngine.Debug.LogError("Reward cannot be empty.");
			}
		}

		private void SetLookAchieved()
		{
			SetReward(_quest.Reward, collected: false);
			_collectButton.interactable = true;
			_completedBanner.gameObject.SetActive(value: true);
			_completedBannerText.LocalizedString = Localization.Key("rewards.collect");
			_raysTweener.PlayIfStopped();
		}

		private void SetLookInProgress()
		{
			SetReward(_quest.Reward, collected: false);
			_collectButton.interactable = false;
			_completedBanner.gameObject.SetActive(value: false);
			_raysTweener.StopIfPlaying();
		}

		private void SetLookCompleted()
		{
			SetReward(_quest.Reward, collected: true);
			_collectButton.interactable = false;
			_completedBanner.gameObject.SetActive(value: true);
			_completedBannerText.LocalizedString = Localization.Key("button.done");
			_raysTweener.StopIfPlaying();
		}

		private IEnumerator CollectRoutine()
		{
			SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.QuestComplete);
			_collectButton.interactable = false;
			_textTweener.Play();
			_quest.Collect(new FlyingCurrenciesData(this));
			SpawnedParticles currencyRewardParticles = SingletonMonobehaviour<ParticlesAssetCollection>.Instance.GetCurrencyRewardParticles(_quest.Reward.GetCurrency(0));
			if (currencyRewardParticles != null)
			{
				Object.Instantiate(currencyRewardParticles, _rewardIcon.transform).Play();
			}
			Material oldMat = _completedBanner.material;
			Material asset = SingletonMonobehaviour<MaterialAssetCollection>.Instance.GetAsset(MaterialType.UITransparent);
			_completedBanner.material = asset;
			_completedBannerText.TextField.material = asset;
			_bannerTweener.Play();
			yield return new WaitWhile(() => _bannerTweener.IsPlaying);
			yield return new WaitWhile(() => _textTweener.IsPlaying);
			_bannerTweener.Reset();
			_textTweener.Reset();
			_completedBanner.material = oldMat;
			_completedBannerText.TextField.material = oldMat;
			_collectRoutine = null;
			FireCollectAnimationFinishedEvent(_questTypeIcon.transform.position, _quest.QuestSpriteType);
			UpdateLook();
		}
	}
}
