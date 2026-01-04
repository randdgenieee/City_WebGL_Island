using CIG;
using SparkLinq;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CIGSceneryManager : MonoBehaviour
{
	[SerializeField]
	private SurfaceType _exclusiveElementType;

	private IsometricGrid _isometricGrid;

	private Builder _builder;

	private CIGExpansions _expansions;

	private readonly List<GridTileProperties> _sceneryProperties = new List<GridTileProperties>();

	private readonly Dictionary<SurfaceType, List<GridTileProperties>> _availablePrefabs = new Dictionary<SurfaceType, List<GridTileProperties>>();

	private DateTime _sceneryTimestamp = DateTime.MinValue;

	private bool _initialSceneryPlaced;

	private const string SceneryTimestampKey = "SceneryTimestamp";

	private const string InitialSceneryPlacedKey = "InitialSceneryIsPlaced";

	private StorageDictionary _storage;

	public void Initialize(StorageDictionary storage, IsometricGrid isometricGrid, Builder builder, CIGExpansions expansions, List<GridTileProperties> sceneryProperties)
	{
		_isometricGrid = isometricGrid;
		_builder = builder;
		_expansions = expansions;
		Deserialize(storage);
		LoadSceneryPrefabs(sceneryProperties);
		PlaceInitialScenery();
	}

	private void OnDestroy()
	{
		_isometricGrid = null;
		_expansions = null;
	}

	private void LoadSceneryPrefabs(List<GridTileProperties> sceneryProperties)
	{
		HashSet<SurfaceType> availableElementTypes = _isometricGrid.AvailableElementTypes;
		int i = 0;
		for (int count = sceneryProperties.Count; i < count; i++)
		{
			GridTileProperties gridTileProperties = sceneryProperties[i];
			if (gridTileProperties.SurfaceType == SurfaceType.AnyTypeOfLand || availableElementTypes.Contains(gridTileProperties.SurfaceType))
			{
				_sceneryProperties.Add(gridTileProperties);
			}
		}
	}

	private void PlaceInitialScenery()
	{
		if (_initialSceneryPlaced)
		{
			return;
		}
		List<GridIndex> list = new List<GridIndex>(64);
		int i = 0;
		for (int count = _expansions.ExpansionData.Count; i < count; i++)
		{
			IslandSetup.Expansion expansion = _expansions.ExpansionData[i];
			GridIndex index = new GridIndex(expansion.Index.u * 8, expansion.Index.v * 8);
			ExpansionBlock blockForIndex = _expansions.GetBlockForIndex(index);
			if (blockForIndex == null || blockForIndex.Unlocked)
			{
				continue;
			}
			list.Clear();
			for (int j = blockForIndex.Origin.v; j < blockForIndex.Origin.v + blockForIndex.Size.v; j++)
			{
				for (int k = blockForIndex.Origin.u; k < blockForIndex.Origin.u + blockForIndex.Size.u; k++)
				{
					GridIndex gridIndex = new GridIndex(k, j);
					GridElement gridElement = _isometricGrid[gridIndex];
					if (HavePrefabForType(gridElement.Type) && !(gridElement.Tile != null))
					{
						list.Add(gridIndex);
					}
				}
			}
			list.Shuffle();
			for (int l = 0; l < list.Count && l < expansion.SceneryItemCount; l++)
			{
				GridIndex index2 = list[l];
				GridElement gridElement2 = _isometricGrid[index2];
				GridTileProperties propertiesForType = GetPropertiesForType(gridElement2.Type);
				GridTile asset = SingletonMonobehaviour<BuildingsAssetCollection>.Instance.GetAsset(propertiesForType.BaseKey);
				bool mirrored = propertiesForType.CanMirror && UnityEngine.Random.value >= 0.5f;
				_builder.BuildAt(asset, propertiesForType, index2, mirrored, forced: true, serialize: true);
			}
		}
		_initialSceneryPlaced = true;
		_sceneryTimestamp = AntiCheatDateTime.UtcNow;
		Serialize();
	}

	private bool HavePrefabForType(SurfaceType type)
	{
		return GetPropertiesForType(type) != null;
	}

	private GridTileProperties GetPropertiesForType(SurfaceType type)
	{
		if (!_availablePrefabs.ContainsKey(type))
		{
			List<GridTileProperties> list = new List<GridTileProperties>();
			foreach (GridTileProperties sceneryProperty in _sceneryProperties)
			{
				bool flag = false;
				if (_exclusiveElementType != type && sceneryProperty.SurfaceType == SurfaceType.AnyTypeOfLand)
				{
					if (type.IsLand())
					{
						flag = true;
					}
				}
				else if (sceneryProperty.SurfaceType == type)
				{
					flag = true;
				}
				if (flag)
				{
					list.Add(sceneryProperty);
				}
			}
			_availablePrefabs[type] = list;
		}
		int count = _availablePrefabs[type].Count;
		if (count == 0)
		{
			return null;
		}
		return _availablePrefabs[type][UnityEngine.Random.Range(0, count)];
	}

	private void Serialize()
	{
		_storage.Set("SceneryTimestamp", _sceneryTimestamp.Ticks);
		_storage.Set("InitialSceneryIsPlaced", _initialSceneryPlaced);
	}

	private void Deserialize(StorageDictionary storage)
	{
		_storage = storage;
		_sceneryTimestamp = new DateTime(_storage.Get("SceneryTimestamp", 0L));
		_initialSceneryPlaced = _storage.Get("InitialSceneryIsPlaced", defaultValue: false);
	}
}
