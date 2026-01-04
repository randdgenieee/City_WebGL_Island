using System;
using UnityEngine;

namespace CIG
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	public class ShowWhenAttribute : PropertyAttribute
	{
		public string CompareTo
		{
			get;
		}

		public object[] CompareValues
		{
			get;
		}

		public ComparisonType ComparisonType
		{
			get;
		}

		public ShowWhenAttribute(string compareTo, ComparisonType comparisonType, params object[] compareValues)
		{
			CompareTo = compareTo;
			CompareValues = compareValues;
			ComparisonType = comparisonType;
		}
	}
}
