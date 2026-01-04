using Tweening;
using UnityEngine;

namespace CIG
{
	public class VisitingHomeButton : HUDCurrencyTweenHelper
	{
		[SerializeField]
		private Tweener _iconTweener;

		public new void Initialize()
		{
			base.Initialize();
			SetActiveTweener(_iconTweener);
		}

		public void Activate()
		{
			SingletonMonobehaviour<CurrencyAnimator>.Instance.SetOverrideAnimationTarget(_currencyAnimationTargets[0]);
		}

		public void Deactivate()
		{
			SingletonMonobehaviour<CurrencyAnimator>.Instance.SetOverrideAnimationTarget(null);
		}

		protected override void UpdateValue(decimal value)
		{
		}
	}
}
