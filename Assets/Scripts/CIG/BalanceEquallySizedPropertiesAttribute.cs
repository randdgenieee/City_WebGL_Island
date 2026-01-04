using System;

namespace CIG
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public class BalanceEquallySizedPropertiesAttribute : Attribute
	{
		public string[] Keys
		{
			get;
			private set;
		}

		public BalanceEquallySizedPropertiesAttribute(params string[] keys)
		{
			Keys = keys;
		}
	}
}
