using System.Collections;
using Tweening;
using UnityEngine;

namespace CIG
{
	public class WheelOfFortuneWinningsElement : MonoBehaviour
	{
		[SerializeField]
		private CurrencyView _currencyView;

		[SerializeField]
		private NumberTweenHelper _currencyValueTweenHelper;

		[SerializeField]
		private Tweener _scaleTweener;

		private Currency _currency;

		private decimal _currencyAmount;

		public void Initialize(Currency currency)
		{
			_currency = currency;
			_currencyAmount = decimal.One;
			_currencyView.Initialize(_currency.ToCurrencyType(), _currencyAmount);
			AddValue(_currency.Value - decimal.One);
		}

		public void AddValue(decimal add)
		{
			StartCoroutine(AddValueRoutine(add));
		}

		private IEnumerator AddValueRoutine(decimal add)
		{
			_currencyValueTweenHelper.TweenTo(_currencyAmount, _currencyAmount + add);
			_scaleTweener.Play();
			yield return new WaitWhile(() => _currencyValueTweenHelper.Running);
			_currencyAmount += add;
		}
	}
}
