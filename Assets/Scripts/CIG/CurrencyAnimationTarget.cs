using UnityEngine;

namespace CIG
{
	public class CurrencyAnimationTarget : MonoBehaviour
	{
		[SerializeField]
		private RectTransform _destination;

		[SerializeField]
		private CurrencyType _currencyType;

		public ICurrencyAnimationTarget Target
		{
			get;
			private set;
		}

		public RectTransform Destination => _destination;

		public void Initialize(ICurrencyAnimationTarget target)
		{
			Target = target;
			SingletonMonobehaviour<CurrencyAnimator>.Instance.RegisterCurrencyTarget(_currencyType, this);
		}

		private void OnDestroy()
		{
			if (SingletonMonobehaviour<CurrencyAnimator>.IsAvailable && Target != null)
			{
				SingletonMonobehaviour<CurrencyAnimator>.Instance.UnregisterCurrencyTarget(_currencyType);
			}
			Target = null;
		}
	}
}
