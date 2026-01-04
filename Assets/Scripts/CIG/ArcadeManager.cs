namespace CIG
{
	public class ArcadeManager
	{
		private readonly StorageDictionary _storage;

		private const string WheelOfFortuneKey = "WheelOfFortune";

		public WheelOfFortune WheelOfFortune
		{
			get;
		}

		public bool CanPlay => WheelOfFortune.CanAffordNormalCost;

		public ArcadeManager(StorageDictionary storage, WebService webService, GameState gameState, WheelOfFortuneProperties wheelOfFortuneProperties)
		{
			_storage = storage;
			WheelOfFortune = new WheelOfFortune(_storage.GetStorageDict("WheelOfFortune"), webService, gameState, wheelOfFortuneProperties);
		}

		public void Serialize()
		{
			_storage.Set("WheelOfFortune", WheelOfFortune);
		}
	}
}
