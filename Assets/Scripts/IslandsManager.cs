using CIG;
using SparkLinq;
using System.Collections.Generic;

public sealed class IslandsManager
{
	public delegate void VisitingStartedEventHandler();

	public delegate void VisitingStoppedEventHandler();

	public delegate void IslandUnlockedEventHandler(IslandId islandId);

	public delegate void IslandChangedEventHandler(IslandId islandId, bool isVisiting);

	private static readonly IslandId[] InitiallyUnlockedIslands = new IslandId[1];

	private readonly StorageDictionary _storage;

	private readonly GameStats _gameStats;

	private readonly IslandVisitingManager _islandVisitingManager;

	private readonly HashSet<IslandId> _unlockedIslands;

	private const string UnlockedIslandsKey = "UnlockedIslands";

	public IslandId CurrentIsland
	{
		get;
		private set;
	}

	public bool IsVisiting => !string.IsNullOrEmpty(VisitingUserId);

	public string VisitingUserId
	{
		get;
		private set;
	}

	public int IslandsUnlocked => _unlockedIslands.Count;

	public event VisitingStartedEventHandler VisitingStartedEvent;

	public event VisitingStoppedEventHandler VisitingStoppedEvent;

	public event IslandUnlockedEventHandler IslandUnlockedEvent;

	public event IslandChangedEventHandler IslandChangedEvent;

	private void FireVisitingStartedEvent()
	{
		this.VisitingStartedEvent?.Invoke();
	}

	private void FireVisitingStoppedEvent()
	{
		this.VisitingStoppedEvent?.Invoke();
	}

	private void FireIslandUnlockedEvent(IslandId islandId)
	{
		this.IslandUnlockedEvent?.Invoke(islandId);
	}

	private void FireIslandChangedEvent(IslandId islandId, bool isVisiting)
	{
		this.IslandChangedEvent?.Invoke(islandId, isVisiting);
	}

	public IslandsManager(StorageDictionary storage, GameStats gameStats, IslandVisitingManager islandVisitingManager)
	{
		_storage = storage;
		_gameStats = gameStats;
		_islandVisitingManager = islandVisitingManager;
		CurrentIsland = IslandId.None;
		List<int> list = _storage.GetList<int>("UnlockedIslands");
		_unlockedIslands = new HashSet<IslandId>(list.ConvertAll((int i) => (IslandId)i));
		int j = 0;
		for (int num = InitiallyUnlockedIslands.Length; j < num; j++)
		{
			UnlockIsland(InitiallyUnlockedIslands[j]);
		}
	}

	public void OpenIsland(IslandId islandId)
	{
		if (islandId.IsValid())
		{
			CurrentIsland = islandId;
			FireIslandChangedEvent(CurrentIsland, IsVisiting);
			Analytics.IslandOpened(islandId.ToString());
		}
	}

	public void CloseCurrentIsland()
	{
		if (CurrentIsland.IsValid())
		{
			CurrentIsland = IslandId.None;
			FireIslandChangedEvent(CurrentIsland, IsVisiting);
		}
	}

	public void UnlockIsland(IslandId islandId)
	{
		if (!_unlockedIslands.Contains(islandId))
		{
			_unlockedIslands.Add(islandId);
			_gameStats.AddIslandUnlocked();
			FireIslandUnlockedEvent(islandId);
		}
	}

	public void StartVisiting(string visitingUserId)
	{
		if (!IsVisiting)
		{
			VisitingUserId = visitingUserId;
			_islandVisitingManager.PullIslandsData(VisitingUserId, OnStartVisitingSuccess, OnStartVisitingError);
		}
	}

	public void StopVisiting()
	{
		if (IsVisiting)
		{
			VisitingUserId = null;
			FireVisitingStoppedEvent();
		}
	}

	public bool IsUnlocked(IslandId islandId)
	{
		if (IsVisiting)
		{
			bool num = _islandVisitingManager.VisitingIslandDataAvaible(VisitingUserId);
			bool flag = _islandVisitingManager.IsUnlocked(islandId, VisitingUserId);
			return num & flag;
		}
		return _unlockedIslands.Contains(islandId);
	}

	private void OnStartVisitingSuccess()
	{
		FireVisitingStartedEvent();
	}

	private void OnStartVisitingError()
	{
		VisitingUserId = string.Empty;
	}

	public void Serialize()
	{
		_storage.Set("UnlockedIslands", _unlockedIslands.ToList().ConvertAll((IslandId i) => (int)i));
	}
}
