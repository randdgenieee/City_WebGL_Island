using System;
using System.Collections;
using UnityEngine;

namespace CIG
{
	public class FlyingStartDealManager : IProductConsumer
	{
		public delegate void ActiveChangedEventHandler(bool isActive);

		private const string IAPGameProductName = "flying_start_deal";

		private const string AlternativeIAPGameProductName = "flying_start_deal_alternative";

		private readonly StorageDictionary _storage;

		private readonly TutorialIntermediary _tutorialManager;

		private readonly GameState _gameState;

		private readonly RoutineRunner _routineRunner;

		private readonly FlyingStartDealProperties _properties;

		private bool _hasActived;

		private IEnumerator _expireRoutine;

		private IEnumerator _delayedStartRoutine;

		private bool _isActive;

		private const string HasActivedKey = "HasActivated";

		private const string IsActiveKey = "IsActive";

		private const string ExpireTimeKey = "ExpireTime";

		public bool IsActive
		{
			get
			{
				return _isActive;
			}
			private set
			{
				if (_isActive != value)
				{
					_isActive = value;
					FireActiveChangedEvent(_isActive);
				}
			}
		}

		public DateTime ExpireTime
		{
			get;
			private set;
		}

		public TimeSpan TimeLeft => ExpireTime - AntiCheatDateTime.UtcNow;

		public TOCIStoreProduct StoreProduct
		{
			get;
		}

		public TOCIStoreProduct AlternativeStoreProduct
		{
			get;
		}

		public event ActiveChangedEventHandler ActiveChangedEvent;

		private void FireActiveChangedEvent(bool isActive)
		{
			this.ActiveChangedEvent?.Invoke(isActive);
		}

		public FlyingStartDealManager(StorageDictionary storage, TutorialIntermediary tutorialManager, GameState gameState, RoutineRunner routineRunner, IAPStore<TOCIStoreProduct> store, FlyingStartDealProperties properties)
		{
			_storage = storage;
			_tutorialManager = tutorialManager;
			_properties = properties;
			_gameState = gameState;
			_routineRunner = routineRunner;
			StoreProduct = store.FindProduct((TOCIStoreProduct p) => p.Category == StoreProductCategory.FlyingStartDeal && p.GameProductName == "flying_start_deal");
			AlternativeStoreProduct = store.FindProduct((TOCIStoreProduct p) => p.Category == StoreProductCategory.FlyingStartDeal && p.GameProductName == "flying_start_deal_alternative", includeUnavailable: true);
			_hasActived = _storage.Get("HasActivated", defaultValue: false);
			_isActive = _storage.Get("IsActive", defaultValue: false);
			ExpireTime = _storage.GetDateTime("ExpireTime", DateTime.MinValue);
			if (IsActive)
			{
				_routineRunner.StartCoroutine(_expireRoutine = ExpireRoutine(ExpireTime));
			}
			else if (!_hasActived)
			{
				_tutorialManager.TutorialFinishedEvent += OnTutorialFinished;
			}
		}

		bool IProductConsumer.ConsumeProduct(TOCIStoreProduct product)
		{
			StoreProductCategory category = product.Category;
			if (category == StoreProductCategory.FlyingStartDeal)
			{
				_gameState.EarnCurrencies(product.Currencies, CurrenciesEarnedReason.FlyingStartDeal, new FlyingCurrenciesData());
				Stop();
				return true;
			}
			UnityEngine.Debug.LogError(string.Format("Cannot consume product for {0} '{1}'", "StoreProductCategory", product.Category));
			return false;
		}

		private void TryStart()
		{
			if (_delayedStartRoutine == null && _properties.Enabled && !IsActive && !_hasActived && _tutorialManager.InitialTutorialFinished)
			{
				_tutorialManager.TutorialFinishedEvent -= OnTutorialFinished;
				_routineRunner.StartCoroutine(_delayedStartRoutine = DelayedStartRoutine(_properties.StartDelaySeconds));
			}
		}

		private void Start()
		{
			ExpireTime = AntiCheatDateTime.UtcNow.AddSeconds(_properties.DurationSeconds);
			_hasActived = true;
			IsActive = true;
			if (_expireRoutine != null)
			{
				_routineRunner.StopCoroutine(_expireRoutine);
			}
			_routineRunner.StartCoroutine(_expireRoutine = ExpireRoutine(ExpireTime));
		}

		private void Stop()
		{
			IsActive = false;
			if (_expireRoutine != null)
			{
				_routineRunner.StopCoroutine(_expireRoutine);
				_expireRoutine = null;
			}
		}

		private void OnTutorialFinished(Tutorial tutorial)
		{
			if (_tutorialManager.InitialTutorialFinished)
			{
				TryStart();
			}
		}

		private IEnumerator DelayedStartRoutine(int waitSeconds)
		{
			yield return new WaitForSecondsRealtime(waitSeconds);
			_delayedStartRoutine = null;
			Start();
		}

		private IEnumerator ExpireRoutine(DateTime waitUntil)
		{
			yield return new WaitUntilUTCDateTime(waitUntil);
			_expireRoutine = null;
			Stop();
		}

		public void Serialize()
		{
			_storage.Set("HasActivated", _hasActived);
			_storage.Set("IsActive", _isActive || _delayedStartRoutine != null);
			_storage.Set("ExpireTime", ExpireTime);
		}
	}
}
