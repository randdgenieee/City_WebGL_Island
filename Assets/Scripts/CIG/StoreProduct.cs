using System;
using System.Collections.Generic;

namespace CIG
{
	public class StoreProduct : IComparable<StoreProduct>
	{
		public const string IdentifierKey = "ProductIdentifier";

		public const string TitleKey = "ProductTitle";

		public const string DescriptionKey = "ProductDescription";

		public const string PriceKey = "ProductPrice";

		public const string CurrencyCodeKey = "ProductCurrencyCode";

		public const string FormattedPriceKey = "ProductFormattedPrice";

		public const string EuroPriceKey = "ProductEuroPrice";

		public const string DollarPriceKey = "ProductDollarPrice";

		public const string CustomPropertiesKey = "CustomProperties";

		public const string AvailableKey = "Available";

		public bool Available
		{
			get;
			set;
		}

		public string Identifier
		{
			get;
			private set;
		}

		public int StorePosition
		{
			get;
			set;
		}

		public string Title
		{
			get;
			private set;
		}

		public string Description
		{
			get;
			private set;
		}

		public decimal Price
		{
			get;
			private set;
		}

		public string CurrencyCode
		{
			get;
			private set;
		}

		public string FormattedPrice
		{
			get;
			private set;
		}

		public decimal DollarPrice
		{
			get;
			private set;
		}

		public decimal EuroPrice
		{
			get;
			private set;
		}

		public Dictionary<string, string> CustomProperties
		{
			get;
			private set;
		}

		public StoreProduct(StorageDictionary props)
		{
			Update(props);
		}

		public virtual void Update(StorageDictionary props)
		{
			Identifier = props.Get("ProductIdentifier", "unknown");
			Title = props.Get("ProductTitle", string.Empty);
			Description = props.Get("ProductDescription", string.Empty);
			CurrencyCode = props.Get("ProductCurrencyCode", string.Empty);
			FormattedPrice = props.Get("ProductFormattedPrice", string.Empty);
			Price = props.Get("ProductPrice", decimal.Zero);
			DollarPrice = props.Get("ProductDollarPrice", decimal.Zero);
			EuroPrice = props.Get("ProductEuroPrice", decimal.Zero);
			Available = props.Get("Available", defaultValue: false);
			if (props.Contains("CustomProperties"))
			{
				Dictionary<string, string> dictionary = props.GetDictionary<string>("CustomProperties");
				CustomProperties = new Dictionary<string, string>();
				foreach (KeyValuePair<string, string> item in dictionary)
				{
					CustomProperties.Add(item.Key, item.Value);
				}
			}
			else
			{
				CustomProperties = new Dictionary<string, string>();
			}
		}

		public int CompareTo(StoreProduct other)
		{
			if (Price > decimal.Zero)
			{
				return Price.CompareTo(other.Price);
			}
			if (EuroPrice > decimal.Zero)
			{
				return EuroPrice.CompareTo(other.EuroPrice);
			}
			if (DollarPrice > decimal.Zero)
			{
				return DollarPrice.CompareTo(other.DollarPrice);
			}
			return 0;
		}

		public override string ToString()
		{
			return $"[StoreProduct: {Identifier}; Available: {Available}]";
		}

		public StorageDictionary Serialize()
		{
			StorageDictionary storageDictionary = new StorageDictionary();
			storageDictionary.Set("ProductIdentifier", Identifier);
			storageDictionary.Set("ProductTitle", Title);
			storageDictionary.Set("ProductDescription", Description);
			storageDictionary.Set("ProductPrice", Price);
			storageDictionary.Set("ProductCurrencyCode", CurrencyCode);
			storageDictionary.Set("ProductFormattedPrice", FormattedPrice);
			storageDictionary.Set("ProductEuroPrice", EuroPrice);
			storageDictionary.Set("ProductDollarPrice", DollarPrice);
			storageDictionary.Set("CustomProperties", CustomProperties);
			storageDictionary.Set("Available", Available);
			return storageDictionary;
		}
	}
}
