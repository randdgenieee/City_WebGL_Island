using System;

namespace CIG
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public class BalanceHiddenPropertyAttribute : BalancePropertyAttribute
	{
		public BalanceHiddenPropertyAttribute(string key, Type parseType)
			: base(key)
		{
			base.ParseType = parseType;
		}
	}
}
