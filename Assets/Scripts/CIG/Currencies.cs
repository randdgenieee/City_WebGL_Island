using GameSparks.Core;
using SparkLinq;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace CIG
{
	[Serializable]
	public class Currencies : IStorable
	{
		private readonly Dictionary<string, decimal> _currencies = new Dictionary<string, decimal>();

		private readonly List<string> _keys = new List<string>();

		public decimal SumValues
		{
			get
			{
				decimal num = default(decimal);
				foreach (KeyValuePair<string, decimal> currency in _currencies)
				{
					num += currency.Value;
				}
				return num;
			}
		}

		public int KeyCount => _currencies.Count;

		public Currencies()
		{
		}

		public Currencies(Currencies currencies)
		{
			foreach (KeyValuePair<string, decimal> currency in currencies._currencies)
			{
				SetValue(currency.Key, currency.Value);
			}
		}

		public Currencies(string currency, decimal value)
		{
			SetValue(currency, value);
		}

		public Currencies(params Currency[] currencies)
		{
			int i = 0;
			for (int num = currencies.Length; i < num; i++)
			{
				Currency currency = currencies[i];
				SetValue(currency.Name, GetValue(currency.Name) + currency.Value);
			}
		}

		private Currencies(Dictionary<string, decimal> currencies)
		{
			_currencies = currencies;
			foreach (KeyValuePair<string, decimal> currency in currencies)
			{
				_keys.Add(currency.Key);
			}
		}

		public Currencies(StorageDictionary storage)
		{
			foreach (KeyValuePair<string, object> item in storage.InternalDictionary)
			{
				SetValue(item.Key, (decimal)item.Value);
			}
		}

		public static Currencies Parse(string currenciesString)
		{
			Currencies currencies = new Currencies();
			if (string.IsNullOrEmpty(currenciesString) || currenciesString == "empty")
			{
				return currencies;
			}
			string[] array = currenciesString.Split(',');
			if (array.Length % 2 != 0)
			{
				throw new FormatException("currenciesString '" + currenciesString + "' does not contains an even amount of entries.");
			}
			int i = 0;
			for (int num = array.Length; i < num; i += 2)
			{
				string currency = array[i].Trim();
				if (!decimal.TryParse(array[i + 1], NumberStyles.Number, CultureInfo.InvariantCulture, out decimal result))
				{
					throw new FormatException("The second value of an entry could not be parsed to long: " + array[i + 1] + ".");
				}
				currencies.SetValue(currency, result);
			}
			return currencies;
		}

		public static Currencies Parse(GSData gsData)
		{
			Currencies currencies = new Currencies();
			foreach (KeyValuePair<string, object> baseDatum in gsData.BaseData)
			{
				double? num = baseDatum.Value as double?;
				if (!num.HasValue)
				{
					UnityEngine.Debug.LogErrorFormat("GS Currency value can not be converted to dobule: {0}", baseDatum.Value.GetType().Name);
				}
				else
				{
					switch (baseDatum.Key)
					{
					case "Cash":
					case "Gold":
					case "GoldKey":
					case "SilverKey":
					case "Token":
						currencies.SetValue(baseDatum.Key, (decimal)num.Value);
						break;
					default:
						UnityEngine.Debug.LogErrorFormat("Unknown currency type in GS Currency: {0}", baseDatum.Key);
						break;
					}
				}
			}
			return currencies;
		}

		public decimal GetValue(string currency)
		{
			if (!string.IsNullOrEmpty(currency) && _currencies.TryGetValue(currency, out decimal value))
			{
				return value;
			}
			return decimal.Zero;
		}

		public Currency GetCurrency(int index)
		{
			if (index >= 0 && index < _keys.Count)
			{
				return new Currency(_keys[index], GetValue(_keys[index]));
			}
			return Currency.Invalid;
		}

		public Currency GetCurrency(string currency)
		{
			if (!string.IsNullOrEmpty(currency) && _currencies.TryGetValue(currency, out decimal value))
			{
				return new Currency(currency, value);
			}
			return Currency.Invalid;
		}

		public bool IsEmpty()
		{
			if (_currencies.Count != 0)
			{
				return _currencies.Count((string key, decimal value) => value != decimal.Zero) == 0;
			}
			return true;
		}

		public bool Contains(string currency)
		{
			return _currencies.ContainsKey(currency);
		}

		public bool ContainsPositive(string currency)
		{
			return GetValue(currency) > decimal.Zero;
		}

		public bool ContainsApproximate(string currency)
		{
			return GetValue(currency) != decimal.Zero;
		}

		public bool ContainsNegative()
		{
			return _currencies.Count((string key, decimal value) => value < decimal.Zero) > 0;
		}

		public Currencies WithoutNegative()
		{
			Dictionary<string, decimal> dictionary = new Dictionary<string, decimal>();
			foreach (KeyValuePair<string, decimal> currency in _currencies)
			{
				if (currency.Value > decimal.Zero)
				{
					dictionary.Add(currency.Key, currency.Value);
				}
			}
			return new Currencies(dictionary);
		}

		public Currencies MissingCurrencies(Currencies c)
		{
			Dictionary<string, decimal> dictionary = new Dictionary<string, decimal>();
			foreach (KeyValuePair<string, decimal> currency in c._currencies)
			{
				decimal num = GetValue(currency.Key) - currency.Value;
				if (num < 0.0m)
				{
					dictionary.Add(currency.Key, -num);
				}
			}
			return new Currencies(dictionary);
		}

		public bool MissingCurrency(Currency c)
		{
			return GetValue(c.Name) - c.Value < 0.0m;
		}

		public Currencies Round()
		{
			return Round(RoundingMethod.Nearest, 0);
		}

		public Currencies Round(RoundingMethod method)
		{
			return Round(method, 0);
		}

		public Currencies Round(int precision)
		{
			return Round(RoundingMethod.Nearest, precision);
		}

		public Currencies Round(RoundingMethod method, int precision)
		{
			Dictionary<string, decimal> dictionary = new Dictionary<string, decimal>();
			foreach (KeyValuePair<string, decimal> currency in _currencies)
			{
				dictionary.Add(currency.Key, CIGUtilities.Round(currency.Value, method, precision));
			}
			return new Currencies(dictionary);
		}

		public Currencies Cap(Currencies input, bool capUnknownWithZero)
		{
			Dictionary<string, decimal> dictionary = new Dictionary<string, decimal>();
			foreach (KeyValuePair<string, decimal> currency in input._currencies)
			{
				string key = currency.Key;
				decimal num = currency.Value;
				decimal value = GetValue(key);
				if (value >= decimal.Zero)
				{
					num = Math.Min(num, value);
				}
				else if (capUnknownWithZero)
				{
					num = default(decimal);
				}
				dictionary.Add(key, num);
			}
			return new Currencies(dictionary);
		}

		public Currencies Filter(params string[] filter)
		{
			Dictionary<string, decimal> dictionary = new Dictionary<string, decimal>(_currencies);
			int i = 0;
			for (int num = filter.Length; i < num; i++)
			{
				dictionary.Remove(filter[i]);
			}
			return new Currencies(dictionary);
		}

		public void Remove(string currency)
		{
			_currencies.Remove(currency);
		}

		public void SetValue(string currency, decimal value)
		{
			if (string.IsNullOrEmpty(currency))
			{
				throw new ArgumentException("Currency can't be null or empty.", "currency");
			}
			_currencies[currency] = value;
			if (!_keys.Contains(currency))
			{
				_keys.Add(currency);
			}
		}

		public Currencies WithoutEmpty()
		{
			Dictionary<string, decimal> dictionary = new Dictionary<string, decimal>();
			foreach (KeyValuePair<string, decimal> currency in _currencies)
			{
				if (currency.Value != decimal.Zero)
				{
					dictionary.Add(currency.Key, currency.Value);
				}
			}
			return new Currencies(dictionary);
		}

		public GSData ToGSData()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			foreach (KeyValuePair<string, decimal> currency in _currencies)
			{
				dictionary.Add(currency.Key, currency.Value);
			}
			return new GSData(dictionary);
		}

		public override int GetHashCode()
		{
			return _currencies.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			Currencies other;
			if (obj == null || (object)(other = (obj as Currencies)) == null)
			{
				return false;
			}
			return Equals(other);
		}

		public override string ToString()
		{
			return "{" + string.Join(",", _currencies) + "}";
		}

		private bool Equals(Currencies other)
		{
			if (other == null)
			{
				return false;
			}
			Currencies currencies = WithoutEmpty();
			Currencies currencies2 = other.WithoutEmpty();
			if (currencies._currencies.Count != currencies2._currencies.Count)
			{
				return false;
			}
			foreach (KeyValuePair<string, decimal> currency in currencies._currencies)
			{
				if (!currencies2._currencies.TryGetValue(currency.Key, out decimal value) || currency.Value != value)
				{
					return false;
				}
			}
			return true;
		}

		StorageDictionary IStorable.Serialize()
		{
			StorageDictionary storageDictionary = new StorageDictionary();
			foreach (KeyValuePair<string, decimal> currency in _currencies)
			{
				storageDictionary.Set(currency.Key, currency.Value);
			}
			return storageDictionary;
		}

		public static bool operator ==(Currencies c1, Currencies c2)
		{
			return c1?.Equals(c2) ?? ((object)c2 == null);
		}

		public static bool operator !=(Currencies c1, Currencies c2)
		{
			return !(c1 == c2);
		}

		public static Currencies operator -(Currencies c)
		{
			if ((object)c == null)
			{
				return null;
			}
			Dictionary<string, decimal> dictionary = new Dictionary<string, decimal>();
			foreach (KeyValuePair<string, decimal> currency in c._currencies)
			{
				dictionary.Add(currency.Key, -currency.Value);
			}
			return new Currencies(dictionary);
		}

		public static Currencies operator +(Currencies c1, Currencies c2)
		{
			if ((object)c1 == null)
			{
				return c2;
			}
			if ((object)c2 == null)
			{
				return c1;
			}
			Currencies currencies = new Currencies(c1);
			foreach (KeyValuePair<string, decimal> currency in c2._currencies)
			{
				currencies.SetValue(currency.Key, currencies.GetValue(currency.Key) + currency.Value);
			}
			return currencies;
		}

		public static Currencies operator +(Currencies c1, Currency c2)
		{
			if ((object)c1 == null)
			{
				return new Currencies(c2);
			}
			Currencies currencies = new Currencies(c1);
			currencies.SetValue(c2.Name, currencies.GetValue(c2.Name) + c2.Value);
			return currencies;
		}

		public static Currencies operator -(Currencies c1, Currencies c2)
		{
			if ((object)c1 == null)
			{
				return -c2;
			}
			if ((object)c2 == null)
			{
				return c1;
			}
			Currencies currencies = new Currencies(c1);
			foreach (KeyValuePair<string, decimal> currency in c2._currencies)
			{
				currencies.SetValue(currency.Key, currencies.GetValue(currency.Key) - currency.Value);
			}
			return currencies;
		}

		public static Currencies operator -(Currencies c1, Currency c2)
		{
			if ((object)c1 == null)
			{
				return new Currencies(-c2);
			}
			Currencies currencies = new Currencies(c1);
			currencies.SetValue(c2.Name, currencies.GetValue(c2.Name) - c2.Value);
			return currencies;
		}

		public static Currencies operator *(Currencies c, decimal m)
		{
			if ((object)c == null)
			{
				return null;
			}
			Dictionary<string, decimal> dictionary = new Dictionary<string, decimal>();
			foreach (KeyValuePair<string, decimal> currency in c._currencies)
			{
				dictionary.Add(currency.Key, currency.Value * m);
			}
			return new Currencies(dictionary);
		}

		public static Currencies operator *(decimal m, Currencies c)
		{
			return c * m;
		}

		public static Currencies operator /(Currencies c, decimal d)
		{
			if ((object)c == null)
			{
				return null;
			}
			Dictionary<string, decimal> dictionary = new Dictionary<string, decimal>();
			foreach (KeyValuePair<string, decimal> currency in c._currencies)
			{
				dictionary.Add(currency.Key, currency.Value / d);
			}
			return new Currencies(dictionary);
		}
	}
}
