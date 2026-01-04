using System.Collections.Generic;

namespace CIGMigrator
{
	public class GameMigratorVersion6 : IMigrator
	{
		private const string OneTimeOfferStorageKey = "OneTimeOfferManager";

		private const string OneTimeBuildingStorageKey = "OneTimeOfferBuilding";

		private const string InitialDiscountKey = "InitialDiscount";

		private const string DiscountIncrementKey = "DiscountIncrement";

		private const string DiscountDecrementKey = "DiscountDecrement";

		private const string MinimumDiscountKey = "MinimumDiscount";

		private const string MaximumDiscountKey = "MaximumDiscount";

		private const string MinimumLevelRequiredKey = "MinimumLevelRequired";

		private const string FeatureEnabledKey = "Enabled";

		private const string CurrentDiscountKey = "CurrentDiscount";

		private const string OfferExpirationSecondsKey = "OfferExpirationSeconds";

		private const string ShownOffersKey = "ShownOffers";

		public void Migrate(Dictionary<string, object> storageRoot)
		{
			if (storageRoot.TryGetValue("OneTimeOfferManager", out object value) && value is Dictionary<string, object>)
			{
				Dictionary<string, object> dictionary = (Dictionary<string, object>)value;
				Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
				if (dictionary.TryGetValue("InitialDiscount", out object value2))
				{
					dictionary2.Add("InitialDiscount", value2);
					dictionary.Remove("InitialDiscount");
				}
				if (dictionary.TryGetValue("DiscountIncrement", out object value3))
				{
					dictionary2.Add("DiscountIncrement", value3);
					dictionary.Remove("DiscountIncrement");
				}
				if (dictionary.TryGetValue("DiscountDecrement", out object value4))
				{
					dictionary2.Add("DiscountDecrement", value4);
					dictionary.Remove("DiscountDecrement");
				}
				if (dictionary.TryGetValue("MinimumDiscount", out object value5))
				{
					dictionary2.Add("MinimumDiscount", value5);
					dictionary.Remove("MinimumDiscount");
				}
				if (dictionary.TryGetValue("MaximumDiscount", out object value6))
				{
					dictionary2.Add("MaximumDiscount", value6);
					dictionary.Remove("MaximumDiscount");
				}
				if (dictionary.TryGetValue("MinimumLevelRequired", out object value7))
				{
					dictionary2.Add("MinimumLevelRequired", value7);
					dictionary.Remove("MinimumLevelRequired");
				}
				if (dictionary.TryGetValue("Enabled", out object value8))
				{
					dictionary2.Add("Enabled", value8);
					dictionary.Remove("Enabled");
				}
				if (dictionary.TryGetValue("CurrentDiscount", out object value9))
				{
					dictionary2.Add("CurrentDiscount", value9);
					dictionary.Remove("CurrentDiscount");
				}
				if (dictionary.TryGetValue("OfferExpirationSeconds", out object value10))
				{
					dictionary2.Add("OfferExpirationSeconds", value10);
					dictionary.Remove("OfferExpirationSeconds");
				}
				if (dictionary.TryGetValue("ShownOffers", out object value11))
				{
					dictionary2.Add("ShownOffers", value11);
					dictionary.Remove("ShownOffers");
				}
				dictionary["OneTimeOfferBuilding"] = dictionary2;
			}
		}
	}
}
