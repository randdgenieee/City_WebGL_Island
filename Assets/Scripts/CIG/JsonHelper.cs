using System;
using UnityEngine;

namespace CIG
{
	public class JsonHelper
	{
		[Serializable]
		private class Wrapper<T>
		{
			public T[] array;
		}

		public static T[] GetJsonArray<T>(string json)
		{
			return JsonUtility.FromJson<Wrapper<T>>($"{{ \"array\": {json}}}").array;
		}
	}
}
