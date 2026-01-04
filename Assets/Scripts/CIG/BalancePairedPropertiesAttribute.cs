using System;

namespace CIG
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public class BalancePairedPropertiesAttribute : Attribute
	{
		public string[] Keys
		{
			get;
		}

		public BalancePairedPropertiesAttribute(params string[] keys)
		{
			Keys = keys;
		}
	}
}
