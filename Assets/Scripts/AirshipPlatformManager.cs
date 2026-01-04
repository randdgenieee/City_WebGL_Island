using CIG;
using System.Collections.Generic;
using UnityEngine;

public class AirshipPlatformManager : MonoBehaviour
{
	private GridIndex _platformIndex;

	private IsometricGrid _isometricGrid;

	private Builder _builder;

	private Properties _properties;

	private bool _platformPlaced;

	private const string PlatformPlacedKey = "PlatformPlacedKey";

	private StorageDictionary _storage;

	public void Initialize(StorageDictionary storage, IslandSetup islandSetup, IsometricGrid isometricGrid, Builder builder, Properties properties)
	{
		_platformIndex = islandSetup.AirshipIndex;
		_isometricGrid = isometricGrid;
		_builder = builder;
		_properties = properties;
		Deserialize(storage);
		PlacePlatform();
	}

	private void OnDestroy()
	{
		_platformIndex = GridIndex.invalid;
		_isometricGrid = null;
		_builder = null;
	}

	private void PlacePlatform()
	{
		if (!_platformPlaced)
		{
			GridElement gridElement = _isometricGrid[_platformIndex];
			GridTileProperties propertiesForType = GetPropertiesForType(gridElement.Type);
			GridTile asset = SingletonMonobehaviour<BuildingsAssetCollection>.Instance.GetAsset(propertiesForType.BaseKey);
			if (!_builder.BuildAt(asset, propertiesForType, _platformIndex, mirrored: false, forced: true, serialize: true))
			{
				UnityEngine.Debug.LogError("Unable to build the Hot Air Balloon Platform");
				return;
			}
			_platformPlaced = true;
			Serialize();
			((AirshipPlatform)_isometricGrid[_platformIndex].Tile).StartCinematic();
		}
	}

	private GridTileProperties GetPropertiesForType(SurfaceType type)
	{
		List<GridTileProperties> allAirshipPlatforms = _properties.AllAirshipPlatforms;
		int i = 0;
		for (int count = allAirshipPlatforms.Count; i < count; i++)
		{
			GridTileProperties gridTileProperties = allAirshipPlatforms[i];
			if ((gridTileProperties.SurfaceType == SurfaceType.AnyTypeOfLand && type.IsLand()) || gridTileProperties.SurfaceType == type)
			{
				return gridTileProperties;
			}
		}
		return null;
	}

	private void Serialize()
	{
		_storage.Set("PlatformPlacedKey", _platformPlaced);
	}

	private void Deserialize(StorageDictionary storage)
	{
		_storage = storage;
		_platformPlaced = _storage.Get("PlatformPlacedKey", defaultValue: false);
	}
}
