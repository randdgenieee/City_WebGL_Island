using CIG;
using System;
using UnityEngine;

public class MaterialAssetCollection : DictionaryAssetCollection<MaterialAssetCollection.Materials, MaterialType, Material, MaterialAssetCollection>
{
	[Serializable]
	public class Materials : SerializableDictionary
	{
	}
}
