using System.Collections.Generic;

namespace CIG
{
	public static class Parser
	{
		public static Dictionary<string, string> ParsePropertyFileFormat(string text)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			string[] array = text.Split('\n');
			foreach (string text2 in array)
			{
				if (!text2.StartsWith("#"))
				{
					int num = text2.IndexOf('=');
					if (num >= 0)
					{
						string key = text2.Substring(0, num).Trim();
						string text4 = dictionary[key] = text2.Substring(num + 1).Trim();
					}
				}
			}
			return dictionary;
		}
	}
}
