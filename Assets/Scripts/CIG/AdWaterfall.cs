using System;
using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	public class AdWaterfall
	{
		private readonly CriticalProcesses _criticalProcesses;

		private readonly AdProviderPool _adProviderPool;

		private readonly List<AdProviderType> _adProviderSequence;

		public bool IsReady
		{
			get
			{
				if (_criticalProcesses.HasCriticalProcess)
				{
					return false;
				}
				int i = 0;
				for (int count = _adProviderSequence.Count; i < count; i++)
				{
					if (_adProviderPool.IsReady(_adProviderSequence[i]))
					{
						return true;
					}
				}
				return false;
			}
		}

		public AdWaterfall(AdProviderPool adProviderPool, CriticalProcesses criticalProcesses, params AdProviderType[] adProviderSequence)
		{
			_adProviderPool = adProviderPool;
			_criticalProcesses = criticalProcesses;
			_adProviderSequence = new List<AdProviderType>(adProviderSequence);
			List<AdProviderType> list = new List<AdProviderType>();
			int count = _adProviderSequence.Count;
			for (int i = 0; i < count; i++)
			{
				AdProviderType item = _adProviderSequence[i];
				if (!list.Contains(item))
				{
					list.Add(item);
				}
			}
			count = list.Count;
			for (int j = 0; j < count; j++)
			{
				AdProviderType providerType = list[j];
				if (!_adProviderPool.AdProviderExists(providerType))
				{
					_adProviderSequence.RemoveAll((AdProviderType x) => x == providerType);
					UnityEngine.Debug.LogWarningFormat("[AdWaterfall] AdProvider '{0}' doesn't exist. -> Removing all occurances from the provider sequence list.", providerType);
				}
			}
		}

		public bool ContainsAdProvider(AdProviderType providerType)
		{
			return _adProviderSequence.Contains(providerType);
		}

		public bool ShowAd(Action<bool, bool> callback, VideoSource source)
		{
			if (!IsReady)
			{
				return false;
			}
			int count = _adProviderSequence.Count;
			for (int i = 0; i < count; i++)
			{
				AdProviderType adProviderType = _adProviderSequence[i];
				if (_adProviderPool.IsReady(adProviderType))
				{
					if (_adProviderPool.ShowAd(adProviderType, callback, source))
					{
						_adProviderSequence.RemoveAt(i);
						_adProviderSequence.Add(adProviderType);
						return true;
					}
					UnityEngine.Debug.LogErrorFormat("[AdWaterfall] Failed to ShowAd for '{0}'", adProviderType);
					return false;
				}
			}
			return false;
		}
	}
}
