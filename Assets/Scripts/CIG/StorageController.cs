using CIGMigrator;
using SUISS.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace CIG
{
	public static class StorageController
	{
		public delegate void SerializeEventHandler();

		public delegate void SerializedEventHandler();

		private const string RootFolder = "Storage";

		private const string GameRootKey = "GameRoot";

		private const string CloudStorageRootKey = "CloudStorageRoot";

		private const string ForeverRootKey = "ForeverRoot";

		private const string DeviceRootKey = "DeviceRoot";

		private const string PurchaseRootKey = "PurchaseRoot";

		private static Storage _storage;

		public static StorageDictionary CloudStorageRoot
		{
			get;
			private set;
		}

		public static StorageDictionary GameRoot
		{
			get;
			private set;
		}

		public static StorageDictionary ForeverRoot
		{
			get;
			private set;
		}

		public static StorageDictionary DeviceRoot
		{
			get;
			private set;
		}

		public static StorageDictionary PurchaseRoot
		{
			get;
			private set;
		}

		public static StorageWarning StorageWarning => _storage.StorageWarning;

		private static string StoragePath => Path.Combine(Application.persistentDataPath, "Storage");

		public static event SerializeEventHandler SerializeEvent;

		public static event SerializedEventHandler SerializedEvent;

		private static void FireSerializeEvent()
		{
			if (StorageController.SerializeEvent != null)
			{
				StorageController.SerializeEvent();
			}
		}

		private static void FireSerializedEvent()
		{
			if (StorageController.SerializedEvent != null)
			{
				StorageController.SerializedEvent();
			}
		}

		static StorageController()
		{
			_storage = new Storage(StoragePath);
			LoadStorageDictionaries();
		}

		public static void Migrate()
		{
			Migrator.CreateForeverMigrator(ForeverRoot.InternalDictionary).Migrate();
			Migrator.CreateDeviceMigrator(DeviceRoot.InternalDictionary).Migrate();
			Migrator.CreateGameMigrator(GameRoot.InternalDictionary).Migrate();
		}

		public static void Save()
		{
			FireSerializeEvent();
			_storage.Save();
			FireSerializedEvent();
		}

		public static string StorageToString(StorageDictionary storageDictionary)
		{
			return StorageToString(storageDictionary.InternalDictionary);
		}

		public static string StorageToString(Dictionary<string, object> storageDictionary)
		{
			return _storage.DictToString(storageDictionary);
		}

		public static StorageDictionary StringToStorage(string storageString)
		{
			Dictionary<string, object> storage;
			try
			{
				storage = _storage.StringToDict(storageString);
			}
			catch (Exception)
			{
				UnityEngine.Debug.LogWarningFormat("Could not convert string to storage! {0}", storageString);
				storage = new Dictionary<string, object>();
			}
			return new StorageDictionary(storage);
		}

		public static void ReplaceGameRootFromString(string input)
		{
			Dictionary<string, object> value = string.IsNullOrEmpty(input) ? new Dictionary<string, object>() : _storage.StringToDict(input);
			_storage.Root["CloudStorageRoot"] = value;
			LoadStorageDictionaries();
			Migrator.CreateGameMigrator(GameRoot.InternalDictionary).Migrate();
		}

		private static void LoadStorageDictionaries()
		{
			CloudStorageRoot = new StorageDictionary(_storage.GetDictionary("CloudStorageRoot"));
			GameRoot = CloudStorageRoot.GetStorageDict("GameRoot");
			ForeverRoot = new StorageDictionary(_storage.GetDictionary("ForeverRoot"));
			DeviceRoot = new StorageDictionary(_storage.GetDictionary("DeviceRoot"));
			PurchaseRoot = new StorageDictionary(_storage.GetDictionary("PurchaseRoot"));
		}
	}
}
