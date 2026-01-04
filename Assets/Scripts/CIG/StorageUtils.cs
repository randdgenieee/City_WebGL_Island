using UnityEngine;

namespace CIG
{
	public static class StorageUtils
	{
		private const string Vector3XKey = "X";

		private const string Vector3YKey = "Y";

		private const string Vector3ZKey = "Z";

		public static StorageDictionary SerializeVector3(Vector3 vector3)
		{
			StorageDictionary storageDictionary = new StorageDictionary();
			storageDictionary.Set("X", vector3.x);
			storageDictionary.Set("Y", vector3.y);
			storageDictionary.Set("Z", vector3.z);
			return storageDictionary;
		}

		public static Vector3 DeserializeVector3(StorageDictionary storage)
		{
			float x = storage.Get("X", 0f);
			float y = storage.Get("Y", 0f);
			float z = storage.Get("Z", 0f);
			return new Vector3(x, y, z);
		}
	}
}
