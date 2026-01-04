using System;

namespace CIG
{
	public static class EventTools
	{
		public static void Fire(Action action)
		{
			action?.Invoke();
		}

		public static void Fire<T0>(Action<T0> action, T0 value0)
		{
			action?.Invoke(value0);
		}

		public static void Fire<T0, T1>(Action<T0, T1> action, T0 value0, T1 value1)
		{
			action?.Invoke(value0, value1);
		}

		public static TResult Fire<TResult>(Func<TResult> func, TResult defaultValue = default(TResult))
		{
			if (func != null)
			{
				return func();
			}
			return defaultValue;
		}

		public static TResult Fire<T0, TResult>(Func<T0, TResult> func, T0 value0, TResult defaultValue = default(TResult))
		{
			if (func != null)
			{
				return func(value0);
			}
			return defaultValue;
		}

		public static TResult Fire<T0, T1, TResult>(Func<T0, T1, TResult> func, T0 value0, T1 value1, TResult defaultValue = default(TResult))
		{
			if (func != null)
			{
				return func(value0, value1);
			}
			return defaultValue;
		}
	}
}
