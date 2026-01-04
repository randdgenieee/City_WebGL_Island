using System.Collections;
using Tweening;
using UnityEngine;

namespace CIG
{
	public abstract class OneTimeOfferPopupContentBase : MonoBehaviour
	{
		[SerializeField]
		protected Tweener _badgeTweener;

		[SerializeField]
		protected GameObject _discountBadge;

		[SerializeField]
		protected LocalizedText _badgeLocalizedText;

		public abstract IEnumerator AnimateInRoutine();

		public abstract void StopAnimation();
	}
}
