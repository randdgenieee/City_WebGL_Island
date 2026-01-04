using System;
using System.Globalization;
using UnityEngine;

namespace CIG
{
	[Serializable]
	public class SurfaceTypeInfo
	{
		[SerializeField]
		private SurfaceType _surfaceType;

		[SerializeField]
		private float _percentage;

		public SurfaceType SurfaceType => _surfaceType;

		public float Percentage => _percentage;

		public SurfaceTypeInfo(SurfaceType surfaceType, float percentage)
		{
			_surfaceType = surfaceType;
			_percentage = percentage;
		}

		public static bool TryParse(string key, string value, out SurfaceTypeInfo result)
		{
			if (EnumExtensions.TryParseEnum(key, out SurfaceType parsedEnum))
			{
				if (float.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out float result2))
				{
					result = new SurfaceTypeInfo(parsedEnum, result2);
					return true;
				}
				UnityEngine.Debug.LogWarning($"Parse error: Cannot parse {value} to {typeof(float).Name}");
				result = null;
				return false;
			}
			UnityEngine.Debug.LogWarning($"Parse error: Cannot parse {key} to {typeof(SurfaceType).Name}");
			result = null;
			return false;
		}
	}
}
