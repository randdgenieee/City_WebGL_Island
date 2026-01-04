using System;

namespace CIG
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class BalancePropertyAttribute : Attribute
	{
		public string Key
		{
			get;
			private set;
		}

		public bool RequiredKey
		{
			get;
			set;
		}

		public bool VerifyValue
		{
			get;
			set;
		}

		public bool AllowEmptyValue
		{
			get;
			set;
		}

		public bool DeepVerify
		{
			get;
			set;
		}

		public Type ParseType
		{
			get;
			set;
		}

		public bool IdenticalValueAcrossBalances
		{
			get;
			set;
		}

		public bool Overridable
		{
			get;
			set;
		}

		public BalancePropertyAttribute(string key)
		{
			Key = key;
			RequiredKey = true;
			VerifyValue = true;
			AllowEmptyValue = false;
			DeepVerify = true;
			ParseType = null;
			IdenticalValueAcrossBalances = false;
			Overridable = true;
		}
	}
}
