using System;

namespace CIG
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
	public class BalancePropertyClassExtensionAttribute : Attribute
	{
		public Type ExtendedPropertyClassType
		{
			get;
			private set;
		}

		public BalancePropertyClassExtensionAttribute(Type extendedPropertyClassType)
		{
			ExtendedPropertyClassType = extendedPropertyClassType;
		}
	}
}
