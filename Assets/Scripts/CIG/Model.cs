namespace CIG
{
	public class Model
	{
		public GameServer GameServer
		{
			get;
		}

		public Device Device
		{
			get;
		}

		public Game Game
		{
			get;
			private set;
		}

		public Model()
		{
			Device = new Device(StorageController.DeviceRoot);
			GameServer = new GameServer(StorageController.ForeverRoot, Device);
		}

		public void Release()
		{
			StorageController.SerializeEvent -= OnSerialize;
			StorageController.SerializedEvent -= OnSerialized;
			GameServer.Release();
			Game?.Release();
			Device.Release();
		}

		public Game StartGame()
		{
			StopGame();
			Game = new Game(StorageController.GameRoot, Device, GameServer, Device.FirstVersion);
			StorageController.SerializeEvent += OnSerialize;
			StorageController.SerializedEvent += OnSerialized;
			return Game;
		}

		public void StopGame()
		{
			if (Game != null)
			{
				StorageController.SerializeEvent -= OnSerialize;
				StorageController.SerializedEvent -= OnSerialized;
				Game.Release();
				Game = null;
			}
		}

		private void OnSerialize()
		{
			GameServer.Serialize();
			Device.Serialize();
			Game.Serialize();
		}

		private void OnSerialized()
		{
			GameServer.GameSparksServer.CloudStorage.TryPush(Game.GameState.Level);
		}
	}
}
