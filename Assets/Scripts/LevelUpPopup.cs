using CIG;
using CIG.Translation;
using SparkLinq;
using System.Collections;
using System.Collections.Generic;
using Tweening;
using UnityEngine;

public class LevelUpPopup : Popup
{
	private const float StartAnimationDelay = 0.5f;

	private const float SlideStartDelay = 0.5f;

	[SerializeField]
	private LevelUpBuilding _levelUpBuildingPrefab;

	[SerializeField]
	private RectTransform _levelUpBuidlingParent;

	[SerializeField]
	private FastRewardsScrollRect _rewardsScrollRect;

	[SerializeField]
	private Tweener[] _openTweeners;

	[SerializeField]
	private LocalizedText _titleText;

	[SerializeField]
	private LocalizedText _subtitleText;

	[SerializeField]
	private GameObject _continueButton;

	[SerializeField]
	private Transform _rewardContainer;

	[SerializeField]
	private LevelUpRewardItem _rewardPrefab;

	[SerializeField]
	private PopupHeader _popupHeader;

	private GameState _gameState;

	private OneTimeOfferManager _oneTimeOfferManager;

	private Properties _properties;

	private readonly List<LevelUpRewardItem> _rewardsList = new List<LevelUpRewardItem>();

	private int _showingLevel;

	private Coroutine _animationRoutine;

	private Currencies _reward;

	private bool _enqueueOneTimeOffer;

	public override string AnalyticsScreenName => "level_up";

	public int Level
	{
		get;
		private set;
	}

	public override void Initialize(Model model, CIGCanvasScaler canvasScaler)
	{
		base.Initialize(model, canvasScaler);
		_gameState = model.Game.GameState;
		_oneTimeOfferManager = model.Game.OneTimeOfferManager;
		_properties = model.Game.Properties;
		_popupHeader.Initialize(model.Game.Timing);
		_rewardsScrollRect.AnimationFinishedEvent += OnAnimationFinished;
	}

	protected override void OnDestroy()
	{
		if (_rewardsScrollRect != null)
		{
			_rewardsScrollRect.AnimationFinishedEvent -= OnAnimationFinished;
		}
		if (SingletonMonobehaviour<FPSLimiter>.IsAvailable)
		{
			SingletonMonobehaviour<FPSLimiter>.Instance.PopUnlimitedFPSRequest(this);
		}
		base.OnDestroy();
	}

	public override void Open(PopupRequest request)
	{
		base.Open(request);
		_enqueueOneTimeOffer = true;
		SingletonMonobehaviour<FPSLimiter>.Instance.PushUnlimitedFPSRequest(this);
		LevelUpPopupRequest request2 = GetRequest<LevelUpPopupRequest>();
		_reward = request2.Reward;
		Level = request2.Level;
		UpdateElements(request2.Level);
		Clip clip = Clip.LevelUp;
		SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.LevelUp);
		float clipLength = SingletonMonobehaviour<AudioManager>.Instance.GetClipLength(clip);
		SingletonMonobehaviour<AudioManager>.Instance.SetMusicVolume(0.2f, clipLength);
	}

	public override void Close(bool instant)
	{
		if (_reward != null)
		{
			_gameState.GiveLevelUpReward(_reward);
			_reward = null;
		}
		OneTimeOfferBase availableOffer = _oneTimeOfferManager.GetAvailableOffer(_showingLevel);
		if (availableOffer != null)
		{
			_popupManager.RequestPopup(new OneTimeOfferPopupRequest(availableOffer, _enqueueOneTimeOffer));
		}
		base.Close(instant);
	}

	public void OnContinueClicked()
	{
		_enqueueOneTimeOffer = false;
		OnCloseClicked();
	}

	protected override void Closed()
	{
		SingletonMonobehaviour<FPSLimiter>.Instance.PopUnlimitedFPSRequest(this);
		int i = 0;
		for (int count = _rewardsList.Count; i < count; i++)
		{
			UnityEngine.Object.Destroy(_rewardsList[i].gameObject);
		}
		_rewardsList.Clear();
		_rewardsScrollRect.Deinitialize();
		_popupHeader.Stop();
		base.Closed();
	}

	private void Show(int level, List<BuildingProperties> buildings)
	{
		_showingLevel = level;
		_titleText.LocalizedString = Localization.Format(Localization.Key("you_have_reached_level_exclamation"), Localization.Integer(level));
		_subtitleText.LocalizedString = Localization.Key("you_have_unlocked_buildings");
		List<RewardScrollRectItem> list = new List<RewardScrollRectItem>();
		int i = 0;
		for (int count = buildings.Count; i < count; i++)
		{
			LevelUpBuilding levelUpBuilding = Object.Instantiate(_levelUpBuildingPrefab, _levelUpBuidlingParent);
			levelUpBuilding.Initialize(_popupManager, level, buildings[i]);
			list.Add(levelUpBuilding);
		}
		_rewardsScrollRect.Initialize(list);
		for (int j = 0; j < _reward.KeyCount; j++)
		{
			Currency currency = _reward.GetCurrency(j);
			LevelUpRewardItem levelUpRewardItem = Object.Instantiate(_rewardPrefab, _rewardContainer);
			levelUpRewardItem.Initialize(SingletonMonobehaviour<CurrencySpriteAssetCollection>.Instance.GetCurrencySprite(currency, CurrencySpriteSize.Medium), Localization.Integer(currency.Value));
			_rewardsList.Add(levelUpRewardItem);
		}
		if (_animationRoutine != null)
		{
			StopCoroutine(_animationRoutine);
		}
		_animationRoutine = StartCoroutine(AnimationRoutine());
	}

	private void UpdateElements(int level)
	{
		List<BuildingProperties> allBuildingProperties = _properties.AllBuildingProperties;
		List<BuildingProperties> list = new List<BuildingProperties>();
		int i = 0;
		for (int count = allBuildingProperties.Count; i < count; i++)
		{
			if (allBuildingProperties[i].UnlockLevels.Any((int l) => l == level))
			{
				list.Add(allBuildingProperties[i]);
			}
		}
		Show(level, list);
	}

	private void OnAnimationFinished()
	{
		_continueButton.SetActive(value: true);
	}

	private IEnumerator AnimationRoutine()
	{
		_continueButton.SetActive(value: false);
		int i = 0;
		for (int num = _openTweeners.Length; i < num; i++)
		{
			_openTweeners[i].StopAndReset();
		}
		yield return new WaitForSeconds(0.5f);
		int j = 0;
		for (int num2 = _openTweeners.Length; j < num2; j++)
		{
			_openTweeners[j].Play();
		}
		yield return new WaitForSeconds(0.5f);
		_popupHeader.Play();
		_rewardsScrollRect.StartAnimation();
	}
}
