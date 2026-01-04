using System;

namespace CIG
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public class BalanceListCountPropertiesAttribute : Attribute
	{
		public string Key
		{
			get;
		}

		public bool EvenCount
		{
			get;
		}

		public BalanceListCountPropertiesAttribute(string key, bool evenCount)
		{
			Key = key;
			EvenCount = evenCount;
		}
	}
}
