using System;

namespace CIG
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class BalanceDistributedPropertyAttribute : BalancePropertyAttribute
	{
		public string DistributedFileName
		{
			get;
			private set;
		}

		public int DistributedPropsPerFile
		{
			get;
			private set;
		}

		public BalanceDistributedPropertyAttribute(string key, Type parseType, string distributedFileName, int distributedPropsPerFile)
			: base(key)
		{
			base.ParseType = parseType;
			DistributedFileName = distributedFileName;
			DistributedPropsPerFile = distributedPropsPerFile;
		}
	}
}
