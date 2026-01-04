using CIG;
using System;

public sealed class PlingAssetCollection : DictionaryAssetCollection<PlingAssetCollection.Plings, PlingType, Pling, PlingAssetCollection>
{
	[Serializable]
	public class Plings : SerializableDictionary
	{
	}
}
