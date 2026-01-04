using CIG.Translation;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class WheelOfFortunePopup : Popup
	{
		private enum State
		{
			WaitingForSpin,
			Spinning,
			CollectReward,
			NoReward,
			Transition
		}

		private const float LampRadius = 292.5f;

		private const int LampAmount = 20;

		private const float RotationDegrees = 2160f;

		private const float DurationPerRotation = 1f;

		private const float TransitionDuration = 0.75f;

		private const string TitleText = "FORTUNE SPINNER";

		[SerializeField]
		private LocalizedText _bottomText;

		[SerializeField]
		private LocalizedText _oddsText;

		[SerializeField]
		private TextMeshProUGUI[] _titleTexts;

		[SerializeField]
		private NumberTweenHelper _normalBalanceAmount;

		[SerializeField]
		private NumberTweenHelper _premiumBalanceAmount;

		[SerializeField]
		private CurrencyView _normalBalanceCurrencyView;

		[SerializeField]
		private CurrencyView _premiumBalanceCurrencyView;

		[Header("Prefabs")]
		[SerializeField]
		private WheelOfFortuneLamp _lampPrefab;

		[SerializeField]
		private RectTransform _lampParent;

		[SerializeField]
		private WheelOfFortunePartition _partitionPrefab;

		[SerializeField]
		private RectTransform _partitionParent;

		[Header("Animation")]
		[SerializeField]
		private RectTransform _rotationParent;

		[SerializeField]
		private AnimationCurve _rotationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

		[SerializeField]
		private Tweener _tickerTweener;

		[SerializeField]
		private AnimationCurve _transitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

		[Header("Particles")]
		[SerializeField]
		private ParticleSystem _confettiParticles;

		[SerializeField]
		private ParticleSystem _spinningParticles;

		[SerializeField]
		private ParticleSystem _successParticles;

		[SerializeField]
		private RectTransform _currencyParticlesParent;

		[Header("Winnings View")]
		[SerializeField]
		private Transform _winningsContentContainer;

		[SerializeField]
		private WheelOfFortuneWinningsElement _winningsElementPrefab;

		[SerializeField]
		private Tweener _winningsContentSlideTweener;

		[Header("Buttons")]
		[SerializeField]
		private CurrencyView _normalCost;

		[SerializeField]
		private InteractableButton _normalCostButton;

		[SerializeField]
		private CurrencyView _premiumCost;

		[SerializeField]
		private InteractableButton _premiumCostButton;

		[SerializeField]
		private InteractableButton _bottomCollectButton;

		[SerializeField]
		private LocalizedText _bottomCollectButtonText;

		[SerializeField]
		private Button _closeButton;

		private WheelOfFortune _wheelOfFortune;

		private GameState _gameState;

		private Timing _timing;

		private readonly List<WheelOfFortunePartition> _partitions = new List<WheelOfFortunePartition>();

		private IEnumerator _spinRoutine;

		private IEnumerator _transitionRoutine;

		private float _anglePerPartition;

		private float _targetAngle;

		private readonly Dictionary<string, WheelOfFortuneWinningsElement> _winningsElements = new Dictionary<string, WheelOfFortuneWinningsElement>();

		public override string AnalyticsScreenName => "wheel_of_fortune";

		private bool ShowOddsText => false;

		public override void Initialize(Model model, CIGCanvasScaler canvasScaler)
		{
			base.Initialize(model, canvasScaler);
			_gameState = model.Game.GameState;
			_wheelOfFortune = model.Game.ArcadeManager.WheelOfFortune;
			_timing = model.Game.Timing;
			for (int i = 0; i < 20; i++)
			{
				float f = (float)Math.PI / 10f * (float)i;
				WheelOfFortuneLamp wheelOfFortuneLamp = UnityEngine.Object.Instantiate(_lampPrefab, _lampParent);
				wheelOfFortuneLamp.Initialize(i % 2 == 0);
				wheelOfFortuneLamp.transform.localPosition = new Vector3(Mathf.Cos(f), Mathf.Sin(f), 0f) * 292.5f;
			}
		}

		protected override void OnDestroy()
		{
			if (_wheelOfFortune != null)
			{
				_wheelOfFortune.WheelSpunEvent -= OnWheelSpun;
				_wheelOfFortune = null;
			}
			if (_gameState != null)
			{
				_gameState.BalanceChangedEvent -= OnBalanceChanged;
				_gameState = null;
			}
			base.OnDestroy();
		}

		public override void Open(PopupRequest request)
		{
			base.Open(request);
			CreatePartitions();
			_normalCost.Initialize(_wheelOfFortune.NormalCost);
			_normalBalanceCurrencyView.Initialize(_wheelOfFortune.NormalCost.ToCurrencyType(), _gameState.Balance.GetValue(_wheelOfFortune.NormalCost.Name));
			_premiumCost.Initialize(_wheelOfFortune.PremiumCost);
			_premiumBalanceCurrencyView.Initialize(_wheelOfFortune.PremiumCost.ToCurrencyType(), _gameState.Balance.GetValue(_wheelOfFortune.PremiumCost.Name));
			_gameState.BalanceChangedEvent += OnBalanceChanged;
			OnBalanceChanged(_gameState.Balance, _gameState.Balance, new FlyingCurrenciesData());
			_wheelOfFortune.WheelSpunEvent += OnWheelSpun;
			if (_wheelOfFortune.PendingReward != null)
			{
				OnWheelSpun(null, _wheelOfFortune.PendingReward.Reward, _wheelOfFortune.PendingReward.Index);
			}
			_winningsContentSlideTweener.StopAndReset();
			UpdateState(DetermineState());
		}

		public override void OnCloseClicked()
		{
			if (_spinRoutine == null)
			{
				_wheelOfFortune.CollectReward();
				_successParticles.Stop();
				base.OnCloseClicked();
			}
		}

		public void OnNormalSpinClicked()
		{
			_wheelOfFortune.NormalSpin();
		}

		public void OnPremiumSpinClicked()
		{
			_wheelOfFortune.PremiumSpin();
		}

		public void OnCollectClicked()
		{
			if (_wheelOfFortune.PendingReward != null)
			{
				int index = _wheelOfFortune.PendingReward.Index;
				Currency currency = (_wheelOfFortune.PendingReward.Reward.KeyCount > 0) ? _wheelOfFortune.PendingReward.Reward.GetCurrency(0) : Currency.Invalid;
				AppendToWinnings(currency);
				_wheelOfFortune.CollectReward(_partitions[index]);
				_successParticles.Stop();
				StartCoroutine(_transitionRoutine = TransitionRoutine(index, currency));
				UpdateState(State.Transition);
			}
		}

		protected override void Closed()
		{
			_gameState.BalanceChangedEvent -= OnBalanceChanged;
			_wheelOfFortune.WheelSpunEvent -= OnWheelSpun;
			ClearPartition();
			if (_spinRoutine != null)
			{
				StopCoroutine(_spinRoutine);
				_spinRoutine = null;
			}
			if (_transitionRoutine != null)
			{
				StopCoroutine(_transitionRoutine);
				_transitionRoutine = null;
			}
			foreach (KeyValuePair<string, WheelOfFortuneWinningsElement> winningsElement in _winningsElements)
			{
				UnityEngine.Object.Destroy(winningsElement.Value.gameObject);
			}
			_winningsElements.Clear();
			base.Closed();
		}

		private void UpdateState(State state)
		{
			switch (state)
			{
			case State.WaitingForSpin:
				UpdateButton(_normalCostButton, active: true, _wheelOfFortune.CanAffordNormalCost);
				UpdateButton(_premiumCostButton, active: true, interactable: true);
				UpdateButton(_bottomCollectButton, active: false, interactable: false);
				UpdateButton(_closeButton, active: true, interactable: true);
				UpdatePartitions();
				SetTitleText("FORTUNE SPINNER");
				SetBottomText(Localization.Key("wheel_of_fortune_spin"));
				ShowOdds(ShowOddsText);
				break;
			case State.Spinning:
				UpdateButton(_normalCostButton, active: false, interactable: false);
				UpdateButton(_premiumCostButton, active: false, interactable: false);
				UpdateButton(_bottomCollectButton, active: false, interactable: false);
				UpdateButton(_closeButton, active: false, interactable: true);
				UpdatePartitions();
				SetTitleText("FORTUNE SPINNER");
				SetBottomText(Localization.Key("wheel_of_fortune_spinning"));
				_bottomCollectButtonText.LocalizedString = Localization.Key("rewards.collect");
				ShowOdds(show: false);
				break;
			case State.CollectReward:
				UpdateButton(_normalCostButton, active: false, interactable: false);
				UpdateButton(_premiumCostButton, active: false, interactable: false);
				UpdateButton(_bottomCollectButton, active: true, interactable: true);
				UpdateButton(_closeButton, active: true, interactable: true);
				UpdatePartitions(_wheelOfFortune.PendingReward.Index);
				SetTitleText("CONGRATULATIONS!");
				SetBottomText(Localization.Key("collect_your_reward"));
				ShowOdds(show: false);
				_bottomCollectButtonText.LocalizedString = Localization.Key("rewards.collect");
				_confettiParticles.Play();
				_successParticles.Play();
				break;
			case State.NoReward:
				UpdateButton(_normalCostButton, active: false, interactable: false);
				UpdateButton(_premiumCostButton, active: false, interactable: false);
				UpdateButton(_bottomCollectButton, active: true, interactable: true);
				UpdateButton(_closeButton, active: true, interactable: true);
				UpdatePartitions(_wheelOfFortune.PendingReward.Index);
				SetTitleText("TOO BAD...");
				SetBottomText(Localization.Key("try_again"));
				ShowOdds(show: false);
				_bottomCollectButtonText.LocalizedString = Localization.Key("ok");
				break;
			case State.Transition:
				UpdateButton(_bottomCollectButton, active: false, interactable: false);
				UpdateButton(_closeButton, active: true, interactable: true);
				SetBottomText(null);
				break;
			}
		}

		private void CreatePartitions()
		{
			ClearPartition();
			int count = _wheelOfFortune.CurrentRewards.Rewards.Count;
			_anglePerPartition = 360f / (float)count;
			int i = 0;
			for (int num = count; i < num; i++)
			{
				Currencies currencies = _wheelOfFortune.CurrentRewards.Rewards[i];
				WheelOfFortunePartition wheelOfFortunePartition = UnityEngine.Object.Instantiate(_partitionPrefab, _partitionParent);
				wheelOfFortunePartition.Initialize(_timing, currencies.GetCurrency(0), _anglePerPartition, _anglePerPartition * (float)i);
				_partitions.Add(wheelOfFortunePartition);
			}
		}

		private void UpdatePartitions(int? index = default(int?))
		{
			int i = 0;
			for (int count = _partitions.Count; i < count; i++)
			{
				if (index.HasValue)
				{
					if (i == index.Value)
					{
						_partitions[i].SetHighlight();
					}
					else
					{
						_partitions[i].SetDisabled();
					}
				}
				else
				{
					_partitions[i].SetNeutral();
				}
			}
		}

		private void SetTitleText(string text)
		{
			int i = 0;
			for (int num = _titleTexts.Length; i < num; i++)
			{
				_titleTexts[i].text = text;
			}
		}

		private void SetBottomText(ILocalizedString text)
		{
			_bottomText.gameObject.SetActive(!Localization.IsNullOrEmpty(text));
			_bottomText.LocalizedString = text;
		}

		private void UpdateButton(Button button, bool active, bool interactable)
		{
			button.gameObject.SetActive(active);
			button.interactable = interactable;
		}

		private void ClearPartition()
		{
			int i = 0;
			for (int count = _partitions.Count; i < count; i++)
			{
				UnityEngine.Object.Destroy(_partitions[i].gameObject);
			}
			_partitions.Clear();
			_rotationParent.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
		}

		private void OnBalanceChanged(Currencies oldBalance, Currencies newBalance, FlyingCurrenciesData flyingCurrenciesData)
		{
			_normalBalanceAmount.TweenTo(newBalance.GetValue(_wheelOfFortune.NormalCost.Name));
			_premiumBalanceAmount.TweenTo(newBalance.GetValue(_wheelOfFortune.PremiumCost.Name));
		}

		private void OnWheelSpun(Currencies spent, Currencies earned, int index)
		{
			_targetAngle = 2160f - (float)index * _anglePerPartition;
			if (_spinRoutine != null)
			{
				StopCoroutine(_spinRoutine);
			}
			StartCoroutine(_spinRoutine = SpinRoutine());
		}

		private void ShowOdds(bool show)
		{
			_oddsText.gameObject.SetActive(show);
			_oddsText.LocalizedString = Localization.Format(Localization.Key("equal_odds_for_all_items"), Localization.Integer(1), Localization.Integer(_partitions.Count));
		}

		private State DetermineState()
		{
			if (_spinRoutine != null)
			{
				return State.Spinning;
			}
			if (_wheelOfFortune.PendingReward != null)
			{
				if (_wheelOfFortune.PendingReward.Reward.KeyCount > 0)
				{
					return State.CollectReward;
				}
				return State.NoReward;
			}
			return State.WaitingForSpin;
		}

		private IEnumerator SpinRoutine()
		{
			UpdateState(State.Spinning);
			_spinningParticles.Play();
			float time = 0f;
			float duration = 1f * (_targetAngle / 360f);
			int previousIndex = 0;
			while (time < duration)
			{
				float num = _rotationCurve.Evaluate(time / duration) * _targetAngle;
				_rotationParent.transform.rotation = Quaternion.Euler(0f, 0f, num);
				time += _timing.GetDeltaTime(DeltaTimeType.Unscaled);
				int num2 = _wheelOfFortune.CurrentRewards.Rewards.Count - (int)(num % 360f / _anglePerPartition) - 1;
				if (num2 != previousIndex)
				{
					SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.WheelOfFortuneTick);
					_tickerTweener.StopAndReset();
					_tickerTweener.Play();
					previousIndex = num2;
				}
				yield return null;
			}
			_spinningParticles.Stop();
			_spinRoutine = null;
			bool flag = _wheelOfFortune.PendingReward.Reward.KeyCount > 0;
			UpdateState(flag ? State.CollectReward : State.NoReward);
			SingletonMonobehaviour<AudioManager>.Instance.PlayClip(flag ? Clip.WheelOfFortuneSuccess : Clip.WheelOfFortuneTooBad);
		}

		private IEnumerator TransitionRoutine(int index, Currency currency)
		{
			if (currency.IsValid)
			{
				WheelOfFortunePartition partition = _partitions[index];
				SpawnedParticles currencyRewardParticles = SingletonMonobehaviour<ParticlesAssetCollection>.Instance.GetCurrencyRewardParticles(currency);
				if (currencyRewardParticles != null)
				{
					UnityEngine.Object.Instantiate(currencyRewardParticles, partition.ParticlePosition, currencyRewardParticles.transform.localRotation, _currencyParticlesParent).Play();
				}
				SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.WheelOfFortuneCollect);
				partition.StartCollectAnimation();
				yield return new WaitWhile(() => partition.IsHighlighting);
				yield return new WaitForSeconds(0.5f);
			}
			yield return TransitionFadeRoutine(show: true);
			CreatePartitions();
			yield return TransitionFadeRoutine(show: false);
			_transitionRoutine = null;
			UpdateState(State.WaitingForSpin);
		}

		private IEnumerator TransitionFadeRoutine(bool show)
		{
			float time = 0f;
			while (time < 0.75f)
			{
				time += _timing.GetDeltaTime(DeltaTimeType.Unscaled);
				float num = Mathf.Clamp01(_transitionCurve.Evaluate(time / 0.75f));
				float amount = show ? (1f - num) : num;
				int i = 0;
				for (int count = _partitions.Count; i < count; i++)
				{
					_partitions[i].Fade(amount);
				}
				yield return null;
			}
		}

		private void AppendToWinnings(Currency currency)
		{
			if (currency.IsValid)
			{
				if (_winningsElements.Count == 0)
				{
					_winningsContentSlideTweener.Play();
				}
				if (_winningsElements.ContainsKey(currency.Name))
				{
					_winningsElements[currency.Name].AddValue(currency.Value);
					return;
				}
				WheelOfFortuneWinningsElement wheelOfFortuneWinningsElement = UnityEngine.Object.Instantiate(_winningsElementPrefab, _winningsContentContainer);
				wheelOfFortuneWinningsElement.Initialize(currency);
				_winningsElements.Add(currency.Name, wheelOfFortuneWinningsElement);
			}
		}
	}
}
