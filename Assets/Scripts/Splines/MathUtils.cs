using UnityEngine;

namespace Splines
{
	public static class MathUtils
	{
		public static float Pow10(int power)
		{
			return Mathf.Pow(10f, power);
		}

		public static float Floor(float value, int digits)
		{
			return Floor(value, Pow10(digits));
		}

		public static float Floor(float value, float pow10)
		{
			return Mathf.Floor(value * pow10) / pow10;
		}
	}
}
