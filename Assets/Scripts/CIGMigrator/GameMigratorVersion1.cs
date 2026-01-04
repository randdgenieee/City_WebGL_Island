using System.Collections.Generic;

namespace CIGMigrator
{
	public class GameMigratorVersion1 : IMigrator
	{
		private const string WorldMapStorageKey = "WorldMap";

		private const string AirshipStorageKey = "Airship";

		private const string AirshipStateKey = "State";

		private const string AirshipTravelDurationKey = "TravelDuration";

		private const string AirshipTravelUpspeedableProcessKey = "TravelUpspeedableProcess";

		private const string AirshipFromIslandKey = "FromIsland";

		private const string AirshipToIslandKey = "ToIsland";

		private const string AirshipCurrentIslandKey = "CurrentIsland";

		private const int TravellingStateInt = 0;

		private const int HoveringStateInt = 1;

		private const int LandedStateInt = 2;

		private const int NoneIslandInt = -1;

		private const int FirstIslandInt = 0;

		public void Migrate(Dictionary<string, object> storageRoot)
		{
			object value2;
			if (!storageRoot.TryGetValue("WorldMap", out object value) || !(value is Dictionary<string, object>) || !((Dictionary<string, object>)value).TryGetValue("Airship", out value2) || !(value2 is Dictionary<string, object>))
			{
				return;
			}
			Dictionary<string, object> dictionary = (Dictionary<string, object>)value2;
			object value6;
			object value5;
			object value4;
			if (!dictionary.TryGetValue("State", out object value3) || !(value3 is int) || !dictionary.TryGetValue("CurrentIsland", out value4) || !(value4 is int) || !dictionary.TryGetValue("FromIsland", out value5) || !(value5 is int) || !dictionary.TryGetValue("ToIsland", out value6) || !(value6 is int))
			{
				return;
			}
			int num = (int)value3;
			int num2 = (int)value4;
			int num3 = (int)value5;
			int num4 = (int)value6;
			switch (num)
			{
			case 0:
				if (num3 == -1 || num4 == -1)
				{
					ResetAirship(dictionary);
				}
				break;
			case 1:
			case 2:
				if (num2 == -1)
				{
					ResetAirship(dictionary);
				}
				break;
			}
		}

		private void ResetAirship(Dictionary<string, object> airshipStorage)
		{
			airshipStorage["State"] = 2;
			airshipStorage["TravelDuration"] = double.MinValue;
			airshipStorage["FromIsland"] = -1;
			airshipStorage["ToIsland"] = -1;
			airshipStorage["CurrentIsland"] = 0;
			airshipStorage.Remove("TravelUpspeedableProcess");
		}
	}
}
