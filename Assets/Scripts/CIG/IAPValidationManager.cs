using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace CIG
{
	public sealed class IAPValidationManager
	{
		private class ValidationPurchase : IStorable
		{
			private const string ProductIDKey = "ProductID";

			private const string TransactionIDKey = "TransactionID";

			private const string ReceiptKey = "Receipt";

			public string TransactionID
			{
				get;
			}

			public string ProductID
			{
				get;
			}

			public string Receipt
			{
				get;
			}

			public ValidationPurchase(string productID, string transactionID, string receipt)
			{
				TransactionID = transactionID;
				ProductID = productID;
				Receipt = receipt;
			}

			public override string ToString()
			{
				return "[PurchaseEntry: TransactionID=" + TransactionID + ", productID=" + ProductID + "]";
			}

			public ValidationPurchase(Dictionary<string, object> storageDictionary)
			{
				TransactionID = (Receipt = string.Empty);
				if (storageDictionary.ContainsKey("TransactionID"))
				{
					TransactionID = (string)storageDictionary["TransactionID"];
				}
				if (storageDictionary.ContainsKey("ProductID"))
				{
					ProductID = (string)storageDictionary["ProductID"];
				}
				if (storageDictionary.ContainsKey("Receipt"))
				{
					Receipt = (string)storageDictionary["Receipt"];
				}
			}

			public StorageDictionary Serialize()
			{
				StorageDictionary storageDictionary = new StorageDictionary();
				storageDictionary.Set("TransactionID", TransactionID);
				storageDictionary.Set("ProductID", ProductID);
				storageDictionary.Set("Receipt", Receipt);
				return storageDictionary;
			}
		}

		private const string UnverifiedPurchasesKey = "UnverifiedPurchases";

		private readonly WebService _webService;

		private readonly User _user;

		private readonly RoutineRunner _routineRunner;

		private List<ValidationPurchase> _unverifiedPurchases = new List<ValidationPurchase>();

		private StorageDictionary Storage => StorageController.PurchaseRoot.GetStorageDict("IAPValidationStorage");

		public IAPValidationManager(User user, WebService webService, RoutineRunner routineRunner)
		{
			_webService = webService;
			_routineRunner = routineRunner;
			_user = user;
			DeserializePurchases();
		}

		public void AddPurchase(string productID, string transactionID, string receipt)
		{
			_unverifiedPurchases.Add(new ValidationPurchase(productID, transactionID, receipt));
			SerializePurchases();
			ValidatePurchases();
		}

		public void ValidatePurchases()
		{
			if (_unverifiedPurchases.Count > 0)
			{
				ValidationPurchase validationPurchase = _unverifiedPurchases[0];
				UnityEngine.Debug.Log("Verifying purchase " + validationPurchase.ProductID);
				_routineRunner.StartCoroutine(ValidateRoutine(validationPurchase));
			}
		}

		private IEnumerator ValidateRoutine(ValidationPurchase purchase)
		{
			using (UnityWebRequest webRequest = _webService.ValidatePurchase(purchase.ProductID, purchase.Receipt))
			{
				yield return webRequest.SendWebRequest();
				if (string.IsNullOrEmpty(webRequest.error) && !string.IsNullOrEmpty(webRequest.downloadHandler.text))
				{
					string value = webRequest.downloadHandler.text.Trim();
					if ("OK".Equals(value) || "ERROR".Equals(value))
					{
						UnityEngine.Debug.Log("Purchase verified " + purchase.ProductID);
						_unverifiedPurchases.Remove(purchase);
						SerializePurchases();
						if ("OK".Equals(value))
						{
							_user.SetPayingUser();
						}
						ValidatePurchases();
					}
					else
					{
						UnityEngine.Debug.LogError("Verify purchase request unknown result: " + webRequest.downloadHandler.text);
					}
				}
				else
				{
					UnityEngine.Debug.LogError("Verify purchase request error: " + webRequest.error);
				}
			}
		}

		private void SerializePurchases()
		{
			Storage.Set("UnverifiedPurchases", _unverifiedPurchases);
			StorageController.Save();
		}

		private void DeserializePurchases()
		{
			_unverifiedPurchases = new List<ValidationPurchase>();
			if (Storage.Contains("UnverifiedPurchases"))
			{
				List<Dictionary<string, object>> list = Storage.GetList<Dictionary<string, object>>("UnverifiedPurchases");
				int i = 0;
				for (int count = list.Count; i < count; i++)
				{
					_unverifiedPurchases.Add(new ValidationPurchase(list[i]));
				}
			}
		}
	}
}
