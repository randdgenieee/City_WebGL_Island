using System;

namespace CIG
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public class BalanceSortedArrayPropertiesAttribute : Attribute
	{
		public string Key
		{
			get;
		}

		public bool Ascending
		{
			get;
		}

		public BalanceSortedArrayPropertiesAttribute(string key, bool ascending)
		{
			Key = key;
			Ascending = ascending;
		}
	}
}
