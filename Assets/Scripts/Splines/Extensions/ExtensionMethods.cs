using System;
using System.Collections.Generic;
using UnityEngine;

namespace Splines.Extensions
{
	public static class ExtensionMethods
	{
		public static T PickRandom<T>(this List<T> list)
		{
			if (list.Count == 0)
			{
				return default(T);
			}
			return list[UnityEngine.Random.Range(0, list.Count)];
		}

		public static Vector2 Floor(this Vector2 vector, int digits)
		{
			return vector.Floor(MathUtils.Pow10(digits));
		}

		public static Vector2 Floor(this Vector2 vector, float pow10)
		{
			vector.x = MathUtils.Floor(vector.x, pow10);
			vector.y = MathUtils.Floor(vector.y, pow10);
			return vector;
		}

		public static void Floor(this List<Vector2> vectors, int digits, Action<int> elementCallback = null)
		{
			float pow = MathUtils.Pow10(digits);
			int count = vectors.Count;
			for (int i = 0; i < count; i++)
			{
				vectors[i] = vectors[i].Floor(pow);
				elementCallback?.Invoke(i);
			}
		}

		public static Vector3 Floor(this Vector3 vector, int digits)
		{
			return vector.Floor(MathUtils.Pow10(digits));
		}

		public static Vector3 Floor(this Vector3 vector, float pow10)
		{
			vector.x = MathUtils.Floor(vector.x, pow10);
			vector.y = MathUtils.Floor(vector.y, pow10);
			vector.z = MathUtils.Floor(vector.z, pow10);
			return vector;
		}

		public static void Floor(this List<Vector3> vectors, int digits, Action<int> elementCallback = null)
		{
			float pow = MathUtils.Pow10(digits);
			int count = vectors.Count;
			for (int i = 0; i < count; i++)
			{
				vectors[i] = vectors[i].Floor(pow);
				elementCallback?.Invoke(i);
			}
		}
	}
}
