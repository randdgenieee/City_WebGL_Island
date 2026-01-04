using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	public sealed class SaleManager
	{
		public delegate void SaleChangedEventHandler(Sale oldSale, Sale newSale);

		private const string CashSale = "cash";

		private const string GoldSale = "gold";

		private readonly StorageDictionary _storage;

		private readonly WebService _webService;

		private readonly RoutineRunner _routineRunner;

		private readonly SaleManagerProperties _properties;

		private IEnumerator _expireRoutine;

		private const string CurrentSaleKey = "CurrentSale";

		public Sale CurrentSale
		{
			get;
			private set;
		}

		public bool HasCashSale
		{
			get
			{
				if (CurrentSale != null)
				{
					return CurrentSale.IsCashSale;
				}
				return false;
			}
		}

		public bool HasGoldSale
		{
			get
			{
				if (CurrentSale != null)
				{
					return CurrentSale.IsGoldSale;
				}
				return false;
			}
		}

		public event SaleChangedEventHandler SaleChangedEvent;

		private void FireSaleChangedEvent(Sale oldSale, Sale newSale)
		{
			this.SaleChangedEvent?.Invoke(oldSale, newSale);
		}

		public SaleManager(StorageDictionary storage, WebService webService, RoutineRunner routineRunner, SaleManagerProperties properties)
		{
			_storage = storage;
			_webService = webService;
			_routineRunner = routineRunner;
			_properties = properties;
			CurrentSale = _storage.GetModel("CurrentSale", (StorageDictionary sd) => new Sale(sd), null);
			_webService.PullRequestCompletedEvent += OnPullRequestCompleted;
			OnPullRequestCompleted(_webService.Properties);
			if (FirebaseManager.IsAvailable)
			{
				FirebaseManager.MessagingSale.ReceivedSaleEvent += OnReceivedFirebaseMessagingSale;
				if (FirebaseManager.MessagingSale.HasInactiveSale)
				{
					Sale currentSale = FirebaseManager.MessagingSale.ClaimSale();
					SetCurrentSale(currentSale);
				}
			}
			StartExpireRoutine();
		}

		public void Release()
		{
			FirebaseManager.MessagingSale.ReceivedSaleEvent -= OnReceivedFirebaseMessagingSale;
			_webService.PullRequestCompletedEvent -= OnPullRequestCompleted;
		}

		private void OnReceivedFirebaseMessagingSale()
		{
			Sale currentSale = FirebaseManager.MessagingSale.ClaimSale();
			SetCurrentSale(currentSale);
		}

		private void StartExpireRoutine()
		{
			if (_expireRoutine != null)
			{
				_routineRunner.StopCoroutine(_expireRoutine);
			}
			if (CurrentSale != null)
			{
				_routineRunner.StartCoroutine(_expireRoutine = ExpireRoutine());
			}
		}

		private void SetCurrentSale(Sale sale)
		{
			if (sale != null)
			{
				if ((CurrentSale == null || CurrentSale.SaleSource != SaleSource.Firebase || sale.SaleSource != SaleSource.EdwinServer) && (CurrentSale == null || sale.SaleType != CurrentSale.SaleType))
				{
					Sale currentSale = CurrentSale;
					CurrentSale = new Sale(sale.Expiration, sale.SaleType, sale.SaleSource);
					FireSaleChangedEvent(currentSale, CurrentSale);
					StartExpireRoutine();
				}
			}
			else
			{
				Sale currentSale2 = CurrentSale;
				CurrentSale = null;
				FireSaleChangedEvent(currentSale2, CurrentSale);
			}
		}

		private IEnumerator ExpireRoutine()
		{
			yield return new WaitUntilUTCDateTime(CurrentSale.Expiration);
			SetCurrentSale(null);
		}

		private void OnPullRequestCompleted(Dictionary<string, string> properties)
		{
			if (_properties.IgnoreEdwinServerSales)
			{
				return;
			}
			string text = properties.Get("saletype", string.Empty);
			double num = properties.Get("saleseconds", double.MinValue);
			if (string.IsNullOrEmpty(text) || !(num > 0.0))
			{
				return;
			}
			DateTime expiration = AntiCheatDateTime.UtcNow.AddSeconds(num);
			string[] array = text.Split(',');
			SaleType saleType = SaleType.None;
			int i = 0;
			for (int num2 = array.Length; i < num2; i++)
			{
				string a = array[i];
				if (!(a == "cash"))
				{
					if (a == "gold")
					{
						saleType |= SaleType.GoldSale;
					}
					else
					{
						UnityEngine.Debug.LogWarningFormat("Unknown sale type: " + array[i]);
					}
				}
				else
				{
					saleType |= SaleType.CashSale;
				}
			}
			SetCurrentSale(new Sale(expiration, saleType, SaleSource.EdwinServer));
		}

		public void Serialize()
		{
			_storage.SetOrRemove("CurrentSale", CurrentSale);
		}
	}
}
