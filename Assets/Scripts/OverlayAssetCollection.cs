using CIG;
using System;

public sealed class OverlayAssetCollection : DictionaryAssetCollection<OverlayAssetCollection.Overlays, OverlayType, Overlay, OverlayAssetCollection>
{
	[Serializable]
	public class Overlays : SerializableDictionary
	{
	}
}
