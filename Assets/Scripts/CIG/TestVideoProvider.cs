using System;
using System.Collections;
using UnityEngine;

namespace CIG
{
	public class TestVideoProvider : IAdProvider
	{
		private static AdProviderState _providerState;

		private AdState _adState;

		private readonly RoutineRunner _routineRunner;

		private AdSequenceType _currentAdSequenceType;

		bool IAdProvider.IsReady
		{
			get
			{
				if (_providerState == AdProviderState.Initialized)
				{
					return _adState == AdState.Available;
				}
				return false;
			}
		}

		AdProviderType IAdProvider.AdProviderType => AdProviderType.TestVideo;

		AdType IAdProvider.AdType => AdType.Video;

		AdProviderState IAdProvider.AdProviderState => _providerState;

		public event AdAvailabilityChangedEventHandler AvailabilityChangedEvent;

		private void FireAvailabilityChangedEvent()
		{
			this.AvailabilityChangedEvent?.Invoke(this);
		}

		public TestVideoProvider(RoutineRunner routineRunner)
		{
			_routineRunner = routineRunner;
		}

		IEnumerator IAdProvider.Initialize()
		{
			_providerState = AdProviderState.Initialized;
			yield break;
		}

		void IAdProvider.Release()
		{
		}

		IEnumerator IAdProvider.StartCaching(AdSequenceType adSequenceType)
		{
			if (_providerState != AdProviderState.Initialized)
			{
				UnityEngine.Debug.LogWarning("TestVideoProvider is not yet initialized!");
			}
			else
			{
				yield return LoadAd(adSequenceType);
			}
		}

		bool IAdProvider.ShowAd(Action<bool, bool> callback)
		{
			_adState = AdState.Showing;
			FireAvailabilityChangedEvent();
			_routineRunner.Invoke(delegate
			{
				_adState = AdState.None;
				EventTools.Fire(callback, value0: true, value1: true);
				FireAvailabilityChangedEvent();
			}, 1f);
			return true;
		}

		private IEnumerator LoadAd(AdSequenceType adSequenceType)
		{
			if (_adState == AdState.None || _currentAdSequenceType != adSequenceType)
			{
				_currentAdSequenceType = adSequenceType;
				_adState = AdState.Requesting;
				yield return new WaitForSeconds(2f);
				_adState = AdState.Available;
				FireAvailabilityChangedEvent();
			}
		}
	}
}
