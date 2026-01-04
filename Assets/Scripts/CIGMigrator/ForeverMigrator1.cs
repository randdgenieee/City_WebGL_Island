using System.Collections.Generic;

namespace CIGMigrator
{
	public class ForeverMigrator1 : IMigrator
	{
		private const string CurrenciesKey = "Currencies";

		private const string BuildingsKey = "Buildings";

		private const string ServerGiftsStorageKey = "ServerGifts";

		private const string ServerGiftsUndeliveredKey = "Undelivered";

		public void Migrate(Dictionary<string, object> storageRoot)
		{
			if (storageRoot.TryGetValue("ServerGifts", out object value) && value is Dictionary<string, object>)
			{
				Dictionary<string, object> dictionary = (Dictionary<string, object>)value;
				if (dictionary.TryGetValue("Undelivered", out object value2) && value2 is Dictionary<string, object>)
				{
					Dictionary<string, object> value3 = (Dictionary<string, object>)value2;
					Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
					dictionary2.Add("Currencies", value3);
					dictionary2.Add("Buildings", new List<object>());
					dictionary["Undelivered"] = dictionary2;
				}
			}
		}
	}
}
