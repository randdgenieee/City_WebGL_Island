using System;

namespace CIG
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
	public class BalancePropertyClassAttribute : Attribute
	{
		public string Key
		{
			get;
			private set;
		}

		public bool IsBaseKey
		{
			get;
			private set;
		}

		public BalancePropertyClassAttribute(string key, bool isBaseKey = false)
		{
			Key = key;
			IsBaseKey = isBaseKey;
		}
	}
}
