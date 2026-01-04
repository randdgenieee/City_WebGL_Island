using System.Collections.Generic;

namespace CIG
{
	public static class ParsingUtility
	{
		public static string Get(this Dictionary<string, string> dictionary, string key, string defaultValue)
		{
			if (dictionary.TryGetValue(key, out string value))
			{
				return value;
			}
			return defaultValue;
		}

		public static bool Get(this Dictionary<string, string> dictionary, string key, bool defaultValue)
		{
			if (dictionary.TryGetValue(key, out string value) && bool.TryParse(value, out bool result))
			{
				return result;
			}
			return defaultValue;
		}

		public static int Get(this Dictionary<string, string> dictionary, string key, int defaultValue)
		{
			if (dictionary.TryGetValue(key, out string value) && int.TryParse(value, out int result))
			{
				return result;
			}
			return defaultValue;
		}

		public static long Get(this Dictionary<string, string> dictionary, string key, long defaultValue)
		{
			if (dictionary.TryGetValue(key, out string value) && long.TryParse(value, out long result))
			{
				return result;
			}
			return defaultValue;
		}

		public static double Get(this Dictionary<string, string> dictionary, string key, double defaultValue)
		{
			if (dictionary.TryGetValue(key, out string value) && double.TryParse(value, out double result))
			{
				return result;
			}
			return defaultValue;
		}

		public static decimal Get(this Dictionary<string, string> dictionary, string key, decimal defaultValue)
		{
			if (dictionary.TryGetValue(key, out string value) && decimal.TryParse(value, out decimal result))
			{
				return result;
			}
			return defaultValue;
		}
	}
}
