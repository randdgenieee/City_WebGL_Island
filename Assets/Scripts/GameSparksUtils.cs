using CIG;
using GameSparks.Core;
using SUISS.Storage;
using System;
using UnityEngine;

public static class GameSparksUtils
{
	private const string AccountStorageKey = "GSAccountStorage";

	private const string UsernameKey = "Username";

	private const string PasswordKey = "Password";

	public static readonly DateTime Epoch = new DateTime(1970, 1, 1);

	public static DateTime GetDateTime(string field, GSData data)
	{
		long? @long = data.GetLong(field);
		if (!@long.HasValue)
		{
			return Epoch;
		}
		return Epoch + TimeSpan.FromMilliseconds(@long.Value);
	}

	public static void LogGameSparksError(GameSparksException exception)
	{
		if (exception == null)
		{
			UnityEngine.Debug.LogError("Can't log a Game Sparks Exception if it's null");
		}
		else if ((exception.Error & GSError.UserOrEnvironmentError) == GSError.None)
		{
			UnityEngine.Debug.LogError(exception.ToString());
		}
	}

	public static void SaveAccountToStorage(string username, string hashedPassword)
	{
		Storage storage = new Storage(Lifecycles.GetPath(StorageLifecycle.Player));
		StorageDictionary storageDictionary = new StorageDictionary(storage.GetDictionary("GSAccountStorage"));
		storageDictionary.Set("Username", username);
		storageDictionary.Set("Password", hashedPassword);
		storage.Save();
	}

	public static void LoadAccountFromStorage(out string username, out string password)
	{
		StorageDictionary storageDictionary = new StorageDictionary(new Storage(Lifecycles.GetPath(StorageLifecycle.Player)).GetDictionary("GSAccountStorage"));
		username = storageDictionary.Get("Username", string.Empty);
		password = storageDictionary.Get("Password", string.Empty);
	}
}
