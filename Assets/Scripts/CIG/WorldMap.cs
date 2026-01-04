using SparkLinq;
using System.Collections.Generic;

namespace CIG
{
	public class WorldMap
	{
		public delegate void WorldMapVisibilityChanged(bool visible);

		public delegate void NewIslandUnlockedEventHandler(IslandId islandId);

		private readonly StorageDictionary _storage;

		private readonly IslandsManager _islandsManager;

		private const string AirshipKey = "Airship";

		private const string VisibleIslandsKey = "VisibleIslands";

		public bool IsVisible
		{
			get;
			private set;
		}

		public AirshipState Airship
		{
			get;
		}

		public IslandConnectionGraph IslandConnectionGraph
		{
			get;
		}

		public HashSet<IslandId> VisibleIslands
		{
			get;
		}

		public IslandId NewlyUnlockedIslandId
		{
			get;
			private set;
		} = IslandId.None;


		public event WorldMapVisibilityChanged VisibilityChangedEvent;

		public event NewIslandUnlockedEventHandler NewIslandUnlockedEvent;

		private void FireVisibilityEvent(bool visible)
		{
			this.VisibilityChangedEvent?.Invoke(visible);
		}

		private void FireNewIslandUnlockedEvent(IslandId islandId)
		{
			this.NewIslandUnlockedEvent?.Invoke(islandId);
		}

		public WorldMap(StorageDictionary storage, Timing timing, RoutineRunner routineRunner, Multipliers multipliers, GameState gameState, IslandsManager islandsManager, IslandConnectionGraphProperties islandConnectionGraphProperties)
		{
			_storage = storage;
			_islandsManager = islandsManager;
			IslandConnectionGraph = new IslandConnectionGraph(islandConnectionGraphProperties.IslandConnectionsProperties);
			Airship = new AirshipState(_storage.GetStorageDict("Airship"), timing, routineRunner, multipliers, gameState, _islandsManager);
			VisibleIslands = new HashSet<IslandId>(_storage.GetList<int>("VisibleIslands").ConvertAll((int i) => (IslandId)i));
			FillVisibleIslands();
			Airship.StateChangedEvent += OnStateChanged;
			SetAirshipState(Airship.CurrentState);
			ToggleWorldMap(visible: true);
		}

		public void ToggleWorldMap(bool visible)
		{
			if (IsVisible != visible)
			{
				IsVisible = visible;
				FireVisibilityEvent(visible);
			}
		}

		public void GoToIsland(IslandId islandId)
		{
			if (IsVisible)
			{
				_islandsManager.OpenIsland(islandId);
			}
		}

		public void IslandShown(IslandId islandId)
		{
			VisibleIslands.Add(islandId);
		}

		public int GetTravelDuration(IslandId toIslandId)
		{
			if (Airship.CanTravel)
			{
				return IslandConnectionGraph.GetTravelTimeBetweenIslands(Airship.CurrentIslandId, toIslandId);
			}
			return -1;
		}

		public bool IsIslandVisible(IslandId islandId)
		{
			return VisibleIslands.Contains(islandId);
		}

		private void FillVisibleIslands()
		{
			IList<IslandId> allIslandIds = IslandExtensions.AllIslandIds;
			int i = 0;
			for (int count = allIslandIds.Count; i < count; i++)
			{
				IslandId islandId = allIslandIds[i];
				if (_islandsManager.IsUnlocked(islandId))
				{
					VisibleIslands.Add(islandId);
				}
			}
		}

		private void OnStateChanged(AirshipState.State previousState, AirshipState.State newState)
		{
			SetAirshipState(newState);
		}

		private void SetAirshipState(AirshipState.State state)
		{
			switch (state)
			{
			case AirshipState.State.Hovering:
				NewlyUnlockedIslandId = Airship.CurrentIslandId;
				FireNewIslandUnlockedEvent(NewlyUnlockedIslandId);
				break;
			case AirshipState.State.Landed:
				if (NewlyUnlockedIslandId != IslandId.None)
				{
					IslandId newlyUnlockedIslandId = NewlyUnlockedIslandId;
					NewlyUnlockedIslandId = IslandId.None;
					FireNewIslandUnlockedEvent(newlyUnlockedIslandId);
				}
				break;
			}
		}

		public void Serialize()
		{
			_storage.Set("VisibleIslands", VisibleIslands.ToList().ConvertAll((IslandId i) => (int)i));
			_storage.Set("Airship", Airship);
		}
	}
}
