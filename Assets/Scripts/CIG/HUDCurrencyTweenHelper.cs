using Tweening;
using UnityEngine;

namespace CIG
{
	public abstract class HUDCurrencyTweenHelper : TweenHelper, ICurrencyAnimationTarget
	{
		public delegate void FlyingCurrencyPlayingChangedEventHandler(bool isPlaying);

		[SerializeField]
		protected CurrencyAnimationTarget[] _currencyAnimationTargets;

		private Tweener _activeTweener;

		private int _flyingCurrenciesPlaying;

		public bool FlyingCurrencyIsPlaying => _flyingCurrenciesPlaying > 0;

		private int FlyingCurrenciesPlaying
		{
			get
			{
				return _flyingCurrenciesPlaying;
			}
			set
			{
				bool flyingCurrencyIsPlaying = FlyingCurrencyIsPlaying;
				_flyingCurrenciesPlaying = value;
				if (flyingCurrencyIsPlaying != FlyingCurrencyIsPlaying)
				{
					FireFlyingCurrencyPlayingChangedEvent(FlyingCurrencyIsPlaying);
				}
			}
		}

		public event FlyingCurrencyPlayingChangedEventHandler FlyingCurrencyPlayingChangedEvent;

		private void FireFlyingCurrencyPlayingChangedEvent(bool isPlaying)
		{
			this.FlyingCurrencyPlayingChangedEvent?.Invoke(isPlaying);
		}

		protected void Initialize()
		{
			int i = 0;
			for (int num = _currencyAnimationTargets.Length; i < num; i++)
			{
				_currencyAnimationTargets[i].Initialize(this);
			}
		}

		protected virtual void OnDestroy()
		{
			if (_activeTweener != null)
			{
				_activeTweener.FinishedPlaying -= OnTweenerFinishedPlaying;
			}
		}

		public virtual void FlyingCurrencyStartedPlaying()
		{
			FlyingCurrenciesPlaying++;
		}

		public virtual void FlyingCurrencyFinishedPlaying(Currency earnedCurrency, bool animateHudElement = true)
		{
			if (animateHudElement)
			{
				if (_activeTweener.IsPlaying)
				{
					_activeTweener.Stop();
					OnTweenerFinishedPlaying(_activeTweener);
				}
				_activeTweener.Play();
			}
		}

		protected void SetActiveTweener(Tweener tweener)
		{
			if (!(tweener == _activeTweener))
			{
				if (_activeTweener != null && _activeTweener.IsPlaying)
				{
					_activeTweener.StopAndReset();
					OnTweenerFinishedPlaying(_activeTweener);
					_activeTweener.FinishedPlaying -= OnTweenerFinishedPlaying;
				}
				_activeTweener = tweener;
				_activeTweener.FinishedPlaying += OnTweenerFinishedPlaying;
			}
		}

		private void OnTweenerFinishedPlaying(Tweener tweener)
		{
			FlyingCurrenciesPlaying--;
		}
	}
}
