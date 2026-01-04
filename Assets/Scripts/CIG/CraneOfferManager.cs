using SparkLinq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	public class CraneOfferManager : IProductConsumer
	{
		public delegate void OfferStartedEventHandler(CraneOffer craneOffer);

		public delegate void OfferEndedEventHandler();

		private readonly StorageDictionary _storage;

		private readonly CraneManager _craneManager;

		private readonly PopupManager _popupManager;

		private readonly RoutineRunner _routineRunner;

		private readonly IAPStore<TOCIStoreProduct> _storeManager;

		private readonly GameState _gameState;

		private readonly CraneOfferManagerProperties _properties;

		private int _hireCraneCloseCount;

		private int _offerShowCount;

		private int _currentOfferIndex;

		private IEnumerator _offerRoutine;

		private const string HireCraneCloseCountKey = "HireCraneCloseCount";

		private const string OfferShowCountKey = "OfferShowCount";

		private const string CurrentOfferIndexKey = "CurrentOfferIndex";

		private const string ActiveCraneOfferKey = "ActiveCraneOffer";

		public CraneOffer CraneOffer
		{
			get;
			private set;
		}

		public bool HasCraneOffer => CraneOffer != null;

		public event OfferStartedEventHandler OfferStartedEvent;

		public event OfferEndedEventHandler OfferEndedEvent;

		private void FireOfferStartedEvent(CraneOffer craneOffer)
		{
			this.OfferStartedEvent?.Invoke(craneOffer);
		}

		private void FireOfferEndedEvent()
		{
			this.OfferEndedEvent?.Invoke();
		}

		public CraneOfferManager(StorageDictionary storage, CraneManager craneManager, PopupManager popupManager, RoutineRunner routineRunner, IAPStore<TOCIStoreProduct> storeManager, GameState gameState, CraneOfferManagerProperties properties)
		{
			_storage = storage;
			_craneManager = craneManager;
			_popupManager = popupManager;
			_routineRunner = routineRunner;
			_storeManager = storeManager;
			_gameState = gameState;
			_properties = properties;
			_craneManager.CraneHireCanceledEvent += OnCraneHireCanceled;
			_hireCraneCloseCount = _storage.Get("HireCraneCloseCount", 0);
			_offerShowCount = _storage.Get("OfferShowCount", 0);
			_currentOfferIndex = _storage.Get("CurrentOfferIndex", 0);
			if (_currentOfferIndex < _properties.OfferOrder.Count)
			{
				string text = _properties.OfferOrder[_currentOfferIndex];
				CraneOfferCurrencyProperties currencyProperties;
				if (TryGetCraneOfferProperties(text, _properties.OfferIAPProperties, out CraneOfferIAPProperties iapProperties))
				{
					CraneOffer = _storage.GetModel("ActiveCraneOffer", (StorageDictionary sd) => new CraneOfferIAP(iapProperties, routineRunner, storeManager, OnOfferStarted, OnOfferEnded, sd), null);
				}
				else if (TryGetCraneOfferProperties(text, _properties.OfferCurrencyProperties, out currencyProperties))
				{
					CraneOffer = _storage.GetModel("ActiveCraneOffer", (StorageDictionary sd) => new CraneOfferCurrency(currencyProperties, routineRunner, _craneManager, OnOfferStarted, OnOfferEnded, sd), null);
				}
				else
				{
					UnityEngine.Debug.LogError("Crane offer properties '" + text + "' could not be found");
				}
				CraneOffer?.StartRoutine();
			}
		}

		private void OnOfferStarted(CraneOffer craneOffer)
		{
			FireOfferStartedEvent(craneOffer);
		}

		private void OnOfferEnded()
		{
			_currentOfferIndex++;
			CraneOffer = null;
			FireOfferEndedEvent();
		}

		private void OnCraneHireCanceled()
		{
			_hireCraneCloseCount++;
			TryStartOffer();
		}

		private void TryStartOffer()
		{
			DateTime startDateTime = AntiCheatDateTime.UtcNow.AddSeconds(_properties.OfferDelaySeconds);
			if (CraneOffer == null && _offerShowCount <= _properties.MaxShowCount && _hireCraneCloseCount >= _properties.RequiredCloseHireCraneCount && TryCreateCraneOffer(_currentOfferIndex, startDateTime, startDateTime.AddSeconds(_properties.OfferDurationSeconds), out CraneOffer craneOffer))
			{
				CraneOffer = craneOffer;
				CraneOffer.StartRoutine();
				_offerShowCount++;
				_hireCraneCloseCount = 0;
			}
		}

		private bool TryCreateCraneOffer(int craneOfferIndex, DateTime startDateTime, DateTime endDateTime, out CraneOffer craneOffer)
		{
			if (craneOfferIndex < _properties.OfferOrder.Count)
			{
				string text = _properties.OfferOrder[craneOfferIndex];
				if (TryGetCraneOfferProperties(text, _properties.OfferIAPProperties, out CraneOfferIAPProperties properties))
				{
					craneOffer = new CraneOfferIAP(properties, _routineRunner, _storeManager, OnOfferStarted, OnOfferEnded, startDateTime, endDateTime);
					return true;
				}
				if (TryGetCraneOfferProperties(text, _properties.OfferCurrencyProperties, out CraneOfferCurrencyProperties properties2))
				{
					craneOffer = new CraneOfferCurrency(properties2, _routineRunner, _craneManager, OnOfferStarted, OnOfferEnded, startDateTime, endDateTime);
					return true;
				}
				UnityEngine.Debug.LogError("Crane offer properties '" + text + "' could not be found");
			}
			craneOffer = null;
			return false;
		}

		private bool TryGetCraneOfferProperties<T>(string offerKey, List<T> propertiesList, out T properties) where T : BaseProperties
		{
			properties = propertiesList.First((T p) => p.BaseKey == offerKey);
			return properties != null;
		}

		bool IProductConsumer.ConsumeProduct(TOCIStoreProduct product)
		{
			StoreProductCategory category = product.Category;
			if (category == StoreProductCategory.CraneOffer)
			{
				if (product.CraneCount == 0)
				{
					UnityEngine.Debug.LogError("Can't consume product '" + product.Title + "' because it has no crane count in its properties.");
					return false;
				}
				_craneManager.AddCranes(product.CraneCount);
				if (_popupManager.IsShowingPopup && _popupManager.TopPopup is CraneOfferPopupRequest)
				{
					_popupManager.CloseAllOpenPopups(instant: false);
				}
				if (CraneOffer != null)
				{
					CraneOffer.EndOffer();
				}
				Analytics.LogEvent("crane_offer_iap_purchased");
				return true;
			}
			UnityEngine.Debug.LogErrorFormat("Missing product consumer for product category '{0}'", product.Category);
			return false;
		}

		public void Serialize()
		{
			_storage.Set("HireCraneCloseCount", _hireCraneCloseCount);
			_storage.Set("OfferShowCount", _offerShowCount);
			_storage.Set("CurrentOfferIndex", _currentOfferIndex);
			_storage.SetOrRemove("ActiveCraneOffer", CraneOffer);
		}
	}
}
