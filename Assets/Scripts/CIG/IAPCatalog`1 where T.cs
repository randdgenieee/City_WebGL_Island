using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Purchasing;

namespace CIG
{
	public class IAPCatalog<T> where T : StoreProduct, IStorable
	{
		private const string StoreProductsKey = "StoreProductsKey";

		private const string GameVersionKey = "GameVersionKey";

		private const string CacheDatetimeKey = "CacheDateTimeKey";

		private const string DefaultPricepointsVersionKey = "DefaultPricePointsVersionKey";

		private const string IapcatalogKey = "IAPCatalogStorage";

		private static readonly TimeSpan PricepointsExpirationtime = new TimeSpan(1, 0, 0, 0);

		private const string PricePointsVersionKey = "version";

		private readonly Func<StorageDictionary, T> _storeProductFactory;

		private readonly WebService _webService;

		private readonly List<T> _products = new List<T>();

		private static string DefaultPricepointsAssetName => "defaultpricepoints-android";

		public T[] AvailableProducts => _products.FindAll((T x) => x.Available).ToArray();

		private StorageDictionary Storage => StorageController.PurchaseRoot.GetStorageDict("IAPCatalogStorage");

		public IAPCatalog(WebService webService, Func<StorageDictionary, T> storeProductFactory)
		{
			_webService = webService;
			_storeProductFactory = storeProductFactory;
			LoadFromStorage();
			LoadDefaultPricePoints();
		}

		public IEnumerator UpdatePricePoints()
		{
			if (AreCachedPricePointsOutDated() && _webService != null)
			{
				using (UnityWebRequest request = _webService.PricePoints())
				{
					request.timeout = 5;
					yield return request.SendWebRequest();
					if (string.IsNullOrEmpty(request.error) && !string.IsNullOrEmpty(request.downloadHandler.text))
					{
						if (ValidatePricePoints(request.downloadHandler.text, out List<StorageDictionary> productMetadata, out int _))
						{
							UpdateRemoteProducts(productMetadata);
						}
						else
						{
							UnityEngine.Debug.LogErrorFormat("Invalid pricepoints received. - Not using them. Error:\n{0}\n\nResponse:\n{1}", request.error, request.downloadHandler.text);
						}
					}
					else
					{
						UnityEngine.Debug.LogErrorFormat("Error downloading pricepoints. Error:\n{0}\n\nResponse:\n{1}", request.error, request.downloadHandler.text);
					}
				}
			}
		}

		public void UpdateProducts(Product[] products)
		{
			int i = 0;
			for (int num = products.Length; i < num; i++)
			{
				Product product = products[i];
				T val = FindProduct(product.definition.id);
				if (val != null)
				{
					StorageDictionary storageDictionary = val.Serialize();
					storageDictionary.SetOrRemove("ProductTitle", product.metadata.localizedTitle);
					storageDictionary.SetOrRemove("ProductDescription", product.metadata.localizedDescription);
					storageDictionary.SetOrRemove("ProductCurrencyCode", product.metadata.isoCurrencyCode);
					storageDictionary.SetOrRemove("ProductFormattedPrice", product.metadata.localizedPriceString);
					storageDictionary.Set("ProductPrice", product.metadata.localizedPrice);
					AddOrUpdateProduct(storageDictionary);
				}
			}
			SaveToStorage();
		}

		public T FindProduct(string identifier)
		{
			return _products.Find((T x) => x.Identifier == identifier);
		}

		public T FindProduct(Predicate<T> predicate, bool includeUnavailable)
		{
			T[] products = GetProducts(predicate, includeUnavailable);
			if (products.Length == 0)
			{
				return null;
			}
			return products[0];
		}

		public T[] GetProducts(Predicate<T> predicate, bool includeUnavailable)
		{
			List<T> list = new List<T>();
			int i = 0;
			for (int count = _products.Count; i < count; i++)
			{
				T val = _products[i];
				if ((val.Available | includeUnavailable) && predicate(val))
				{
					list.Add(val);
				}
			}
			list.Sort();
			return list.ToArray();
		}

		private T AddOrUpdateProduct(StorageDictionary productMetadata)
		{
			T val = FindProduct(productMetadata.Get("ProductIdentifier", "unknown"));
			if (val != null)
			{
				val.Update(productMetadata);
			}
			else
			{
				val = _storeProductFactory(productMetadata);
				_products.Add(val);
			}
			return val;
		}

		private void LoadDefaultPricePoints()
		{
			int num = Storage.Get("DefaultPricePointsVersionKey", -1);
			TextAsset textAsset = Resources.Load<TextAsset>(DefaultPricepointsAssetName);
			List<StorageDictionary> productMetadata;
			int version;
			if (textAsset == null)
			{
				UnityEngine.Debug.LogErrorFormat("Unable to open TextAsset {0}.", DefaultPricepointsAssetName);
			}
			else if (!ValidatePricePoints(textAsset.text, out productMetadata, out version))
			{
				UnityEngine.Debug.LogWarningFormat("Invalid default pricepoints. - Not using them.");
			}
			else if (num == -1 || version > num)
			{
				UpdateProducts(productMetadata);
				Storage.Set("DefaultPricePointsVersionKey", version);
				SaveToStorage();
			}
		}

		private void UpdateRemoteProducts(List<StorageDictionary> productMetadata)
		{
			UpdateProducts(productMetadata);
			Storage.Set("GameVersionKey", "1.11.8");
			Storage.Set("CacheDateTimeKey", AntiCheatDateTime.UtcNow.ToBinary());
			SaveToStorage();
		}

		private void UpdateProducts(List<StorageDictionary> productMetadata)
		{
			int i = 0;
			for (int count = _products.Count; i < count; i++)
			{
				_products[i].Available = false;
			}
			int j = 0;
			for (int count2 = productMetadata.Count; j < count2; j++)
			{
				AddOrUpdateProduct(productMetadata[j]);
			}
		}

		private bool AreCachedPricePointsOutDated()
		{
			string text = Storage.Get("GameVersionKey", string.Empty);
			if (string.IsNullOrEmpty(text) || text != "1.11.8")
			{
				return true;
			}
			if (!Storage.Contains("CacheDateTimeKey"))
			{
				return true;
			}
			DateTime d = DateTime.FromBinary(Storage.Get("CacheDateTimeKey", -1L));
			if (AntiCheatDateTime.UtcNow - d > PricepointsExpirationtime)
			{
				return true;
			}
			return false;
		}

		private bool ValidatePricePoints(string pricepointsRawText, out List<StorageDictionary> productMetadata, out int version)
		{
			productMetadata = ParsePricePoints(pricepointsRawText, out version);
			if (productMetadata.Count == 0)
			{
				return false;
			}
			StorageDictionary storageDictionary = productMetadata[0];
			if (!storageDictionary.Contains("ProductIdentifier") || !storageDictionary.Contains("ProductPrice") || !storageDictionary.Contains("CustomProperties"))
			{
				return false;
			}
			return true;
		}

		private static List<StorageDictionary> ParsePricePoints(string pricePoints, out int version)
		{
			version = -1;
			List<StorageDictionary> list = new List<StorageDictionary>();
			if (string.IsNullOrEmpty(pricePoints))
			{
				return list;
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			string[] array = pricePoints.Split('\n');
			int i = 0;
			for (int num = array.Length; i < num; i++)
			{
				string text = array[i];
				int num2 = text.IndexOf('=');
				if (num2 >= 0)
				{
					string text2 = text.Substring(0, num2).Trim();
					string value = text.Substring(num2 + 1).Trim();
					if (dictionary.ContainsKey(text2))
					{
						UnityEngine.Debug.LogWarning("Result dictionary already contains key '" + text2 + "'");
					}
					dictionary[text2] = value;
				}
			}
			string objA = null;
			foreach (KeyValuePair<string, string> item in dictionary)
			{
				if (item.Key == "version")
				{
					if (!int.TryParse(item.Value, out version))
					{
						UnityEngine.Debug.LogWarning("Failed to parse pricepoints version '" + item.Value + "' to int.");
					}
				}
				else
				{
					int num3 = item.Key.LastIndexOf('.');
					if (num3 >= 0)
					{
						string text3 = item.Key.Substring(0, num3);
						string text4 = item.Key.Substring(num3 + 1, item.Key.Length - num3 - 1);
						StorageDictionary storageDictionary;
						if (!object.Equals(objA, text3))
						{
							storageDictionary = new StorageDictionary();
							list.Add(storageDictionary);
							objA = text3;
						}
						else
						{
							storageDictionary = list[list.Count - 1];
						}
						if (text4.Equals("name"))
						{
							storageDictionary.Set("ProductIdentifier", item.Value);
						}
						else if (text4.Equals("dollar"))
						{
							if (decimal.TryParse(item.Value, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal result))
							{
								storageDictionary.Set("ProductPrice", result);
								storageDictionary.Set("ProductDollarPrice", result);
								storageDictionary.Set("ProductFormattedPrice", "$ " + result.ToString("0.00"));
								storageDictionary.Set("ProductCurrencyCode", "USD");
							}
						}
						else if (text4.Equals("euro"))
						{
							if (decimal.TryParse(item.Value, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal result2))
							{
								storageDictionary.Set("ProductPrice", result2);
								storageDictionary.Set("ProductEuroPrice", result2);
								storageDictionary.Set("ProductFormattedPrice", "€ " + result2.ToString("0.00"));
								storageDictionary.Set("ProductCurrencyCode", "EUR");
							}
						}
						else if (text4.Equals("description"))
						{
							storageDictionary.Set("ProductDescription", item.Value);
						}
						else if (text4.Equals("available"))
						{
							if (bool.TryParse(item.Value, out bool result3))
							{
								storageDictionary.Set("Available", result3);
							}
						}
						else
						{
							Dictionary<string, string> dictionary2 = storageDictionary.GetDictionary<string>("CustomProperties");
							dictionary2.Add(text4, item.Value);
							storageDictionary.Set("CustomProperties", dictionary2);
						}
					}
				}
			}
			return list;
		}

		private void SaveToStorage()
		{
			Storage.Set("StoreProductsKey", _products);
			StorageController.Save();
		}

		private void LoadFromStorage()
		{
			if (Storage.Contains("StoreProductsKey"))
			{
				List<StorageDictionary> models = Storage.GetModels("StoreProductsKey", (StorageDictionary sd) => sd);
				int i = 0;
				for (int count = models.Count; i < count; i++)
				{
					AddOrUpdateProduct(models[i]);
				}
			}
		}
	}
}
