using CIG;
using System.Collections.Generic;

public class CIGIslandStats
{
	private const string RoadCountKey = "roads";

	private const string MaxLevelBuildingCountKey = "maxLevelBuildings";

	private const string DecorationCountKey = "decorations";

	private readonly StorageDictionary _storage;

	public int RoadCount
	{
		get
		{
			return _storage.Get("roads", 0);
		}
		set
		{
			_storage.Set("roads", value);
		}
	}

	public int DecorationsCount
	{
		get
		{
			return _storage.Get("decorations", 0);
		}
		set
		{
			_storage.Set("decorations", value);
		}
	}

	public int MaxLevelBuildingCount
	{
		get
		{
			return _storage.Get("maxLevelBuildings", 0);
		}
		set
		{
			_storage.Set("maxLevelBuildings", value);
		}
	}

	public CIGIslandStats(StorageDictionary storage)
	{
		_storage = storage;
	}

	public void PopuplateStats(IsometricIsland island)
	{
		RoadCount = CountRoadsOn(island);
		DecorationsCount = CountDecorationsOn(island);
		MaxLevelBuildingCount = CountMaxLevelBuildings(island);
	}

	private int CountDecorationsOn(IsometricIsland island)
	{
		int num = 0;
		List<Building> buildingsOnIsland = island.BuildingsOnIsland;
		int i = 0;
		for (int count = buildingsOnIsland.Count; i < count; i++)
		{
			if (buildingsOnIsland[i] is CIGDecoration)
			{
				num++;
			}
		}
		return num;
	}

	private int CountMaxLevelBuildings(IsometricIsland island)
	{
		int num = 0;
		List<Building> buildingsOnIsland = island.BuildingsOnIsland;
		int i = 0;
		for (int count = buildingsOnIsland.Count; i < count; i++)
		{
			CIGBuilding cIGBuilding = buildingsOnIsland[i] as CIGBuilding;
			if (cIGBuilding != null && cIGBuilding.BuildingProperties.MaximumLevel > 1 && cIGBuilding.BuildingProperties.MaximumLevel == cIGBuilding.CurrentLevel)
			{
				num++;
			}
		}
		return num;
	}

	private int CountRoadsOn(IsometricIsland island)
	{
		int num = 0;
		GridSize size = island.IsometricGrid.Size;
		for (int i = 0; i < size.v; i++)
		{
			for (int j = 0; j < size.u; j++)
			{
				if (island.IsometricGrid[j, i].Tile as Road != null)
				{
					num++;
				}
			}
		}
		return num;
	}
}
