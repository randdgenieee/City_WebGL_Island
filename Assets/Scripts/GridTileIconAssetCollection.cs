using CIG;
using System;

public sealed class GridTileIconAssetCollection : DictionaryAssetCollection<GridTileIconAssetCollection.GridTileIcons, GridTileIconType, GridTileIcon, GridTileIconAssetCollection>
{
	[Serializable]
	public class GridTileIcons : SerializableDictionary
	{
	}
}
