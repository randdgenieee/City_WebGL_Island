using UnityEngine;

namespace CIG
{
	public class Device
	{
		private const string UserStorageKey = "Device";

		private const string SettingsStorageKey = "Settings";

		private const string FirstVersionKey = "FirstVersion";

		private const string NumberOfLowMemoryWarningsKey = "NumberOfLowMemoryWarnings";

		private StorageDictionary _storage;

		public User User
		{
			get;
		}

		public Settings Settings
		{
			get;
		}

		public CIGGameVersion FirstVersion
		{
			get;
		}

		public int NumberOfLowMemoryWarnings
		{
			get;
			private set;
		}

		public Device(StorageDictionary storage)
		{
			_storage = storage;
			User = new User(storage.GetStorageDict("Device"));
			Settings = new Settings(storage.GetStorageDict("Settings"));
			NumberOfLowMemoryWarnings = storage.Get("NumberOfLowMemoryWarnings", 0);
			if (StorageController.GameRoot.Contains("FirstVersion"))
			{
				FirstVersion = new CIGGameVersion(StorageController.GameRoot.GetStorageDict("FirstVersion"));
			}
			else
			{
				FirstVersion = CIGGameVersion.GetCurrentVersion();
				StorageController.GameRoot.Set("FirstVersion", FirstVersion);
			}
			Application.lowMemory += OnLowMemory;
		}

		public void Release()
		{
			Application.lowMemory -= OnLowMemory;
		}

		private void OnLowMemory()
		{
			NumberOfLowMemoryWarnings++;
		}

		public void Serialize()
		{
			User.Serialize();
			Settings.Serialize();
			_storage.Set("NumberOfLowMemoryWarnings", NumberOfLowMemoryWarnings);
		}
	}
}
