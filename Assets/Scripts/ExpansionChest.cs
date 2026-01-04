using CIG;
using CIG.Translation;
using System.Collections;
using Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ExpansionChest : GridTile, IPointerClickHandler, IEventSystemHandler
{
	[SerializeField]
	private PlingManager _plingManager;

	[SerializeField]
	private CurrencyAnimationSource _currencyAnimationSource;

	[SerializeField]
	private Tweener _enterTweener;

	[SerializeField]
	private AnimatedSprite _enterAnimatedSprite;

	[SerializeField]
	private GameObject _lockedIdleObjectLand;

	[SerializeField]
	private GameObject _lockedIdleObjectWater;

	[SerializeField]
	private GameObject _unlockedIdleObjectLand;

	[SerializeField]
	private GameObject _unlockedIdleObjectWater;

	[SerializeField]
	private AnimatedSprite _openAnimatedSprite;

	[SerializeField]
	private GameObject _coinParticles;

	[SerializeField]
	private AnimatedSprite _closeAnimatedSprite;

	[SerializeField]
	private Tweener _exitTweener;

	[SerializeField]
	private AnimatedSprite _exitAnimatedSprite;

	private ExpansionBlock _expansionBlock;

	private ExpansionChestProperties _properties;

	private IEnumerator _enterAnimationRoutine;

	private bool _collecting;

	public override void Initialize(StorageDictionary storage, IsometricGrid isometricGrid, IslandsManagerView islandsManagerView, WorldMap worldMap, BuildingWarehouseManager buildingWarehouseManager, CraneManager craneManager, GameStats gameStats, GameState gameState, PopupManager popupManager, Multipliers multipliers, Timing timing, RoutineRunner routineRunner, GridTileProperties properties, OverlayManager overlayManager, CIGIslandState islandState, GridIndex? index = default(GridIndex?))
	{
		_properties = (ExpansionChestProperties)properties;
		_plingManager.Initialize(overlayManager);
		base.Initialize(storage, isometricGrid, islandsManagerView, worldMap, buildingWarehouseManager, craneManager, gameStats, gameState, popupManager, multipliers, timing, routineRunner, properties, overlayManager, islandState, index);
	}

	public void Initialize(ExpansionBlock expansionBlock, bool enter)
	{
		_expansionBlock = expansionBlock;
		_currencyAnimationSource.Initialize(this);
		base.Element.UnlockedChangedEvent += OnElementUnlockedChanged;
		if (enter)
		{
			StartCoroutine(_enterAnimationRoutine = EnterAnimationRoutine());
		}
		else
		{
			SetIdleState(active: true, base.Element.Unlocked);
		}
	}

	protected override void OnDestroy()
	{
		if (base.Element != null)
		{
			base.Element.UnlockedChangedEvent -= OnElementUnlockedChanged;
		}
		base.OnDestroy();
	}

	void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
	{
		if (_collecting)
		{
			return;
		}
		if (base.Element.Unlocked)
		{
			if (_enterAnimationRoutine == null)
			{
				Collect();
			}
		}
		else
		{
			GenericPopupRequest request = new GenericPopupRequest("expansion_chest_locked").SetTexts(Localization.Key("locked"), Localization.Key("expansion_chest_locked")).SetGreenButton(Localization.Key("buy"), null, OnOkClicked).SetRedCancelButton()
				.SetIcon(SingletonMonobehaviour<UISpriteAssetCollection>.Instance.GetAsset(UISpriteType.ExpansionChest));
			_popupManager.RequestPopup(request);
		}
	}

	private void Collect()
	{
		_collecting = true;
		Currency currency = Currency.TokenCurrency(Random.Range(_properties.MinTokens, _properties.MaxTokens + 1));
		_gameState.EarnCurrencies(currency, CurrenciesEarnedReason.ExpansionChest, new FlyingCurrenciesData(this));
		_plingManager.ShowCurrencyPlings(_timing, currency);
		StartCoroutine(OpenAnimationRoutine());
	}

	private void SetIdleState(bool active, bool unlocked)
	{
		bool flag = base.Element.Type == SurfaceType.Water;
		_lockedIdleObjectLand.SetActive(active && !unlocked && !flag);
		_lockedIdleObjectWater.SetActive((active && !unlocked) & flag);
		_unlockedIdleObjectLand.SetActive(active && unlocked && !flag);
		_unlockedIdleObjectWater.SetActive((active && unlocked) & flag);
	}

	private void OnElementUnlockedChanged(bool unlocked)
	{
		SetIdleState(_enterAnimationRoutine == null, unlocked);
	}

	private void OnOkClicked()
	{
		_popupManager.RequestPopup(new BuyExpansionPopupRequest(_expansionBlock));
	}

	private IEnumerator EnterAnimationRoutine()
	{
		_enterAnimatedSprite.gameObject.SetActive(value: true);
		yield return null;
		SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.ChestAppear);
		yield return new WaitWhile(() => _enterTweener.IsPlaying);
		SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.ChestHitFloor);
		yield return new WaitWhile(() => _enterAnimatedSprite.IsPlaying);
		_enterAnimatedSprite.gameObject.SetActive(value: false);
		SetIdleState(active: true, base.Element.Unlocked);
		_enterAnimationRoutine = null;
	}

	private IEnumerator OpenAnimationRoutine()
	{
		SetIdleState(active: false, unlocked: false);
		_openAnimatedSprite.gameObject.SetActive(value: true);
		_coinParticles.SetActive(value: true);
		SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.ChestCollect);
		for (int i = 0; i < 10; i++)
		{
			SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.ChestCoins);
			yield return new WaitForSeconds(0.15f);
		}
		yield return new WaitWhile(() => _openAnimatedSprite.IsPlaying);
		_openAnimatedSprite.gameObject.SetActive(value: false);
		_closeAnimatedSprite.gameObject.SetActive(value: true);
		yield return new WaitForSeconds(0.2f);
		SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.ChestClose);
		yield return new WaitWhile(() => _closeAnimatedSprite.IsPlaying);
		_closeAnimatedSprite.gameObject.SetActive(value: false);
		_exitAnimatedSprite.gameObject.SetActive(value: true);
		yield return new WaitForSeconds(0.8f);
		SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.ChestDisappear);
		yield return new WaitWhile(() => _exitAnimatedSprite.IsPlaying);
		yield return new WaitWhile(() => _exitTweener.IsPlaying);
		DestroyTile();
	}
}
