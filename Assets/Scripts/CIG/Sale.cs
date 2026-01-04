using System;
using System.Collections.Generic;

namespace CIG
{
	public class Sale : IStorable
	{
		public const string SaleTypeKey = "saleType";

		private const string ExpirationKey = "expiration";

		private const string SaleSourceKey = "saleSource";

		private static readonly HashSet<string> NewSaleTypeKey = new HashSet<string>
		{
			"saletype",
			"typesale"
		};

		private static readonly HashSet<string> SaleDurationKey = new HashSet<string>
		{
			"saleduration",
			"durationsale"
		};

		private static readonly HashSet<string> CashSaleKey = new HashSet<string>
		{
			"cashsale",
			"salecash"
		};

		private static readonly HashSet<string> GoldSaleKey = new HashSet<string>
		{
			"goldsale",
			"salegold"
		};

		private static readonly HashSet<string> CashGoldSaleKey = new HashSet<string>
		{
			"cashgoldsale",
			"goldcashsale",
			"salecashgold",
			"salegoldcash"
		};

		public DateTime Expiration
		{
			get;
		}

		public SaleType SaleType
		{
			get;
		}

		public SaleSource SaleSource
		{
			get;
		}

		public bool IsCashSale => SaleType.Contains(SaleType.CashSale);

		public bool IsGoldSale => SaleType.Contains(SaleType.GoldSale);

		public Sale(Sale sale)
		{
			Expiration = sale.Expiration;
			SaleType = sale.SaleType;
			SaleSource = sale.SaleSource;
		}

		public Sale(DateTime expiration, SaleType saleType, SaleSource saleSource)
		{
			Expiration = expiration;
			SaleType = saleType;
			SaleSource = saleSource;
		}

		public override string ToString()
		{
			return $"[Sale: {SaleType} (Expires in: {TimeSpan.FromSeconds(Expiration.Subtract(AntiCheatDateTime.UtcNow).TotalSeconds).ToString()})]";
		}

		public static Sale ParseFromFirebaseMessage(IDictionary<string, string> value)
		{
			DateTime? dateTime = null;
			SaleType saleType = SaleType.None;
			foreach (KeyValuePair<string, string> item3 in value)
			{
				string item = item3.Key.ToLower();
				if (NewSaleTypeKey.Contains(item))
				{
					string item2 = item3.Value.ToLower();
					if (CashSaleKey.Contains(item2))
					{
						saleType = SaleType.CashSale;
					}
					else if (GoldSaleKey.Contains(item2))
					{
						saleType = SaleType.GoldSale;
					}
					else if (CashGoldSaleKey.Contains(item2))
					{
						saleType = SaleType.CashGoldSale;
					}
				}
				if (SaleDurationKey.Contains(item))
				{
					dateTime = AntiCheatDateTime.UtcNow.AddSeconds(Convert.ToDouble(item3.Value));
				}
			}
			if (saleType == SaleType.None || !dateTime.HasValue)
			{
				return null;
			}
			return new Sale(dateTime.Value, saleType, SaleSource.Firebase);
		}

		public Sale(StorageDictionary storage)
		{
			Expiration = new DateTime(storage.Get("expiration", 0L), DateTimeKind.Utc);
			SaleType = (SaleType)storage.Get("saleType", 0);
			SaleSource = (SaleSource)storage.Get("saleSource", 0);
		}

		StorageDictionary IStorable.Serialize()
		{
			StorageDictionary storageDictionary = new StorageDictionary();
			storageDictionary.Set("expiration", Expiration.Ticks);
			storageDictionary.Set("saleType", (int)SaleType);
			storageDictionary.Set("saleSource", (int)SaleSource);
			return storageDictionary;
		}
	}
}
