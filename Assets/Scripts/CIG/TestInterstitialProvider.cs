using System;
using System.Collections;
using UnityEngine;

namespace CIG
{
	public class TestInterstitialProvider : IAdProvider
	{
		private static AdProviderState _providerState;

		private AdState _adState;

		private readonly RoutineRunner _routineRunner;

		private AdSequenceType _currentAdSequenceType;

		public bool IsReady
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

		AdType IAdProvider.AdType => AdType.Interstitial;

		AdProviderType IAdProvider.AdProviderType => AdProviderType.TestInterstitial;

		AdProviderState IAdProvider.AdProviderState => _providerState;

		public event AdAvailabilityChangedEventHandler AvailabilityChangedEvent;

		private void FireAvailabilityChangedEvent()
		{
			this.AvailabilityChangedEvent?.Invoke(this);
		}

		public TestInterstitialProvider(RoutineRunner routineRunner)
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

		bool IAdProvider.ShowAd(Action<bool, bool> callback)
		{
			_adState = AdState.Showing;
			FireAvailabilityChangedEvent();
			UnityEngine.Debug.Log("[TestInterstitialProvider] Playing interstitial");
			_routineRunner.Invoke(delegate
			{
				_adState = AdState.None;
				EventTools.Fire(callback, value0: true, value1: true);
				FireAvailabilityChangedEvent();
			}, 1f);
			return true;
		}

		IEnumerator IAdProvider.StartCaching(AdSequenceType adSequenceType)
		{
			if (_providerState != AdProviderState.Initialized)
			{
				UnityEngine.Debug.LogWarning("TestInterstitialProvider is not yet initialized!");
			}
			else
			{
				yield return LoadAd(adSequenceType);
			}
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
