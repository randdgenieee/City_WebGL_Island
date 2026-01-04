using SparkLinq;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	[BalancePropertyClass("adSequence", true)]
	[BalanceHiddenProperty("sequence", typeof(List<string>))]
	public class AdSequenceProperties : BaseProperties
	{
		private const string AdSequenceKey = "sequence";

		private readonly List<AdSequenceType> _adSequence;

		public AdSequenceProperties(PropertiesDictionary propsDict, string baseKey)
			: base(propsDict, baseKey)
		{
			List<string> property = GetProperty("sequence", new List<string>());
			_adSequence = new List<AdSequenceType>();
			int i = 0;
			for (int count = property.Count; i < count; i++)
			{
				if (Enum.TryParse(property[i], ignoreCase: true, out AdSequenceType result) && result != 0)
				{
					_adSequence.Add(result);
				}
				else
				{
					UnityEngine.Debug.LogWarning("Invalid value for AdSequenceType: " + property[i] + ". This value is ignored");
				}
			}
		}

		public AdSequenceType GetAdSequenceType(int index)
		{
			if (index >= 0 && index < _adSequence.Count)
			{
				return _adSequence[index];
			}
			return _adSequence.Last();
		}
	}
}
