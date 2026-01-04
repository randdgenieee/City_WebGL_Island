using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	public sealed class CurrencyAnimator : SingletonMonobehaviour<CurrencyAnimator>
	{
		private class EarnedCurrency
		{
			public Currency Currency
			{
				get;
			}

			public object EarnSource
			{
				get;
			}

			public EarnedCurrency(Currency currency, object earnSource)
			{
				Currency = currency;
				EarnSource = earnSource;
			}
		}

		[Serializable]
		private class FlyingCurrencyPrefab
		{
			[SerializeField]
			private FlyingCurrency _prefab;

			[SerializeField]
			private CurrencyType _currencyType;

			public FlyingCurrency Prefab => _prefab;

			public CurrencyType CurrencyType => _currencyType;
		}

		[SerializeField]
		private Camera _uiCamera;

		[SerializeField]
		private FlyingCurrencyPrefab[] _flyingCurrencyPrefabs;

		private readonly List<CurrencyAnimationSource> _animationSources = new List<CurrencyAnimationSource>();

		private readonly Dictionary<CurrencyType, CurrencyAnimationTarget> _animationTargets = new Dictionary<CurrencyType, CurrencyAnimationTarget>();

		private readonly Dictionary<CurrencyType, FlyingCurrencyPrefab> _prefabLookupTable = new Dictionary<CurrencyType, FlyingCurrencyPrefab>();

		private readonly List<EarnedCurrency> _animationQueue = new List<EarnedCurrency>();

		private CurrencyAnimationTarget _overrideAnimationTarget;

		private GameState _gameState;

		private BuildingWarehouseManager _buildingWarehouseManager;

		private CraneManager _craneManager;

		private Timing _timing;

		private RectTransform _rectTransform;

		protected override void Awake()
		{
			base.Awake();
			int i = 0;
			for (int num = _flyingCurrencyPrefabs.Length; i < num; i++)
			{
				FlyingCurrencyPrefab flyingCurrencyPrefab = _flyingCurrencyPrefabs[i];
				_prefabLookupTable[flyingCurrencyPrefab.CurrencyType] = flyingCurrencyPrefab;
			}
			_flyingCurrencyPrefabs = null;
		}

		public void Initialize(GameState gameState, BuildingWarehouseManager buildingWarehouseManager, CraneManager craneManager, Timing timing)
		{
			_gameState = gameState;
			_buildingWarehouseManager = buildingWarehouseManager;
			_craneManager = craneManager;
			_timing = timing;
			_rectTransform = (RectTransform)base.transform;
			_gameState.BalanceChangedEvent += OnBalanceChanged;
			_buildingWarehouseManager.WarehouseBuildingAddedEvent += OnWarehouseBuildingAdded;
			_craneManager.CraneCountChangedEvent += OnCraneCountChanged;
		}

		protected override void OnDestroy()
		{
			if (_gameState != null)
			{
				_gameState.BalanceChangedEvent -= OnBalanceChanged;
				_gameState = null;
			}
			if (_buildingWarehouseManager != null)
			{
				_buildingWarehouseManager.WarehouseBuildingAddedEvent -= OnWarehouseBuildingAdded;
				_buildingWarehouseManager = null;
			}
			if (_craneManager != null)
			{
				_craneManager.CraneCountChangedEvent -= OnCraneCountChanged;
				_craneManager = null;
			}
			_animationSources.Clear();
			this.CancelInvoke(PlayAnimationQueue);
			base.OnDestroy();
		}

		public void RegisterCurrencySource(CurrencyAnimationSource source)
		{
			if (_animationSources.Contains(source))
			{
				UnityEngine.Debug.LogWarning("CurrencyAnimationSource '" + source.name + "' already registered.");
			}
			else
			{
				_animationSources.Add(source);
			}
		}

		public void UnregisterCurrencySource(CurrencyAnimationSource source)
		{
			_animationSources.Remove(source);
		}

		public void RegisterCurrencyTarget(CurrencyType currencyType, CurrencyAnimationTarget target)
		{
			if (_animationTargets.TryGetValue(currencyType, out CurrencyAnimationTarget value))
			{
				UnityEngine.Debug.LogWarning(string.Format("{0} for {1} '{2}' is already registered by '{3}'.", "CurrencyAnimationTarget", "CurrencyType", currencyType, value.name));
			}
			else
			{
				_animationTargets.Add(currencyType, target);
			}
		}

		public void UnregisterCurrencyTarget(CurrencyType currencyType)
		{
			_animationTargets.Remove(currencyType);
		}

		public void SetOverrideAnimationTarget(CurrencyAnimationTarget target)
		{
			_overrideAnimationTarget = target;
		}

		private void PlayAnimations(Currencies currencies, FlyingCurrenciesData flyingCurrenciesProperties)
		{
			int i = 0;
			for (int keyCount = currencies.KeyCount; i < keyCount; i++)
			{
				Currency currency = currencies.GetCurrency(i);
				if (currency.Value > decimal.Zero)
				{
					if (flyingCurrenciesProperties.AmountOfFlyingCurrencies > 1)
					{
						StartCoroutine(FlyingCurrenciesRoutine(currency, flyingCurrenciesProperties));
					}
					else
					{
						PlayAnimation(currency, flyingCurrenciesProperties);
					}
				}
			}
		}

		private Vector3 CalcSourcePosition(CurrencyAnimPositionType positionType, Vector3 position)
		{
			switch (positionType)
			{
			case CurrencyAnimPositionType.RawPosition:
				return position;
			case CurrencyAnimPositionType.ScreenPosition:
				return _uiCamera.ScreenToWorldPoint(position);
			case CurrencyAnimPositionType.WorldPosition:
				if (IsometricIsland.Current != null)
				{
					Vector3 result = _uiCamera.ScreenToWorldPoint(IsometricIsland.Current.CameraOperator.CameraToOperate.WorldToScreenPoint(position));
					result.z = 0f;
					return result;
				}
				UnityEngine.Debug.LogError("There is no island loaded. This is necessary for an animated currency to play from a world position.");
				return _rectTransform.position;
			default:
				UnityEngine.Debug.LogWarningFormat("Did you forget to cover a case in the CurrencyAnimator? - Missing case: '{0}'", positionType);
				return _rectTransform.position;
			}
		}

		private CurrencyAnimationSource FindAnimationSource(object earnSource)
		{
			if (earnSource != null)
			{
				return _animationSources.Find((CurrencyAnimationSource x) => x.AnimationSource == earnSource);
			}
			return null;
		}

		private CurrencyAnimationTarget FindAnimationTarget(CurrencyType currencyType)
		{
			if (!_animationTargets.TryGetValue(currencyType, out CurrencyAnimationTarget value))
			{
				return null;
			}
			return value;
		}

		private void QueueAnimation(Currency currency, object earnSource)
		{
			_animationQueue.Add(new EarnedCurrency(currency, earnSource));
			if (!this.IsInvoking(PlayAnimationQueue))
			{
				this.InvokeNextFrame(PlayAnimationQueue);
			}
		}

		private void PlayAnimationQueue()
		{
			int count = _animationQueue.Count;
			for (int i = 0; i < count; i++)
			{
				PlayAnimation(_animationQueue[i]);
			}
			_animationQueue.Clear();
		}

		private void PlayAnimation(Currency currency, FlyingCurrenciesData flyingCurrenciesProperties)
		{
			CurrencyAnimationSource currencyAnimationSource = FindAnimationSource(flyingCurrenciesProperties.EarnSource);
			if (flyingCurrenciesProperties.EarnSource != null && currencyAnimationSource == null)
			{
				QueueAnimation(currency, flyingCurrenciesProperties.EarnSource);
			}
			else
			{
				PlayAnimation(currency, currencyAnimationSource);
			}
		}

		private void PlayAnimation(EarnedCurrency earnedCurrency)
		{
			CurrencyAnimationSource currencyAnimationSource = FindAnimationSource(earnedCurrency.EarnSource);
			if (currencyAnimationSource == null)
			{
				UnityEngine.Debug.LogWarning($"No CurrencyAnimationSource found for Earn Source '{earnedCurrency.EarnSource}'");
			}
			PlayAnimation(earnedCurrency.Currency, currencyAnimationSource);
		}

		private FlyingCurrency PlayAnimation(Currency currency, CurrencyAnimationSource animationSource = null)
		{
			CurrencyType currencyType = currency.ToCurrencyType();
			CurrencyAnimationTarget currencyAnimationTarget = _overrideAnimationTarget ?? FindAnimationTarget(currencyType);
			if (currencyAnimationTarget == null)
			{
				UnityEngine.Debug.LogWarning(string.Format("No {0} found for {1}.", "CurrencyAnimationTarget", currency));
				return null;
			}
			Vector3 startPosition = (animationSource == null) ? _rectTransform.position : CalcSourcePosition(animationSource.PositionType, animationSource.Position);
			FlyingCurrency flyingCurrency = UnityEngine.Object.Instantiate(GetFlyingCurrencyPrefab(currencyType), base.transform);
			flyingCurrency.gameObject.SetActive(value: true);
			flyingCurrency.PlayAnimation(currency, startPosition, currencyAnimationTarget);
			return flyingCurrency;
		}

		private FlyingCurrency GetFlyingCurrencyPrefab(CurrencyType currencyType)
		{
			if (_prefabLookupTable.TryGetValue(currencyType, out FlyingCurrencyPrefab value))
			{
				return value.Prefab;
			}
			UnityEngine.Debug.LogError(string.Format("No {0} for {1}: {2}", "FlyingCurrencyPrefab", "CurrencyType", currencyType));
			return null;
		}

		private void OnBalanceChanged(Currencies oldBalance, Currencies newBalance, FlyingCurrenciesData flyingCurrenciesProperties)
		{
			Currencies currencies = newBalance - oldBalance;
			CheckForFlyingCurrencies(currencies, flyingCurrenciesProperties);
		}

		private void CheckForFlyingCurrencies(Currencies currencies, FlyingCurrenciesData flyingCurrenciesProperties)
		{
			if (flyingCurrenciesProperties.AmountOfFlyingCurrencies >= 1)
			{
				PlayAnimations(currencies, flyingCurrenciesProperties);
				return;
			}
			int i = 0;
			for (int keyCount = currencies.KeyCount; i < keyCount; i++)
			{
				Currency currency = currencies.GetCurrency(i);
				CurrencyAnimationTarget currencyAnimationTarget = FindAnimationTarget(currency.ToCurrencyType());
				if (currencyAnimationTarget != null)
				{
					currencyAnimationTarget.Target.FlyingCurrencyFinishedPlaying(currency, animateHudElement: false);
				}
			}
		}

		private void OnWarehouseBuildingAdded(BuildingProperties buildingProperties)
		{
			Currency currency = new Currency("Building", decimal.One);
			CurrencyAnimationSource animationSource = FindAnimationSource(buildingProperties);
			FlyingCurrency flyingCurrency = PlayAnimation(currency, animationSource);
			GridTile asset = SingletonMonobehaviour<BuildingsAssetCollection>.Instance.GetAsset(buildingProperties.BaseKey);
			flyingCurrency.CurrencyImage.sprite = asset.SpriteRenderer.sprite;
		}

		private void OnCraneCountChanged(int delta)
		{
			if (delta > 0)
			{
				CheckForFlyingCurrencies(new Currencies("Crane", delta), new FlyingCurrenciesData(null, delta));
			}
		}

		private IEnumerator FlyingCurrenciesRoutine(Currency currency, FlyingCurrenciesData flyingCurrenciesProperties)
		{
			int amountOfFlyingCurrencies = flyingCurrenciesProperties.AmountOfFlyingCurrencies;
			for (int i = 0; i < amountOfFlyingCurrencies; i++)
			{
				Currency currency2 = (i < amountOfFlyingCurrencies - 1) ? new Currency(currency.Name, decimal.Zero) : new Currency(currency.Name, currency.Value);
				PlayAnimation(currency2, flyingCurrenciesProperties);
				yield return new WaitForGameTimeSeconds(_timing, 0.10000000149011612);
			}
		}
	}
}
