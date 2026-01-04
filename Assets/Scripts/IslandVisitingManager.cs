using CIG;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class IslandVisitingManager
{
	public delegate void PullCompleteHandler(string userId);

	private static readonly TimeSpan CacheRetention = TimeSpan.FromDays(1.0);

	public const string IslandsKey = "Islands";

	public const string LevelKey = "Level";

	public const string LevelProgressKey = "LevelProgress";

	public const string HappinessKey = "Happiness";

	public const string PopulationKey = "Population";

	public const string HousingKey = "Housing";

	public const string EmployeesKey = "Employees";

	public const string JobsKey = "Jobs";

	private readonly StorageDictionary _storage;

	private readonly GameSparksAuthenticator _gameSparksAuthenticator;

	private readonly GameSparksIslandVisiting _gameSparksIslandVisiting;

	private readonly GameState _gameState;

	private readonly LikeRegistrar _likeRegistrar;

	private readonly Dictionary<string, IslandsVisitingData> _islandCache;

	private readonly StorageDictionary _lastSentIslandData;

	private const string IslandCacheKey = "IslandCache";

	private const string LastSentIslandDataKey = "LastSentIslandData";

	public event PullCompleteHandler PullCompleteEvent;

	private void FirePullCompleteEvent(string userId)
	{
		this.PullCompleteEvent?.Invoke(userId);
	}

	public IslandVisitingManager(StorageDictionary storage, GameSparksAuthenticator gameSparksAuthenticator, GameSparksIslandVisiting gameSparksIslandVisiting, GameState gameState, LikeRegistrar likeRegistrar, RoutineRunner routineRunner)
	{
		_storage = storage;
		_gameSparksAuthenticator = gameSparksAuthenticator;
		_gameSparksIslandVisiting = gameSparksIslandVisiting;
		_gameState = gameState;
		_likeRegistrar = likeRegistrar;
		_islandCache = _storage.GetDictionaryModels("IslandCache", (StorageDictionary sd) => new IslandsVisitingData(sd));
		_lastSentIslandData = _storage.GetStorageDict("LastSentIslandData");
		routineRunner.StartCoroutine(SyncMyIslands());
	}

	public void PullIslandsData(string userId, Action onSucces, Action onError)
	{
		if (_islandCache.ContainsKey(userId) && _islandCache[userId].Timestamp + CacheRetention > AntiCheatDateTime.UtcNow)
		{
			onSucces();
		}
		else
		{
			PullIslandsDataInternal(userId, onSucces, onError);
		}
	}

	public IslandsVisitingData GetIslandsVisitingData(string userId)
	{
		_islandCache.TryGetValue(userId, out IslandsVisitingData value);
		return value;
	}

	public bool LoadVisitingIslandData(IslandId islandId, string userId)
	{
		if (_islandCache.TryGetValue(userId, out IslandsVisitingData value))
		{
			ImportIsland(islandId, StorageController.StringToStorage(value.GetIslandData(islandId)));
			return true;
		}
		return false;
	}

	public bool VisitingIslandDataAvaible(string userId)
	{
		return _islandCache.ContainsKey(userId);
	}

	public bool IsUnlocked(IslandId islandId, string userId)
	{
		if (_islandCache.TryGetValue(userId, out IslandsVisitingData value))
		{
			return value.IsIslandUnlocked(islandId);
		}
		return false;
	}

	public bool HasIslandChanged(string islandKey, string data)
	{
		return _lastSentIslandData.Get(islandKey, string.Empty) != data;
	}

	private void PullIslandsDataInternal(string userId, Action onSucces, Action onError)
	{
		_gameSparksIslandVisiting.PullIslandData(_likeRegistrar, userId, delegate(IslandsVisitingData islandsData)
		{
			_islandCache[userId] = islandsData;
			FirePullCompleteEvent(userId);
			onSucces();
		}, delegate(GameSparksException exception)
		{
			GameSparksUtils.LogGameSparksError(exception);
			onError();
		});
	}

	private IEnumerator SyncMyIslands()
	{
		yield return null;
		while (true)
		{
			if (_gameSparksAuthenticator.CurrentAuthentication.IsAuthenticated)
			{
				yield return PushIslandsData();
				yield return new WaitForSeconds(300f);
			}
			else
			{
				yield return new WaitForSeconds(60f);
			}
		}
	}

	private IEnumerator PushIslandsData()
	{
		if (ExportIslands(out Dictionary<string, object> islandsData))
		{
			bool receivedResponse = false;
			_gameSparksIslandVisiting.PushIslandData(islandsData, delegate
			{
				receivedResponse = true;
			}, delegate(GameSparksException exception)
			{
				receivedResponse = true;
				GameSparksUtils.LogGameSparksError(exception);
			});
			while (!receivedResponse)
			{
				yield return null;
			}
		}
	}

	private bool ExportIslands(out Dictionary<string, object> islandsData)
	{
		islandsData = new Dictionary<string, object>();
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		bool result = false;
		IList<IslandId> allIslandIds = IslandExtensions.AllIslandIds;
		int i = 0;
		for (int count = allIslandIds.Count; i < count; i++)
		{
			string text = allIslandIds[i].ToString();
			string text2 = ExportIsland(text);
			dictionary.Add(text, text2);
			if (HasIslandChanged(text, text2))
			{
				_lastSentIslandData.Set(text, text2);
				result = true;
			}
		}
		islandsData.Add("Islands", dictionary);
		islandsData.Add("Level", _gameState.Level);
		islandsData.Add("LevelProgress", _gameState.LevelProgress);
		islandsData.Add("Happiness", _gameState.GlobalHappiness);
		islandsData.Add("Population", _gameState.GlobalPopulation);
		islandsData.Add("Housing", _gameState.GlobalHousing);
		islandsData.Add("Employees", _gameState.GlobalEmployees);
		islandsData.Add("Jobs", _gameState.GlobalJobs);
		return result;
	}

	private string ExportIsland(string islandKey)
	{
		StorageDictionary storageDict = StorageController.GameRoot.GetStorageDict(islandKey).GetStorageDict("IsometricGrid");
		if (storageDict.InternalDictionary.Count != 0)
		{
			return StorageController.StorageToString(storageDict);
		}
		return string.Empty;
	}

	private void ImportIsland(IslandId islandId, StorageDictionary gridStorage)
	{
		StorageController.GameRoot.GetStorageDict(ReadOnlyIslandBootstrapper.GetReadOnlyIslandStorageKey(islandId)).Set("IsometricGrid", gridStorage);
	}

	public void Serialize()
	{
		_storage.Set("IslandCache", _islandCache);
		_storage.Set("LastSentIslandData", _lastSentIslandData);
	}
}
