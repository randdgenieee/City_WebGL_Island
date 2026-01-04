using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace CIG
{
	public class BaseProperties
	{
		public delegate bool TryParseDelegate<T>(string input, out T output);

		private static readonly char[] ValueSeparator = new char[1]
		{
			','
		};

		private static readonly char[] ObjectSeparator = new char[1]
		{
			'|'
		};

		protected const string UnknownString = "unknown";

		protected PropertiesDictionary _propsDict;

		private readonly Dictionary<string, object> _cache;

		private readonly string _baseKey;

		public string BaseKey => _baseKey;

		public string Type
		{
			get;
			private set;
		}

		public BaseProperties(PropertiesDictionary propsDict, string baseKey)
		{
			_cache = new Dictionary<string, object>();
			_propsDict = propsDict;
			_baseKey = baseKey;
			if (!propsDict.HasBaseKey(baseKey))
			{
				UnityEngine.Debug.LogErrorFormat("Properties does not have a base key '{0}'", baseKey);
			}
			Type = GetProperty("type", string.Empty, optional: true);
		}

		public static object Parse<T>(string valueString)
		{
			return Parse(typeof(T), valueString);
		}

		public static object Parse(Type parseType, string valueString)
		{
			if (parseType == typeof(bool))
			{
				return bool.Parse(valueString.ToLowerInvariant());
			}
			if (parseType == typeof(int))
			{
				return ParseInt(valueString);
			}
			if (parseType == typeof(long))
			{
				return ParseLong(valueString);
			}
			if (parseType == typeof(double))
			{
				return ParseDouble(valueString);
			}
			if (parseType == typeof(float))
			{
				return ParseFloat(valueString);
			}
			if (parseType == typeof(decimal))
			{
				return ParseDecimal(valueString);
			}
			if (parseType == typeof(string))
			{
				return valueString;
			}
			if (parseType == typeof(Currencies))
			{
				return Currencies.Parse(valueString);
			}
			if (parseType == typeof(Currency))
			{
				return Currency.Parse(valueString);
			}
			if (parseType == typeof(List<string>))
			{
				return ParseList(valueString, (string s) => s);
			}
			if (parseType == typeof(List<int>))
			{
				return ParseList(valueString, ParseInt);
			}
			if (parseType == typeof(List<long>))
			{
				return ParseList(valueString, ParseLong);
			}
			if (parseType == typeof(List<double>))
			{
				return ParseList(valueString, ParseDouble);
			}
			if (parseType == typeof(List<float>))
			{
				return ParseList(valueString, ParseFloat);
			}
			if (parseType == typeof(List<decimal>))
			{
				return ParseList(valueString, ParseDecimal);
			}
			if (parseType == typeof(List<Currencies>))
			{
				return ParseList(valueString, Currencies.Parse, ObjectSeparator);
			}
			if (parseType == typeof(List<Currency>))
			{
				return ParseList(valueString, Currency.Parse, ObjectSeparator);
			}
			if (parseType == typeof(List<List<string>>))
			{
				return ParseJaggedList(valueString, (string s) => s);
			}
			if (parseType == typeof(List<List<int>>))
			{
				return ParseJaggedList(valueString, ParseInt);
			}
			if (parseType == typeof(List<List<long>>))
			{
				return ParseJaggedList(valueString, ParseLong);
			}
			if (parseType == typeof(List<List<double>>))
			{
				return ParseJaggedList(valueString, ParseDouble);
			}
			if (parseType == typeof(List<List<float>>))
			{
				return ParseJaggedList(valueString, ParseFloat);
			}
			if (parseType == typeof(List<List<decimal>>))
			{
				return ParseJaggedList(valueString, ParseDecimal);
			}
			throw new FormatException($"Type {parseType} is not a supported property type");
		}

		public static Dictionary<string, T> ParseKeyValue<T>(List<string> list, TryParseDelegate<T> tryParse)
		{
			Dictionary<string, T> dictionary = new Dictionary<string, T>();
			if (list.Count % 2 == 0)
			{
				int i = 0;
				for (int count = list.Count; i < count; i += 2)
				{
					string key = list[i];
					if (tryParse(list[i + 1], out T output))
					{
						dictionary.Add(key, output);
					}
					else
					{
						UnityEngine.Debug.LogError($"Cannot parse value '{list[i + 1]}' to '{typeof(T)}'");
					}
				}
			}
			else
			{
				UnityEngine.Debug.LogError("Cannot parse to Key-Value as the list contains an uneven amount of elements.");
			}
			return dictionary;
		}

		public bool HasProperty(string key)
		{
			return _propsDict.HasKey(_baseKey, key);
		}

		public T GetProperty<T>(string key, T defaultValue, bool optional = false)
		{
			string text = PropertiesDictionary.CombineKeys(_baseKey, key);
			if (_cache.TryGetValue(text, out object value) && value is T)
			{
				return (T)value;
			}
			if (_propsDict.TryGetValue(text, out string value2))
			{
				try
				{
					object obj = Parse<T>(value2);
					_cache.Add(text, obj);
					return (T)obj;
				}
				catch (FormatException ex)
				{
					UnityEngine.Debug.LogError($"Failed to parse property '{text} = {value2}' as {typeof(T)} : {ex.Message}");
				}
			}
			if (!optional)
			{
				UnityEngine.Debug.LogWarningFormat("Property of type {0} with key {1} does not exist.", typeof(T), text);
			}
			return defaultValue;
		}

		protected T[] GetEnumList<T>(string key) where T : struct, IConvertible
		{
			List<string> property = GetProperty(key, new List<string>());
			int count = property.Count;
			List<T> list = new List<T>(count);
			for (int i = 0; i < count; i++)
			{
				string text = property[i];
				try
				{
					list.Add((T)Enum.Parse(typeof(T), text, ignoreCase: true));
				}
				catch (ArgumentException)
				{
					UnityEngine.Debug.LogErrorFormat("Failed to parse '{0}' (index {1}) in {2}.", text, i, PropertiesDictionary.CombineKeys(BaseKey, key));
				}
			}
			return list.ToArray();
		}

		private static int ParseInt(string valueString)
		{
			return int.Parse(valueString, NumberStyles.Number, CultureInfo.InvariantCulture);
		}

		private static long ParseLong(string valueString)
		{
			return long.Parse(valueString, NumberStyles.Number, CultureInfo.InvariantCulture);
		}

		private static float ParseFloat(string valueString)
		{
			return float.Parse(valueString, NumberStyles.Number, CultureInfo.InvariantCulture);
		}

		private static double ParseDouble(string valueString)
		{
			return double.Parse(valueString, NumberStyles.Number, CultureInfo.InvariantCulture);
		}

		private static decimal ParseDecimal(string valueString)
		{
			return decimal.Parse(valueString, NumberStyles.Number, CultureInfo.InvariantCulture);
		}

		private static List<T> ParseList<T>(string valueString, Func<string, T> parse)
		{
			return ParseList(valueString, parse, ValueSeparator);
		}

		private static List<T> ParseList<T>(string valueString, Func<string, T> parse, char[] separator)
		{
			string[] array = ParseStringList(valueString, separator);
			List<T> list = new List<T>();
			int num = array.Length;
			for (int i = 0; i < num; i++)
			{
				list.Add(parse(array[i].Trim()));
			}
			return list;
		}

		private static string[] ParseStringList(string valueString, char[] separator)
		{
			int num = valueString.IndexOf('[');
			int num2 = valueString.IndexOf(']');
			if (num >= 0 && num2 > 0 && num2 > num)
			{
				return valueString.Substring(num + 1, num2 - num - 1).Split(separator, StringSplitOptions.RemoveEmptyEntries);
			}
			throw new FormatException("Unable to parse array");
		}

		private static List<List<T>> ParseJaggedList<T>(string valueString, Func<string, T> parse)
		{
			int num = valueString.IndexOf('[');
			int num2 = valueString.IndexOf(']');
			if (num >= 0 && num2 > 0 && num2 > num)
			{
				List<List<T>> list = new List<List<T>>();
				string[] array = valueString.Substring(num + 1, num2 - num - 1).Split(ObjectSeparator, StringSplitOptions.RemoveEmptyEntries);
				int num3 = array.Length;
				for (int i = 0; i < num3; i++)
				{
					List<T> list2 = new List<T>();
					string[] array2 = array[i].Split(ValueSeparator, StringSplitOptions.RemoveEmptyEntries);
					int num4 = array2.Length;
					for (int j = 0; j < num4; j++)
					{
						list2.Add(parse(array2[j].Trim()));
					}
					list.Add(list2);
				}
				return list;
			}
			throw new FormatException("Unable to parse nested array");
		}
	}
}
