using System;
using System.Globalization;

namespace CIG
{
	public struct Currency : IStorable
	{
		public const string InvalidCurrency = "Invalid";

		public static readonly Currency Invalid = new Currency("Invalid", decimal.Zero);

		private const string NameKey = "Name";

		private const string ValueKey = "Value";

		public string Name
		{
			get;
		}

		public decimal Value
		{
			get;
		}

		public bool IsValid => !IsMatchingName("Invalid");

		public Currency(string name, decimal value)
		{
			Name = name;
			Value = value;
		}

		public Currency(StorageDictionary storageDictionary)
		{
			Name = storageDictionary.Get("Name", "unknown");
			Value = storageDictionary.Get("Value", decimal.Zero);
		}

		public static Currency Parse(string currencyString)
		{
			if (string.IsNullOrEmpty(currencyString) || currencyString == "empty")
			{
				return Invalid;
			}
			string[] array = currencyString.Split(',');
			if (array.Length != 2)
			{
				throw new FormatException("currencyString does not contain two entries.");
			}
			if (!decimal.TryParse(array[1], out decimal result))
			{
				throw new FormatException(array[1] + " could not be parsed to decimal.");
			}
			return new Currency(array[0], result);
		}

		public Currency Multiply(decimal scalar, RoundingMethod? roundingMethod, int precision = 0)
		{
			decimal value = Value * scalar;
			if (roundingMethod.HasValue)
			{
				value = CIGUtilities.Round(value, roundingMethod.Value, precision);
			}
			return new Currency(Name, value);
		}

		public bool IsMatchingName(string currency)
		{
			return Name.Equals(currency, StringComparison.Ordinal);
		}

		public bool Equals(Currency other)
		{
			if (string.Equals(Name, other.Name))
			{
				return Value == other.Value;
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			object obj2;
			if ((obj2 = obj) is Currency)
			{
				Currency other = (Currency)obj2;
				return Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (Name.GetHashCode() * 397) ^ Value.GetHashCode();
		}

		public override string ToString()
		{
			return Name + ":" + Value.ToString(CultureInfo.InvariantCulture);
		}

		public static Currency GoldCurrency(decimal amount)
		{
			return new Currency("Gold", amount);
		}

		public static Currency CashCurrency(decimal amount)
		{
			return new Currency("Cash", amount);
		}

		public static Currency XPCurrency(decimal amount)
		{
			return new Currency("XP", amount);
		}

		public static Currency SilverKeyCurrency(decimal amount)
		{
			return new Currency("SilverKey", amount);
		}

		public static Currency GoldKeyCurrency(decimal amount)
		{
			return new Currency("GoldKey", amount);
		}

		public static Currency TokenCurrency(decimal amount)
		{
			return new Currency("Token", amount);
		}

		public static Currency CraneCurrency(decimal amount)
		{
			return new Currency("Crane", amount);
		}

		public static Currency LevelUpCurrency(decimal amount)
		{
			return new Currency("LevelUp", amount);
		}

		public CurrencyType ToCurrencyType()
		{
			switch (Name)
			{
			case "XP":
			case "LevelUp":
				return CurrencyType.XP;
			case "Cash":
				return CurrencyType.Cash;
			case "Gold":
				return CurrencyType.Gold;
			case "SilverKey":
				return CurrencyType.SilverKey;
			case "GoldKey":
				return CurrencyType.GoldKey;
			case "Token":
				return CurrencyType.Token;
			case "Crane":
				return CurrencyType.Crane;
			case "FishingRod":
				return CurrencyType.FishingRod;
			case "Fish":
				return CurrencyType.Fish;
			case "Building":
				return CurrencyType.Building;
			default:
				return CurrencyType.Unknown;
			}
		}

		public static Currency operator +(Currency c, decimal amount)
		{
			return new Currency(c.Name, c.Value + amount);
		}

		public static Currency operator -(Currency c)
		{
			return new Currency(c.Name, -c.Value);
		}

		StorageDictionary IStorable.Serialize()
		{
			StorageDictionary storageDictionary = new StorageDictionary();
			storageDictionary.Set("Name", Name);
			storageDictionary.Set("Value", Value);
			return storageDictionary;
		}
	}
}
